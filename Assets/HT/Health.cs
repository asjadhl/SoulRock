using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
      
    public void Awake()
    {
        InstanteHealthBar();
    }



    void InstanteHealthBar()
    {
        GameObject rectHealthBar = new GameObject();
        rectHealthBar.AddComponent<RectTransform>();
    }
}



[CustomEditor(typeof(Health))]
public class Test: Editor
{

    public GUIStyle centeredStyle;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        centeredStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 13
        };


        GUILayout.Label("HEALTH-BAR-Rect");

        GUILayout.BeginHorizontal();



        GUILayout.EndHorizontal();

    }



}
