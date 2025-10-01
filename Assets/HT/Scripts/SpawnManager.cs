using System.Collections.Generic;
using UnityEngine; 
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
  public Transform Scanner;
  [Range(0f, 10f)]
  public float SpawnRate;
  public float radius;
  public GameObject SpawnerPosition;
  public List<Entity> EntityList;

  public static  AreaSpawn operator+ (AreaSpawn a ,AreaSpawn b)
  {  
    a.spawnoption = b.spawnoption;
    a.arcHeight = b.arcHeight;
    a.Scanner = b.Scanner;
    a.SpawnRate = b.SpawnRate;
    a.radius = b.radius;
    a.SpawnerPosition = b.SpawnerPosition;
    a.EntityList = new();
    for (int i = 0; i < b.EntityList.Count; i++)
    a.EntityList.Add(new Entity() { EntityObj = b.EntityList[i].EntityObj, EntitySpawnCount = b.EntityList[i].EntitySpawnCount });

    
    return a;
  }

}

public class SpawnManager : MonoBehaviour
{

  [Header("SpawnManager(0.v5)")]
  [SerializeField]
  Transform m_targetTransform;


  [Space(5)]
  public List<AreaSpawn> areaSpawns;
          List<AreaSpawn> Save_areaSpawns;
  [SerializeField, HideInInspector]
  private string selectedTag;
  CancellationTokenSource cts;
  //optional
  bool IsSpawning = false;


  
 

  public Transform FindPlayer()
  {
    return GameObject.FindGameObjectWithTag(selectedTag).transform;
  }
  

  public   void OnEnable()
  {
      
   
    if(areaSpawns != null)
    {
      
      if (Save_areaSpawns != null)
      {
        for (int i = 0; i < Save_areaSpawns.Count; i++)
        {

          areaSpawns.Add(new AreaSpawn() + Save_areaSpawns[i]);

        }
         
      }
    }
    cts = new();
  }

  
  private void Start()
  {
    m_targetTransform = FindPlayer();
    
    Save_areaSpawns = new();
   
    if (areaSpawns != null)
    {
      if (areaSpawns.Count > 0)
      {
        for (int i = 0; i < areaSpawns.Count; i++)
        {
          Save_areaSpawns.Add(new AreaSpawn() + areaSpawns[i]);
        }

      }
      
    }
   
  }
  public async UniTaskVoid SpawnNow(AreaSpawn areaspawn, CancellationTokenSource cts)
  {

    if (areaspawn.EntityList.Count > 0)
    {
      int randomindex = 0;



      switch (areaspawn.spawnoption)
      {
        case AreaSpawn.SpawnOption.random:

          Vector3 spawnPos;
          while (areaspawn.EntityList.Count > 0)
          {
            randomindex = Random.Range(0, areaspawn.EntityList.Count);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Mathf.Sqrt(Random.value) * areaspawn.radius;
            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;
            spawnPos = new Vector3(areaspawn.SpawnerPosition.transform.position.x + x,
                                    areaspawn.SpawnerPosition.transform.position.y,
                                    areaspawn.SpawnerPosition.transform.position.z + z);
            Instantiate(areaspawn.EntityList[0].EntityObj, spawnPos, Quaternion.identity);

            areaspawn.EntityList[0].EntitySpawnCount--;
            if (areaspawn.EntityList[0].EntitySpawnCount <= 0)
              areaspawn.EntityList.Remove(areaspawn.EntityList[randomindex]);

            await UniTask.WaitForSeconds(areaspawn.SpawnRate, cancellationToken: cts.Token);
          }

          break;

        case AreaSpawn.SpawnOption.target:
          {
            
            while (areaspawn.EntityList.Count > 0)
            {
              randomindex = Random.Range(0, areaspawn.EntityList.Count);
              //
              float angle = Random.Range(0f, Mathf.PI * 2f);
              float distance = Mathf.Sqrt(Random.value) * areaspawn.radius;
              float x = Mathf.Cos(angle) * distance;
              float z = Mathf.Sin(angle) * distance;
              spawnPos = new Vector3(areaspawn.SpawnerPosition.transform.position.x + x,
                                      areaspawn.SpawnerPosition.transform.position.y,
                                      areaspawn.SpawnerPosition.transform.position.z + z);
              Instantiate(areaspawn.EntityList[randomindex].EntityObj, spawnPos, Quaternion.identity);
              //
              //Instantiate(areaspawn.EntityList[randomindex].EntityObj, areaspawn.SpawnerPosition.transform.position, Quaternion.identity);

              areaspawn.EntityList[randomindex].EntitySpawnCount -= 1;

              if (areaspawn.EntityList[randomindex].EntitySpawnCount <= 0)
                areaspawn.EntityList.Remove(areaspawn.EntityList[randomindex]);

              await UniTask.WaitForSeconds(areaspawn.SpawnRate, cancellationToken: cts.Token);
            }

          }
          break;

      }

    }

    areaSpawns.Remove(areaSpawns[0]);
    IsSpawning = false;
  }
  public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
  {
    return (pos - center).sqrMagnitude <= radius * radius;
  }
  public void PlayerCheckUpdate()
  {

    if (m_targetTransform == null)
      return;
    if (areaSpawns == null)
      return;
    if (IsSpawning)
      return;
    if (areaSpawns.Count >= 1)
    {   

      if (IsInsideCircle(m_targetTransform.position,
        areaSpawns[0].spawnoption == AreaSpawn.SpawnOption.random ? areaSpawns[0].SpawnerPosition.transform.position : areaSpawns[0].Scanner.transform.position
        , areaSpawns[0].radius))
      {
        IsSpawning = true;
        SpawnNow(areaSpawns[0], cts).Forget();
        //areaSpawns.Remove(areaSpawns[0]);

      }
    }
  }
  private void Update()
  {
    PlayerCheckUpdate();
  }


