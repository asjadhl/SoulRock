using System.Collections.Generic;
using UnityEngine; 
using Cysharp.Threading.Tasks;
using System.Threading;


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
  public Transform ScannerPosition;
  public float ScannerRadius;
  public float SpawnRadius;
  [Range(0f, 10f)]
  public float SpawnRate;
 
  public Transform SpawnerPosition;
  public List<Entity> EntityList;

  public static  AreaSpawn operator+ (AreaSpawn a ,AreaSpawn b)
  {  
    a.spawnoption = b.spawnoption;
    a.arcHeight = b.arcHeight;
    a.ScannerPosition = b.ScannerPosition;
    a.SpawnRate = b.SpawnRate;
    a.ScannerRadius = b.ScannerRadius;
    a.SpawnRadius = b.SpawnRadius;
    a.SpawnerPosition = b.SpawnerPosition;
    a.EntityList = new();
    for (int i = 0; i < b.EntityList.Count; i++)
    a.EntityList.Add(new Entity() { EntityObj = b.EntityList[i].EntityObj, EntitySpawnCount = b.EntityList[i].EntitySpawnCount });

    
    return a;
  }

}

public enum Mode
{
  NormalMode,FeverMode
}

public class SpawnManager : MonoBehaviour
{

  [Header("SpawnManager(0.v5)")]
  [SerializeField]
  Transform m_targetTransform;
  [SerializeField] Vector3 SpawnAreaOffset;
  public Mode MyMode;
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
  public GameObject FindScanner()
  {
        return GameObject.FindGameObjectWithTag("Scanner") != null ? GameObject.FindGameObjectWithTag("Scanner") : GameObject.Find("Scanner") != null ? GameObject.Find("Scanner"): null;
  }

