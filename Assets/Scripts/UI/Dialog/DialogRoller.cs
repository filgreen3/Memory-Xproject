using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogRoller : MonoBehaviour
{
    public DialogSystem DialogSystemExemp;
    public Text[] VariosTexts;
    public float Radious;
    private Vector3 _startVector3;
    private int _choosenVar;

    void Start()
    {
        _startVector3 = transform.position;
 
    }

    void Update()
    {
        try
        {


            float multip = Radious/2f;
            Vector3 dir = (Input.mousePosition - _startVector3).normalized * multip + _startVector3;

            if ((Input.mousePosition - _startVector3).y > 0)
            {
                transform.position = new Vector3(dir.x, Mathf.Abs(dir.y), dir.z);

                for (int i = 0; i < VariosTexts.Length; i++)
                {
                    float localDist = Vector3.Distance(transform.position, VariosTexts[i].transform.position);

                    if (localDist < Vector3.Distance(transform.position, VariosTexts[_choosenVar].transform.position))
                    {
                        VariosTexts[_choosenVar].color = Color.white;
                        _choosenVar = i;
                        VariosTexts[_choosenVar].color = Color.red;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject(DialogSystemExemp.GetInstanceID()))
                {
                    DialogSystemExemp.DialogNextIndex(DialogSystemExemp.VariationString[_choosenVar]);
                    Debug.Log(DialogSystemExemp.VariationString[_choosenVar]);
                    foreach (Text n in VariosTexts)
                    {
                        Destroy(n.gameObject);
                    }
                    DialogSystemExemp.VariationString.Clear();
                    transform.parent.gameObject.SetActive(false);
                }
            }
        }
        catch
        {
        }
    }

    public void GetAllVarios()
    {
        VariosTexts = transform.parent.GetComponentsInChildren<Text>();
        _choosenVar = 0;
        VariosTexts[_choosenVar].color = Color.red;
    }
}
