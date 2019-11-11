using UnityEditor;
using UnityEngine;

namespace NM
{
    public class NMMenuItem : MonoBehaviour
    {
        [MenuItem("GameObject/NMSceneObject", false, 0)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject(NMMain.NMSceneGoName);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.transform.SetAsFirstSibling();
            Selection.activeObject = go;
        }
    }
}