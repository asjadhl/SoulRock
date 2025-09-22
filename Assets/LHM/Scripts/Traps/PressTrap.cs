using UnityEngine;

public class PressTrap : MonoBehaviour
{
    public Transform LeftPrees;
    public Transform RightPress;
    public int maxHealth = 2;
    int currentHealth;

    public float pressSpeed = 5f;
    public float leftPressLocalX = -2f;
    public float rightPressLocalX = 2f;

    float leftOpenLocalX;
    float rightOpenLocalX;
    Vector3 leftPressLocal;
    Vector3 rightPressLocal;
    Coroutine closeRoutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
