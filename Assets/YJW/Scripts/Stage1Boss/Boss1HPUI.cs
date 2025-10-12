using UnityEngine;

public class Boss1HPUI : MonoBehaviour
{
    [SerializeField] GameObject[] heartImage;

    int index = 2;

    private void BossHPUI()
    {
        heartImage[index].SetActive(false);
    }
}
