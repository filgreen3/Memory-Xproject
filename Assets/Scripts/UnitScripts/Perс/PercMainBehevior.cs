using System.Collections;
using System;
using UnityEngine;

public class PercMainBehevior : MonoBehaviour
{
    public ManagerMovment manager;
    public GridABS GridManager;
    public PercData data;
    public UnitBehevior ThisUnit;
    public int AParm, BAPram, CAParm;
    public int PowerPerc, Duratin, Radious;
    public Sprite Icon;
    GameObject Show;
    private bool _granadeUsable;
    Vector3 point = Vector2.zero;
    private Vector2[] vectorsOfGrande;
    public ParticleSystem VisualEffect;
    public void SetParams()
    {
        AParm = data.A_param;
        BAPram = data.BA_param;
        CAParm = data.СA_param;
        PowerPerc = data.PowerOfPerc;
        Duratin = data.DuratinOfPerc;
        Radious = data.RadiousOfattack;
    }
    public void ShowTargets()
    {
        //Show = Instantiate(GridManager.RadiousShow, transform.position, Quaternion.identity, transform);
        //Show.transform.localScale = new Vector3(Radious, 0, Radious);
        switch (BAPram)
        {
            case 0:

                GridManager.CreateNodeShow(Color.yellow, GridManager.NodeFromWorldPoint(ThisUnit.transform.position).Position);

                break;
            case 1:
                foreach (UnitBehevior n in !ThisUnit.Enemy ? manager.EnemyList : manager.FriendList)
                {
                    if (Vector3.Distance(transform.position, n.transform.position) < Radious)
                        GridManager.CreateNodeShow(Color.yellow, GridManager.NodeFromWorldPoint(n.transform.position).Position);
                }
                break;
            case 2:
                foreach (UnitBehevior n in ThisUnit.Enemy ? manager.EnemyList : manager.FriendList)
                {
                    if (Vector3.Distance(transform.position, n.transform.position) < Radious)
                        GridManager.CreateNodeShow(Color.yellow, GridManager.NodeFromWorldPoint(n.transform.position).Position);
                }
                break;
        }
    }

    public void Start()
    {
        SetParams();
        if (ThisUnit.Enemy) return;

        if (AParm == 2)
            VisualEffect = manager.OnTurnEffectFX[1].GetComponent<ParticleSystem>();
        if(AParm==3)
            VisualEffect = manager.OnTurnEffectFX[9].GetComponent<ParticleSystem>();
        VisualEffect = Instantiate(VisualEffect, transform.position, Quaternion.identity, transform);
    }



    void Update()
    {
        if (manager.CorrectUnit == ThisUnit && manager.PercUse)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit, Mathf.Infinity);



            if (AParm == 1 && hit.rigidbody != null)
            {
                Debug.Log(hit.rigidbody.name);
                UnitBehevior PointedTarget = hit.rigidbody.GetComponent<UnitBehevior>();

                if (ThisUnit.TimeToReusePerc <= 0 && manager.PercUse && Input.GetKeyDown(KeyCode.Mouse0) && IsTarget(PointedTarget) && Vector3.Distance(transform.position, PointedTarget.transform.position) < Radious)
                {
                    GridManager.CleatGrid();
                    manager.CleanJunk();


                    UnitBehevior unitTarget = manager.CorrectUnit;



                    if (unitTarget.transform.position.x - manager.CorrectUnit.transform.position.x > 1)
                    {
                        unitTarget.Weapon.SpriteRendererValue.sortingOrder = 1;
                        unitTarget.Weapon.SpriteRendererValue.flipX = false;

                        unitTarget.HandRenderer.sortingOrder = 1;
                        unitTarget.HandRenderer.flipX = false;

                        unitTarget.BodyRenderer.sprite = unitTarget.data.IconBodyFace;
                    }

                    if (unitTarget.transform.position.x - manager.CorrectUnit.transform.position.x < -1)
                    {
                        unitTarget.Weapon.SpriteRendererValue.sortingOrder = -2;
                        unitTarget.Weapon.SpriteRendererValue.flipX = true;

                        unitTarget.HandRenderer.sortingOrder = -2;
                        unitTarget.HandRenderer.flipX = true;

                        unitTarget.BodyRenderer.sprite = unitTarget.data.IconBodyBack;
                    }



                    manager.CorrectUnit.PercBehevior.UsePerc(manager.CorrectUnit.Enemy, PointedTarget);
                    manager.PercUse = false;
                    Destroy(Show);
                    manager.CorrectUnit.TimeToReusePerc = data.TimeToReuse;
                    manager.EndTurn();



                }

            }


