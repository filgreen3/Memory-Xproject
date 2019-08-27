using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubButtonControl : MonoBehaviour
{


    public GameObject MainObject, SubOBJ1, SubOBJ2;
    public Shadow Mainsh, subShO1, subSh02;
    public void ToggleObject()
    {
        if (!MainObject.activeInHierarchy)
        {
            Mainsh.enabled = true;
            MainObject.SetActive(true);
            SubOBJ1.SetActive(false);
            SubOBJ2.SetActive(false);
            subSh02.enabled = false;
            subShO1.enabled = false;
        }
        else
        {
            MainObject.SetActive(false);
            subSh02.enabled = true;
            subShO1.enabled = true;
        }

    }
}
