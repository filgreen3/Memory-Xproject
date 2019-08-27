using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{

	public UnityEngine.UI.Slider SliderOfLoad;


private int _sceneID;
	public void StartLoading(int sceneID)
	{
		_sceneID = sceneID;
		gameObject.SetActive(true);
		StartCoroutine(LoadAsyn());
	}

	IEnumerator LoadAsyn ()
	{
	    float loadingProgress;
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneID);
		while(!operation.isDone)
		{
		    loadingProgress = operation.progress/0.9f; 

            SliderOfLoad.value= loadingProgress;
			yield return null;
		} 
	}

}
