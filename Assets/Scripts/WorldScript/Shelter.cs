using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shelter : MonoBehaviour
{
    public bool wall;
    public int ShelterLevl;
    public GameObject FX;
    public List<Transform> ListofPivots = new List<Transform>();
    public GameObject image;
    public Text InfoText;



    private void Start()
    {
        
        Invoke("SetSelterBlock", 0.1f);
    }


    private void OnMouseEnter()
    {
        InfoText.gameObject.SetActive(true);
        image.SetActive(true);
        InfoText.text = ShelterLevl+"%";
    }

    private void OnMouseExit()
    {
        InfoText.gameObject.SetActive(false);
        image.SetActive(false);
    }


    void SetSelterBlock()
    {
        Debug.Log("Shelter is set");
        GridABS grid = FindObjectOfType<GridABS>();
        int xC;
        int yC;


        foreach (Transform n in ListofPivots)
        {
            grid.NodeCoordinat(n.position, out xC, out yC);
            grid.ShelterList.Add(new Vector2(xC, yC));
            grid.CurrentShelterList.Add(this);
            grid.GridOfArena[xC, yC]._Bloc = (!wall)? NodeA.TypeOfNode.Shelter: NodeA.TypeOfNode.Wall;
        }
    }
    private void OnDestroy()
    {
        try
        {


            GridABS grid = FindObjectOfType<GridABS>();
            int xC;
            int yC;

            foreach (Transform n in ListofPivots)
            {
                grid.NodeCoordinat(n.position, out xC, out yC);
                grid.CurrentShelterList.Remove(this);
                grid.ShelterList.Remove(new Vector2(xC, yC));
                grid.GridOfArena[xC, yC]._Bloc = NodeA.TypeOfNode.Walkable;
            }
        }
        catch
        { }
    }
    
}
