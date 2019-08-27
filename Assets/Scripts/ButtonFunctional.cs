using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctional : MonoBehaviour {

	public UnityEngine.UI.Text MainText;
	public MenuManger Manger; 
	public string SaveName;
	public void SetText (string textToSet) {
		MainText.text = textToSet;
		
	}
	public void OnClick()
	{
		Manger.LoadGameScene(SaveName);
	}

}