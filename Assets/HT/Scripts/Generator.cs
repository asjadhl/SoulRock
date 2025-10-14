
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bridge
{

}

public class Generator : MonoBehaviour
{
  List<GameObject> Queune;
  public Transform PlayerTransform;
  public GameObject g;
  public Vector3 GenerateOffSet;
  //zpos between two 95
  private void Generate()
  {
    //GameManager.instance.GetRandomPath

     int dex = UnityEngine.Random.Range(0,2);
    GameObject EmptyGameObject;


    if (Queune == null)
      Queune = new();

    //Left
    if (dex == 0)
    {
      Vector3 temp = GenerateOffSet;
      temp.x = -temp.x;
      EmptyGameObject = Instantiate(g, transform.position + temp, Quaternion.identity);
      EmptyGameObject.transform.eulerAngles = new Vector3(0,-90f,0);
      Queune.Add(EmptyGameObject);
    }
    else if(dex == 1)
    {
      EmptyGameObject = Instantiate(g, transform.position + GenerateOffSet, Quaternion.identity);
      EmptyGameObject.transform.eulerAngles = new Vector3(0, 90f, 0);
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
    Generate();
  }

  public void Update()
  {
     if(IsInsideCircle(PlayerTransform.position, Queune[0].transform.position,5f))
     {  
        Queune.Remove(Queune[0]);
        Generate();
     }
  }
}
