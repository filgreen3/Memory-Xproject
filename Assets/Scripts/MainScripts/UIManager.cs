using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public List<WeaponData> wepTest = new List<WeaponData> ();
    public List<UnitData> untTest = new List<UnitData> ();
    public List<int> WeaponCanUseCount, UnitsCanUseCout;
    public static List<int> G_WeaponCanUseCount;

    public string MisText;
    public Text MissionInfo;

    public LoadingScript LoadManger;

    public Image UnitImage, WeaponImage;
    public Text UnitText, WeaponText;
    public Text[] AllText;



    public UnitBehevior UnitThis, _emptyPrefabOfUnit, SHUnitThis;
    public WeaponBehevior WeaponThis, _emptyPrefabOfWeapon, SHWeaponThis;

    public UnitBehevior[] UnitsCanUse, AllUnitsCanUse;
    public static UnitBehevior[] G_UnitsCanUse;
    public WeaponBehevior[] WeaponCanUse, AllWeaponCanUse;
    public static WeaponBehevior[] G_WeaponCanUse;
    

    public int UnitTurn, WeaponTurn;
    
    public GameObject[] UnitConfirmd = new GameObject[12];

    public int UnitNumber;
    public bool EnemyBool;

    public Image WeaponRareType;
    public GameObject ErorWeapon, ErorUnit;

    ///------------------------------------------------------Shop------------------------------------------------------------------------------\\\;
    public Image SHImage, SWImage;
    public Image SHRareImage;
    public Text SHText, SWText, UCost, WCost;
    private int WeaponID = 0, UnitID = 0;
    private void ShopWeaponInit (int pp) {
        SHWeaponThis = AllWeaponCanUse[pp];

        SHImage.sprite = SHWeaponThis.data.ImageOFGun;
        SHRareImage.color = GetRareColor ((int) SHWeaponThis.data.rareType);
        SHText.text = SHWeaponThis.name + "\n" +
            "Damage: " + SHWeaponThis.Damage.ToString () + "\n" +
            "Ammo: " + SHWeaponThis.MaxLoad.ToString () + "\n" +
            "Optimal Distance: " + SHWeaponThis.MaxDistance.ToString () + "\n" +
            "Price:" + SHWeaponThis.WhatACost () + "\n" +
            "Rare: " + SHWeaponThis.data.rareType.ToString ();

        WCost.text = "Cost: " + SHWeaponThis.Cost.ToString ();
    }
    private void ShopUnitInit (int pp) {
        SHUnitThis = AllUnitsCanUse[pp];

        SWImage.sprite = SHUnitThis.data.IconBodyFace;
        SWText.text = "Name: " + SHUnitThis.name + "\n" +
            "Helty: " + SHUnitThis.data.Helty + "\n" +
            "Shild: " + SHUnitThis.data.Shild + "\n" +
            "Armor: " + SHUnitThis.data.Armor + "\n" +
            "Armor qulity: " + SHUnitThis.data.ArmorQulity + "\n" +
            "Skill for this weapon:" + SHUnitThis.Skills[SHWeaponThis.TypeWeapon];
        UCost.text = "Cost: " + SHUnitThis.Cost.ToString ();
    }

    public void ShopNextWeapon () {
        if (WeaponID < AllWeaponCanUse.Length - 1) {
            WeaponID += 1;
        } else {
            WeaponID = 0;
        }
        ShopUnitInit (UnitID);
        ShopWeaponInit (WeaponID);
    }
    public void ShopPastWeapon () {
        if (WeaponID != 0) {
            WeaponID -= 1;
        } else {
            WeaponID = AllWeaponCanUse.Length - 1;
        }
        ShopUnitInit (UnitID);
        ShopWeaponInit (WeaponID);
    }
    public void ShopNextUnit () {
        if (UnitID < AllUnitsCanUse.Length - 1) {
            UnitID += 1;
        } else {
            UnitID = 0;
        }
        ShopUnitInit (UnitID);
    }
    public void ShopPastUnit () {
        if (UnitID != 0) {
            UnitID -= 1;
        } else {
            UnitID = AllUnitsCanUse.Length - 1;
        }
        ShopUnitInit (UnitID);
    }
    public void ShopBay () {
        Save.PutInInventory (SHWeaponThis.data.name, 1);
        Debug.Log (SHWeaponThis.data.name + ": " + Save.WeaponInvent[WeaponID]);
        Save.Game_Save ();
    }
    public void ShopUnitBay () {
        Save.PutInInventory (SHUnitThis.data.name, 2);
        Debug.Log (SHUnitThis.data.name + ": " + Save.UnitInvent[UnitID]);
        Save.Game_Save ();
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------\\\

    void GlobaliseVoid () {
        G_WeaponCanUse = AllWeaponCanUse;
        G_WeaponCanUseCount = WeaponCanUseCount;
        G_UnitsCanUse = AllUnitsCanUse;

    }

    void LoadDataFromRes () {
        AllUnitsCanUse = new UnitBehevior[Resources.LoadAll<UnitData> ("UnitData").Length];

        int i = 0;
        foreach (UnitData n in Resources.LoadAll<UnitData> ("UnitData")) {
            AllUnitsCanUse[i] = Instantiate (_emptyPrefabOfUnit, Vector3.one * 1000, Quaternion.identity, transform);
            AllUnitsCanUse[i].data = n;
            AllUnitsCanUse[i].ChekDataMetod ();
            AllUnitsCanUse[i].name = AllUnitsCanUse[i].data.name.Remove (0, 2);
            i += 1;
        }

        AllWeaponCanUse = new WeaponBehevior[Resources.LoadAll<WeaponData> ("WeaponData").Length];

        i = 0;
        foreach (WeaponData n in Resources.LoadAll<WeaponData> ("WeaponData")) {
            AllWeaponCanUse[i] = Instantiate (_emptyPrefabOfWeapon, Vector3.one * 1000, Quaternion.identity, transform);
            AllWeaponCanUse[i].data = n;
            AllWeaponCanUse[i].ChekDataMetod ();
            AllWeaponCanUse[i].name = AllWeaponCanUse[i].data.name.Remove (0, 2);
            i += 1;

        }
    }

    public void Start () {
        LoadDataFromRes ();

        ShopWeaponInit (0);
        ShopUnitInit (0);
        GlobaliseVoid ();
        Save.Game_Load ();
        Save.Game_Save ();
    }

    public void OpenMenu (List<UnitData> _unitList, List<WeaponData> _weaponList) {

        transform.GetChild (0).gameObject.SetActive (true);

        UnitsCanUse = new UnitBehevior[_unitList.Count];
        WeaponCanUse = new WeaponBehevior[_weaponList.Count];
        WeaponCanUseCount = new List<int> ();

        for (int i = 0; i < _unitList.Count; i++) {
            WeaponCanUse[i] = Instantiate (_emptyPrefabOfWeapon, Vector3.one * 1000, Quaternion.identity, transform);
            WeaponCanUse[i].data = _weaponList[i];
            WeaponCanUse[i].ChekDataMetod ();
            WeaponCanUseCount.Add (99);
        }

        for (int i = 0; i < _unitList.Count; i++) {
            UnitsCanUse[i] = Instantiate (_emptyPrefabOfUnit, Vector3.one * 1000, Quaternion.identity, transform);
            UnitsCanUse[i].data = _unitList[i];
            UnitsCanUse[i].ChekDataMetod ();
        }

        DontDestroyOnLoad (gameObject);
        UnitThis = Instantiate (UnitsCanUse[UnitTurn], transform);
        WeaponThis = Instantiate (WeaponCanUse[WeaponTurn], transform);

        Refresh ();
    }

    /*
     * 
     * 
     * 
     * 
     * 
     *
     */

    public void MissInit () {

        MissionInfo.text = MisText;
        for (int i = 0; i < untTest.Count; i++) {
            MissionInfo.text += "\n" + untTest[i].name.Remove (0, 2) + " : " + wepTest[i].name.Remove (0, 2) + "\n";
        }
    }
    public void OpenMenu () {
        UnitsCanUse = G_UnitsCanUse;

        WeaponCanUse = G_WeaponCanUse;
        WeaponCanUseCount = new List<int> ();
        for (int i = 0; i < WeaponCanUse.Length; i++) {
            WeaponCanUseCount.Add (Save.GetCoutInventory (WeaponCanUse[i].data.name, 1));

        }
        UnitsCanUse = G_UnitsCanUse;
        UnitsCanUseCout = new List<int> ();
        for (int i = 0; i < UnitsCanUse.Length; i++) {
            UnitsCanUseCout.Add (Save.GetCoutInventory (UnitsCanUse[i].data.name, 2));

        }

        GameObject.DontDestroyOnLoad (this.gameObject);
        UnitThis = Instantiate (UnitsCanUse[UnitTurn], transform);
        UnitThis.transform.position = Vector3.one * 1000;

        WeaponThis = Instantiate (WeaponCanUse[WeaponTurn], transform);
        WeaponThis.transform.position = Vector3.one * 1000;
        Refresh ();

        MissInit ();
    }

    void Refresh () {
        NextObject (true);
        NextObject (false);
        PreviosObject (true);
        PreviosObject (false);
    }

    public void NextObject (bool UnitBool) {
        if (UnitBool) {
            if (UnitThis != null)
                Destroy (UnitThis.gameObject);
            UnitTurn = UnitTurn + 1;
            if (UnitTurn >= UnitsCanUse.Length)
                UnitTurn = 0;

            if (UnitsCanUseCout[UnitTurn] > 0)
                GetUnit ();
            else
                NextObject (true);
        } else {
            if (WeaponThis != null)
                Destroy (WeaponThis.gameObject);

            WeaponTurn = WeaponTurn + 1;
            if (WeaponTurn >= WeaponCanUse.Length)
                WeaponTurn = 0;

            if (WeaponCanUseCount[WeaponTurn] > 0)
                GetWeapon ();
            else
                NextObject (false);

        }
    }

    public void PreviosObject (bool UnitBool) {
        if (UnitBool) {
            if (UnitThis != null)
                Destroy (UnitThis.gameObject);
            UnitTurn = UnitTurn - 1;

            if (UnitTurn < 0)
                UnitTurn = UnitsCanUse.Length - 1;

            if (UnitsCanUseCout[UnitTurn] > 0)
                GetUnit ();
            else
                PreviosObject (true);

        } else {
            if (WeaponThis != null)
                Destroy (WeaponThis.gameObject);
            WeaponTurn = WeaponTurn - 1;
            if (WeaponTurn < 0)
                WeaponTurn = WeaponCanUse.Length - 1;

            if (WeaponCanUseCount[WeaponTurn] > 0)
                GetWeapon ();
            else
                PreviosObject (false);

        }
    }

    void GetUnit () {

        UnitThis = Instantiate<UnitBehevior> (UnitsCanUse[UnitTurn], transform);
        UnitThis.transform.position = Vector3.one * 1000;

        UnitText.text = UnitsCanUse[UnitTurn].name;
        UnitImage.sprite = UnitThis.transform.Find ("Body").GetComponent<SpriteRenderer> ().sprite;
        AllText[0].text = "Helty:" + UnitThis.data.Helty;
        AllText[1].text = "Shild:" + UnitThis.data.Shild;
        AllText[2].text = "Armor:" + UnitThis.data.Armor;
        AllText[3].text = "Armor qulity:" + UnitThis.ArmorQulity;
        AllText[4].text = "Skill for this weapon:" + UnitThis.Skills[WeaponThis.TypeWeapon];
        AllText[9].text = "Unit Cost:" + UnitThis.WhatACost ();

        AllText[10].text = "Unit and Weapon Cost:" + (UnitThis.WhatACost () + WeaponThis.WhatACost ());
        AllText[13].text = UnitThis.data.PercOfUnit.Discription;
        AllText[14].text = "";
        for (int i = 0; i < 6; i++) {
            AllText[14].text += GetWeaponType (i) + " : " + UnitThis.Skills[i] + "\n";
        }
        AllText[15].text = UnitThis.data.Discription;
        AllText[16].text = UnitThis.data.PercOfUnit.name;
        AllText[17].text = "кол-во:" + UnitsCanUseCout[UnitTurn];
    }

    void GetWeapon () {

        WeaponThis = Instantiate (WeaponCanUse[WeaponTurn], transform);
        WeaponThis.transform.position = Vector3.one * 1000;

        WeaponText.text = WeaponCanUse[WeaponTurn].name;
        WeaponImage.sprite = WeaponThis.data.ImageOFGun;
        AllText[4].text = "Skill for this weapon:" + UnitThis.Skills[WeaponThis.TypeWeapon];
        AllText[5].text = "Ammo:" + WeaponThis.MaxLoad;
        AllText[6].text = "Damage:" + WeaponThis.Damage;
        AllText[7].text = "Optimal Distance:" + WeaponThis.NormalDistance.ToString () + "\n" +
            "Max Distance:" + WeaponThis.MaxDistance;
        AllText[8].text = "Weapon Cost:" + WeaponThis.WhatACost () + "\n" +
            "кол-во:" + WeaponCanUseCount[WeaponTurn];
        AllText[12].text = "weapon fire:" + WeaponThis.fireTypeThis;
        AllText[10].text = "Unit and Weapon Cost:" + (UnitThis.WhatACost () + WeaponThis.WhatACost ());
        WeaponRareType.color = GetRareColor ((int) WeaponThis.data.rareType);

    }

    public void NextLevel () {
        if (WeaponCanUseCount[WeaponTurn] != 0) {
            if (UnitNumber > 3) {

                SetEnemy (untTest, wepTest);

                LoadManger.StartLoading (2);
            } else {

                WeaponCanUseCount[WeaponTurn] -= 1;
                UnitsCanUseCout[UnitTurn] -= 1;

                UnitBehevior UnitVar = Instantiate (UnitThis, transform);
                UnitVar.transform.position = Vector3.one * 1000;
                WeaponBehevior WeaponVar = Instantiate (WeaponThis, UnitVar.transform);
                WeaponVar.name = WeaponVar.data.name.Remove (0, 2) + " : " + Random.Range (0, 1001);
                WeaponVar.transform.localPosition = Vector3.zero + Vector3.up * 0.091f;
                WeaponVar.transform.localScale = Vector3.one;
                WeaponVar.transform.eulerAngles = Vector3.zero;
                UnitVar.Weapon = WeaponVar;
                UnitConfirmd[UnitNumber] = UnitVar.gameObject;
                UnitNumber = UnitNumber + 1;
                GetComponent<AudioSource> ().Play ();
                UnitVar.Enemy = false;
                Refresh ();

            }
        }

    }

    public void SetEnemy (List<UnitData> unitlist, List<WeaponData> weaponlist) {
        for (int i = 0; i < unitlist.Count; i++) {
            UnitBehevior UnitVar = Instantiate (UnitThis, transform);
            UnitVar.transform.position = Vector3.one * 1000;
            WeaponBehevior WeaponVar = Instantiate (WeaponThis, UnitVar.transform);
            WeaponVar.name = WeaponVar.data.name.Remove (0, 2) + " : " + Random.Range (0, 1001);
            WeaponVar.transform.localPosition = Vector3.zero + Vector3.up * 0.091f;
            WeaponVar.transform.localScale = Vector3.one;
            WeaponVar.transform.eulerAngles = Vector3.zero;
            UnitVar.Weapon = WeaponVar;

            UnitVar.data = unitlist[i];
            UnitVar.ChekDataMetod ();

            WeaponVar.data = weaponlist[i];
            WeaponVar.ChekDataMetod ();

            UnitVar.Enemy = true;
            UnitConfirmd[UnitNumber] = UnitVar.gameObject;
            UnitNumber = UnitNumber + 1;
        }

    }

    public static string GetWeaponType (int _type) {

        switch (_type) {
            case 0:
                return "Assult Rifle";

            case 1:
                return "Sniper Rifle";

            case 2:
                return "Energy Assult Rifle";

            case 3:
                return "Energy Sniper Rifle";

            case 4:
                return "MachinGun";

            case 5:
                return "Grenade";
            default:
                return "UNKNOWNWEAPON!";

        }

    }

    public static string GetResistanceName (int _type) {

        switch (_type) {
            case 0:
                return "Resitance fire";

            case 1:
                return "Resitance Poison";

            case 2:
                return "Resitance Stun";

            case 3:
                return "Resitance Electric Shok";

            case 4:
                return "MachinGun";

            case 5:
                return "Grenade";
            default:
                return "UNKNOWNWEAPON!";

        }

    }

    Color GetRareColor (int RareType) {
        Color color = Color.white;

        switch (RareType) {
            case 0:
                return Color.white;
            case 1:
                return new Color (0.501f, 1, 0, 1);
            case 2:
                return new Color (0, 0.501f, 1, 1);
            case 3:
                return new Color (0.501f, 0, 1, 1);
            case 4:
                return new Color (1, 0.501f, 0, 1);
            case 5:
                return new Color (1, 0, 1, 1);
            case 6:
                return new Color (0, 0.501f, 1, 1);
            default:
                return Color.white;
        }

    }
}