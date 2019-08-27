
using UnityEngine;
using UnityEditor;


public class UnitEditorClass : EditorWindow
{


    Vector2 scrollPos;

    #region perc
    public enum TypeParams
    {
        ChengingParams,
        Spell,
        Granade
    }
    public TypeParams _typeParams;

    public enum TypeTarget
    {
        Self,
        Enemy,
        Friend
    }
    public TypeTarget _typeTarget;


    public enum TypeTargetParametr
    {
        Helty,
        Shild,
        Armor
    }
    public TypeTargetParametr _typeTargetParametr;

    public enum TypeSpell
    {
        Fire,
        Poisen,
        Stun,
        ElectricShock,
        Heal
    }
    public TypeSpell _typeSpell;


    float a = 0;
    public string NameOfItem = "null";
    bool ShowEditoe = false;
    Sprite ChoosenSprite = null;
    int HitPower = 0;
    int Duration = 0;
    int TimeToReuse = 0;
    #endregion
    #region unit

    bool unitShower = false, mainShower = false;

    public Color ColorOfFraction;

    public int MaxWalk;

    public string _discription;

    public float Helty, Shild, Armor;

    public int ArmorQulity;

    Sprite ChoosenSpriteSecond = null;

    public int[] Skills = new int[6];

    public int[] Resistans = new int[4];

    public PercData _percData;

    public enum TypeWalk
    {
        Plus,
        Cross,
        CrossPlusOne


    };
    public Sprite IconBody;

    public TypeWalk WalkIngType;

    #endregion


    [MenuItem("Window/Create Unit")]
    public static void ShowWindow()
    {
        AssetDatabase.Refresh();
        GetWindow<UnitEditorClass>("Create Element");
    }