    public GameObject FindScanners()
    {  

        GameObject result = GameObject.FindGameObjectWithTag("Scanner") != null ? GameObject.FindGameObjectWithTag("Scanner") : GameObject.Find("Scanner") != null ? GameObject.Find("Scanner") : null;

        if (result != null)
        {
            return result;
        }
        else
        {
            Transform ParentObject = FindPlayer();
            if (ParentObject != null)
            {
                result = ParentObject.Find("Scanner").gameObject;
                if (result != null)
                {
                    return result;
                }
                else
                {
                    for (int i = 0; i < ParentObject.childCount; i++)
                    {
                        if (ParentObject.GetChild(i).tag == "Scanner")
                        {
                            result = ParentObject.GetChild(i).gameObject;
                            return result;
                        }
                    }

                    
                }
            }
                return null;

        }

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
                    switch (areaSpawns[i].spawnoption)
                    {
                        case AreaSpawn.SpawnOption.target:
                            areaSpawns[i].ScannerPosition = FindScanners().transform;
                            break;
                        case AreaSpawn.SpawnOption.random:
                            areaSpawns[i].SpawnerPosition = FindScanners().transform;
                            break;
                    }



                   
                    Save_areaSpawns.Add(new AreaSpawn() + areaSpawns[i]);
                }

            }
             
                
      
    }
        cts = new(); //New
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
            float distance = Mathf.Sqrt(Random.value) * areaspawn.SpawnRadius;
            float x = Mathf.Cos(angle) * distance;
            float z = Mathf.Sin(angle) * distance;
            spawnPos = new Vector3(areaspawn.SpawnerPosition.transform.position.x + x + SpawnAreaOffset.x,
                                    areaspawn.SpawnerPosition.transform.position.y + SpawnAreaOffset.y,
                                    areaspawn.SpawnerPosition.transform.position.z + z + SpawnAreaOffset.z);
             Instantiate(areaspawn.EntityList[0].EntityObj, spawnPos, Quaternion.identity).GetComponent<Enemy>().SetMode(MyMode);

            areaspawn.EntityList[randomindex].EntitySpawnCount -= 1;

            if (areaspawn.EntityList[randomindex].EntitySpawnCount <= 0)
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
              float distance = Mathf.Sqrt(Random.value) * areaspawn.SpawnRadius;
              float x = Mathf.Cos(angle) * distance;
              float z = Mathf.Sin(angle) * distance;
              spawnPos = new Vector3(areaspawn.SpawnerPosition.transform.position.x + x + SpawnAreaOffset.x,
                                      areaspawn.SpawnerPosition.transform.position.y+ SpawnAreaOffset.y,
                                      areaspawn.SpawnerPosition.transform.position.z + z + SpawnAreaOffset.z);
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
          areaSpawns[0].spawnoption == AreaSpawn.SpawnOption.random ? areaSpawns[0].SpawnerPosition.transform.position : areaSpawns[0].ScannerPosition.transform.position
        , areaSpawns[0].spawnoption == AreaSpawn.SpawnOption.random ? areaSpawns[0].ScannerRadius : areaSpawns[0].ScannerRadius))
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
        //Debug.Log("OnDisable");
    if (cts != null)
    { 
      areaSpawns.Clear();
      IsSpawning = false;
      cts.Cancel();
      cts.Dispose();
      cts = new();
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

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(t.areaSpawns[i].SpawnerPosition.transform.position, Vector3.up, t.areaSpawns[i].ScannerRadius);
        UnityEditor.Handles.Label(t.areaSpawns[i].SpawnerPosition.transform.position + Vector3.up * 0.5f, $"SpawnPosition[{i}]");
      }
      else
      {
        if (t.areaSpawns[i].SpawnerPosition == null || t.areaSpawns[i].ScannerPosition == null)
          continue;

        Vector3 start = t.areaSpawns[i].SpawnerPosition.transform.position;
        Vector3 end = t.areaSpawns[i].ScannerPosition.position;







        // Midpoint for curved arrow
        Vector3 mid = (start + end) * 0.5f;
        mid.y += 7f;

        // Determine color based on index
         

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(start, Vector3.up, t.areaSpawns[i].SpawnRadius);


     
        UnityEditor.Handles.DrawBezier(start, end, mid, mid, Color.green, null, 1f);



        // Optional: label with index
        UnityEditor.Handles.Label(end + Vector3.up * 0.5f, $"ScannerPosition[{i}]");

        UnityEditor.Handles.color = Color.cyan;

        UnityEditor.Handles.DrawWireDisc(end, Vector3.up, t.areaSpawns[i].ScannerRadius);
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
      SerializedProperty targetProp = property.FindPropertyRelative("ScannerPosition");
      Rect targetRect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(targetRect, targetProp);
      y += lineHeight + spacing;
      //Optional
      targetProp = property.FindPropertyRelative("SpawnRadius");
      targetRect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(targetRect, targetProp);
      y += lineHeight + spacing;

      targetProp = property.FindPropertyRelative("ScannerRadius");
      targetRect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(targetRect, targetProp);
      y += lineHeight + spacing;
    }
    else if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.random)
    {
      SerializedProperty targetProp = property.FindPropertyRelative("ScannerRadius");
      Rect targetRect = new Rect(position.x, y, position.width, lineHeight);
      EditorGUI.PropertyField(targetRect, targetProp);
      y += lineHeight + spacing;
    }

      // Draw remaining fields

      string[] otherFields = {"SpawnRate", "SpawnerPosition", "EntityList" };
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
    float spacing = 0f;

    SerializedProperty spawnOptionProp = property.FindPropertyRelative("spawnoption");

    if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.random)
      spacing = 7f;
    else if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.target)
      spacing = 10f;

    float height = lineHeight + spacing; // for spawnoption

    

    if ((AreaSpawn.SpawnOption)spawnOptionProp.enumValueIndex == AreaSpawn.SpawnOption.target)
      height += lineHeight + spacing; // for Scanner

    // Add remaining fields
    string[] otherFields = {"SpawnRate", "SpawnerPosition", "EntityList" };
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



