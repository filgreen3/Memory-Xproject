using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : UnitBehevior
{
    private int _globDeep = 1;
    private Vector3 _playerDirection;
    public float k = 10;

    private float _ipriority;
    public float Priority
    {

        get
        {
            if (true)
                return GetHeltyTarget()-1;
            return _ipriority;
        }
        set
        {
            if (value >= 0 && value <= 1)
                _ipriority = value;
            else
            {
                if (value > 1)
                    _ipriority = 1;
                else
                    _ipriority = 0;
            }
        }
    }

    public struct CheckedNode
    {
        public NodeA Node;
        public int Cost;

        public CheckedNode(NodeA node, int cost)
        {
            Node = node;
            Cost = cost;
        }
    }
    private Dictionary<int, CheckedNode> _checkedNodes = new Dictionary<int, CheckedNode>();


    private Vector3[] GetAllUnitPosition
    {
        get
        {
            int i = 0;
            Vector3[] result = new Vector3[manager.FriendList.Count];
            foreach (UnitBehevior n in manager.FriendList)
            {
                result[i] = n.transform.position;
            }
            return result;

        }
    }
    private Vector3[] _allUnitPositionMain;


    public virtual void ChooseTactic()
    {

        if(_effectOnUnit.HaveEffect(2))
        {
            return;         

        }
        _allUnitPositionMain = GetAllUnitPosition;
        TestAllEnemyUnitPosition();
        if (manager.CorrectUnit == this && !_effectOnUnit.HaveEffect(2))
        {
            if (Weapon.Load > 0)
            {
                StartCoroutine(MainActive(1f));
            }
            else
            {
                Weapon.Reload();
                manager.RetarderCall(1f);
            }
        }
    }

    private int TestPointOnShoot(int x, int y, out UnitBehevior Target)
    {
        var mainPoint = 0;
        var localPoint = 0;
        UnitBehevior mainTarget = null;
        UnitBehevior localTarget;
        for (var d = 0; d < 4; d++)
        {
            for (var i = 1; i <= Weapon.MaxDistance; i++)
            {
                var nodes = GridManager.GridOfArena[x + i * MPAer(d, true), y + i * MPAer(d, false)];

                if (nodes._Bloc == NodeA.TypeOfNode.Wall)
                {
                    break;
                }
                else
                {
                    localPoint = GetPointCord(nodes.x1, nodes.y1, out localTarget);
                    if (localPoint > mainPoint)
                    {
                        mainPoint = localPoint;
                        mainTarget = localTarget;
                    }
                    if (nodes.UnitOnNode != null && i >= 1)
                        break;
                }

            }
        }
        Target = mainTarget;
        return mainPoint;
    }
    private int TestPointOnMovement(int x, int y, out Vector3 movetarget, int deep)
    {
        if (deep > 0 && deep < 8)
        {

            var subdeep = deep - 1;
            var mainPoint = 0;
            var localPoint = 0;
            movetarget = Vector3.back;



            var mainMoveTarget = Vector3.one * 999f;
            for (var d = 0; d < 4; d++)
            {
                for (var i = 1; i <= MaxWalk; i++)
                {
                    try
                    {

                        UnitBehevior mainTarget;
                        Vector3 nonMoveTarget;
                        var nodes = GridManager.GridOfArena[x + i * MPAer(d, true), y + i * MPAer(d, false)];


                        int value = TestPointOnMovement(nodes.x1, nodes.y1, out nonMoveTarget, subdeep);
                        localPoint = value;

                        if (_checkedNodes.ContainsKey(nodes.x1 * 1000 + nodes.y1))
                        {
                            localPoint += _checkedNodes[nodes.x1 * 1000 + nodes.y1].Cost;
                        }
                        else
                        {
                            int valueInt = ShelterTest(nodes.x1, nodes.y1);//proverca ykritia

                            if (CanItShoot(nodes.Position))
                                valueInt += TestPointOnShoot(nodes.x1, nodes.y1, out mainTarget);
                            localPoint += valueInt;
                            _checkedNodes.Add(nodes.x1 * 1000 + nodes.y1, new CheckedNode(nodes, valueInt));
                        }

                        #region Test if it wall
                        if (localPoint > mainPoint
                            && nodes.UnitOnNode == null
                            && nodes._Bloc == NodeA.TypeOfNode.Walkable)
                        {
                            mainPoint = localPoint;
                            mainMoveTarget = nodes.Position;
                        }

                        else if (nodes._Bloc == NodeA.TypeOfNode.Wall)
                        {
                            break;
                        }
                        #endregion
                    }
                    catch { }
                }
            }

            movetarget = mainMoveTarget;

            return mainPoint;
        }

        movetarget = Vector3.zero;
        return 0;
    }

    private int GetPointCord(int x, int y, out UnitBehevior target)
    {
        target = null;
        try
        {
            target = GridManager.GridOfArena[x, y].UnitOnNode;
        }
        catch { }

        if (target != null && !target.Enemy)
        {
            WeaponData wp = Weapon.data;
            int result = (int)
                (wp.Damage * wp.BulletPerShoot                                                                      // Урон вчистом виде
                * Mathf.Clamp01((wp.NormalDistance / Vector3.Distance(target.transform.position, transform.position)// Учет дистанции
                + Skills[wp.TypeWeapon] - target.GetShelterLevel())                                                 // Учет навыков и защищщенности цели
                * target.GetHeltyTarget()));                                                                        // Учет процента жизней
            return result;
        }
        return 0;
    }
    private int ShelterTest(int x, int y)
    {
        var _localpoint = 0;
        for (var d = 0; d < 4; d++)
        {
            var nodes = GridManager.GridOfArena[x + MPAer(d, true), y + MPAer(d, false)];

            if ((int)nodes._Bloc == 2)
                _localpoint += (int)(50f * (_playerDirection.x * MPAer(d, true)));

        }
        return _localpoint;
    }

    private void TestAllEnemyUnitPosition()
    {
        Vector3 _lerpedVector = Vector3.zero;
        foreach (UnitBehevior n in manager.FriendList)
        {
            _lerpedVector = (_lerpedVector + (n.transform.position - transform.position).normalized) / 2f;
        }

        _playerDirection = _lerpedVector;


    }
    private int TestPercUsable(out UnitBehevior target)
    {
        target = null;
        if (TimeToReusePerc > 0) return 0;
        int maxPoint=0;
        switch (PercBehevior.AParm)
        {
            case 1:
                if (PercBehevior.BAPram == 1)
                {
                    
                    foreach (UnitBehevior unit in manager.FriendList)
                    {
                        if (Vector3.Distance(transform.position, unit.transform.position) > PercBehevior.Radious) continue;

                        if (maxPoint < PercBehevior.PowerPerc * (1 + PercBehevior.Duratin) * unit.GetHeltyTarget() &&
                             !unit._effectOnUnit.HaveEffect(PercBehevior.CAParm) || PercBehevior.CAParm == 2)
                        {
                            maxPoint = PercBehevior.CAParm != 2
                                  ? (int)(PercBehevior.PowerPerc * (1 + PercBehevior.Duratin) * unit.GetHeltyTarget() *
                                           (1 - unit.Resistans[PercBehevior.CAParm] / 100f)) : 1000;
                            target = unit;
                        }

                    }
                    return maxPoint;
                }
                break;

            case 2:
                int localPoint=0,countInBoom=0;
                int allUnitCount = manager.FriendList.Count;
                for (int i = 0; i < allUnitCount; i++)
                {
                    if (Vector3.Distance(manager.FriendList[i].transform.position, transform.position)
                            < PercBehevior.Radious)
                    {
                        UnitBehevior unit = manager.FriendList[i];
                        localPoint += PercBehevior.PowerPerc;
                        countInBoom++;
                        for (int j = 0; j < allUnitCount; j++)
                        {
                            if (Vector3.Distance(manager.FriendList[j].transform.position, unit.transform.position) < PercBehevior.Duratin
                                && unit != manager.FriendList[j])
                            {
                                countInBoom++;
                                if (PercBehevior.CAParm != 1)
                                    localPoint += PercBehevior.PowerPerc;
                                else
                                    localPoint *= PercBehevior.PowerPerc;
                            }
                        }
                        if (localPoint > maxPoint&& countInBoom>1)
                        {
                            maxPoint = localPoint;
                            target = unit;
                        }
                        countInBoom = 0;
                    }
                }
                return maxPoint;
        }


        return 0;
    }

    public IEnumerator MainActive(float second)
    {
        if (Priority > 0.8f&&false)
        {
           manager.ShowInfoText("Overwatch",Color.white,transform.position);
            AmbushZone = true;
            manager.RetarderCall(1f);
            yield return null;
        }
        else
        {


        UnitBehevior percTarget;
        UnitBehevior shootTarget;
        var moveTargetFinal = Vector3.one * 999f;
        var endAction = false;

        int x, y;
        GridManager.NodeCoordinat(transform.position, out x, out y);

        var sorceShoot = TestPointOnShoot(x, y, out shootTarget);
        var sorcePerc = TestPercUsable(out percTarget);


        yield return new WaitForSeconds(1);
        while (!endAction)
        {
            float sorceMovement = TestPointOnMovement(x, y, out moveTargetFinal, _globDeep);

            if ((sorceShoot + sorcePerc) * k > sorceMovement ||
                Vector3.Distance(moveTargetFinal, transform.position) < 1f && sorceShoot != 0f)
            {
                endAction = true;
                if (sorceShoot > sorcePerc)
                {
                    StartCoroutine(ShootByUnit(this, shootTarget));
                }
                else
                {
                    PercBehevior.UsePerc(Enemy, percTarget);
                    manager.PercUse = false;

                    TimeToReusePerc = PercBehevior.data.TimeToReuse;
                    manager.RetarderCall(1f);
                }
            }
            else
            {
                if (moveTargetFinal != Vector3.one * 999f)
                {
                    Debug.Log("MakeMove by deep : " + _globDeep);
                    endAction = true;
                    _checkedNodes.Clear();
                    GridManager.ClearNode(transform.position);
                    
                    StartCoroutine(LerpMove(moveTargetFinal + Vector3.forward * 0.12f + Vector3.up * 0.34f));
                    transform.eulerAngles = new Vector3(20, -45, 0);
                    GridManager.ClearNode(moveTargetFinal + Vector3.forward * 0.12f + Vector3.up * 0.34f, this);
                    GridManager.CleatGrid();
                }
                else
                {
                    _globDeep++;
                }
            }
        }

        
        _globDeep = 1;
        }
    }

    public int MPAer(int stage, bool isItx)
    {
        switch (stage)
        {
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

            default: return 0;
        }
    }

    bool CanItShoot(Vector3 sp)
    {
        foreach (Vector3 n in _allUnitPositionMain)
        {
            if (Vector3.Distance(sp, n) < (Weapon.MaxDistance + 1) && (Math.Abs(sp.x - n.x) < 0.25f || Math.Abs(sp.y - n.y) < 0.25f))
                return true;

        }
        return false;
    }
    bool CanItShelter(Vector3 sp)
    {

        foreach (Vector2 n in GridManager.ShelterList)
        {
            if (Vector3.Distance(sp, n) < (1) && (Math.Abs(sp.x - n.x) < 0.25f || Math.Abs(sp.y - n.y) < 0.25f))
                return true;

        }
        return false;
    }

    private void AddMapOfItresting()
    {
        Vector2[] vectors = GridManager.ShelterList.ToArray();
        foreach (Vector2 node in vectors)
            for (var d = 0; d < 4; d++)
            {
                try
                {
                    var nodes = GridManager.GridOfArena[(int)node.x + MPAer(d, true), (int)node.y + MPAer(d, false)];
                    if (_checkedNodes.ContainsKey(nodes.x1 * 1000 + nodes.y1))
                        _checkedNodes.Add(nodes.x1 * 1000 + nodes.y1, new CheckedNode(nodes, (int)(ShelterTest(nodes.x1, nodes.y1) * Priority)));

                }
                catch { }
            }
        foreach (UnitBehevior unit in manager.EnemyList)
        {
            Vector3 poseofunit = unit.transform.position;
            int x, y;
            GridManager.NodeCoordinat(poseofunit, out x, out y);
            for (var d = 0; d < 4; d++)
            {
                for (var i = 1; i <= Weapon.MaxDistance; i++)
                {
                    var nodes = GridManager.GridOfArena[x + i * MPAer(d, true), y + i * MPAer(d, false)];
                    if (nodes._Bloc == NodeA.TypeOfNode.Wall)
                    {
                        break;
                    }
                    else
                    {
                        if (_checkedNodes.ContainsKey(nodes.x1 * 1000 + nodes.y1))
                            _checkedNodes.Add(nodes.x1 * 1000 + nodes.y1, new CheckedNode(
                                nodes,
                                (int)
                            #region
                            ((1 - Priority) * (Weapon.Damage * Weapon.BulletPerShoot * Mathf.Clamp01(
                                      Mathf.Clamp01(manager.CorrectUnit.Weapon.NormalDistance /
                                                    Vector3.Distance(unit.transform.position,
                                                        manager.CorrectUnit.transform.position)) +
                                      (manager.CorrectUnit.Skills[manager.CorrectUnit.Weapon.TypeWeapon] -
                                       unit.GetShelterLevel()) / 100f) * unit.GetHeltyTarget()))
                            #endregion
                            ));

                        if (nodes.UnitOnNode != null && i >= 1)
                            break;
                    }
                }
            }
        }
    }

}