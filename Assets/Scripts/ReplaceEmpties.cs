using UnityEngine;
using UnityEditor;

public class ReplaceEmpties : EditorWindow
{
    public GameObject prefab;  
    private GameObject[] empties;

    [MenuItem("Tools/Replace Empties with Prefab")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceEmpties>("Replace Empties with Prefab");
    }

    private void OnGUI()
    {
        //Field to assign the prefab
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        //Button to initiate the replacement
        if (GUILayout.Button("Replace Selected Empties"))
        {
            ReplaceSelectedEmpties();
        }
    }

    private void ReplaceSelectedEmpties()
    {
        
        if (prefab == null)
        {
            Debug.LogError("Please assign a prefab to replace the empty objects.");
            return;
        }

        //Get selected GameObjects in the hierarchy
        empties = Selection.gameObjects;

        foreach (GameObject empty in empties)
        {
            //Check if the selected GameObject is an empty
            if (empty != null && empty.transform.childCount == 0)
            {
                //Store the original position and rotation of the empty
                Vector3 position = empty.transform.position;
                Quaternion rotation = empty.transform.rotation;

                //Instantiate the prefab at the empty's position and rotation
                GameObject newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (newObject != null)
                {
                    newObject.transform.position = position;
                    newObject.transform.rotation = rotation;

                    //Optionally, set the new object's parent to the empty's parent
                    newObject.transform.parent = empty.transform.parent;

                    //Destroy the empty object
                    DestroyImmediate(empty);
                }
            }
            else
            {
                Debug.LogWarning($"{empty.name} is not an empty GameObject. SKIP!");
            }
        }
    }
}