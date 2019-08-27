
using UnityEngine;

public class PercData : ScriptableObject {
    
    [Tooltip("0 - переход к параметрам")]
    public int A_param;
    [Tooltip("0 - на себя , 1 - на врагов , 2 на себя и друзей")]
    public int BA_param;
    [Tooltip("0 - здоровье , 1 - щит , 2 броня")]
    public int СA_param;
    public int PowerOfPerc;
    public int DuratinOfPerc;
    public int TimeToReuse;
    public Sprite Icon ;
    public int RadiousOfattack;

    public string Discription;
}
