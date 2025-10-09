using UnityEngine;
using UnityEngine.UI;

public class Boss2HPUI : MonoBehaviour
{
    [SerializeField] Image bossHPBar;

    private void Update()
    {
        bossHPBar.fillAmount = (float)GetComponent<BossHP>().bossHP/50;

    }
}
