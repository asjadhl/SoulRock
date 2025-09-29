
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.PackageManager.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;










#if UNITY_EDITOR
using UnityEditor;
#endif




[System.Serializable]
public class Entity
{
  public GameObject EntityObj;
  public int EntitySpawnCount;
}

[System.Serializable]
public class AreaSpawn
{
  public enum SpawnOption
  {
    random, target
  }

  public SpawnOption spawnoption;



  [Range(0f, 10f)] public float arcHeight = 2f;
  public Transform targetTransform;
  [Range(0f, 10f)]
  public float SpawnRate;
  public float radius;
  public GameObject SpawnerPosition;
  public List<Entity> EntityList;


}

public class SpawnMaster : MonoBehaviour
{

  [Header("SpawnMaster(0.v2)")]
  [SerializeField]
  Transform m_currentPlayerTransform;


  [Space(5)]
  public List<AreaSpawn> areaSpawns;
  [SerializeField, HideInInspector]
  private string selectedTag;
  CancellationTokenSource cts;



  public string GetSelectedTag()
  {
    return selectedTag;
  }

  public void SetSelectedTag(string _tags)
  {

    selectedTag = _tags;
  }

  public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
  {
    return (pos - center).sqrMagnitude <= radius * radius;
  }

  public Transform FindPlayer()
  {
    return GameObject.FindGameObjectWithTag(selectedTag).transform;
  }
  public void PlayerCheckUpdate()
  {

    if (m_currentPlayerTransform == null)
      return;

    for (int i = 0; i < areaSpawns.Count; i++)
    {
      if (IsInsideCircle(m_currentPlayerTransform.position, areaSpawns[i].SpawnerPosition.transform.position, areaSpawns[i].radius))
      {
         
        SpawnNow(areaSpawns[i],cts).Forget();
         areaSpawns.Remove(areaSpawns[i]);
      }
    }
  }


  private void Start()
  {
    m_currentPlayerTransform = FindPlayer();
    cts = new();
  }

  private void Update()
  {
    PlayerCheckUpdate();
  }


  public void OnDisable()
  {
    if(cts != null)
       cts.Cancel();
  }


  public async UniTaskVoid SpawnNow(AreaSpawn areaspawn, CancellationTokenSource cts)
  {

    if (areaspawn.EntityList.Count > 0)
    {
      int randomindex = 0;


      switch (areaspawn.spawnoption)
      {
        case AreaSpawn.SpawnOption.random:
          {
           
            Vector3 spawnPos;
            while (areaspawn.EntityList.Count > 0)
            {
              randomindex = Random.Range(0, areaspawn.EntityList.Count);
              float angle = Random.Range(0f, Mathf.PI * 2f);
              float distance = Mathf.Sqrt(Random.value) * areaspawn.radius;
              float x = Mathf.Cos(angle) * distance;
              float z = Mathf.Sin(angle) * distance;
              spawnPos = new Vector3( areaspawn.SpawnerPosition.transform.position.x + x,
                                      areaspawn.SpawnerPosition.transform.position.y,
                                      areaspawn.SpawnerPosition.transform.position.z + z);


              Instantiate(areaspawn.EntityList[randomindex].EntityObj, spawnPos, Quaternion.identity);


                  areaspawn.EntityList[randomindex].EntitySpawnCount -= 1;
              if (areaspawn.EntityList[randomindex].EntitySpawnCount <= 0)
                  areaspawn.EntityList.Remove(areaspawn.EntityList[randomindex]);

              await UniTask.WaitForSeconds(areaspawn.SpawnRate, cancellationToken: cts.Token);
            }
          }
          break;
        case AreaSpawn.SpawnOption.target:
          {   
            while (areaspawn.EntityList.Count > 0)
            {
              randomindex = Random.Range(0, areaspawn.EntityList.Count);
              Instantiate(areaspawn.EntityList[randomindex].EntityObj, areaspawn.targetTransform.position, Quaternion.identity);

                  areaspawn.EntityList[randomindex].EntitySpawnCount -= 1;
              if (areaspawn.EntityList[randomindex].EntitySpawnCount <= 0)
                  areaspawn.EntityList.Remove(areaspawn.EntityList[randomindex]);

                  await UniTask.WaitForSeconds(areaspawn.SpawnRate, cancellationToken: cts.Token);
            }
          }
          break;
        default:
          await UniTask.Yield();
          break;
      }
    }



  }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnMaster))]
