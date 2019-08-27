using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : ScriptableObject {
    [Header ("MainParametr")]
    public Color ColorOfFraction;
    [Space (5)]
    public int MaxWalk;
    [Range (0, 1000)]
    public float Helty, Shild, Armor;
    [Range (1, 10)]
    public int ArmorQulity;
    public PercData PercOfUnit;
    public float WeaponPosVector;

    [Range (0, 100)]
    public int[] Skills = new int[6];
    [Range (0, 100)]
    public int[] Resistans = new int[4];

    public enum TypeWalk {
        Plus,
        Cross,
        CrossPlusOne

        };

        public Sprite IconBodyFace, IconBodyBack;

        public TypeWalk WalkIngType;

        public string Discription;

}