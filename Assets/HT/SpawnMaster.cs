using System.Collections;
using System.Collections.Generic;
using UnityEngine;




#if UNITY_EDITOR
using UnityEditor;
#endif


public enum SpawnOption
{
    random, target
}

[System.Serializable]
public class Entity
{
    public GameObject EntityObj;
    public int EntitySpawnCount;
}

[System.Serializable]
public class AreaSpawn
{
    public SpawnOption spawnoption;
    [Range(0f, 10f)]
    public float SpawnRate;
    public float radius;
    public GameObject SpawnerPos;
    public List<Entity> EntityList;
}

public class SpawnMaster : MonoBehaviour
{

    [Header("SpawnMaster(0.1v)")]
    [SerializeField]
    Transform m_currentPlayerTransform;


    [Space(5)]
    [SerializeField]
    List<AreaSpawn> areaSpawns;
    [SerializeField, HideInInspector]
    private string selectedTag;



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
            if (IsInsideCircle(m_currentPlayerTransform.position, areaSpawns[i].SpawnerPos.transform.position, areaSpawns[i].radius))
            {
                StartCoroutine(SpawnNow(areaSpawns[i]));
                areaSpawns.Remove(areaSpawns[i]);
            }
        }
    }


    private void Awake()
    {
        m_currentPlayerTransform = FindPlayer();
        Debug.Log(selectedTag);
    }

    private void Update()
    {
        PlayerCheckUpdate();
    }


    IEnumerator SpawnNow(AreaSpawn a)
    {
        int randomindex = 0;
        Vector3 spawnPos;
        while (a.EntityList.Count > 0)
        {
            //Permant Random Enemy Spawn Not Position
            randomindex = Random.Range(0, a.EntityList.Count);

            if (a.EntityList[randomindex].EntitySpawnCount > 0)
            {
                if (a.spawnoption == SpawnOption.random)
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    float distance = Mathf.Sqrt(Random.value) * a.radius;
                    float x = Mathf.Cos(angle) * distance;
                    float z = Mathf.Sin(angle) * distance;
                    spawnPos = new Vector3(a.SpawnerPos.transform.position.x + x,
                                            a.SpawnerPos.transform.position.y,
                                            a.SpawnerPos.transform.position.z + z);
                }
                else
                {
                    // Center spawn
                    spawnPos = a.SpawnerPos.transform.position;
                }
                Instantiate(a.EntityList[randomindex].EntityObj, spawnPos, Quaternion.identity);

                a.EntityList[randomindex].EntitySpawnCount -= 1;
                if (a.EntityList[randomindex].EntitySpawnCount <= 0)
                    a.EntityList.Remove(a.EntityList[randomindex]);

                yield return new WaitForSeconds(a.SpawnRate);
            }
        }




    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnMaster))]
public class TagExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {

        SpawnMaster t = (SpawnMaster)target;



        // Draw a nice Tag dropdown like Unity Layer dropdown
        t.SetSelectedTag(EditorGUILayout.TagField("Target-Tag", t.GetSelectedTag()));

        if (GUI.changed)
        {
            EditorUtility.SetDirty(t);
        }
        DrawDefaultInspector();

    }
}
#endif