using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindTrap : MonoBehaviour
{
    public GameObject openwind;
    public GameObject closewind;
    public GameObject trap;

    public int maxHealth = 2;
    public int currentHealth;
    public float rollSpeed = 10f;

    public bool isRotating = true;

    public void Start()
    {
        currentHealth = maxHealth;

    }
    private void FixedUpdate()
    {
        if(isRotating)
        TrapRotate();
    }
    void TrapRotate()
    {
        trap.transform.Rotate(Vector3.right * rollSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ = GameObject.FindWithTag("Player").GetComponent<PlayerHP>().PlayerHPMinus();
        }
    }
    public void StopBox()
    {
        isRotating = !isRotating;

    }
    
}
