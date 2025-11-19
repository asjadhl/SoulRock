using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoulRock : MonoBehaviour
{
    // 박자

    // ====================== 스테이지 1 보스 ======================
    public class Stage1BossAttack : MonoBehaviour
    {
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

        private BossHP BossHP;

        private bool isDelay = false;

        private bool isHAttacking = false;
        private bool miniHeartTrue = false;

        [SerializeField] GameObject spinWheel;
        [SerializeField] GameObject spinCircle;
        private float spinSpeed = 10f;
        private bool isCAttacking = false;
        bool isAttack = false;
        public bool wheelStop = true;
        private int index = 0;
        [SerializeField] GameObject[] clubStackImage;
        NormalMusicBox normalMusicBox;
        private bool bossDialogueTriggered = false;
        [SerializeField] private DialogueUIManager dialogueUI;

        public bool isDAttacking = false;

        PlayerHP playerHP;

        private void Start()
        {
            clubStack = 0;
            ComboSave.Instance.maxComboData.maxCombo = 0;

            for (int i = 0; i < clubStackImage.Length; i++)
                clubStackImage[i].SetActive(false);


            player = GameObject.FindWithTag("Player");
            normalMusicBox = GameObject.FindWithTag("MusicBox").GetComponent<NormalMusicBox>();
            BossHP = GetComponent<BossHP>();
            playerHP = player.GetComponent<PlayerHP>();
            if (isDelay == false)
                StartDelay().Forget();
            dialogueUI = FindAnyObjectByType<DialogueUIManager>();
        }
        private void FixedUpdate()
        {
            ClubStackUIUpdate();
            if (!isDelay) return;
            if (!isAttack)
            {
                BossPattern();
            }
            if (normalMusicBox.MusicFin && !bossDialogueTriggered)
            {
                bossDialogueTriggered = true;
                BossState.isBoss1Dead = true;
                dialogueUI.ShowDialogueUI(true);
            }
        }

        // 보스패턴 전체 관리
        private void BossPattern()
        {
            if (BossState.isBoss1Dead == false)
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
        }

        // ====================== 카드데이터 받아오기 ======================
        private void SetCardData()
        {
            bossCardImage.sprite = currentCard.icon;
            curShape = currentCard.shape;
        }

        // ====================== 다음 카드로 바꾸기 ======================
        private async UniTask ChangeNextRanCard()
        {
            var token = this.GetCancellationTokenOnDestroy();

            await RollCardEffect(rollCount: 12, delay: 80);

            Card nextCard;
            do
            {
                nextCard = cards[Random.Range(0, cards.Length)];
            }
            while (nextCard == currentCard && !token.IsCancellationRequested);

            if (!token.IsCancellationRequested)
            {
                currentCard = nextCard;
                SetCardData();
            }

        }

        // ====================== 카드 바뀌는 이펙트 ======================
        private async UniTask RollCardEffect(int rollCount = 10, int delay = 100)
        {
            var token = this.GetCancellationTokenOnDestroy();
            for (int i = 0; i < rollCount; i++)
            {
                if (!token.IsCancellationRequested)
                {
                    var randomCard = cards[Random.Range(0, cards.Length)];
                    bossCardImage.sprite = randomCard.icon; // UI에만 반영
                    bossCardImage.transform.localScale = Vector3.one * 1.2f;
                    await UniTask.Delay(delay / 2);
                    bossCardImage.transform.localScale = Vector3.one;
                    await UniTask.Delay(delay / 2);
                }
                else
                {
                    break;
                }
            }
        }

        // ====================== 하트 패턴 ======================
        private async UniTask HAttack()
        {
            isAttack = true;
            await UniTask.Delay(500);
            reMiniH = clubStack + 4;
            for (int i = 0; i < clubStack + 4; i++)
            {
                miniBoss[i].SetActive(true);
            }
            miniBossSpawned = true;
            await UniTask.Delay(10000);
            usedPos.Clear();

            checkFindTure();

            for (int i = 0; i < clubStack + 4; i++)
            {
                if (miniBoss[i].activeSelf == true)
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
            miniHeartTrue = false;
            isAttack = false;
        }


        // ====================== 스페이드 패턴 ======================
        private async UniTask SAttack()
        {
            isAttack = true;
            Debug.Log("스페이드");
            for (int i = 0; i < 6; i++)
            {
                GoldOrRedCardOn();
                await UniTask.Delay(1500);
            }
            await ChangeNextRanCard();
            isAttack = false;
        }

        // ====================== 다이아 패턴 ======================
        private async UniTask DAttack()
        {
            isAttack = true;
            isDAttacking = true;
            if (teleportCount < 5)
            {
                if (teleportTimer >= 2)
                {
                    MoveToRanPos();
                    teleportTimer = 0;
                    isDAttacking = false;
                }
            }
            else
            {
                if (playerHitCount < 5)
                {
                    BossRush();
                }
                else
                {
                    transform.position = new Vector3(0, 0, player.transform.position.z + 17);
                    await ChangeNextRanCard();
                    playerHitCount = 0;
                    teleportTimer = 0;
                    teleportCount = 0;
                    isDAttacking = false;
                }

            }
            isAttack = false;
        }

        // ====================== 클로버 패턴 ======================
        private async UniTask CAttack()
        {
            isAttack = true;
            isCAttacking = true;
            spinWheel.SetActive(true);
            await SpinWheel();

            int result = spinWheel.GetComponentInChildren<Roulette>().GetNum();
            Debug.Log(result);
            clubStack += result;
            if (clubStack >= 7)
            {
                playerHP.PlayerHPMinus().Forget();
                clubStack = 0;
            }

            await ChangeNextRanCard();

            isCAttacking = false;
            spinWheel.SetActive(false);
            isAttack = false;
            wheelStop = true;
        }

        // ====================== 다이아 패턴 관련 함수 ======================
        private void MoveToRanPos()
        {
            Vector3 currentPos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            Stage1ParticleManager.Instance.PlayClownEffect(currentPos);

            int x = Random.Range(-8, 9);
            int z = (int)player.transform.position.z + Random.Range(10, 20);

            transform.position = new Vector3(x, 0, z);

            teleportCount++;
        }

        private void BossRush()
        {
            transform.LookAt(player.transform);
            transform.Translate(Vector3.forward * 30 * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player.GetComponent<PlayerHP>().PlayerHPMinus().Forget();
                ChangeNextRanCard().Forget();
                transform.rotation = Quaternion.Euler(0, 180, 0);
                transform.position = new Vector3(0, 0, player.transform.position.z + 17);
                teleportCount = 0;
                playerHitCount = 0;
                teleportTimer = 0;
                isDAttacking = false;
            }
        }

        // ====================== 하트 패턴 관련 함수 ======================
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

        // ====================== 스페이드 패턴 관련 함수 ======================
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

        // ====================== 클로버 패턴 관련 함수 ======================
        private async UniTask SpinWheel()
        {
            wheelStop = false;
            int randomSpin = Random.Range(800, 1300);
            for (int i = 0; i < randomSpin; i++)
            {
                if (wheelStop == true)
                {
                    break;
                }
                spinCircle.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime * 60); // 속도 보정
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

        // ====================== 시작 전 딜레이 ======================
        private async UniTask StartDelay()
        {
            await UniTask.Delay(3500);
            isDelay = true;
            await ChangeNextRanCard();
        }
    }

    // ====================== 스테이지 2 보스 ======================

    // ====================== 스테이지 3 보스 ======================

    // ====================== 맵 ======================

    // ====================== 적 ======================

    // ====================== 대사 ======================

    // ====================== 저장 ======================
    public class ComboSave : MonoBehaviour
    {   // 콤보 저장
        public static ComboSave Instance { get; private set; }

        public MaxComboData maxComboData;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        public void MaxcomboSaveScr()
        {
            maxComboData.maxComboValue = CircleHit.Instance.combo;
        }

    }

    public class Datamanager : MonoBehaviour
    {
        // 제이슨 저장
        public static Datamanager instance;

        public PlayerData curPlayerData = new PlayerData();
        public PlayerDataList allData = new PlayerDataList();

        string path;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

#if UNITY_EDITOR
            string folder = Path.Combine(Application.dataPath, "Json");
#else
        string folder = Path.Combine(Application.dataPath, "../Json");
#endif

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            path = Path.Combine(folder, "PlayerData.json");
        }

        public void SaveToJson()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                allData = JsonUtility.FromJson<PlayerDataList>(json);
            }
            else
            {
                allData = new PlayerDataList();
            }

            allData.players.Add(curPlayerData);

            string newJson = JsonUtility.ToJson(allData, true);
            File.WriteAllText(path, newJson);

        }

        public void LoadFromJson()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                allData = JsonUtility.FromJson<PlayerDataList>(json);
            }
            else
            {
                allData = new PlayerDataList();
            }
        }
    }

    [CreateAssetMenu(fileName = "MaxComboData", menuName = "MaxComboData")]
    public class MaxComboData : ScriptableObject
    {
        // 최고 콤보 저장용 스크립터블 오브젝트
        public int maxCombo;
        public int maxComboValue
        {
            get { return maxCombo; }
            set
            {
                if (value > maxCombo)
                {
                    maxCombo = value;
                }
                else
                {
                    return;
                }
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        // 저장할 플레이어 데이터
        public string playerName;
        public int combo;
    }

    public class PlayerDataList
    {
        // 플레이어 데이터들
        public List<PlayerData> players = new List<PlayerData>();
    }


    // ====================== 가상 키보드 ======================
    public class VirtualKeyboard : MonoBehaviour
    {
        [SerializeField] private TMP_InputField targetInput;

        public void OnKeyPress(string key)
        {
            if (targetInput == null) return;

            switch (key)
            {
                case "Backspace":
                    if (targetInput.text.Length > 0)
                        targetInput.text = targetInput.text.Substring(0, targetInput.text.Length - 1);
                    break;

                case "Enter":
                    break;

                default:
                    targetInput.text += key;
                    break;
            }

            targetInput.caretPosition = targetInput.text.Length;
        }

        public void SetTarget(TMP_InputField input)
        {
            targetInput = input;
        }
    }

    public class KeyboardButton : MonoBehaviour
    {
        [SerializeField] private string keyValue;
        private VirtualKeyboard keyboard;

        void Start()
        {
            keyboard = GetComponentInParent<VirtualKeyboard>();
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            keyboard.OnKeyPress(keyValue);
        }
    }
}
