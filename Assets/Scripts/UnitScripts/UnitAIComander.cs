using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAIComander : UnitAI {

	[Header ("Comand of main unit")]
	public string DialogNameToLoad;
	public string[] LoadedDialogs;
	int lastUnitDialog;
	private bool _needLoadPhrase;

	public override void ChooseTactic () {
		if (_needLoadPhrase) {
			ReadAndSetArray ();
		}
		float waittime = 2f;
		string texttospeach = LoadedDialogs[Random.Range (0, lastUnitDialog)];
		manager.ShowInfoText (texttospeach, Color.white, transform.position);

		if (texttospeach == string.Empty)
			waittime = 0;

		StartCoroutine (ILateCallTactic ());

	}

	IEnumerator ILateCallTactic () {
		yield return new WaitForSeconds (1);
		base.ChooseTactic ();
	}
	void ReadAndSetArray () {
		lastUnitDialog = LoadedDialogs.Length - 1;
		try {
			string[] local = LoadDialog.GetDialogs (DialogNameToLoad);
			int size = LoadedDialogs.Length + local.Length;
			string[] localsecond = new string[size];
			for (int i = 0; i < size; i++)
				if (LoadedDialogs.Length < i)
					localsecond[i] = LoadedDialogs[i];
				else
					localsecond[i] = local[i - LoadedDialogs.Length];
		} catch {

		}
	}
}