using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainDialogue : MonoBehaviour {

public Text L_dialogueText;
public Text L_leftText;
public Text L_rightText;
public Text L_middleText;
public GameObject L_backPanel;
public  GameObject L_nextTreeButton;
public Button L_leftButton;
public Button L_rightButton;
public Button L_middleButton;

public static Text dialogueText;
public static Text leftText;
public static Text rightText;
public static Text middleText;
public static GameObject backPanel;
public static GameObject nextTreeButton;
public static Button leftButton;
public static Button rightButton;
public static Button middleButton;


void Start()
{
	dialogueText = L_dialogueText;
	leftText = L_leftText;
	rightText = L_rightText;
	middleText = L_middleText;
	backPanel = L_backPanel;
	nextTreeButton = L_nextTreeButton;
	leftButton = L_leftButton;
	rightButton = L_rightButton;
	middleButton = L_middleButton;
}

}
