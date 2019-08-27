using System.Collections;
using UnityEngine;
[RequireComponent (typeof (AudioSource))]

public class WeaponBehevior : MonoBehaviour {
    public float range;
    [HideInInspector]
    public GameObject BulletObjectStandart;
    public int TypeWeapon;
    public int Damage, NormalDistance, MaxDistance;
    [HideInInspector]
    public int Load;
    public int MaxLoad;
    public int BulletPerShoot;
    public float SecondPerShoot;
    public SpriteRenderer SpriteRendererValue;
    public AudioClip WeaponSound;
    private AudioSource _audioSource;
    private UnitBehevior _selfUnit;
    private bool _testSound;
    private int _prevAmmoCount, _lastAmmoCount;

    public int Cost;
    public Color BulletColor;
    public enum FireType {
        Plus,
        Cross
    }
    public FireType fireTypeThis;
    public WeaponData data;

    void Awake () {
        if (transform.parent.GetComponent<UnitBehevior> () == null)
            enabled = false;
        ChekDataMetod ();

    }

    public void SetAmmoCountCheck () {
        _prevAmmoCount = Load;
        _lastAmmoCount = Load - BulletPerShoot;
    }

    void Start () {
        BulletObjectStandart = FindObjectOfType<ManagerMovment> ().transform.GetChild (0).gameObject;
        _selfUnit = GetComponentInParent<UnitBehevior> ();

        _audioSource = GetComponent<AudioSource> ();
        Load = MaxLoad;
        SpriteRendererValue = GetComponent<SpriteRenderer> ();
        transform.localPosition = _selfUnit.data.WeaponPosVector * Vector3.up;
    }

    public void Shoot (UnitBehevior Target) {

        if (Target == null)
            return;

        GameObject BulletObject;
        TrailRenderer BulletT;
        _audioSource.clip = WeaponSound;
        _audioSource.Play ();

        BulletObject = Instantiate (BulletObjectStandart, transform.GetChild (0).position, Quaternion.identity);

        float DistanceTarget = Vector3.Distance (transform.position, Target.transform.position + Vector3.up * (Random.Range (-5, 11) / 20f));
        float n = Random.Range (0, 100);
        Load = Load - 1;
        _selfUnit.candosomething = false;
        StartCoroutine (FinalHitUnit (Target, DistanceTarget / 25f));
        StartCoroutine (Shooting (Target, Target.transform.position + Vector3.up * (Random.Range (-5, 11) / 20f), DistanceTarget, n, BulletObject));

        BulletT = BulletObject.GetComponent<TrailRenderer> ();
        BulletT.startColor = BulletColor;

    }

    public IEnumerator FinalHitUnit (UnitBehevior Target, float second) {
        float ambushValue = 1;

        if (_selfUnit.manager.CorrectUnit != _selfUnit)
            ambushValue = 0.80f;
        yield return new WaitForSeconds (second);

        if (Target == null) {
            if (_selfUnit.manager.CorrectUnit = _selfUnit) {
                _selfUnit.manager.EndTurn ();
            }
            yield break;
        }
        float n = Random.Range (0, 100);
        Shelter _shelter;

        float DistanceTarget = Vector3.Distance (_selfUnit.transform.position, Target.transform.position + Vector3.up * (Random.Range (-5, 11) / 20f));

        float Range = NormalDistance / DistanceTarget;

        if (Range > 1) {
            Range = 1;
        }
        if (n < Range * 100f * ambushValue + _selfUnit.Skills[TypeWeapon] - Target.GetShelterLevel ())
            Target.GetHit (Damage, TypeWeapon);
        else {
            if (Random.Range (0, 101) > Target.GetShelterLevel () * 0.75f) {
                Target.GetShelterLevel (out _shelter);

                if (_shelter != null) {
                    Instantiate (_shelter.FX, _shelter.transform.position, Quaternion.identity);

                    _shelter.ShelterLevl -= Damage * 3;
                    if (_shelter.ShelterLevl < 1)
                        Destroy (_shelter.gameObject);
                }

            }
            Target.GetHit (0, 999);
        }
        if (_prevAmmoCount == _lastAmmoCount)
            _selfUnit.manager.EndTurn ();

    }

    public void Reload () {
        _selfUnit.manager.ShowInfoText ("Reload", Color.white, _selfUnit.transform.position);
        _selfUnit.manager.PlaySpecific (_selfUnit.manager.ReloadSound);
        Load = MaxLoad;
    }

    public IEnumerator Shooting (UnitBehevior Target, Vector3 TargetVector, float Distance, float RandomN, GameObject BulletObject) {

        if (!_testSound) {
            BulletObject.transform.position = transform.GetChild (0).position;
            yield return new WaitForSeconds (range);
        }
        _testSound = true;

        while (Target != null && Vector3.Distance (BulletObject.transform.position, Target.transform.position) > 0.3f) {
            BulletObject.transform.position = Vector3.MoveTowards (BulletObject.transform.position, TargetVector, Distance / 5);

            yield return new WaitForFixedUpdate ();
        }

        Destroy (BulletObject);

        _testSound = false;

    }

    public void ChekDataMetod () {
        if (data != null) {
            GetComponent<SpriteRenderer> ().sprite = data.ImageOFGun;
            TypeWeapon = data.TypeWeapon;
            Damage = data.Damage;
            NormalDistance = data.NormalDistance;
            MaxLoad = data.MaxLoad;
            MaxDistance = data.MaxDistance;
            WeaponSound = data.WeaponSound;
            fireTypeThis = (FireType) data.fireTypeThis;
            BulletColor = data.BulletColor;
            GetComponent<AudioSource> ().clip = data.WeaponSound;
            Cost = WhatACost ();
            BulletPerShoot = data.BulletPerShoot;
            SecondPerShoot = data.secondPerShoot;
        }
    }

    public int WhatACost () {
        int n = new int ();

        n = n + Damage * 5;
        n = n + Load * 10;
        n = n + TypeWeapon * Load * 10;

        return n;

    }
}