  public void OnDisable()
  {
    if (cts != null)
    { 
      areaSpawns.Clear();
      IsSpawning = false;
      cts.Cancel();
      cts.Dispose();
    }
  }


 




  public string GetSelectedTag()
  {
    return selectedTag;
  }

  public void SetSelectedTag(string _tags)
  {

    selectedTag = _tags;
  }

}



#if UNITY_EDITOR
 
 
[CustomEditor(typeof(SpawnManager))]
public class ShowScanners : Editor
{


  public override void OnInspectorGUI()
  {



    SpawnManager t = (SpawnManager)target;



    t.SetSelectedTag(EditorGUILayout.TagField("Tag", t.GetSelectedTag()));

    if (GUI.changed)
    {
      EditorUtility.SetDirty(t);
    }




     DrawDefaultInspector();
    

  }

  void OnSceneGUI()
  {
    SpawnManager t = (SpawnManager)target;



    for (int i = 0; i < t.areaSpawns.Count; i++)
    {
      if (t.areaSpawns[i].spawnoption == AreaSpawn.SpawnOption.random)
      {

        if (t.areaSpawns[i].SpawnerPosition == null)
          continue;

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(t.areaSpawns[i].SpawnerPosition.transform.position, Vector3.up, t.areaSpawns[i].radius);
        UnityEditor.Handles.Label(t.areaSpawns[i].SpawnerPosition.transform.position + Vector3.up * 0.5f, $"SpawnPosition[{i}]");
      }
      else
      {
        if (t.areaSpawns[i].SpawnerPosition == null || t.areaSpawns[i].Scanner == null)
          continue;

        Vector3 start = t.areaSpawns[i].SpawnerPosition.transform.position;
        Vector3 end = t.areaSpawns[i].Scanner.position;







        // Midpoint for curved arrow
        Vector3 mid = (start + end) * 0.5f;
        mid.y += 7f;

        // Determine color based on index
        Color arrowColor = Color.HSVToRGB(0.6f, 1f, 1f);




        UnityEditor.Handles.DrawBezier(start, end, mid, mid, arrowColor, null, 1f);



        // Optional: label with index
        UnityEditor.Handles.Label(end + Vector3.up * 0.5f, $"Scanner[{i}]");

        UnityEditor.Handles.color = Color.cyan;

        UnityEditor.Handles.DrawWireDisc(end, Vector3.up, t.areaSpawns[i].radius);
        UnityEditor.Handles.Label(start + Vector3.up * 0.5f, $"SpawnPosition[{i}]");

      }
    }


  }

}



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

    // Draw Scanner only if spawnoption == target
    if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.target)
    {
      SerializedProperty targetProp = property.FindPropertyRelative("Scanner");
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
      height += lineHeight + spacing; // for Scanner

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




//[CustomPropertyDrawer(typeof(SpawnMaster))]
//public class SpawnMasterDrawer : PropertyDrawer
//{
//  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//  {
//    EditorGUI.BeginProperty(position, label, property);
//  }
//}
#endif



