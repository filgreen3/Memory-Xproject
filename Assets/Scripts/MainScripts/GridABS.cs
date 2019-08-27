using System;
using System.Collections.Generic;
using UnityEngine;

public class GridABS : MonoBehaviour {
    public float gridscale;
    public Texture2D Map;
    public int ArenaSizeX, ArenaSizeY;
    public NodeA[, ] GridOfArena;
    public List<Vector2> MoveMentList = new List<Vector2> ();
    public List<SpriteRenderer> NodeList = new List<SpriteRenderer> ();
    public List<Vector2> ShelterList = new List<Vector2> ();
    public List<Shelter> CurrentShelterList = new List<Shelter> ();

    public GameObject NodaLevel, MiniNodaLevel;
    public ManagerMovment Manager;
    private Vector3 UnitPositionRad;
    public GameObject block, Bloc1, Shelter, RadiousShow;

    public List<NodeA> WalkableArea = new List<NodeA> ();

    //Pool marker region
    SpriteRenderer[] markerpool = new SpriteRenderer[50];
    int imarkerPoolIndex = 0;
    int indexInPool {
        get {
            imarkerPoolIndex++;
            if (imarkerPoolIndex >= 50)
                imarkerPoolIndex = 0;
            return imarkerPoolIndex;
        }
    }

    void Awake () {
        for (int i = 0; i < 50; i++) {
            markerpool[i] = Instantiate (NodaLevel, Vector3.down, Quaternion.identity).GetComponent<SpriteRenderer> ();
            markerpool[i].transform.localScale = Vector3.one * 2.25f;
            markerpool[i].gameObject.SetActive (false);
        }

        ArenaSizeX = Map.width;
        ArenaSizeY = Map.height;

        CreateGrid ();

    }

    public NodeA NodeFromWorldPoint (Vector3 worldPosition) {
        float percentX = (worldPosition.x + ArenaSizeX / 2) / ArenaSizeX;
        float percentY = (worldPosition.z + ArenaSizeY / 2) / ArenaSizeY;
        percentX = Mathf.Clamp01 (percentX);
        percentY = Mathf.Clamp01 (percentY);

        int x = Mathf.RoundToInt ((ArenaSizeX - 1) * percentX);
        int y = Mathf.RoundToInt ((ArenaSizeY - 1) * percentY);
        return GridOfArena[x, y];
    }
    public void NodeCoordinat (Vector3 worldPosition, out int x, out int y) {
        float percentX = (worldPosition.x + ArenaSizeX / 2) / ArenaSizeX;
        float percentY = (worldPosition.z + ArenaSizeY / 2) / ArenaSizeY;
        percentX = Mathf.Clamp01 (percentX);
        percentY = Mathf.Clamp01 (percentY);

        x = Mathf.RoundToInt ((ArenaSizeX - 1) * percentX);
        y = Mathf.RoundToInt ((ArenaSizeY - 1) * percentY);

    }

    void CreateGrid () {
        GridOfArena = new NodeA[ArenaSizeX, ArenaSizeY];

        Vector3 BotomLeft = transform.position - Vector3.right * (ArenaSizeX / 2) - Vector3.forward * (ArenaSizeY / 2);
        for (int x = 0; x < ArenaSizeX; x++)
            for (int y = 0; y < ArenaSizeY; y++) {

                GridOfArena[x, y] = new NodeA (BotomLeft + Vector3.right * x + Vector3.right * 0.5f + Vector3.forward * y, x, y, WhatTypeNode (Map.GetPixel (x, y)));
                if (Map.GetPixel (x, y) == Color.green || Map.GetPixel (x, y) == Color.blue) {

                    Vector3 positionofnode = (BotomLeft + Vector3.right * x + Vector3.right * 0.5f + Vector3.forward * y);
                    Transform node = Instantiate (MiniNodaLevel, positionofnode, Quaternion.identity).transform;
                    node.eulerAngles = new Vector3 (90, 0, 0);
                    node.localScale *= 0.6f;

                }

            }

    }

    NodeA.TypeOfNode WhatTypeNode (Color ColorOnMap) {
        if (ColorOnMap == Color.green)
            return NodeA.TypeOfNode.Walkable;
        if (ColorOnMap == Color.red)
            return NodeA.TypeOfNode.Wall;
        if (ColorOnMap == Color.blue)
            return NodeA.TypeOfNode.Shelter;

        return NodeA.TypeOfNode.Wall;
    }

    public void MaxMovement (Vector3 UnitPosition, int MaxUnitMove, UnitBehevior Unit) //показывает возможно движение
    {
        CleatGrid ();

        int x = 0, y = 0;
        NodeCoordinat (UnitPosition, out x, out y);
        for (var d = 0; d < 4; d++) {
            for (var i = 0; i <= MaxUnitMove; i++) {
                var nodes = GridOfArena[x + i * MPAer (d, true), y + i * MPAer (d, false)];
                if (nodes._Bloc == 0 &&
                    nodes.UnitOnNode == null) {
                    WalkableArea.Add (nodes);
                    MoveMentList.Add (new Vector2 (nodes.x1, nodes.y1));
                }
                if (nodes._Bloc == NodeA.TypeOfNode.Wall)
                    break;
            }
        }
        CreateNodeShow (Color.green);

    }

