using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs;
    private float attackTime = 4f;
    private float attackTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BossAttack_()
    {
        attackTimer += Time.deltaTime;
    }
}