    void OnGUI()
    {



        GUILayout.Label("Main editor", EditorStyles.boldLabel);
        if (GUILayout.Button("Refresh"))
        {

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        ShowEditoe = EditorGUILayout.Foldout(ShowEditoe, "Perc Editor Menu");
        if (ShowEditoe)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            PercData NewItemData = (PercData)CreateInstance("PercData");
            GUILayout.Box("Main Parametrs");
            GUILayout.Label("Name", EditorStyles.boldLabel);
            NameOfItem = GUILayout.TextField(NameOfItem);


            GUILayout.Label("Image", EditorStyles.boldLabel);
            ChoosenSprite = (Sprite)EditorGUILayout.ObjectField("Image", ChoosenSprite, typeof(Sprite), false);

            _typeParams = (TypeParams)EditorGUILayout.EnumPopup(_typeParams);
            NewItemData.A_param = (int)_typeParams;
            switch (_typeParams)
            {

                case TypeParams.ChengingParams:

                    EditorGUILayout.BeginHorizontal();

                    _typeTarget = (TypeTarget)EditorGUILayout.EnumPopup("Target", _typeTarget);
                    NewItemData.BA_param = (int)_typeTarget;
                    ChoosePatametrTarget(NewItemData);
                    EditorGUILayout.Space();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    HitPower = EditorGUILayout.IntField((_typeTarget == TypeTarget.Friend || _typeTarget == TypeTarget.Self) ? "Add" : "Minus", HitPower);
                    HitPower = EditorGUILayout.IntSlider(HitPower, 0, 200);
                    NewItemData.PowerOfPerc = (_typeTarget == TypeTarget.Friend || _typeTarget == TypeTarget.Self) ? -HitPower : HitPower;
                    EditorGUILayout.EndHorizontal();







                    break;
                case TypeParams.Spell:
                    GUILayout.Label("Spell");

                    _typeTarget = (TypeTarget)EditorGUILayout.EnumPopup("Target", _typeTarget);
                    NewItemData.BA_param = (int)_typeTarget;



                    _typeSpell = (TypeSpell)EditorGUILayout.EnumPopup("Effect", _typeSpell, EditorStyles.popup);
                    switch (_typeSpell)
                    {
                        case TypeSpell.Fire:
                            NewItemData.СA_param = 0;
                            break;
                        case TypeSpell.Poisen:
                            NewItemData.СA_param = 1;
                            break;
                        case TypeSpell.Stun:
                            NewItemData.СA_param = 2;
                            break;
                        case TypeSpell.ElectricShock:
                            NewItemData.СA_param = 3;
                            break;
                        case TypeSpell.Heal:
                            NewItemData.СA_param = 4;
                            break;
                    }

                    if (_typeSpell != TypeSpell.Stun)
                        HitPower = EditorGUILayout.IntSlider("Power", HitPower, 0, 200);

                    if (_typeSpell != TypeSpell.ElectricShock&& _typeSpell != TypeSpell.Heal)

                        Duration = EditorGUILayout.IntSlider("Duration", Duration, 0, 10);


                    NewItemData.PowerOfPerc = HitPower;
                    NewItemData.DuratinOfPerc = Duration;

                    break;
                case TypeParams.Granade:
                    GUILayout.Label("Granade");

                    _typeSpell = (TypeSpell)EditorGUILayout.EnumPopup("Effect", _typeSpell, EditorStyles.popup);
                    switch (_typeSpell)
                    {
                        case TypeSpell.Fire:
                            NewItemData.СA_param = 3;
                            break;
                        case TypeSpell.Poisen:
                            NewItemData.СA_param = 1;
                            break;
                        case TypeSpell.Stun:
                            NewItemData.СA_param = 2;
                            break;
                        case TypeSpell.ElectricShock:
                            NewItemData.СA_param = 4;
                            break;
                        case TypeSpell.Heal:
                            NewItemData.СA_param = 0;
                            break;
                    }

                    if (_typeSpell != TypeSpell.Stun)
                        HitPower = EditorGUILayout.IntSlider("Power", HitPower, 0, 200);
                        Duration = EditorGUILayout.IntSlider("Size", Duration, 1, 3);
                    


                    NewItemData.PowerOfPerc = HitPower;
                    NewItemData.DuratinOfPerc = Duration;
                    break;

            }

            TimeToReuse = EditorGUILayout.IntSlider("Duration of reset", TimeToReuse, 0, 10);
            NewItemData.TimeToReuse = TimeToReuse;


            GUILayout.Label("Discription", EditorStyles.boldLabel);
            _discription = GUILayout.TextField(_discription, EditorStyles.textArea);
            NewItemData.Discription = _discription;



            if (NameOfItem != null)
            {
                CreateItem(NewItemData);
            }
            EditorGUILayout.EndScrollView();
        }


        unitShower = EditorGUILayout.Foldout(unitShower, "Unit Editor Menu");
        if (unitShower)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            UnitData NewItemData = (UnitData)CreateInstance("UnitData");
            NameOfItem = GUILayout.TextField(NameOfItem);
            if (!NameOfItem.StartsWith("U_"))
                EditorGUILayout.HelpBox(" Name must start with 'U_' !", MessageType.Warning);

            GUILayout.BeginHorizontal();
            ChoosenSprite = (Sprite)EditorGUILayout.ObjectField( "Face/Back",ChoosenSprite, typeof(Sprite), false);
            ChoosenSpriteSecond = (Sprite)EditorGUILayout.ObjectField("",ChoosenSpriteSecond, typeof(Sprite), false);
            GUILayout.EndHorizontal();



            GUILayout.Box("Main Parametrs");


            Helty = EditorGUILayout.FloatField("Helty", Helty, "flow node 6", GUILayout.Height(25), GUILayout.Width(Helty + 160));
            Helty = EditorGUILayout.Slider(Helty, 0, 400);
            Helty = Helty - (Helty % 5);
            NewItemData.Helty = Helty;

            Shild = EditorGUILayout.FloatField("Shild", Shild, "flow node 2", GUILayout.Height(25), GUILayout.Width(Shild + 160));
            Shild = EditorGUILayout.Slider(Shild, 0, 400);
            Shild = Shild - (Shild % 5);
            NewItemData.Shild = Shild;

            Armor = EditorGUILayout.FloatField("Armor", Armor, "flow node 5", GUILayout.Height(25), GUILayout.Width(Armor + 160));
            Armor = EditorGUILayout.Slider(Armor, 0, 400);
            Armor = Armor - (Armor % 5);
            NewItemData.Armor = Armor;

            if (Helty == 0)
                EditorGUILayout.HelpBox(" Helty can't be zero !", MessageType.Warning);
            else
            {
                EditorGUILayout.HelpBox("", MessageType.None);
            }


            EditorGUILayout.Space();
            ArmorQulity = EditorGUILayout.IntField("Armor Qulity", ArmorQulity);
            ArmorQulity = EditorGUILayout.IntSlider(ArmorQulity, 1, 10);
            NewItemData.ArmorQulity = ArmorQulity;


            MaxWalk = EditorGUILayout.IntField("Max walk", MaxWalk);
            MaxWalk = EditorGUILayout.IntSlider(MaxWalk, 1, 10);
            NewItemData.MaxWalk = MaxWalk;


          

            EditorGUILayout.Space();
            mainShower = EditorGUILayout.Foldout(mainShower, "Skill editor menu");
            if (mainShower)
            {
                Skills[0] = EditorGUILayout.IntField("Assult rifle", Skills[0]);
                Skills[0] = EditorGUILayout.IntSlider(Skills[0], 0, 50);
                NewItemData.Skills[0] = Skills[0];

                Skills[1] = EditorGUILayout.IntField("Energy Assult rifle", Skills[1]);
                Skills[1] = EditorGUILayout.IntSlider(Skills[1], 0, 50);
                NewItemData.Skills[1] = Skills[1];

                Skills[2] = EditorGUILayout.IntField("Sniper rifle", Skills[2]);
                Skills[2] = EditorGUILayout.IntSlider(Skills[2], 0, 50);
                NewItemData.Skills[2] = Skills[2];

                Skills[3] = EditorGUILayout.IntField("Energy sniper rifle", Skills[3]);
                Skills[3] = EditorGUILayout.IntSlider(Skills[3], 0, 50);
                NewItemData.Skills[3] = Skills[3];

                Skills[4] = EditorGUILayout.IntField("Machin gun", Skills[4]);
                Skills[4] = EditorGUILayout.IntSlider(Skills[4], 0, 50);
                NewItemData.Skills[4] = Skills[4];

                Skills[5] = EditorGUILayout.IntField("Genader", Skills[5]);
                Skills[5] = EditorGUILayout.IntSlider(Skills[5], 0, 50);
                NewItemData.Skills[5] = Skills[5];

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Resitance");
                Resistans[0] = EditorGUILayout.IntSlider("Fire", Resistans[0], 0, 100);
                NewItemData.Resistans[0] = Resistans[0];


                Resistans[1] = EditorGUILayout.IntSlider("Poison", Resistans[1], 0, 100);
                NewItemData.Resistans[1] = Resistans[1];


                Resistans[2] = EditorGUILayout.IntSlider("Stun", Resistans[2], 0, 100);
                NewItemData.Resistans[2] = Resistans[2];

                Resistans[3] = EditorGUILayout.IntSlider("Electric Shok", Resistans[3], 0, 100);
                NewItemData.Resistans[3] = Resistans[3];


            }



            EditorGUILayout.Space();
            _percData = (PercData)EditorGUILayout.ObjectField("Perc", _percData, typeof(PercData), false);
            NewItemData.PercOfUnit = _percData;

            GUILayout.Label("Discription", EditorStyles.boldLabel);
            _discription = GUILayout.TextField(_discription, EditorStyles.textArea);
            NewItemData.Discription = _discription;

            GUILayout.Box("Cost : " + WhatACost().ToString() + "$");


            if (NameOfItem.StartsWith("U_") && Helty != 0)
            {
                CreateItem(NewItemData);
            }
            EditorGUILayout.EndScrollView();
        }
    }


    void ChoosePatametrTarget(PercData DataStore)
    {
        _typeTargetParametr = (TypeTargetParametr)EditorGUILayout.EnumPopup("Target parametr", _typeTargetParametr, EditorStyles.popup);
        switch (_typeTargetParametr)
        {
            case TypeTargetParametr.Helty:
                DataStore.СA_param = 0;
                break;
            case TypeTargetParametr.Shild:
                DataStore.СA_param = 1;
                break;
            case TypeTargetParametr.Armor:
                DataStore.СA_param = 2;
                break;
        }
    }

    int WhatACost()
    {
        int n = new int();

        foreach (int i in Skills)
        {
            n = n + i;
        }
        n = n + (int)Helty;
        n = n + (int)Shild * 2;
        n = n + (int)Armor * ArmorQulity;
        n = n + MaxWalk * 10;
        return n;

    }


    void CreateItem(PercData NewItemDataLocal)
    {
        GUILayout.Label("Finish create", EditorStyles.boldLabel);
        if (GUILayout.Button("Create", EditorStyles.miniButton))
        {
            NewItemDataLocal.Icon = ChoosenSprite;
            AssetDatabase.CreateAsset(NewItemDataLocal, "Assets/Resources/PercData/" + NameOfItem + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    void CreateItem(UnitData NewItemDataLocal)
    {
        GUILayout.Label("Finish create", EditorStyles.boldLabel);
        if (GUILayout.Button("Create", EditorStyles.miniButton))
        {
            NewItemDataLocal.IconBodyFace = ChoosenSprite;
            NewItemDataLocal.IconBodyBack = ChoosenSpriteSecond;
            AssetDatabase.CreateAsset(NewItemDataLocal, "Assets/Resources/UnitData/" + NameOfItem + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }


}