            if (AParm == 2 && ThisUnit.TimeToReusePerc < 1)
            {



                if (!VisualEffect.gameObject.activeInHierarchy)
                    VisualEffect.gameObject.SetActive(true);

                GridManager.CleatGrid();
                point = GridManager.NodeFromWorldPoint(ray.GetPoint(15)).Position;
                vectorsOfGrande = GetMatrix();
                VisualEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);


                var vel = VisualEffect.velocityOverLifetime;
                Vector3 dir = (point - transform.position).normalized;


                float dist = Vector3.Distance(point, transform.position);
                if (dist > Radious)
                    dist = Radious;
                vel.x = new ParticleSystem.MinMaxCurve(dir.x * dist);
                vel.z = new ParticleSystem.MinMaxCurve(dir.z * dist);


                VisualEffect.Play();

                Vector3 vector = GridManager.NodeFromWorldPoint(transform.position + dist * dir).Position;
                for (int i = 0; i < vectorsOfGrande.Length; i++)
                {
                    vectorsOfGrande[i] += new Vector2(vector.x, vector.z);
                    GridManager.CreateNodeShow(Color.cyan, new Vector3(vectorsOfGrande[i].x, 0, vectorsOfGrande[i].y));
                }


                if (Input.GetKeyDown(KeyCode.Mouse0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {

                    var granadeLineFx = Instantiate(manager.OnTurnEffectFX[8].GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
                    granadeLineFx.Stop();
                    var fxvel = granadeLineFx.velocityOverLifetime;

                    fxvel.x = new ParticleSystem.MinMaxCurve(dir.x * dist);
                    fxvel.z = new ParticleSystem.MinMaxCurve(dir.z * dist);

                    granadeLineFx.gameObject.SetActive(true);
                    granadeLineFx.Play();

                    VisualEffect.gameObject.SetActive(false);

                    point = vector;
                    Invoke("MakeBoom", 1);

                   
                   
                    ThisUnit.TimeToReusePerc = data.TimeToReuse;
                    manager.RetarderCall(1.5f);
                }
            }

            if (AParm == 3 && ThisUnit.TimeToReusePerc < 1)
            {
                NodeA node = new NodeA(Vector3.right*1000,1111,1111,NodeA.TypeOfNode.Wall);

                if (Vector3.Distance(point, ray.GetPoint(15)) > 0.1f)
                {
                    if (!VisualEffect.gameObject.activeInHierarchy)
                        VisualEffect.gameObject.SetActive(true);

                    VisualEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    var vel = VisualEffect.velocityOverLifetime;

                    GridManager.CleatGrid();
                    point = GridManager.NodeFromWorldPoint(ray.GetPoint(15)).Position;
                    Vector3 dir = (point - transform.position).normalized;

                    float dist = Vector3.Distance(point, transform.position);
                    if (dist > Radious)
                        dist = Radious;
                    vel.x = new ParticleSystem.MinMaxCurve(dir.x * dist);
                    vel.z = new ParticleSystem.MinMaxCurve(dir.z * dist);


                    VisualEffect.Play();

                    node = GridManager.NodeFromWorldPoint(transform.position + dir * dist);

                    if (node._Bloc == NodeA.TypeOfNode.Walkable && node.UnitOnNode == null)
                        GridManager.CreateNodeShow(manager.ColorOfIfo[1], node.Position);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0) && node._Bloc== NodeA.TypeOfNode.Walkable &&!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    GameObject fx = Instantiate(manager.OnTurnEffectFX[10 + CAParm], node.Position, Quaternion.identity);
                    fx.transform.eulerAngles = new Vector3(0,-87.69f,0);
                    VisualEffect.gameObject.SetActive(false);
                    ThisUnit.TimeToReusePerc = data.TimeToReuse;
                    manager.RetarderCall(1f);
                }
            }

        }
        else if (VisualEffect != null && VisualEffect.gameObject.activeInHierarchy)
            VisualEffect.gameObject.SetActive(false);
    }


    bool IsTarget(UnitBehevior _testTarget)
    {
        switch (BAPram)
        {
            case 0:
                if (_testTarget == manager.CorrectUnit)
                    return true;
                break;
            case 1:
                if (_testTarget.Enemy != manager.CorrectUnit.Enemy)
                    return true;
                break;
            case 2:
                if (_testTarget.Enemy == manager.CorrectUnit.Enemy)
                    return true;
                break;
        }
        return false;
    }

    public void UsePerc(bool _Enemy, UnitBehevior _Target)
    {

        switch (AParm)
        {
            case 0:
                ChengParams(_Target);

                break;



            case 1:
                SpellPerc(_Target);
                break;

            case 2:
                vectorsOfGrande = GetMatrix();
              
                               
                Vector3 vector = GridManager.NodeFromWorldPoint(_Target.transform.position).Position;
                Vector3 dir = vector - transform.position;
                for (int i = 0; i < vectorsOfGrande.Length; i++)
                {
                    vectorsOfGrande[i] += new Vector2(vector.x, vector.z);
                }


                var granadeLineFx = Instantiate(manager.OnTurnEffectFX[8].GetComponent<ParticleSystem>(), transform.position, Quaternion.identity);
                granadeLineFx.Stop();
                var fxvel = granadeLineFx.velocityOverLifetime;

                fxvel.x = new ParticleSystem.MinMaxCurve(dir.x);
                fxvel.z = new ParticleSystem.MinMaxCurve(dir.z);

                granadeLineFx.gameObject.SetActive(true);
                granadeLineFx.Play();

                point = vector;
                Invoke("MakeBoom", 1);
                break;

        }


    }
    void ChengParams(UnitBehevior Target)
    {
        switch (CAParm)
        {
            case 0:
                Target.Helty -= PowerPerc;
                Target.GetHit(0, 0);
                break;


            case 1:
                Target.Shild -= PowerPerc;
                Target.GetHit(0, 0);
                break;


            case 2:
                Target.Armor -= PowerPerc;
                Target.GetHit(0, 0);
                break;
        }
    }
    void SpellPerc(UnitBehevior Target)
    {
        switch (CAParm)
        {
            case 0:
                Target._effectOnUnit.MakeFired(PowerPerc, Duratin);
                break;


            case 1:
                Target._effectOnUnit.MakePoisend(PowerPerc, Duratin);
                break;


            case 2:
                Target._effectOnUnit.MakeStuned(Duratin);
                break;

            case 3:
                Target._effectOnUnit.MakeElectricShock(PowerPerc);
                break;
            case 4:
                Target._effectOnUnit.HealHuman(PowerPerc);
                break;
        }
    }

    void MakeBoom()
    {
        for (int i = 0; i < vectorsOfGrande.Length; i++)
        {
            Vector2 n = vectorsOfGrande[i];
            UnitBehevior unitTarget = GridManager.NodeFromWorldPoint(new Vector3(n.x, 0, n.y)).UnitOnNode;
            if (unitTarget != null)
            {
                switch (CAParm)
                {

                    case 0:
                        unitTarget.GetHit(PowerPerc, 434);
                        break;
                    case 1:
                        Vector3 poitionToChek = unitTarget.transform.position + (unitTarget.transform.position - point).normalized * PowerPerc;//Check

                        if (GridManager.NodeFromWorldPoint(poitionToChek)._Bloc == NodeA.TypeOfNode.Walkable &&
                            GridManager.NodeFromWorldPoint(poitionToChek).UnitOnNode == null)
                        {
                            GridManager.ClearNode(unitTarget.transform.position);
                            StartCoroutine(unitTarget.LerpMove(GridManager.NodeFromWorldPoint(poitionToChek).Position +
                                                                Vector3.forward * 0.12f + Vector3.up * 0.34f));
                            unitTarget.transform.eulerAngles = new Vector3(20, -45, 0);
                            GridManager.ClearNode(
                                GridManager.NodeFromWorldPoint(poitionToChek).Position + Vector3.forward * 0.12f +
                                Vector3.up * 0.34f, unitTarget);

                            unitTarget.GetHit(PowerPerc * 4, 434);
                        }
                        else
                            unitTarget.GetHit(PowerPerc * 12, 434);
                        break;
                    case 2:
                        unitTarget._effectOnUnit.MakeStuned(1);
                        break;
                    case 3:
                        unitTarget._effectOnUnit.MakeFired(PowerPerc, 2);
                        break;
                    case 4:
                        unitTarget._effectOnUnit.MakeElectricShock(PowerPerc);
                        break;
                }
            }
            foreach (Shelter sh in GridManager.CurrentShelterList)
            {
                if (Vector3.Distance(sh.transform.position, new Vector3(n.x, 0, n.y)) < 1)
                {
                    Instantiate(sh.FX, sh.transform.position, Quaternion.identity);

                    sh.ShelterLevl -= PowerPerc * 3;
                    if (sh.ShelterLevl < 1)
                        Destroy(sh.gameObject);
                }


            }
        }


        Instantiate(manager.OnTurnEffectFX[CAParm + 2], point+Vector3.up*0.75f, Quaternion.identity);
        manager.PlaySpecific(manager.Expl);
    }

    Vector2[] GetMatrix()
    {
        Vector2[] result = new Vector2[(Duratin * 2 - 1) * (Duratin * 2 - 1)];
        int k = 0;
        for (int i = -(Duratin - 1); i <= Duratin - 1; i++)
        {
            for (int j = -(Duratin - 1); j <= Duratin - 1; j++)
            {
                result[k] = new Vector2(i, j);
                k++;
            }
        }

        return result;

    }



}
