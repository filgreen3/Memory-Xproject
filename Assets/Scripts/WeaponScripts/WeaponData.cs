using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject {


    public int TypeWeapon;
    public int Damage, NormalDistance, MaxDistance;
    public int MaxLoad;
    public AudioClip WeaponSound;
    public int BulletPerShoot;
    public float secondPerShoot;
    public Color BulletColor;
    public enum FireType
    {
        Plus,
        Cross
    }
    public FireType fireTypeThis;
    public Sprite ImageOFGun;
    public enum RareTypeEnum
    {
        UnRare,
        LowRare,
        Rare,
        HigeRare,
        Legendary,
        Legendar
    }
    public RareTypeEnum rareType;

    public int RareInt;

}
