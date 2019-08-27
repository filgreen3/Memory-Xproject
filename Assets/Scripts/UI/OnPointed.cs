using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnPointed : MonoBehaviour
{
    public ManagerMovment Manager;
    private GameObject _effeck;
    private ParticleSystem _effeckPS;


    void Start()
    {
        _effeck = Instantiate(Manager.OnTurnEffectFX[7]);
        _effeckPS = _effeck.GetComponent<ParticleSystem>();
    }

    public void Active()
    {
        if (!Manager.CorrectUnit.Enemy&& !_effeck.activeInHierarchy)
        {
            _effeck.transform.position = Manager.GridABSManager.NodeFromWorldPoint( Manager.CorrectUnit.transform.position).Position;
            var sp = _effeckPS.main.startSize;
            sp = Manager.CorrectUnit.Weapon.MaxDistance*3;
            _effeck.SetActive(true);
        }
    }
    public void DeActive()
    {

        _effeck.SetActive(false);

    }
}