public class ShowTag : Editor
{
  public override void OnInspectorGUI()
  {

    SpawnMaster t = (SpawnMaster)target;



    t.SetSelectedTag(EditorGUILayout.TagField("Target-Tag", t.GetSelectedTag()));

    if (GUI.changed)
    {
      EditorUtility.SetDirty(t);
    }




    DrawDefaultInspector();

  }

 
}
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnMaster))]
public class ShowScanners : Editor
{
  void OnSceneGUI()
  {
    SpawnMaster t = (SpawnMaster)target;



    for (int i = 0; i < t.areaSpawns.Count; i++)
    {
      if (t.areaSpawns[i].spawnoption == AreaSpawn.SpawnOption.random)
        continue;

      if (t.areaSpawns[i].SpawnerPosition == null || t.areaSpawns[i].targetTransform == null)
        continue;

        Vector3 start = t.areaSpawns[i].SpawnerPosition.transform.position;
        Vector3 end = t.areaSpawns[i].targetTransform.position;







      // Midpoint for curved arrow
      Vector3 mid = (start + end) * 0.5f;
      mid.y += 7f;

      // Determine color based on index
      Color arrowColor = Color.HSVToRGB(0.6f, 1f, 1f);




      UnityEditor.Handles.DrawBezier(start, end, mid, mid, arrowColor, null, 1f);

      

      // Optional: label with index
      UnityEditor.Handles.Label(start + Vector3.up * 0.5f, $"Scanner[{i}]");

      UnityEditor.Handles.color = Color.cyan;

      UnityEditor.Handles.DrawWireDisc(start, Vector3.up, t.areaSpawns[i].radius);
      UnityEditor.Handles.Label(end + Vector3.up * 0.5f, $"SpawnPosition[{i}]");


    }


  }

}
 

#endif
[CustomPropertyDrawer(typeof(AreaSpawn))]
public class AreaSpawnDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);

    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;

    // Rects for each field
    float lineHeight = EditorGUIUtility.singleLineHeight;
    float spacing = 2f;
    float y = position.y;

    // Draw the enum
    SerializedProperty spawnOptionProp = property.FindPropertyRelative("spawnoption");
    Rect enumRect = new Rect(position.x, y, position.width, lineHeight);
    EditorGUI.PropertyField(enumRect, spawnOptionProp);
    y += lineHeight + spacing;

    // Draw targetTransform only if spawnoption == target
    if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.target)
    {
      SerializedProperty targetProp = property.FindPropertyRelative("targetTransform");
      Rect targetRect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(targetRect, targetProp);
      y += lineHeight + spacing;


    }



    // Draw remaining fields
    string[] otherFields = { "SpawnRate", "radius", "SpawnerPosition", "EntityList" };
    foreach (string fieldName in otherFields)
    {
      SerializedProperty prop = property.FindPropertyRelative(fieldName);
      Rect rect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(rect, prop, true);
      y += lineHeight + spacing;

      // Adjust height for lists like EntityList
      if (prop.isArray && prop.isExpanded)
        y += EditorGUI.GetPropertyHeight(prop, true) - lineHeight;
    }

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    float lineHeight = EditorGUIUtility.singleLineHeight;
    float spacing = 2f;
    float height = lineHeight + spacing; // for spawnoption

    SerializedProperty spawnOptionProp = property.FindPropertyRelative("spawnoption");

    if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.target)
      height += lineHeight + spacing; // for targetTransform

    // Add remaining fields
    string[] otherFields = { "SpawnRate", "radius", "SpawnerPosition", "EntityList" };
    foreach (string fieldName in otherFields)
    {
      SerializedProperty prop = property.FindPropertyRelative(fieldName);
      height += lineHeight + spacing;

      // Add extra height if array is expanded
      if (prop.isArray && prop.isExpanded)
        height += EditorGUI.GetPropertyHeight(prop, true) - lineHeight;
    }

    return height;
  }




}
#endif



