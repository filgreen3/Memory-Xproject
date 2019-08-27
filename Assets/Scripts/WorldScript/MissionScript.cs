using UnityEngine;
using System.Collections.Generic;

public class MissionScript : MonoBehaviour {
    public UnitBehevior[] AllUnitsCanUse;
    public WeaponBehevior[] AllWeaponCanUse;
    public WeaponBehevior _emptyPrefabOfWeapon;
    public UnitBehevior _emptyPrefabOfUnit;

    public UIManager Uimag;

    public List<UnitData> _finallistunit = new List<UnitData>();
    public List<WeaponData> _finallistweapon = new List<WeaponData>();
    public string MisText;


    public void LoadDataFromRes()
    {
        AllUnitsCanUse = new UnitBehevior[Resources.LoadAll<UnitData>("UnitData").Length];




        int i = 0;
        foreach (UnitData n in Resources.LoadAll<UnitData>("UnitData"))
        {
            AllUnitsCanUse[i] = Instantiate(_emptyPrefabOfUnit, Vector3.one * 1000, Quaternion.identity, transform);
            AllUnitsCanUse[i].data = n;
            AllUnitsCanUse[i].ChekDataMetod();
            AllUnitsCanUse[i].name = AllUnitsCanUse[i].data.name.Remove(0, 2);
            i += 1;
        }


        AllWeaponCanUse = new WeaponBehevior[Resources.LoadAll<WeaponData>("WeaponData").Length];

        i = 0;
        foreach (WeaponData n in Resources.LoadAll<WeaponData>("WeaponData"))
        {
            AllWeaponCanUse[i] = Instantiate(_emptyPrefabOfWeapon, Vector3.one * 1000, Quaternion.identity, transform);
            AllWeaponCanUse[i].data = n;
            AllWeaponCanUse[i].ChekDataMetod();
            AllWeaponCanUse[i].name = AllWeaponCanUse[i].data.name.Remove(0, 2);
            i += 1;

        }
    }
    public void StartMission()
    {
        Uimag.wepTest = _finallistweapon;
        Uimag.untTest = _finallistunit;
        Uimag.MisText = MisText;
    }


}
