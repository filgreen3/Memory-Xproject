using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeA  {


    public enum TypeOfNode
    {
        Walkable,
        Wall,
        Shelter
    }
    public TypeOfNode _Bloc;
    public Vector3 Position;
    public bool FireZone;
    public int x1, y1;

    UnitBehevior iUnitOnNode;
    public UnitBehevior UnitOnNode
    {
        get
        {
            return iUnitOnNode;
        }
        

        set
        {
            if (value != null)
            {
                iUnitOnNode = value;
                value.CreateMarcer(Position);
            }
            else
                iUnitOnNode = null;

        }
    }

    public NodeA(Vector3 _position,int x,int y,TypeOfNode _InMap)
    {
        _Bloc = _InMap;
        x1 = x;
        y1 = y;
        Position = _position;
        FireZone = false;
        
    }
}
