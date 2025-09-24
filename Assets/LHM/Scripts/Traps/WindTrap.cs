using System.Collections.Generic;
using UnityEngine;

public class WindTrap : MonoBehaviour
{
    public GameObject openwind;
    public GameObject closewind;
    public GameObject trap;

    public int maxHealth = 5;
    public int currentHealth;
    public float rollSpeed = 10f;

    public void Awake()
    {
        
    }
    private void FixedUpdate()
    {
        trap.transform.Rotate(Vector3.forward * rollSpeed * Time.deltaTime);
    }
    void StopBox()
    {
       
    }
    
}
