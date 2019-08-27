using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManger : MonoBehaviour {

    public GameObject ContinueButtonGM, LoadButtonGM;
    public InputField NameOfNewGame;

    public ButtonFunctional StandartButton;
    public Transform ContextTransform;

    public LoadingScript LoadingManger;

    void Start () {
        if (!Directory.Exists ("Saves")) Directory.CreateDirectory ("Saves");
        if (Directory.GetDirectories ("Saves").Length > 1) {
            ContinueButtonGM.SetActive (true);
            LoadButtonGM.SetActive (true);
        }
        SetListLoad();
    }

    public void StartNewGame () {
        if (NameOfNewGame.text == string.Empty) return;
        string lastSaveLogText = NameOfNewGame.text;
        if (TestDirectoris (lastSaveLogText)) return;

        FileInfo lastSaveLog = new FileInfo ("Saves/LastSaveLog.txt");
        if (!lastSaveLog.Exists)
            lastSaveLog.Create ();

        StreamWriter logStream = new StreamWriter ("Saves/LastSaveLog.txt");
        logStream.WriteLine (lastSaveLogText);
        logStream.Close ();

        Directory.CreateDirectory ("Saves/" + lastSaveLogText);
        CopyDefluttSaveFile (lastSaveLogText);
        LoadGameScene ();
    }
    public void LoadGameScene () {
        LoadingManger.StartLoading(1);
    }
    public void LoadGameScene (string saveName) {
        StreamWriter logStream = new StreamWriter ("Saves/LastSaveLog.txt");
        logStream.WriteLine (saveName);
        logStream.Close ();
        LoadingManger.StartLoading(1);
    }

    private void CopyDefluttSaveFile (string directoryName) {
        FileInfo weaponSaveFile = new FileInfo ("Saves/Deflat/WSave.txt");
        FileInfo unitSaveFile = new FileInfo ("Saves/Deflat/USave.txt");
        weaponSaveFile.CopyTo ("Saves/" + directoryName + "/WSave.txt");
        unitSaveFile.CopyTo ("Saves/" + directoryName + "/USave.txt");

    }
    private bool TestDirectoris (string nameOfNewDir) {
        string[] allNames = Directory.GetDirectories ("Saves");
        foreach (string n in allNames) {
            if (n == nameOfNewDir) return false;
        }
        return false;

    }
    private void SetListLoad()
    {

         string[] allNames = Directory.GetDirectories ("Saves");
          foreach (string n in allNames) {
              string localString = n.Remove(0,6);
            if (localString == "Deflat") continue;

            ButtonFunctional bf = Instantiate(StandartButton,ContextTransform);
            bf.Manger = this;
            bf.SaveName = localString;
            bf.SetText(localString +"\n"+ Directory.GetLastWriteTime(n).ToLocalTime().ToString());
        }


    }
}