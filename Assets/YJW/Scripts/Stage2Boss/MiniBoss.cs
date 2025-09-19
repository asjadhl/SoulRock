using Unity.VisualScripting;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    [SerializeField] int cardShape = 0;
    int HP = 0;

    private void Start()
    {
        HP = GetComponentInParent<Stage2BossAttack>().currentCard.num;
    }

    private void MiniBossMove()
    {

    }

    private void CompareCard()
    {
        if (GetComponentInParent<Stage2BossAttack>().currentCard.shpae == cardShape)
        {
            GetDamage();
        }
    }

    private void MiniBossDie()
    {
        if (HP <= 0)
            gameObject.SetActive(false);
    }

    private void GetDamage()
    {
        HP--;
    }
}
