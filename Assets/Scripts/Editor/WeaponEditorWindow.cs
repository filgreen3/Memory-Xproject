
using UnityEngine;
using UnityEditor;

public class WeaponEditorWindow : EditorWindow
{

    public Vector2 scrollPos;

    public string NameOfItem = "null";
    Sprite ChoosenSprite = null;

    public enum TypeWeapon
    {
        AssultRifle,
        SniperRifle,
        EnergyAssultRifle,
        EnergySniperRifle,
        MachinGun,
        Grenade

    };
    public TypeWeapon _typeWeapon;
    public int Damage, NormalDistance, MaxDistance, ShootPerTurn;
    public int MaxLoad;
    public AudioClip WeaponSound;
    public int Cost;
    public Color BulletColor = Color.white;

    public enum FireType
    {
        Plus,
        Cross
    }
    public FireType fireTypeThis;

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

    public float SecondPerShoot;

    [MenuItem("Window/Create Weapon")]
    public static void ShowWindow()
    {
        AssetDatabase.Refresh();
        GetWindow<WeaponEditorWindow>("Create Weapon");
    }



    void OnGUI()
    {
        WeaponData NewItemData = (WeaponData)CreateInstance("WeaponData");

        GUILayout.Label("Main editor", EditorStyles.boldLabel);
        if (GUILayout.Button("Refresh"))
        {

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);



        GUILayout.Label("Название", EditorStyles.boldLabel);
        NameOfItem = GUILayout.TextField(NameOfItem);
        if (!NameOfItem.StartsWith("W_"))
            EditorGUILayout.HelpBox(" Name must start with 'W_' !", MessageType.Warning);


        ChoosenSprite = (Sprite)EditorGUILayout.ObjectField("Картинка оружия", ChoosenSprite, typeof(Sprite), false);

        _typeWeapon = (TypeWeapon)EditorGUILayout.EnumPopup(_typeWeapon);
        NewItemData.TypeWeapon = (int)_typeWeapon;

        GUILayout.Label("Урон");
        Damage = EditorGUILayout.IntSlider(Damage, 0, 100);
        NewItemData.Damage = Damage;

        GUILayout.Label("Кол-во амуниции");
        MaxLoad = EditorGUILayout.IntSlider(MaxLoad, 1, 20);
        NewItemData.MaxLoad = MaxLoad;

        GUILayout.Label("Дистанция", EditorStyles.boldLabel);

        MaxDistance = EditorGUILayout.IntSlider("Максимальная", MaxDistance, 1, 10);
        NewItemData.MaxDistance = MaxDistance;

        NormalDistance = EditorGUILayout.IntSlider("Отимальная", NormalDistance, 1, 10);
        NewItemData.NormalDistance = NormalDistance;

        ShootPerTurn = EditorGUILayout.IntSlider("Выстрелов за ход", ShootPerTurn, 1, 10);
        NewItemData.BulletPerShoot = ShootPerTurn;

        SecondPerShoot = EditorGUILayout.FloatField("Секунд на выстрел", SecondPerShoot);
        NewItemData.secondPerShoot = SecondPerShoot;

        GUILayout.Label("Остальное", EditorStyles.boldLabel);

        WeaponSound = (AudioClip)EditorGUILayout.ObjectField("Звук выстрела", WeaponSound, typeof(AudioClip), false);
        NewItemData.WeaponSound = WeaponSound;

        BulletColor = EditorGUILayout.ColorField("Цвет шлейфа пули", BulletColor);
        NewItemData.BulletColor = BulletColor;

        rareType = (RareTypeEnum)EditorGUILayout.EnumPopup(rareType);
        NewItemData.rareType = (WeaponData.RareTypeEnum)rareType;


        GUILayout.Box("Cost : " + WhatACost() + "$");


        if (NameOfItem.StartsWith("W_"))
            CreateItem(NewItemData);

        EditorGUILayout.EndScrollView();


    }

    public int WhatACost()
    {
        int n = new int();


        n = n + Damage * 5;
        n = n + MaxLoad * 10;
        n = n + (int)_typeWeapon * 10;

        return n;

    }

    void CreateItem(WeaponData NewItemDataLocal)
    {
        GUILayout.Label("Finish create", EditorStyles.boldLabel);
        if (GUILayout.Button("Create", EditorStyles.miniButton))
        {
            NewItemDataLocal.ImageOFGun = ChoosenSprite;
            AssetDatabase.CreateAsset(NewItemDataLocal, "Assets/Resources/WeaponData/" + NameOfItem + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