    public void ShowAllEnemy (bool NeedAteam, bool IsOnLine) // показывает вех врагов на сетке
    {
        foreach (UnitBehevior n in !NeedAteam ? Manager.EnemyList : Manager.FriendList) {
            if (NodeFromWorldPoint (n.transform.position).FireZone || !IsOnLine) {
                GridOfArena[NodeFromWorldPoint (n.transform.position).x1, NodeFromWorldPoint (n.transform.position).y1].FireZone = true;
                MoveMentList.Add (new Vector2 (NodeFromWorldPoint (n.transform.position).x1, NodeFromWorldPoint (n.transform.position).y1));
                CreateNodeShow (Color.yellow, GridOfArena[NodeFromWorldPoint (n.transform.position).x1, NodeFromWorldPoint (n.transform.position).y1].Position);
                if (IsOnLine) {

                    Manager.ShowInfoText (Mathf.Clamp01 (Mathf.Clamp01 (Manager.CorrectUnit.Weapon.NormalDistance / (Vector3.Distance (n.transform.position, Manager.CorrectUnit.transform.position))) + (Manager.CorrectUnit.Skills[Manager.CorrectUnit.Weapon.TypeWeapon] - n.GetShelterLevel ()) / 100f), n.transform.position + Vector3.right * 0.1f + Vector3.forward * 0.3f + Vector3.up * 0.6f);
                }
            }
        }
    }

    public void CleatGrid () {
        Manager.CleanJunk ();
        foreach (Vector2 n in MoveMentList) {
            GridOfArena[(int) n.x, (int) n.y].FireZone = false;
        }
        foreach (SpriteRenderer n in NodeList) {
            n.gameObject.SetActive (false);
            n.transform.localScale = Vector3.one * 2.25f;
        }

        WalkableArea.Clear ();
        MoveMentList.Clear ();
    }

    public void ShowRadiousAttack (int _DistanceOfAttack) {
        Vector3 UnitPosition = Manager.CorrectUnit.transform.position;
        WeaponBehevior UnitWeapon = Manager.CorrectUnit.Weapon;

        CleatGrid ();
        foreach (Vector2 n in MoveMentList) {
            GridOfArena[(int) n.x, (int) n.y].FireZone = false;
        }

        #region Walking type , create normales grid
        int x;
        int y;
        switch ((int) UnitWeapon.fireTypeThis) {
            case 0:
                NodeCoordinat (Manager.CorrectUnit.transform.position, out x, out y);
                for (var d = 0; d < 4; d++) {
                    for (var i = 1; i <= _DistanceOfAttack; i++) {

                        var nodes = GridOfArena[x + i * MPAer (d, true), y + i * MPAer (d, false)];

                        if (nodes._Bloc != NodeA.TypeOfNode.Wall) {
                            GridOfArena[nodes.x1, nodes.y1].FireZone = true;
                            MoveMentList.Add (new Vector2 (nodes.x1, nodes.y1));
                            if (nodes.UnitOnNode != null && i > 1)
                                break;
                        } else {
                            break;
                        }
                    }
                }
                break;

            case 1:
                break;

        }
        #endregion
        CreateNodeShow (Color.red);

    }
    #region создание цветной сетки
    void CreateNodeShow (Color NodeColor) {

        foreach (Vector2 n in MoveMentList) {
            Vector3 PoseN = GridOfArena[(int) n.x, (int) n.y].Position;

            SpriteRenderer NodeShow = markerpool[indexInPool];
            NodeShow.transform.position = PoseN;
            NodeShow.transform.eulerAngles = new Vector3 (90, 0, 0);
            NodeShow.color = NodeColor;
            NodeShow.transform.localScale = Vector3.one * 2.25f;
            NodeShow.gameObject.SetActive (true);
            NodeList.Add (NodeShow);
        }

    }
    public void CreateNodeShow (Color NodeColor, Vector3 NodePosition) {

        SpriteRenderer NodeShow = markerpool[indexInPool];
        NodeShow.transform.position = NodePosition;
        NodeShow.transform.eulerAngles = new Vector3 (90, 0, 0);
        NodeShow.sortingOrder = -31;
        NodeShow.transform.localScale = Vector3.one * 2.25f;
        NodeShow.color = NodeColor;
        NodeShow.gameObject.SetActive (true);
        NodeList.Add (NodeShow);

    }
    public void CreateNodeShow (Vector3 NodePosition) {

        SpriteRenderer NodeShow = markerpool[indexInPool];
        NodeShow.transform.position = NodePosition;
        NodeShow.transform.eulerAngles = new Vector3 (90, 0, 0);
        NodeShow.transform.localScale = Vector3.one * 2.25f;
        NodeShow.transform.localScale *= 0.6f;
        if (NodeFromWorldPoint (NodePosition)._Bloc == NodeA.TypeOfNode.Shelter)
            NodeShow.color *= new Color (0, 0, 1, 0.6f);
        else
            NodeShow.color *= new Color (1, 1, 1, 0.6f);

        NodeShow.sortingOrder = -31;
        NodeShow.gameObject.SetActive (true);
    }
    #endregion

    public void ClearNode (Vector3 _Position) {
        GridOfArena[NodeFromWorldPoint (_Position).x1, NodeFromWorldPoint (_Position).y1].UnitOnNode = null;
    }
    public void ClearNode (Vector3 _Position, UnitBehevior _unit) {
        GridOfArena[NodeFromWorldPoint (_Position).x1, NodeFromWorldPoint (_Position).y1].UnitOnNode = _unit;
    }

    public int MPAer (int stage, bool isItx) {
        switch (stage) {
            case 0:
                if (isItx) return 1;
                else return 0;
            case 1:
                if (isItx) return -1;
                else return 0;
            case 2:
                if (!isItx) return 1;
                else return 0;
            case 3:
                if (!isItx) return -1;
                else return 0;

            default:
                return 0;
        }
    }

}