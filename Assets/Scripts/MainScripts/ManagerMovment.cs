using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManagerMovment : MonoBehaviour {

    public bool AttackControl = false, MoveControl = false, PercUse = false;
    public int ViewFeild;

    [Header ("Main")]
    public LayerMask Mask;
    public AudioSource audioSource;
    public Vector3 PoitionToChek;
    public GridABS GridABSManager;
    public UnitBehevior[] AllUnits;
    public UnitBehevior CorrectUnit;
    public List<UnitBehevior> ListOfGo, FriendList, EnemyList;
    public int Turn;
    public Transform CameraFollow;
    [Header ("Bar")]
    public GameObject HeltyBar, ShildBar, ArmorBar, ShootBar;
    public Text AmmoText, PercText, HeltyBText, ShildBText, ArmorBText, NameText, StatsOfUnit, ShootBarText;
    public GameObject[] UnitsGameObject;
    public TextMeshPro InfoText;
    public Color[] ColorOfIfo = new Color[4];
    Vector3 NeedMoveVectorCam = Vector3.zero;
    public List<GameObject> Junk = new List<GameObject> ();
    int _indexrot = 0;
    public Vector3 mosepose;
    [Header ("Effeact")]
    public GameObject FireFX, StunFX, PoisnFX, ElectricShockFX, ShildFx;
    public GameObject[] OnTurnEffectFX;
    public AudioClip NeedReloadAudioClip, ReloadSound, Expl;

    Camera TerrainCam;

    [Header ("NextTurnParams")]
    public Text NextTurnText;
    public Image NextTurnIcon, NextTurnBackImage;
    public GameObject unitShowNextTurn;
    public Toggle toggle;
    [Header ("MarcerCorrectUnit")]
    public GameObject unitShowCorrectUnit;

    public void Awake () {
        TerrainCam = Camera.main.transform.GetChild (3).GetComponent<Camera> ();
        audioSource = GetComponent<AudioSource> ();
        UnitsGameObject = FindObjectOfType<UIManager> ().UnitConfirmd;
        int x = 0;
        int y = 0;
        int enmpos = 1;
        for (int i = 0; i < UnitsGameObject.Length; i++) {
            if (UnitsGameObject[i] != null) {
                x += 1;
                GameObject n = UnitsGameObject[i];
                y = UnitsGameObject[i].GetComponent<UnitBehevior> ().Enemy ? Random.Range (-4, -3) : Random.Range (4, 6);
                if (!UnitsGameObject[i].GetComponent<UnitBehevior> ().Enemy) {

                    GameObject local;

                    local = Instantiate (n, Vector3.back * x + Vector3.right * y, Quaternion.identity);

                    n.GetComponent<UnitBehevior> ().enabled = true;

                    Destroy (n);
                    local.GetComponent<UnitBehevior> ().Invoke ("StartBattle", 0.1f);
                } else {
                    if (false) {

                        GameObject local;
                        UnitData localData;
                        UnitAI localunitAI;
                        WeaponBehevior localunitWeapon;

                        local = Instantiate (n, Vector3.back * enmpos + Vector3.right * y, Quaternion.identity);

                        localunitWeapon = n.GetComponent<UnitBehevior> ().Weapon;
                        localData = n.GetComponent<UnitBehevior> ().data;
                        Destroy (local.GetComponent<UnitBehevior> ());
                        localunitAI = local.AddComponent<UnitAI> ();
                        localunitAI.Weapon = Instantiate (localunitWeapon, localunitAI.transform);

                        localunitAI.Marker = n.GetComponent<UnitBehevior> ().Marker;
                        localunitAI.data = localData;
                        localunitAI.ChekDataMetod ();
                        localunitAI.Enemy = true;

                        enmpos++;

                        Destroy (n);
                        localunitAI.Invoke ("StartBattle", 0.1f);
                    }
                }
            }

        }

        Destroy (FindObjectOfType<UIManager> ().gameObject);

        CreateLIst ();

        AmmoText.text = (CorrectUnit.Weapon.Load + "/" + CorrectUnit.Weapon.MaxLoad);
        PercText.text = ((CorrectUnit.TimeToReusePerc < 1) ? " Redy !" : ("  Wait " + CorrectUnit.TimeToReusePerc));
        NewPointedUnit (CorrectUnit);
    }

    void LateUpdate () {

        mosepose = Input.mousePosition;

        if (MoveControl && !CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2) &&
            !EventSystem.current.IsPointerOverGameObject ()) {
            if (Input.GetKeyDown (KeyCode.Mouse0)) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast (ray, out hit, 5000, Mask)) {
                    Vector3 PoitionToChek;
                    PoitionToChek = hit.point;
                    var node = GridABSManager.NodeFromWorldPoint (PoitionToChek);

                    if (GridABSManager.WalkableArea.Contains(node) && node !=
                        GridABSManager.NodeFromWorldPoint (CorrectUnit.transform.position)) {

                        GridABSManager.ClearNode (CorrectUnit.transform.position);
                        StartCoroutine (CorrectUnit.LerpMove (node.Position + Vector3.forward * 0.12f + Vector3.up * 0.34f));

                        CorrectUnit.transform.eulerAngles = new Vector3 (20, -45, 0);
                        GridABSManager.ClearNode (node.Position + Vector3.forward * 0.12f + Vector3.up * 0.34f, CorrectUnit);
                        GridABSManager.CleatGrid ();

                    }
                }

            }

        }
        if (!CorrectUnit.Enemy) {
            NeedMoveVectorCam = Vector3.zero;
            if (Input.GetKeyDown (KeyCode.Alpha1))
                ChengBoolMoveControl ();
            if (Input.GetKeyDown (KeyCode.Alpha2))
                ChengBoolAttackControl ();
            if (Input.GetKeyDown (KeyCode.Alpha3))
                ChengBoolPercUseControl ();
            if (Input.GetKeyDown (KeyCode.R))
                ReloadUnitWeapon ();
        }

    }

    void FixedUpdate () {

        Vector3 NeedMoveVectorCam = Vector3.zero;

        if (Input.GetKey (KeyCode.W))
            CameraFollow.transform.Translate (-0.1f, 0, 0.1f);
        if (Input.GetKey (KeyCode.D))
            CameraFollow.transform.Translate (0.1f, 0, 0.1f);
        if (Input.GetKey (KeyCode.A))
            CameraFollow.transform.Translate (-0.1f, 0, -0.1f);
        if (Input.GetKey (KeyCode.S))
            CameraFollow.transform.Translate (0.1f, 0, -0.1f);

        if (!Input.GetKey (KeyCode.LeftAlt)) {
            if (Input.GetAxis ("Mouse ScrollWheel") != 0 && (TerrainCam.orthographicSize > 1 || Input.GetAxis ("Mouse ScrollWheel") < 0)) {
                Camera.main.orthographicSize -= (Input.GetAxis ("Mouse ScrollWheel") * (Camera.main.transform.position.y * 0.3f));
                TerrainCam.orthographicSize -= (Input.GetAxis ("Mouse ScrollWheel") * (Camera.main.transform.position.y * 0.3f));
            }
        } else {
            if (Input.GetAxis ("Mouse ScrollWheel") != 0 && (Camera.main.transform.eulerAngles.x > 25 || Input.GetAxis ("Mouse ScrollWheel") < 0) && (Camera.main.transform.eulerAngles.x < 90 || Input.GetAxis ("Mouse ScrollWheel") > 0)) {
                Camera.main.transform.eulerAngles -= new Vector3 (Input.GetAxis ("Mouse ScrollWheel") * 10, 0, 0);
            }

        }

        CameraFollow.transform.position = CameraFollow.transform.position + NeedMoveVectorCam;

    }
    #region show text
    public void ShowInfoText (float _Procency, Vector3 PositionSelf) {
        TextMeshPro meshPro = Instantiate (InfoText, PositionSelf + Vector3.back * 0.15f + Vector3.up * 1.2f + Vector3.left * 0.3f, Quaternion.identity);
        meshPro.transform.eulerAngles = CorrectUnit.transform.eulerAngles;
        meshPro.text = ((int) (_Procency * 100)).ToString () + "%";
        meshPro.color = new Color (1, 0.43f, 0);
        Junk.Add (meshPro.gameObject);
    }
    public void ShowInfoText (int _HitInt, Color _Color, Vector3 PositionSelf) {
        TextMeshPro meshPro = Instantiate (InfoText, PositionSelf + 2 * Vector3.up + (Vector3.right + Vector3.forward) * (Random.Range (-11, 11) / 50f), Quaternion.identity);
        meshPro.transform.eulerAngles = CorrectUnit.transform.eulerAngles;
        meshPro.GetComponent<UpInfo> ().HitInfoMetod ();

        if (_HitInt != 0)
            meshPro.text = "-" + _HitInt.ToString ();
        else
            meshPro.text = "miss";
        meshPro.color = _Color;
    }
    public void ShowInfoText (string _HitInfo, Color _Color, Vector3 PositionSelf) {
        TextMeshPro meshPro = Instantiate (InfoText, PositionSelf + 2 * Vector3.up, Quaternion.identity);
        meshPro.transform.eulerAngles = CorrectUnit.transform.eulerAngles;
        meshPro.GetComponent<UpInfo> ().HitInfoMetod ();
        meshPro.color = _Color;
        meshPro.text = _HitInfo;

    }
    #endregion

    public void ChengBoolAttackControl () {

        if (!CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2)) {
            if (CorrectUnit.Weapon.Load > 0) {
                GridABSManager.CleatGrid ();
                AttackControl = !AttackControl;
                if (AttackControl)
                    CorrectUnit.Attacking ();
                MoveControl = false;
                PercUse = false;
            } else {
                ShowInfoText ("No ammo", Color.white, CorrectUnit.transform.position);
                PlaySpecific (NeedReloadAudioClip);
            }
        }
    }

    public void ChengBoolPercUseControl () {
        if (!CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2)) {
            if (CorrectUnit.TimeToReusePerc < 1) {
                GridABSManager.CleatGrid ();
                AttackControl = false;

                PercUse = !PercUse;
                MoveControl = false;
                if (PercUse)
                    CorrectUnit.PercUse ();
            } else {
                ShowInfoText ("perc not reload", Color.white, CorrectUnit.transform.position);
                PlaySpecific (NeedReloadAudioClip);
            }
        }
    }

    public void ChengBoolMoveControl () {
        if (!CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2)) {

            GridABSManager.CleatGrid ();

            MoveControl = !MoveControl;
            if (MoveControl)
                CorrectUnit.Moving ();
            AttackControl = false;
            PercUse = false;

        }

    }

    public void Ambush () {
        if (!CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2) && CorrectUnit.candosomething) {
            ShowInfoText ("Overwatch", Color.white, CorrectUnit.transform.position);
            CorrectUnit.AmbushZone = true;
            RetarderCall (0.4f);
        }
    }

    public void ReloadUnitWeapon () {
        if (!CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2) && CorrectUnit.candosomething) {
            if (CorrectUnit.Weapon.Load < CorrectUnit.Weapon.MaxLoad) {
                CorrectUnit.Weapon.Reload ();
                AmmoText.text = (CorrectUnit.Weapon.Load + "/" + CorrectUnit.Weapon.MaxLoad);
                RetarderCall (0.2f);
            } else {
                ShowInfoText ("Max ammo", Color.white, CorrectUnit.transform.position);
                PlaySpecific (NeedReloadAudioClip);
            }
        }
    }

    public void CleanJunk () {
        foreach (GameObject n in Junk) {
            Destroy (n);

        }
        Junk.Clear ();
    }
    public void CreateLIst () {
        ListOfGo = new List<UnitBehevior> ();
        AllUnits = FindObjectsOfType<UnitBehevior> ();
        FriendList = new List<UnitBehevior> ();
        EnemyList = new List<UnitBehevior> ();

        for (int i = 0; i < AllUnits.Length; i++) {
            if (!AllUnits[i].Enemy) {
                FriendList.Add (AllUnits[i]);

            } else {
                EnemyList.Add (AllUnits[i]);

            }

        }
        int F = 0, E = 0;
        while (ListOfGo.Count <= AllUnits.Length - 1) {
            if (F < FriendList.Count) {
                ListOfGo.Add (FriendList[F]);

                F = F + 1;
            }
            if (E < EnemyList.Count) {
                ListOfGo.Add (EnemyList[E]);

                E = E + 1;
            }
        }
        CorrectUnit = ListOfGo[0];
    }

    public void NewPointedUnit (UnitBehevior _UnitPointed) {

        HeltyBar.transform.localScale = new Vector3 (_UnitPointed.Helty / _UnitPointed.MaxHelty, 1, 1);
        if (_UnitPointed.Shild > 0)
            ShildBar.transform.localScale = new Vector3 (_UnitPointed.Shild / _UnitPointed.MaxShild, 1, 1);
        if (_UnitPointed.Armor > 0)
            ArmorBar.transform.localScale = new Vector3 (_UnitPointed.Armor / _UnitPointed.MaxArmor, 1, 1);
        NameText.text = _UnitPointed.data.name.Remove (0, 2);

        HeltyBText.text = ((int) _UnitPointed.Helty).ToString ();
        ShildBText.text = ((int) _UnitPointed.Shild).ToString ();
        ArmorBText.text = ((int) _UnitPointed.Armor).ToString ();

        PercText.text = ((_UnitPointed.TimeToReusePerc < 1) ? "Redy !" : ("Wait " + _UnitPointed.TimeToReusePerc));
        string _statsunit = "";

        _statsunit += "- Навык владения оружеем  -" + "\n" + "\n";

        int num = 0;
        foreach (int n in _UnitPointed.Skills) {
            _statsunit += UIManager.GetWeaponType (num) + " : " + n + "\n";
            num++;
        }

        _statsunit += "\n" + "\n" + "- Сопротивление -" + "\n" + "\n";

        num = 0;
        foreach (int n in _UnitPointed.Resistans) {
            _statsunit += UIManager.GetResistanceName (num) + " : " + n + "\n";
            num++;
        }

        StatsOfUnit.text = _statsunit;

    }

    public void EndTurn () {
        CorrectUnit.TimeToReusePerc = CorrectUnit.TimeToReusePerc - 1;
        AttackControl = false;
        MoveControl = false;
        PercUse = false;
        CorrectUnit.candosomething = true;

        Turn++;
        if (Turn >= ListOfGo.Count)
            Turn = 0;

        CorrectUnit = ListOfGo[Turn];
        NewPointedUnit (CorrectUnit);

        AmmoText.text = (CorrectUnit.Weapon.Load + "/" + CorrectUnit.Weapon.MaxLoad);
        PercText.text = ((CorrectUnit.TimeToReusePerc < 1) ? "Redy !" : ("Wait " + CorrectUnit.TimeToReusePerc));

        CameraFollow.transform.position = new Vector3 (CorrectUnit.transform.position.x + Camera.main.transform.position.y / 1.5f, 0, CorrectUnit.transform.position.z - Camera.main.transform.position.y / 1.5f) + new Vector3 (0, Camera.main.transform.position.y, 0);

        unitShowCorrectUnit.SetActive (true);
        unitShowCorrectUnit.transform.position = GridABSManager.NodeFromWorldPoint (CorrectUnit.transform.position).Position;

        CleanJunk ();
        GridABSManager.CleatGrid ();

        CorrectUnit.AmbushZone = false;

        if (Turn + 1 < ListOfGo.Count) {
            NextTurnBackImage.color = (!ListOfGo[Turn + 1].Enemy) ? new Color (0, 244 / 255f, 1) : new Color (1, 30 / 255f, 0);
            NextTurnIcon.sprite = ListOfGo[Turn + 1].data.IconBodyFace;
            NextTurnText.text = "Next turn :" + "\n" + ListOfGo[Turn + 1].data.name.Remove (0, 2);
            unitShowNextTurn.transform.position = ListOfGo[Turn + 1].transform.position;
        } else {
            NextTurnBackImage.color = (!ListOfGo[0].Enemy) ? new Color (0, 244 / 255f, 1) : new Color (1, 30 / 255f, 0);
            NextTurnIcon.sprite = ListOfGo[0].data.IconBodyFace;
            NextTurnText.text = "Next turn :" + "\n" + ListOfGo[0].data.name.Remove (0, 2);
            unitShowNextTurn.transform.position = ListOfGo[0].transform.position;
        }

        if (CorrectUnit.Enemy && !CorrectUnit._effectOnUnit.HaveEffect (2)) {

            CorrectUnit.GetComponent<UnitAI> ().ChooseTactic ();

        }
        if (CorrectUnit._effectOnUnit.HaveEffect (2))
            RetarderCall (1f);
        CorrectUnit.candosomething = true;
        CorrectUnit._effectOnUnit.CheckEffects ();

    }

    public void RetarderCall (float time) {
        Invoke ("EndTurn", time);
    }
    public void PlaySpecific (AudioClip audioClip) {
        GetComponent<AudioSource> ().clip = audioClip;
        GetComponent<AudioSource> ().Play ();
    }

    

}