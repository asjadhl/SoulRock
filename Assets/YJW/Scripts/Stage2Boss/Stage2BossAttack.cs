using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Stage2BossAttack : MonoBehaviour
{
    /*
    문양 맞추기 패턴
    보스가 트럼프 모양을 보여준뒤 갑자기 연기속으로 사라진후 4마리의 각 문양을 가진 미니 보스가 등장
    이때 플레이어는 보스가 사라지기전 보여준 트럼프 모양을 가진 미니 보스를 처치해야 패턴 상쇄

    플레이어 총 앞에 하트,클로버,스페이드,클럽 모양중 하나가 뜨고 보스의 머리위 모양이 2초마다 바뀌는데 
    플레이어는 해당 문양이 뜰때 맞춰야함 총 2번 진행되며 보스를 타격 못한 횟수만큼 플레이어 데미지

    순간이동 패턴
    랜덤으로 순간이동해서 플레이어를 혼란시킴
    거리가 되면 플레이어가 직접 타격 가능
    일반 기믹
    빨간 카드 = 일반 공격 / 골드 카드 = 스턴
    플레이어는 맞춰야함
    스턴 효과는 도트를 투명하게 처리


    */

    [SerializeField] Card[] Cards;
    [SerializeField] GameObject card;
    public Card currentCard;

    [SerializeField] ParticleSystem hideEffect;

    [SerializeField] GameObject[] miniBoss;
    bool miniBossSpawned = false;

    private void Start()
    {

    }

    private void FixedUpdate()
    {

    }

    public async UniTask BossAttack1()
    {
        card.SetActive(true);
        currentCard = Cards[Random.Range(0, Cards.Length)];
        card.GetComponent<SpriteRenderer>().sprite = currentCard.icon;
        int cardNum = currentCard.num;
        int cardShpae = currentCard.shpae;
        await UniTask.Delay(1000);
        card.SetActive(false);

        hideEffect.Play();

        for(int i = 0; i < miniBoss.Length; i++)
            miniBoss[i].SetActive(true);
        miniBossSpawned = true;

        gameObject.SetActive(false);


        int case1CoolTime = Random.Range(7, 23);
        await UniTask.Delay(case1CoolTime * 1000);
    }

    private void BossAttack2()
    {

    }

    private void MoveToRanPos()
    {

    }


}
