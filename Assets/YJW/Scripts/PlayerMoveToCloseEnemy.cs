using UnityEngine;

public class PlayerMoveToCloseEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;

    
    private void PlayerMove()
    {
        GameObject newEnemy = Instantiate(enemyPrefab);
        EnemyTest.EnemySpawned(newEnemy);
    }
    public void HandleEnemySpawned(GameObject enemy)
    {
        PlayerMove();
    }
}
