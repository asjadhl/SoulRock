using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Stage2BossAttack : MonoBehaviour
{
    /*
    보스 머리 위 4개의 트럼프 문양에 따라 달라지는 패턴
    
    하트 : 패턴 발동시 떠다니는 쪼만한 하트들이 생김.
    그 하트들을 죽이지 못한만큼 보스는 그대로 흡수해 피회복.
    쪼만한 하트들 갯수는 default 4개 + 클럽 스택 수만큼

    스페이드 : 골카, 빨카로 빠른 공격
    빨카는 걍 데미지, 골카 맞으면 3초동안 도트 투명해짐.
    이미 맞은 상태에서 골카 또 맞을 시 시간만 3초로 초기화.

    다이아 : 뿅뿅보스
    보스 랜덤위치로 순간이동. 3번정도 연속으로 놓치면 돌진 후 공격.
    돌진 패턴 끝난 후에는 다시 뿅뿅

    클럽 : 빨강색 구슬과 검정색 구슬을 던짐.
    빨간색 구슬은 쏘지 않고 맞아야 하고, 검정색 구슬은 쏴야함.
    빨간색을 잘못 쏘거나 검정색을 쏘지 않으면 아래에 스택이 쌓임
    스택은 7개 모으면 즉사
    */

    public static int clubStack = 0;
    int reMiniH;


    [SerializeField] Card[] cards;
    Card currentCard;

    [SerializeField] Image bossCardImage;
    public Shape curShape;

    GameObject player;

    private float teleportTimer = 0;
    private int teleportCount = 0;
    public int playerHitCount = 0;
    private bool miniBossSpawned = false;


    [SerializeField] GameObject[] miniBoss;
    public Transform[] spawnPos;
    public List<int> usedPos;


    [SerializeField] GameObject[] spadeCards;
    private float spadeTimer = 0;
    private bool isCardSpawn = false;


    [SerializeField] GameObject[] clubBalls;
    private float clubTimer = 0;
    private bool isBallSpawn = false;

    private void Start()
    {
        ChangeNextRanCard();
        player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        switch (curShape)
        {
            case Shape.H:
                if(miniBossSpawned == false) 
                    HAttack();
                break;
            case Shape.S:
                spadeTimer += Time.fixedDeltaTime;
                if(spadeTimer <= 10)
                {
                    if(isCardSpawn == false)
                        _ = SAttack();
                }
                else
                {
                    ChangeNextRanCard();
                    spadeTimer = 0;
                }
                break;
            case Shape.D:
                teleportTimer += Time.fixedDeltaTime;
                DAttack();
                break;
            case Shape.C:
                clubTimer += Time.fixedDeltaTime;
                if (clubTimer <= 10)
                {
                    if (isBallSpawn == false)
                        _ = CAttack();
                }
                else
                {
                    ChangeNextRanCard();
                    clubTimer = 0;
                }
                break;
        }
    }

    private void SetCardData()
    {
        bossCardImage.sprite = currentCard.icon;
        curShape = currentCard.shape;
    }

    private void ChangeNextRanCard()
    {
        currentCard = cards[Random.Range(0, cards.Length)];
        SetCardData();
    }

    private async void HAttack()
    {
        reMiniH = clubStack;
        for(int i = 0; i < clubStack + 4; i++)
        {
            miniBoss[i].SetActive(true);
        }
        miniBossSpawned = true;
        await UniTask.Delay(10000);
        usedPos.Clear();
        ChangeNextRanCard();
        for(int i = 0; i <= clubStack + 4; i++)
            miniBoss[i].SetActive(false);
        miniBossSpawned = false;
        GetComponent<BossHP>().bossHP += reMiniH;
    }
    
    private async UniTask SAttack()
    {
        Debug.Log("스페이드");

        GoldOrRedCardOn();
        isCardSpawn = true;
        await UniTask.Delay(1500);
        isCardSpawn = false;
    }

    private void DAttack()
    {
        Debug.Log("다이아");
        if(teleportCount < 5)
        {
            if (teleportTimer >= 2)
            {
                MoveToRanPos();
                teleportTimer = 0;
            }
        }
        else
        {
            if(playerHitCount < 8)
            {
                BossRush();
            }
            else
            {
                transform.position = new Vector3(0, transform.position.y, player.transform.position.z + 17);
                ChangeNextRanCard();
            }
        }
    }

    private async UniTask CAttack()
    {
        Debug.Log("클로버");

        RedOrBlackBallOn();
        isBallSpawn = true;
        await UniTask.Delay(2500);
        isBallSpawn = false;
    }

    // 다이이 패턴
    private void MoveToRanPos()
    {
        int x = Random.Range(-8, 9);
        int z = (int)player.transform.position.z + Random.Range(10, 20);

        transform.position = new Vector3(x, transform.position.y , z);

        teleportCount++;
    }

    private void BossRush()
    {
        transform.LookAt(player.transform);
        transform.Translate(Vector3.forward * 15 * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerHP>().PlayerHPMinus();
            transform.rotation = Quaternion.Euler(0,180,0);
            transform.position = new Vector3(0, 2, player.transform.position.z + 17);
            ChangeNextRanCard();
            teleportCount = 0;
            playerHitCount = 0;
        }
    }

    // 하트 패턴
    public int SetMiniBossRanPos()
    {
        int index = Random.Range(0, spawnPos.Length);
        return index;
    }

    public void AddList(int a)
    {
        usedPos.Add(a);
    }

    public void reMiniHMinus()
    {
        reMiniH--;
    }

    // 스페이드 패턴
    private void GoldOrRedCardOn()
    {
        int ranIndex = Random.Range(0, spadeCards.Length);

        spadeCards[ranIndex].SetActive(true);
    }

    // 클럽 패턴
    private void RedOrBlackBallOn()
    {
        int ranIndex = Random.Range(0, clubBalls.Length);

        clubBalls[ranIndex].SetActive(true);
    }
}
