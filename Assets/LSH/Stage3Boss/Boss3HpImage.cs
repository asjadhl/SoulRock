using UnityEngine;
using UnityEngine.UI;

public class Boss3HpImage : MonoBehaviour
{
    [SerializeField] Image bossHPBar;
    [SerializeField] Image bossHPBarBack;
    [SerializeField] Sprite hpBarAngry;

    private void Update()
    {
        bossHPBar.fillAmount = Mathf.Abs(((float)GetComponent<BossHP>().bossHP / 100) - 1);
        if (bossHPBar.fillAmount >= 0.7f)
        {
            bossHPBar.sprite = hpBarAngry;
            bossHPBarBack.sprite = hpBarAngry;
        }
    }
}
