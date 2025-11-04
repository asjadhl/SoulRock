using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{

    [SerializeField] Image playerHPImage;
    public PlayerHP playerHp;

    private void Start()
    {
        playerHp = GetComponent<PlayerHP>();
         
    }

    private void Update()
    {
        UpdatePlayerHPUI();
    }

    private void UpdatePlayerHPUI()
    {
        playerHPImage.fillAmount = playerHp.playerHP / 100f;
    }
}
