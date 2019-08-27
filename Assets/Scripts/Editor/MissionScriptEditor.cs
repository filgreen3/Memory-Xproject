using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(MissionScript))]
public class MissionScriptEditor : Editor
{

    #region unit
    UnitBehevior[] _allunit;
    UnitData _thisunit;
    string MissionText;
    int _iunitturn;
    int _unitturn
    {
        get
        {
            return _iunitturn;
        }

        set
        {

            if (value > _allunit.Length - 1)
            {

                _iunitturn = 0;

                return;
            }
            if (value < 0)
            {

                _iunitturn = _allunit.Length - 1;

                return;
            }
            _iunitturn = value;


        }



    }
    string Unitstring;
    #endregion
    #region weapon
    WeaponBehevior[] _allweapon;
    WeaponData _thisweapon;
    int _iweaponturn;
    int _weaponturn
    {
        get
        {
            return _iweaponturn;
        }

        set
        {

            if (value > _allweapon.Length - 1)
            {

                _iweaponturn = 0;

                return;
            }
            if (value < 0)
            {

                _iweaponturn = _allweapon.Length - 1;

                return;
            }
            _iweaponturn = value;


        }



    }
    string Weaponstring;
    #endregion

    bool ShowList;
    MissionScript ThisScript;



    public override void OnInspectorGUI()
    {
        ThisScript = (MissionScript)target;



        if (ThisScript.AllUnitsCanUse.Length < 1 || ThisScript.AllWeaponCanUse.Length < 1)
        {
            if (ThisScript._emptyPrefabOfUnit != null && ThisScript._emptyPrefabOfWeapon != null) { ThisScript.LoadDataFromRes(); }

            base.OnInspectorGUI();

        }
        else
            ChooseEnemy(ThisScript.AllWeaponCanUse, ThisScript.AllUnitsCanUse);
    }

    private void ChooseEnemy(WeaponBehevior[] weaponArray, UnitBehevior[] unitArray)
    {
        _allweapon = weaponArray;
        _allunit = unitArray;

        GUI.Box(new Rect(EditorGUILayout.GetControlRect().position + Vector2.left * 10, new Vector2(EditorGUIUtility.currentViewWidth - 32, 215)), " ", EditorStyles.helpBox);



        #region unit
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("<", GUILayout.Width(25), GUILayout.Height(85)))
            SetUnit(++_unitturn);

        GUILayout.Box("Name: " + Unitstring + "\n" +
            "Helty: " + _allunit[_unitturn].data.Helty + "\n" +
            "Shild: " + _allunit[_unitturn].data.Shild + "\n" +
            "Armor: " + _allunit[_unitturn].data.Armor + "\n" +
            "Armor qulity: " + _allunit[_unitturn].data.ArmorQulity + "\n" +
            "Skill for this weapon:" + _allunit[_unitturn].Skills[_allweapon[_weaponturn].TypeWeapon] + "\n"
        , GUILayout.Width(EditorGUIUtility.currentViewWidth - 200), GUILayout.Height(85));


        GUILayout.Box(_allunit[_unitturn].data.IconBodyFace.texture, GUILayout.Width(85), GUILayout.Height(85));

        if (GUILayout.Button(">", GUILayout.Width(25), GUILayout.Height(85)))
            SetUnit(--_unitturn);

        GUILayout.EndHorizontal();

        #endregion

        #region weapon
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("<", GUILayout.Width(25), GUILayout.Height(85)))
            SetWeapon(++_weaponturn);


        GUILayout.Box("Name: " + Weaponstring + "\n" +
            "Ammo: " + _allweapon[_weaponturn].MaxLoad + "\n" +
            "Damage: " + _allweapon[_weaponturn].Damage + "\n" +
            "Optimal Distance: " + _allweapon[_weaponturn].NormalDistance + "\n" +
            "Max Distance: " + _allweapon[_weaponturn].data.MaxDistance + "\n" +
            "Weapon fire: " + _allweapon[_weaponturn].fireTypeThis + "\n"
        , GUILayout.Width(EditorGUIUtility.currentViewWidth - 200), GUILayout.Height(85));


        GUILayout.Box(_allweapon[_weaponturn].data.ImageOFGun.texture, GUILayout.Width(85), GUILayout.Height(85));

        if (GUILayout.Button(">", GUILayout.Width(25), GUILayout.Height(85)))
            SetWeapon(--_weaponturn);

        GUILayout.EndHorizontal();



        #endregion
        if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.Width(EditorGUIUtility.currentViewWidth - 52)) && _thisunit != null && _thisweapon != null)
        {
            if (ThisScript._finallistunit.Count < 6)
            {
                ThisScript._finallistunit.Add(_thisunit);
                ThisScript._finallistweapon.Add(_thisweapon);
                ThisScript.MisText = MissionText;

            }
            else
            {
                Debug.Log("Max Size");
            }
        }

        GUILayout.Label("Описание : ", EditorStyles.boldLabel);
        if (MissionText == string.Empty && ThisScript.MisText != string.Empty)
            MissionText = ThisScript.MisText;
        MissionText = GUILayout.TextArea(ThisScript.MisText, GUILayout.Width(EditorGUIUtility.currentViewWidth - 52));
        ThisScript.MisText = MissionText;


        ShowList = EditorGUILayout.Foldout(ShowList, "List");
        if (ShowList)
        {

            for (int i = 0; i < ThisScript._finallistunit.Count; i++)
            {
                string _localstring = "";
                _localstring += ThisScript._finallistunit[i].name;
                _localstring += " ";
                _localstring += ThisScript._finallistweapon[i].name;
                _localstring += "\n";


                GUILayout.BeginHorizontal();

                GUILayout.Box(_localstring, GUILayout.Width(EditorGUIUtility.currentViewWidth - 84));

                if (GUILayout.Button("X", GUILayout.Width(32), GUILayout.Height(32)))
                {
                    ThisScript._finallistunit.RemoveAt(i);
                    ThisScript._finallistweapon.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

            }
        }
    }

    void SetUnit(int UnitTurn)
    {
        Unitstring = _allunit[_unitturn].data.name.Remove(0, 2);
        _thisunit = _allunit[_unitturn].data;
    }
    void SetWeapon(int WeaponTurn)
    {

        Weaponstring = _allweapon[_weaponturn].data.name.Remove(0, 2);
        _thisweapon = _allweapon[_weaponturn].data;
    }



}
