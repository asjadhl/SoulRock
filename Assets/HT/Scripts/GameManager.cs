using System.Collections.Generic;
using UnityEngine;





public enum EntityType
{
    Walker = 0,Fly =1
}
public enum EntityI
{
    Friendly,chonglib,Hostile
}
[System.Serializable]
public class EnemiesType
{
    public  GameObject enemyobject;
    public  EntityType type;
    public  EntityI    entityI;

}


 
public class GameManager : MonoBehaviour
{

    public Dictionary<string, List<EnemiesType>> dictionarys;
    public Dictionary<string, List<GameObject>> dict1;
    public int LenghtOfStringData;
    public static GameManager instance;
    
    public List<EnemiesType> GhostEnemies;

    public  List<GameObject>  Particales;

  public GameObject GetParticale(int index, Vector3 current)
  {

   return Instantiate(Particales[index], current, Quaternion.identity);
  
  }


   public   Transform Min;
   public   Transform Max; 

    public Vector3 Clamp(Vector3 current)
    {
     
    Vector3 result;
    float xmin = Min.localPosition.x;
   
    float xmax = Max.localPosition.x;
    
    float ymin = Min.localPosition.y;
  
    float ymax = Max.localPosition.y;
    
    result.x = Mathf.Clamp(current.x, xmin, xmax);
    
    result.y = Mathf.Clamp(current.y, ymin, ymax);
 
    result.z = current.z;
    return result;
    }
    public GameObject CreateEnemy(string datas)
    {

        if (dict1.ContainsKey(datas))
        {   
         
            int randomIndex = UnityEngine.Random.Range(0, dict1[datas].Count);
            var obj = Instantiate(dict1[datas][randomIndex], GameObject.Find("EnemySpawner").transform.position, Quaternion.identity);
           
            obj.name += $"[{(EntityI)int.Parse(datas[1].ToString())}]";
            return obj;
        }
        else  
            return null;
    }
   
  
    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
        {
            dictionarys = new();
            if(GhostEnemies != null)
            {
                 
                dict1 = new();
                foreach(var each in GhostEnemies)
                {
                    string newdata = ((int)each.type).ToString() + ((int)each.entityI).ToString();
                  
                    if (!dict1.ContainsKey(newdata))
                        dict1.Add(newdata, new List<GameObject>());

                    dict1[newdata].Add(each.enemyobject);
                     
                }
               
            }

     
       
      
       
        
      }
       
 
      instance = this;

        }


    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {

            var a = hit.collider.GetComponent<LockOnDodgeEnemy>();
            if (a != null)
            {
                a.TriggerDodge();
            }


        }
    }





}
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(GameManager))]
public class ShowSize : UnityEditor.Editor
{
  void OnSceneGUI()
  {
    GameManager t = (GameManager)target;
    
    if(t != null)
    {

      Vector3 start = t.Min.localPosition;
      Vector3 end = new Vector3(t.Max.localPosition.x, t.Min.localPosition.y, 0);

      UnityEditor.Handles.color = Color.cyan;
      UnityEditor.Handles.DrawLine(start, end);  
      end = new Vector3(t.Min.localPosition.x, t.Max.localPosition.y, 0);
      UnityEditor.Handles.DrawLine(start, end); 
      start = new Vector3(t.Min.localPosition.x, t.Max.localPosition.y, 0);
      end = t.Max.localPosition;
      UnityEditor.Handles.color = Color.yellow;
      UnityEditor.Handles.DrawLine(start, end);
      start = new Vector3(t.Max.localPosition.x, t.Min.localPosition.y, 0);
      UnityEditor.Handles.DrawLine(start, end);

    }
  }

}
#endif