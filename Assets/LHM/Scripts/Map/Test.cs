using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class A
{
  public int age;
}
public class Test : MonoBehaviour
{

  public List<A> list;
  public List<A> list2;

  public void Start()
  {
    list2.Add(new A() { age = list[0].age });
  }



}
