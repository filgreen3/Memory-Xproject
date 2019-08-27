using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Save : MonoBehaviour {

	public static int GodInt;
	public static int[] UnitInvent = new int[64];
	static string US;
	public static int[] G_UnitInvent = new int[64];
	public static char[] ChUnitSave = new char[1024];
	public static int[] WeaponInvent = new int[64];
	static string WS;
	public static int[] G_WeaponInvent = new int[64];
	public static char[] ChWeaponSave = new char[1024];
	static int charIndex = 0;


	public static void Game_Save () {
		charIndex = 0;
		FileInfo WFile = new FileInfo (CorrectSave()+"WSave.txt");
		FileInfo UFile = new FileInfo (CorrectSave()+"USave.txt");
		if (!WFile.Exists)
			WFile.Create ();
		if (!UFile.Exists) { UFile.Create (); }
		for (int i = 0; i < 64; i++) {
			for (int j = 0; j < WeaponInvent[i].ToString ().Length; j++) {
				ChWeaponSave[j + charIndex] = WeaponInvent[i].ToString ().ToCharArray () [j];
			}
			ChWeaponSave[charIndex + WeaponInvent[i].ToString ().Length] = '#';
			charIndex += WeaponInvent[i].ToString ().Length + 1;

		}
		ChWeaponSave[charIndex] = '%';

		charIndex = 0;

		for (int i = 0; i < 64; i++) {
			for (int j = 0; j < UnitInvent[i].ToString ().Length; j++) {
				ChUnitSave[j + charIndex] = UnitInvent[i].ToString ().ToCharArray () [j];
			}
			ChUnitSave[charIndex + UnitInvent[i].ToString ().Length] = '#';
			charIndex += UnitInvent[i].ToString ().Length + 1;

		}
		ChUnitSave[charIndex] = '%';
		US = new string (ChUnitSave);
		WS = new string (ChWeaponSave);
		StreamWriter Wsw = new StreamWriter (CorrectSave()+"WSave.txt");
		StreamWriter Usw = new StreamWriter (CorrectSave()+"USave.txt");
		Wsw.WriteLine (WS);
		Usw.WriteLine (US);

		Wsw.Close ();
		Usw.Close ();
		Game_Load ();
	}

	public static void Game_Load () {
		FileInfo WFile = new FileInfo (CorrectSave()+"WSave.txt");
		FileInfo UFile = new FileInfo (CorrectSave()+"USave.txt");

		if (!WFile.Exists) { Game_Save (); } else {

			StreamReader Wsw = new StreamReader (CorrectSave()+"WSave.txt");
			StreamReader Usw = new StreamReader (CorrectSave()+"USave.txt");
			string WeaponCon = Wsw.ReadLine ();
			string UnitCon = Usw.ReadLine ();

			charIndex = 0;
			for (int i = 0; i < 64; i++) {
				UnitInvent[i] = 0;
				WeaponInvent[i] = 0;
			}

			for (int j = 0; j < 64; j++) {
				int i = 0;
				if (UnitCon.ToCharArray () [charIndex] != '%') {
					while (UnitCon.ToCharArray () [charIndex + i] != '#') {
						if (i == 1) {
							UnitInvent[j] *= 10;
						}

						UnitInvent[j] += (UnitCon.ToCharArray () [charIndex + i] - 48);

						i++;
					}

					charIndex += i + 1;
				} else {
					j = 65;
				}
			}

			charIndex = 0;

			for (int j = 0; j < 64; j++) {
				int i = 0;
				if (WeaponCon.ToCharArray () [charIndex] != '%') {
					while (WeaponCon.ToCharArray () [charIndex + i] != '#') {
						if (i == 1) {
							WeaponInvent[j] *= 10;
						}
						WeaponInvent[j] += (WeaponCon.ToCharArray () [i + charIndex] - 48);

						i++;
					}

					charIndex += i + 1;
				} else {
					j = 65;
				}
			}

			Wsw.Close ();
			Usw.Close ();
		}
	}

	public static void PutInInventory (string Name, int type) {
		if (type == 1) {
			for (int pp = 0; pp < UIManager.G_WeaponCanUse.Length; pp++) {
				if (Name == UIManager.G_WeaponCanUse[pp].data.name) {
					WeaponInvent[pp] += 1;
				}
			}
		} else {
			for (int pp = 0; pp < UIManager.G_UnitsCanUse.Length; pp++) {
				if (Name == UIManager.G_UnitsCanUse[pp].data.name) {
					UnitInvent[pp] += 1;
				}
			};

		}
	}
	public static int GetCoutInventory (string Name, int type) {
		int Cout = 0;
		if (type == 1) {
			for (int pp = 0; pp < UIManager.G_WeaponCanUse.Length; pp++) {
				if (Name == UIManager.G_WeaponCanUse[pp].data.name) {
					Cout = WeaponInvent[pp];
				}
			}
		} else {
			for (int pp = 0; pp < UIManager.G_UnitsCanUse.Length; pp++) {
				if (Name == UIManager.G_UnitsCanUse[pp].data.name) {
					Cout = UnitInvent[pp];
				}
			}

		}

		return Cout;
	}



static string CorrectSave()
{
	
        StreamReader logStream = new StreamReader ("Saves/LastSaveLog.txt");
        string result = "Saves/"+logStream.ReadLine()+"/";
        logStream.Close();
		return result;
}
}