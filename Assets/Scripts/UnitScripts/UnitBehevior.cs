using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitBehevior : MonoBehaviour {
    [Header ("MainParametr")] public EffectOnUnit _effectOnUnit = new EffectOnUnit ();

    private SpriteRenderer _localmarcer;
    public SpriteRenderer BodyRenderer, HandRenderer;
    public bool ActivatedWorld;

    [Range (1, 10)] public int ArmorQulity;

    [HideInInspector] public GameObject BarHelty, BarShild, BarArmor;

    private readonly Text[] BarValue = new Text[3];

    public bool candosomething = true; //чтобы игрок не мог двигаться во врем стрельбы

    public int Cost;
    public UnitData data;

    public bool StartWhitsMap;
    public bool Enemy;

    [HideInInspector] public GridABS GridManager;

    [Range (0, 1000)] public float Helty, Shild, Armor;

    private bool _isEnemyMarced;

    [HideInInspector] public ManagerMovment manager;
    [HideInInspector] public bool AmbushZone;
    [HideInInspector] private bool _isPointed;

    public SpriteRenderer Marker;

    [HideInInspector] public float MaxHelty, MaxShild, MaxArmor;

    [Space (5)] public int MaxWalk;

    public PercMainBehevior PercBehevior;
    public int[] Resistans = new int[4];

    [Range (0, 100)] public int[] Skills = new int[6];

    public int TimeToReusePerc;
    public WeaponBehevior Weapon;

    public int WhatACost () {
        var n = new int ();

        foreach (var i in Skills) n = n + i;
        n = n + (int) MaxHelty;
        n = n + (int) MaxShild * 2;
        n = n + (int) MaxArmor * ArmorQulity;
        n = n + MaxWalk * 10;
        return n;
    }

    [ContextMenu ("ChekData")]
    public void ChekDataMetod () {
        if (data != null) {
            MaxWalk = data.MaxWalk;
            Helty = data.Helty;
            Shild = data.Shild;
            Armor = data.Armor;
            ArmorQulity = data.ArmorQulity;
            Skills = data.Skills;

            BodyRenderer = transform.GetChild (1).GetComponent<SpriteRenderer> ();
            BodyRenderer.sprite = data.IconBodyFace;
            for (var i = 0; i < 6; i++) Skills[i] = data.Skills[i];
            for (var i = 0; i < 3; i++) Resistans[i] = data.Resistans[i];
        }
    }

    void Start () {
        if (StartWhitsMap)
            StartBattle ();
    }

    public void StartBattle () {
        _effectOnUnit.Unit = this;

        BarHelty = transform.GetChild (0).GetChild (0).GetChild (0).gameObject;
        BarShild = transform.GetChild (0).GetChild (0).GetChild (1).gameObject;

        ChekDataMetod ();

        Cost = WhatACost ();

        MaxHelty = Helty;
        MaxShild = Shild;
        MaxArmor = Armor;
        if (FindObjectOfType<ManagerMovment> () == null) {
            ActivatedWorld = false;
            enabled = false;
        } else {
            name = data.name.Remove (0, 2) + " : " + Random.Range (0, 1001);

            HandRenderer = transform.GetChild (2).GetComponent<SpriteRenderer> ();

            enabled = true;
            Weapon.enabled = true;

            ActivatedWorld = true;
            manager = FindObjectOfType<ManagerMovment> ();
            GridManager = FindObjectOfType<GridABS> ();
            candosomething = true;

            #region установка значений показателей здоровья брони щита

            BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);

            if (Shild > 0)
                BarShild.transform.localScale = new Vector3 (1, Shild / MaxShild, 1);
            else
                BarShild.transform.localScale = new Vector3 (1, 0, 1);

            #endregion

            #region нормализация положения

            transform.eulerAngles = new Vector3 (20, -45, 0);
            GridManager.GridOfArena[GridManager.NodeFromWorldPoint (transform.position).x1,
                GridManager.NodeFromWorldPoint (transform.position).y1].UnitOnNode = this;
            transform.position = GridManager.NodeFromWorldPoint (transform.position).Position + Vector3.forward * 0.12f +
                Vector3.up * 0.34f;
            transform.parent = GridManager.transform;

            #endregion

            PercBehevior = gameObject.AddComponent<PercMainBehevior> ();
            PercBehevior.enabled = false;
            PercBehevior.manager = manager;
            PercBehevior.GridManager = GridManager;
            PercBehevior.data = data.PercOfUnit;

            PercBehevior.SetParams ();
            PercBehevior.ThisUnit = this;
            GridManager.GridOfArena[GridManager.NodeFromWorldPoint (transform.position).x1,
                GridManager.NodeFromWorldPoint (transform.position).y1].UnitOnNode = this;
            _effectOnUnit.FireFx = manager.FireFX;
            _effectOnUnit.PoisnFx = manager.PoisnFX;
            _effectOnUnit.ElectricShockFx = manager.ElectricShockFX;

            BarValue[0] = transform.GetChild (0).GetChild (0).GetChild (3).GetComponent<Text> ();
            BarValue[1] = transform.GetChild (0).GetChild (0).GetChild (4).GetComponent<Text> ();
            BarValue[2] = transform.GetChild (0).GetChild (0).GetChild (5).GetComponent<Text> ();

            name = data.name.Remove (0, 2) + " : " + Random.Range (0, 1001);

            HandRenderer = transform.GetChild (2).GetComponent<SpriteRenderer> ();
        }
    }

    private void Update () {
        if (!EventSystem.current.IsPointerOverGameObject () && _isEnemyMarced && !_effectOnUnit.HaveEffect (2)) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

            Physics.Raycast (ray, out hit, Mathf.Infinity);

            if (hit.rigidbody != null) {
                var enemyTarget = hit.rigidbody.GetComponent<UnitBehevior> ();

                if (Input.GetKeyDown (KeyCode.Mouse0) &&
                    GridManager.NodeFromWorldPoint (enemyTarget.transform.position).FireZone &&
                    enemyTarget.Enemy != Enemy) {

                    if (manager.AttackControl) //стрельба для оружия
                    {
                        StartCoroutine (ShootByUnit (this, hit.rigidbody.GetComponent<UnitBehevior> ()));
                        manager.AttackControl = false;
                        GridManager.CleatGrid ();
                    }

                    _isEnemyMarced = false;
                }
            }
        }
    }

    #region  Основные действия
    public void Attacking () {
        if (candosomething) {
            GridManager.ShowRadiousAttack (Weapon.MaxDistance);
            GridManager.ShowAllEnemy (Enemy, true);
            _isEnemyMarced = true;
        }
    }

    public void Moving () {

        _isEnemyMarced = false;
        GridManager.MaxMovement (transform.position, MaxWalk, this);

    }

    public void PercUse () {

        PercBehevior.enabled = true;
        PercBehevior.ShowTargets ();

    }

    #endregion

    private void OnMouseEnter () {
        if (manager.MoveControl) {
            if (this != manager.CorrectUnit) {
                BodyRenderer.material.color *= new Color (1, 1, 1, 0.3f);
                HandRenderer.material.color *= new Color (1, 1, 1, 0.3f);
                Weapon.SpriteRendererValue.material.color *= new Color (1, 1, 1, 0.3f);
                _isPointed = true;
            }
        } else {
            SetValueToBar ();
            transform.GetChild (0).gameObject.SetActive (true);
            StartCoroutine (ReSizeBar ());

            if (Enemy && manager.AttackControl && GridManager.NodeFromWorldPoint (transform.position).FireZone) {
                manager.ShootBar.SetActive (true);
                manager.ShootBarText.text =
                    " * " + 100 * Mathf.Clamp01 (manager.CorrectUnit.Weapon.NormalDistance /
                        Vector3.Distance (transform.position,
                            manager.CorrectUnit.transform.position)) + "% дальность" + "\n" +
                    " + " + manager.CorrectUnit.Skills[manager.CorrectUnit.Weapon.TypeWeapon] + " от навыка" + "\n" +
                    " - " + GetShelterLevel () + " от укрытия" + "\n" +
                    " = " + 100f * Mathf.Clamp01 (
                        Mathf.Clamp01 (manager.CorrectUnit.Weapon.NormalDistance /
                            Vector3.Distance (transform.position, manager.CorrectUnit.transform.position)) +
                        (manager.CorrectUnit.Skills[manager.CorrectUnit.Weapon.TypeWeapon] - GetShelterLevel ()) / 100f) +
                    "%";
            }
        }
    }

    private void OnMouseExit () {
        manager.ShootBar.SetActive (false);
        transform.GetChild (0).gameObject.SetActive (false);

        if (_isPointed) {
            BodyRenderer.material.color = new Color (1, 1, 1, 1);
            HandRenderer.material.color = new Color (1, 1, 1, 1);
            Weapon.SpriteRendererValue.material.color = new Color (1, 1, 1, 1);
            _isPointed = false;
        }
    }

    private void BarMenu () {
        transform.GetChild (0).gameObject.SetActive (!transform.GetChild (0).gameObject.activeInHierarchy);
    }

    #region GetHit
    public void GetHit (float hitPower, int typeOfWeapon) {
        if (Helty <= 1) Destroy (gameObject);

        if (typeOfWeapon == 999) //вывод "промох"
        {
            manager.ShowInfoText (0, Color.white, transform.position);
            return;
        }

        if (Mathf.Abs (hitPower - 0) < 0.001f) //проверка всех значений
        {
            BarShild.transform.localScale = new Vector3 (1, Shild / MaxShild, 1);
            BarArmor.transform.localScale = new Vector3 (1, Armor / MaxArmor, 1);
            BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);
            return;
        }

        // обычное нанесение урона
        if (Shild > 1) {
            Shild = Shild - hitPower;
            if (Shild <= 0)
                Shild = 0;
            manager.ShowInfoText ((int) hitPower, manager.ColorOfIfo[0], transform.position);
            BarShild.transform.localScale = new Vector3 (1, Shild / MaxShild, 1);
            Instantiate (manager.ShildFx, transform.position, Quaternion.identity);
        } else {
            if (hitPower - Armor > 0) {
                Helty -= hitPower - Armor;
                manager.ShowInfoText ((int) (hitPower - Armor), manager.ColorOfIfo[3],
                    transform.position);
                BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);
            } else
                manager.ShowInfoText ("Armor", Color.white,
                    transform.position);

        }

        SetValueToBar ();
        if (Helty <= 1) Destroy (gameObject);
    }

    public void GetHit (float hitPower, string typeofparametr) {
        if (Helty <= 1) Destroy (gameObject);

        switch (typeofparametr) {
            case "FireAttack":

                Helty -= hitPower;
                manager.ShowInfoText ((int) (hitPower), manager.ColorOfIfo[3],
                    transform.position);
                BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);

                break;

            case "PoisonAttack":

                Helty -= hitPower;
                manager.ShowInfoText ((int) hitPower, manager.ColorOfIfo[3], transform.position);
                BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);
                if (Helty <= 1) Destroy (gameObject);

                break;

            case "ElectricShock":

                Helty -= hitPower;
                manager.ShowInfoText ((int) hitPower, manager.ColorOfIfo[3], transform.position);
                BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);
                if (Helty <= 1) Destroy (gameObject);
                break;

            case "Heal":

                if (Helty < MaxHelty) {
                    Helty += hitPower;
                    manager.ShowInfoText ("+" + hitPower, Color.green, transform.position);

                    if (Helty >= MaxHelty) Helty = MaxHelty;
                    BarHelty.transform.localScale = new Vector3 (1, Helty / MaxHelty, 1);
                } else {
                    manager.ShowInfoText ("Max Helty", Color.white, transform.position);
                }

                break;

            default:
                Debug.LogWarning ("Eror: unknow effect");
                break;
        }

        SetValueToBar ();
        if (Helty <= 1) Destroy (gameObject);
    }

    #endregion

    private void SetValueToBar () {
        BarValue[0].text = ((int) Helty).ToString ();
        BarValue[1].text = ((int) Shild).ToString ();
        BarValue[2].text = ((int) Armor).ToString ();

        if (transform.localScale.x < 0) {
            BarValue[0].transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
            BarValue[1].transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
            BarValue[2].transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
        } else {
            BarValue[0].transform.localScale = Vector3.one;
            BarValue[1].transform.localScale = Vector3.one;
            BarValue[2].transform.localScale = Vector3.one;
        }
    }

    #region GetShelterLevel
    public int GetShelterLevel () {
        RaycastHit raycast;
        Physics.Raycast (transform.position, manager.CorrectUnit.transform.position - transform.position, out raycast,
            1);

        if (raycast.collider != null && raycast.collider.GetComponent<Shelter> () != null)
            return raycast.collider.GetComponent<Shelter> ().ShelterLevl;
        return 0;
    }

    public int GetShelterLevel (out Shelter shelter) {
        shelter = null;
        RaycastHit raycast;
        Physics.Raycast (transform.position, manager.CorrectUnit.transform.position - transform.position, out raycast,
            1);

        if (raycast.collider != null && raycast.collider.GetComponent<Shelter> () != null) {
            shelter = raycast.collider.GetComponent<Shelter> ();
            return raycast.collider.GetComponent<Shelter> ().ShelterLevl;
        }

        return 0;
    }
    #endregion

    public float GetHeltyTarget () {
        return 1 + (1 - Helty / MaxHelty + 1 - Shild / MaxShild) / 2;
    }

    public void CreateMarcer (Vector3 poseition) {
        if (_localmarcer != null)
            Destroy (_localmarcer.gameObject);
        _localmarcer = Instantiate (Marker, poseition, Quaternion.identity);
        _localmarcer.color = !Enemy ? new Color (0, 244 / 255f, 1) : new Color (1, 30 / 255f, 0);
        _localmarcer.transform.eulerAngles = new Vector3 (90, 45);
    }

    public IEnumerator ShootByUnit (UnitBehevior unitShooter, UnitBehevior unitTarget) {
        unitShooter = this;
        if (unitTarget.transform.position.x - unitShooter.transform.position.x > 1) {
            Weapon.SpriteRendererValue.sortingOrder = 1;
            Weapon.SpriteRendererValue.flipX = false;

            HandRenderer.sortingOrder = 1;
            HandRenderer.flipX = false;

            BodyRenderer.sprite = data.IconBodyFace;
        }

        if (unitTarget.transform.position.x - unitShooter.transform.position.x < -1&& false) {
            Weapon.SpriteRendererValue.sortingOrder = -2;
            Weapon.SpriteRendererValue.flipX = true;

            HandRenderer.sortingOrder = -2;
            HandRenderer.flipX = true;

            BodyRenderer.sprite = data.IconBodyBack;
        }
        Weapon.SetAmmoCountCheck();

        for (var i = 0; i < Weapon.BulletPerShoot; i++)
            if (Weapon.Load > 0 && unitShooter != null) {
                Weapon.Shoot (unitTarget);
                yield return new WaitForSeconds (Weapon.SecondPerShoot);
            }
        if (manager.CorrectUnit == unitShooter)
            manager.RetarderCall (Vector3.Distance (unitShooter.transform.position, unitTarget.transform.position) / 5f +
                Weapon.SecondPerShoot * Weapon.BulletPerShoot);
        yield return new WaitForEndOfFrame ();
    }

    public IEnumerator LerpMove (Vector3 finalePose) {
        manager.unitShowCorrectUnit.SetActive (false);

        foreach (UnitBehevior n in (Enemy) ? manager.FriendList : manager.EnemyList) {
            if (n.AmbushZone && Vector3.Distance (transform.position, n.transform.position) <
                n.Weapon.MaxDistance - 0.5f&& !n._effectOnUnit.HaveEffect(2)) {
                StartCoroutine (n.ShootByUnit (n, this));
                n.AmbushZone = false;
            }
        }

        float timer = 0;
        while (Vector3.Distance (transform.position, finalePose) > 0.1f && timer < 1f) {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp (transform.position, finalePose, 0.1f);
            yield return new WaitForEndOfFrame();
        }

        transform.position = finalePose;

        foreach (UnitBehevior n in (Enemy) ? manager.FriendList : manager.EnemyList) {
            if (n.AmbushZone && Vector3.Distance (transform.position, n.transform.position) <
                n.Weapon.MaxDistance - 0.5f&& !n._effectOnUnit.HaveEffect(2)) {
                StartCoroutine (n.ShootByUnit (n, this));
                n.AmbushZone = false;
            }
        }

        if (manager.CorrectUnit == this)
            manager.EndTurn ();
    }

    private IEnumerator ReSizeBar () {
        float timer = 0;
        while (timer < 0.01f) {
            timer += Time.deltaTime / 20;
            transform.GetChild (0).localScale = 1.36f * Vector3.one * timer;
            yield return new WaitForEndOfFrame ();
        }
    }

    private void OnDestroy () {
        try {
            StopAllCoroutines ();
            if (ActivatedWorld) {
                if (_localmarcer != null)
                    Destroy (_localmarcer.gameObject);
                if (manager.ListOfGo[(manager.Turn + 1) % manager.ListOfGo.Count]) {
                    manager.ListOfGo[(manager.Turn + 1) % manager.ListOfGo.Count] =
                        manager.ListOfGo[(manager.Turn + 2) % manager.ListOfGo.Count];
                    manager.CreateLIst ();
                    manager.EndTurn ();
                } else {
                    manager.CreateLIst ();
                }

                manager.transform.GetChild (0).position = new Vector3 (0, 0, 1000);
            }
        } catch { }
    }

    public class EffectOnUnit {
        public int DurationOfFire;
        public int DurationOfPoisen;

        public int DurationOfStun;

        public GameObject FireFx, PoisnFx, ElectricShockFx;

        public GameObject InsFireFx, InsStunFx, InsPoisnFx, InsElectricShockFx;

        public GameObject InsOnTurnEffectFx;

        private int _onTurnEffectPower;

        public int PowerOfFire;

        public int PowerOfPoisen;
        public UnitBehevior Unit;

        public bool HaveEffect (int type) {
            switch (type) {
                case 0:
                    if (DurationOfFire > 0)
                        return true;
                    break;
                case 1:
                    if (DurationOfPoisen > 0)
                        return true;
                    break;
                case 2:
                    if (DurationOfStun > 0)
                        return true;
                    break;
            }

            return false;
        }

        public void MakePoisend (int damage, int duration) {
            if (Unit.Resistans[1] < 100) {
                if (InsPoisnFx == null)
                    InsPoisnFx = Instantiate (PoisnFx, Unit.transform.position + Vector3.down * 0.7f,
                        Quaternion.identity, Unit.transform);
                InsPoisnFx.transform.eulerAngles = Unit.transform.eulerAngles;
                InsPoisnFx.transform.localPosition = new Vector3 (InsPoisnFx.transform.localPosition.x,
                    InsPoisnFx.transform.localPosition.y);

                PowerOfPoisen = (int) (damage * ((100 - Unit.Resistans[1]) / 100f));
                DurationOfPoisen = duration;
            } else {
                Unit.manager.ShowInfoText ("Resistans", Color.red, Unit.transform.position);
            }
        }

        public void MakeFired (int damage, int duration) {
            if (Unit.Resistans[0] < 100) {
                if (InsFireFx == null)
                    InsFireFx = Instantiate (FireFx, Unit.transform.position + Vector3.down * 0.7f, Quaternion.identity,
                        Unit.transform);
                InsFireFx.transform.eulerAngles = Unit.transform.eulerAngles;
                InsFireFx.transform.localPosition = new Vector3 (InsFireFx.transform.localPosition.x,
                    InsFireFx.transform.localPosition.y);
                PowerOfFire = (int) (damage * ((100 - Unit.Resistans[0]) / 100f));
                DurationOfFire = duration;
                Unit.GetHit (PowerOfFire, "FireAttack");
            } else {
                Unit.manager.ShowInfoText ("Resistans", Color.red, Unit.transform.position);
            }
        }

        public void MakeStuned (int _duration) {
            if (Random.Range (0, 100) > Unit.Resistans[2]) {
                if (InsStunFx == null)
                    InsStunFx = Instantiate (Unit.manager.StunFX, Unit.transform.position, Quaternion.identity, Unit.transform);
                InsStunFx.transform.eulerAngles = Unit.transform.eulerAngles;
                InsStunFx.transform.localPosition = new Vector3 (InsStunFx.transform.localPosition.x,
                    InsStunFx.transform.localPosition.y);

                DurationOfStun = _duration;
            } else {
                Unit.manager.ShowInfoText ("Stun faild", Color.white, Unit.transform.position);
            }
        }

        public void MakeElectricShock (int _damage) {
            if (Unit.Shild > 0) {
                InsPoisnFx = Instantiate (ElectricShockFx, Unit.transform.position + Vector3.down * 0.7f,
                    Quaternion.identity, Unit.transform);
                InsPoisnFx.transform.eulerAngles = Unit.transform.eulerAngles;
                InsPoisnFx.transform.localPosition = new Vector3 (InsPoisnFx.transform.localPosition.x,
                    InsPoisnFx.transform.localPosition.y);
                Unit.GetHit (_damage, 674);
            } else {
                if (Unit.Resistans[3] < 100) {
                    InsOnTurnEffectFx = Instantiate (ElectricShockFx, Unit.transform.position + Vector3.down * 0.7f,
                        Quaternion.identity, Unit.transform);
                    InsOnTurnEffectFx.transform.eulerAngles = Unit.transform.eulerAngles;
                    InsOnTurnEffectFx.transform.localPosition = new Vector3 (InsPoisnFx.transform.localPosition.x,
                        InsPoisnFx.transform.localPosition.y);
                    Unit.GetHit ((int) (_damage * ((100 - Unit.Resistans[3]) / 100f)), "ElectricShock");
                } else {
                    Unit.manager.ShowInfoText ("Resistans", Color.red, Unit.transform.position);
                }
            }
        }

        public void HealHuman (int _healPower) {
            if (DurationOfPoisen > 0) {
                Unit.manager.ShowInfoText ("Poison off", Color.white, Unit.transform.position + Vector3.back * 0.4f);
                DurationOfPoisen = 0;
            }

            InsOnTurnEffectFx = Instantiate (Unit.manager.OnTurnEffectFX[0],
                Unit.transform.position + Vector3.down * 0.7f, Quaternion.identity, Unit.transform);
            InsOnTurnEffectFx.transform.eulerAngles = Unit.transform.eulerAngles;
            InsOnTurnEffectFx.transform.localPosition = new Vector3 (InsOnTurnEffectFx.transform.localPosition.x,
                InsOnTurnEffectFx.transform.localPosition.y);
            _onTurnEffectPower = _healPower;

            Unit.GetHit (_onTurnEffectPower, "Heal");
        }

        public void CheckEffects () {
            if (DurationOfFire > 0) {
                Unit.GetHit (PowerOfFire, "FireAttack");
                DurationOfFire -= 1;
            } else if (InsFireFx != null) {
                Destroy (InsFireFx);
            }

            if (DurationOfPoisen > 0) {
                Unit.GetHit (PowerOfPoisen, "PoisonAttack");
                DurationOfPoisen -= 1;
            } else if (InsPoisnFx != null) {
                Destroy (InsPoisnFx);
            }

            if (DurationOfStun > 0) {
                DurationOfStun -= 1;
                Unit.manager.ShowInfoText ("Stun", Color.white, Unit.transform.position);
               Unit.candosomething=false;
            } else if (InsStunFx != null) {
                Destroy (InsStunFx);
            }
        }
    }
}