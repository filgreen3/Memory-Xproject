using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateZone : EditorWindow {
    private int _xField;
    private int _zField;
    private List<GameObject> _arrayList = new List<GameObject>();
    private List<Vector2Int> _arrayListSize = new List<Vector2Int>();
    
    // Use this for initialization
    private void OnGUI()
    {
        _xField = EditorGUILayout.IntField("X : ", _xField);
        _zField = EditorGUILayout.IntField("Y : ", _zField);
        for (int i = 0; i < _arrayList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            _arrayList[i] = (GameObject)EditorGUILayout.ObjectField(_arrayList[i], typeof(GameObject), true);
            _arrayListSize[i] = EditorGUILayout.Vector2IntField("Размер дома",_arrayListSize[i], GUILayout.Width(140), GUILayout.Height(60));
            GUILayout.EndHorizontal();

        }


        Rect rAddItemList = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(rAddItemList, GUIContent.none))
        {
            _arrayList.Add(Resources.Load("null") as GameObject);
            _arrayListSize.Add(Vector2Int.zero);
        }
        GUILayout.Label("Добавить постройку");
        EditorGUILayout.EndHorizontal();



        Rect rSaveZone = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(rSaveZone, GUIContent.none))
        {
            int _xCount = 0;
            int _zCount = 0;
            GameObject newParent = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            while (_zCount < _zField)
            {
                while (true)
                {
                    int NumEd= Random.Range(0, _arrayList.Count);
                    GameObject newChild = Instantiate(_arrayList[NumEd]);
                    newChild.transform.Translate(new Vector3Int(_xCount + _arrayListSize[NumEd].x / 2, 0, _zCount + _arrayListSize[NumEd].y / 2));
                    newChild.transform.Rotate(new Vector3(0, 90 * Random.Range(-1, 5), 0));
                    _xCount += _arrayListSize[NumEd].x + 5;
                    newChild.transform.SetParent(newParent.transform);
                    if (_xCount >= _xField)
                    {
                        _zCount += _arrayListSize[NumEd].y + 5;
                        _xCount = 0;
                        break;
                    }
                }
            }
        }
        GUILayout.Label("Создать район");
        EditorGUILayout.EndHorizontal();

    }
    [MenuItem("Tools/Create Zone")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(t: typeof(CreateZone));
    }
}
