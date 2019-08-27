using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LoadDialog {

	public static string[] GetDialogs (string dialogName) {
		string[] result;

		StreamReader sr = new StreamReader ("Dialogs/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name + "/" + dialogName + ".txt", Encoding.Default);
		result = sr.ReadToEnd ().Split ('\n');

		for (int i = 0; i < result.Length - 1; i++) {
			result[i] = result[i].Trim ();
		}
		return result;
	}
}