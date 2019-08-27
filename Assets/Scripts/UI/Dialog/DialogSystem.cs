using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;


public class DialogSystem : MonoBehaviour
{

    public float Diametr;
    public string DialogName;
    public GameObject MissionGameObject, VariationRoll, NextButton;
    public Text MainText,NameText, TextForRoller;
    public string[] _allText;
    public int _indexText = 0;
    public List<int> VariationString = new List<int>();



    void Start()
    {
        ReadFile();
        DialogNextIndex();
    }


    void ReadFile()
    {
        try
        {
           StreamReader sr = new StreamReader("Dialogs/" + DialogName + ".txt", Encoding.Default);
            _allText = sr.ReadToEnd().Split('\n');

            for (int i = 0; i < _allText.Length - 1; i++)
            {
                _allText[i] = _allText[i].Trim();
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }



    public void DialogNextIndex()
    {
        _indexText++;
        if (_indexText < _allText.Length)
        {
            ActionCommand(_allText[_indexText]);
            
        }
        else
        {
            EndDialog();
        }
    }
    public void DialogNextIndex(int variation)
    {
        _indexText++;
        NextButton.SetActive(true);
        _indexText = variation;
        ActionCommand(_allText[_indexText]);
        

    }

    void EndDialog()
    {
        _indexText = 0;
        transform.parent.gameObject.SetActive(false);
    }


    void ActionCommand(string command)
    {
        try
        {
            switch (command)
            {
                case "ActiveSubObject":
                    MissionGameObject.SetActive(true);
                    EndDialog();
                    break;
                case "Variation":

                    #region Variation

                    NextButton.SetActive(false);
                    MainText.text = _allText[_indexText + 1].Split('|')[0].Remove(0, 6);
                    NameText.text = _allText[_indexText + 1].Split('|')[1];
                    VariationRoll.SetActive(true);
                    int i = 2;
                    while (_allText[_indexText + i] != "EndVariation")
                    {
                        Debug.Log(_allText[_indexText + i].Remove(0, 1).Remove(2));
                        VariationString.Add(Convert.ToInt32(_allText[_indexText + i].Remove(0, 1).Remove(2)));
                        i++;
                    }

                    for (i = 0; i < VariationString.Count; i++)
                    {
                        float xh = Diametr / VariationString.Count;
                        Text localText = Instantiate(TextForRoller,
                            y(xh * addable(i)) + VariationRoll.transform.position, Quaternion.identity,
                            VariationRoll.transform);
                        localText.text = _allText[_indexText + i + 2].Remove(0, 10);
                        if ((VariationString.Count) / 2 > i)
                            localText.alignment = TextAnchor.MiddleLeft;
                        else
                            localText.alignment = TextAnchor.MiddleRight;
                    }

                    VariationRoll.transform.GetChild(0).GetComponent<DialogRoller>().GetAllVarios();

                    #endregion

                    break;

                case "SetNextLine":
                    NextButton.SetActive(false);
                    DialogNextIndex(Convert.ToInt32(_allText[_indexText + 1].Remove(0, 1).Remove(2)));
                    
                    break;

                default:
                    MainText.text = command.Split('|')[0].Remove(0, 6);
                    NameText.text = command.Split('|')[1];
                    break;
            }
        }
        catch (Exception e)
        {
            MainText.text = "ERROR : " + e.Message;
            NextButton.SetActive(false);
            enabled = false;
        }
    }


    int addable(int i)
    {
        if ((i + 1) == VariationString.Count)
            return i + 1;
        else
            return i;


    }

    Vector3 y(float x)
    {
        return new Vector3(x - Diametr / 2f, Mathf.Sqrt(Mathf.Pow(Diametr / 2f, 2) - Mathf.Pow(x - Diametr / 2f, 2)), 0);
    }

}
