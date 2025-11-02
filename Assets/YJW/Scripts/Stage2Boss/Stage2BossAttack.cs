using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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


    /// <summary>
    /// 1. 클럽 패턴 수정
    /// 2. 하트에서 넘어갈 때 패턴 동시에 나옴.
    /// 3. 하트 패턴 실패 시 상호작용
    /// </summary>

    private static int _clubStack = 0;
    public static int clubStack
    {
        get => _clubStack;
        set => _clubStack = Mathf.Clamp(value, 0, 7);
    }
    int reMiniH;
    private bool bossRecover = false;

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

    private BossHP BossHP;

    private bool isDelay = false;

    private bool cardChanged = false;

    private bool isHAttacking = false;
    private bool miniHeartTrue = false;

    [SerializeField] GameObject spinWheel;
    [SerializeField] GameObject spinCircle;
    private float spinSpeede = 10f;
    private bool isCAttacking = false;
    bool isAttack = false;
    public bool wheelStop = true;
    private int index = 0;
    [SerializeField] GameObject[] clubStackImage;
    NormalMusicBox normalMusicBox;
    TextManager textManager;
    private bool bossDialogueTriggered = false;
    [SerializeField] private DialogueUIManager dialogueUI;
    private void Start()
    {

        player = GameObject.FindWithTag("Player");
        textManager = FindObjectOfType<TextManager>();
        normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();
        BossHP = GetComponent<BossHP>();
        if (isDelay == false)
            StartDelay().Forget();
        dialogueUI = FindObjectOfType<DialogueUIManager>();
    }
    private void FixedUpdate()
    {
        ClubStackUIUpdate();
        if (!isDelay) return;
        if(!isAttack)
        {
            BossPattern();
        }
        if (normalMusicBox.MusicFin && !bossDialogueTriggered)
        {
            bossDialogueTriggered = true; 
            BossState.isBoss1Dead = true;
            dialogueUI.ShowDialogueUI(true);
            textManager.BossDialogueCheackAsync().Forget();
        }
    }
    private async UniTaskVoid PlayBossDialogueAsync()
    {
        await textManager.BossDialogueCheackAsync();
    }
    //public async UniTaskVoid DelayedDialogueCheckAsync()
    //{

    //    await bossTextManager.StartStageDialogueAsync(1);

    //    await bossTextManager.StartStageDialogueAsync(2);

    //    await bossTextManager.StartStageDialogueAsync(3);




    //    await UniTask.Delay(1000);
    //    SceneManager.LoadScene("StageSelect");
    //}

    private void BossPattern()
    {
        switch (curShape)
        {
            case Shape.H:
                if (!miniBossSpawned && !isHAttacking)
                {
                    isHAttacking = true;
                    HAttack().Forget();
                }
                break;

            case Shape.S:
                  SAttack().Forget();
                  
                break;

            case Shape.D:
                teleportTimer += Time.fixedDeltaTime;
                DAttack().Forget();
                break;
            case Shape.C:
                CAttack().Forget();
                break;

        }
    }

    private void SetCardData()
    {
        //bossCardImage.sprite = currentCard.icon;
        curShape = currentCard.shape;
    }

    private async UniTask ChangeNextRanCard()
    {
        //await RollCardEffect(rollCount: 12, delay: 80);
        await RollCardEffect(rollCount: 12, delay: 80);

        //currentCard = cards[Random.Range(0, cards.Length)];
        SetCardData();
    }

    private async UniTask RollCardEffect(int rollCount = 10, int delay = 100)
    {
        for (int i = 0; i < rollCount; i++)
        {
            var randomCard = cards[Random.Range(0, cards.Length)];
            currentCard = randomCard;
            bossCardImage.sprite = randomCard.icon;

            // 살짝 커졌다 줄어드는 효과
            bossCardImage.transform.localScale = Vector3.one * 1.2f;
            await UniTask.Delay(delay / 2);
            bossCardImage.transform.localScale = Vector3.one;
            await UniTask.Delay(delay / 2);
        }
    }

    private async UniTask HAttack()
    {
        isAttack = true;
        await UniTask.Delay(500);
        reMiniH = clubStack + 4;
        for(int i = 0; i < clubStack + 4; i++)
        {
            miniBoss[i].SetActive(true);
        }
        miniBossSpawned = true;
        await UniTask.Delay(10000);
        usedPos.Clear();


        //currentCard = cards[1];
        //SetCardData();

        checkFindTure();

        for (int i = 0; i <= clubStack + 4; i++)
        {
            if(miniBoss[i].activeSelf == true)
            {
                miniBoss[i].GetComponent<MiniBoss>().ReturnOriPos().Forget();
                miniBoss[i].SetActive(false);
            }
        }
        miniBossSpawned = false;

        isHAttacking = false;

        if (miniHeartTrue == false && isHAttacking == false)
        {
            player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
        }

        await ChangeNextRanCard();
        //cardChanged = false;
        miniHeartTrue = false;
        isAttack = false;
    }

    private async UniTask SAttack()
    {
        isAttack = true;
        Debug.Log("스페이드");
        for(int i = 0; i < 6; i++)
        {
            GoldOrRedCardOn();
            await UniTask.Delay(1500);
        }
        //isCardSpawn = true;
        //isCardSpawn = false;
        //cardChanged = false;
        await ChangeNextRanCard();
        isAttack = false;
    }

    private async UniTask DAttack()
    {
        isAttack = true;
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
            if(playerHitCount < 5)
            {
                BossRush();
            }
            else
            {
                transform.position = new Vector3(0, 0, player.transform.position.z + 17);
                //if (!cardChanged)
                //{
                    await ChangeNextRanCard();
                    //cardChanged = true;
                //}
                playerHitCount = 0;
                teleportTimer = 0;
                teleportCount = 0;
            }

        }
        isAttack = false;
    }

    private async UniTask CAttack()
    {
        isAttack = true;
        isCAttacking = true;
        spinWheel.SetActive(true);
        await SpinWheel();

        int result = spinWheel.GetComponentInChildren<Roulette>().GetNum();
        Debug.Log(result);
        clubStack += result;

        //await UniTask.Delay(7000);
        //if(cardChanged == false)
        //{
        await ChangeNextRanCard();
        //cardChanged = true;
        //}

        

        isCAttacking = false;
        //cardChanged = false;
        spinWheel.SetActive(false);
        isAttack = false;
        wheelStop = true;
    }

    // 다이이 패턴
    private void MoveToRanPos()
    {
        Vector3 currentPos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        Stage1ParticleManager.Instance.PlayClownEffect(currentPos);

        int x = Random.Range(-8, 9);
        int z = (int)player.transform.position.z + Random.Range(10, 20);

        transform.position = new Vector3(x, 0 , z);

        teleportCount++;
    }

    private void BossRush()
    {
        transform.LookAt(player.transform);
        transform.Translate(Vector3.forward * 30 * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
            transform.rotation = Quaternion.Euler(0,180,0);
            transform.position = new Vector3(0, 0, player.transform.position.z + 17);
            ChangeNextRanCard().Forget();
            teleportCount = 0;
            playerHitCount = 0;
            teleportTimer = 0;
            //cardChanged = false;

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

    public void HeartTrue()
    {
        miniHeartTrue = true;
    }

    // 스페이드 패턴
    private void GoldOrRedCardOn()
    {
        int ranIndex = Random.Range(0, spadeCards.Length);

        spadeCards[ranIndex].SetActive(true);
    }

    private void checkFindTure()
    {
        if (miniBoss[0].activeSelf == false)
        {
            miniBoss[0].GetComponent<MiniBoss>().ReturnOriPos().Forget();
            player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
        }
        if (miniBoss[0].activeSelf == true)
        {
            miniBoss[0].GetComponent<MiniBoss>().miniHTureReturnOriState();
        }
    }

    // 클럽 패턴
    private async UniTask SpinWheel()
    {
        wheelStop = false;
        int randomSpin = Random.Range(800,1300);
        for (int i = 0; i < randomSpin; i++)
        {
            if(wheelStop == true)
            {
                break;
            }
            //spinCircle.transform.Rotate(new Vector3(0, 10, 0) * spinSpeede * Time.fixedDeltaTime);
            //await UniTask.Delay(10);
            spinCircle.transform.Rotate(Vector3.up * spinSpeede * Time.deltaTime * 60); // 속도 보정
            await UniTask.Yield(PlayerLoopTiming.Update); // 프레임마다 체크
        }
    }

    public void WheelStop()
    {
        wheelStop = true;
    }

    private void ClubStackUIUpdate()
    {
        for (int i = 0; i < clubStackImage.Length; i++)
            clubStackImage[i].SetActive(i < Stage2BossAttack.clubStack);
    }

    private async UniTask StartDelay()
    {
        await UniTask.Delay(3000);
        isDelay = true;
        await ChangeNextRanCard();
        //currentCard = cards[3];
        //SetCardData();
    }
}
