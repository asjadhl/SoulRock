using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LastBossMove : MonoBehaviour
{
    //GameObject player;
    private void Start()
    {
        //player = GameObject.FindWithTag("Player");
    }
    
    private void Update()
    {
        //transform.LookAt(player.transform);
    }
    public void HitGhostBoss()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
    }
}


