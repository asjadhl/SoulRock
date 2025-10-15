
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class Bridge
{

}

public class Generator : MonoBehaviour
{
  public List<GameObject> Queune;
  public Transform PlayerTransform;
  public Transform PrevTransform;
  public GameObject g;
  public Vector3 StartGenerate;
  public Vector3 GenerateOffSet;
  public Vector3 TriggeredOffset;
  [HideInInspector]
  public Vector3 TrueOffset;
  public float Radius;
  //zpos between two 95
  private void Generate()
  {
    //GameManager.instance.GetRandomPath

     int dex = UnityEngine.Random.Range(0,2);
    GameObject EmptyGameObject;


 

    //Left
    if (dex == 0)
    {
      Debug.Log("LEFT");
   
      EmptyGameObject = Instantiate(g,transform.position, Quaternion.identity);
      EmptyGameObject.transform.position = PrevTransform.position + (PrevTransform.transform.right *GenerateOffSet.x);
      Debug.Log($"PrevTransform.position: {PrevTransform.position}, PrevTransform.transform.right: {PrevTransform.transform.right},-GenerateOffSet.x: {GenerateOffSet.x},(PrevTransform.transform.right * -GenerateOffSet.x) : {(PrevTransform.transform.right * GenerateOffSet.x)} ");
      EmptyGameObject.transform.position = PrevTransform.position + (PrevTransform.transform.forward *GenerateOffSet.z);
      EmptyGameObject.transform.rotation = Quaternion.LookRotation(-PrevTransform.transform.right);
      Queune.Add(EmptyGameObject);
    }
    //Right
    else if(dex == 1)
    {
      Debug.Log("RIGHT");
      EmptyGameObject = Instantiate(g, transform.position, Quaternion.identity);
      EmptyGameObject.transform.position = PrevTransform.position + (PrevTransform.transform.right * -GenerateOffSet.x);
      EmptyGameObject.transform.position = PrevTransform.position + (PrevTransform.transform.forward * -GenerateOffSet.z);
      EmptyGameObject.transform.rotation = Quaternion.LookRotation(PrevTransform.transform.right);
      Queune.Add(EmptyGameObject);
    }
    

    
   

  }
  public static bool IsInsideCircle(Vector3 pos, Vector3 center, float radius)
  {
    return (pos - center).sqrMagnitude <= radius * radius;
  }

  public void FindPlayer()
  {
    PlayerTransform = GameObject.Find("Player").transform;
    if (PlayerTransform != null)
      return;
    PlayerTransform = GameObject.FindWithTag("Player").transform;
    if (PlayerTransform != null)
      return;

    Debug.LogError("ImplementPlayer");
  }
  private void Start()
  {
    FindPlayer();
    StartGenerate = transform.position;
    GameObject EmptyGameObject;
    EmptyGameObject = Instantiate(g, PlayerTransform.position+new Vector3(0,0,GenerateOffSet.x), Quaternion.identity);
    EmptyGameObject.transform.eulerAngles = new Vector3 (0, 180, 0);
    Queune = new();
    Queune.Add(EmptyGameObject);
    //Generate();
  }
   

public void Update() 
  {

    if (Queune != null)
      if (Queune.Count <= 0)
        return;

   

      TrueOffset = Queune[0].transform.forward * TriggeredOffset.x;
      TrueOffset += Queune[0].transform.forward * TriggeredOffset.y;
      TrueOffset += Queune[0].transform.forward * TriggeredOffset.z;
     if(IsInsideCircle(PlayerTransform.position, Queune[0].transform.position+ TrueOffset, Radius))
     {

       
      PrevTransform = Queune[0].transform;
      Queune.Remove(Queune[0]); 
        Generate();
     }
  }
}


[CustomEditor(typeof(Generator))]
public class ShowTrueOffset : Editor
{


 

  void OnSceneGUI()
  {
    Generator t = (Generator)target;

    if (t.Queune != null)
      if (t.Queune.Count <= 0)
        return;
    UnityEditor.Handles.color = Color.yellow;

     

    Vector3 start = t.Queune[0].transform.position + t.TrueOffset;


    UnityEditor.Handles.DrawWireDisc(start, Vector3.up,t.Radius);

    UnityEditor.Handles.Label(start + Vector3.up * 0.5f, $"TriggeredOffset{start}");

      
    


  }

}

