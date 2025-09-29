using UnityEngine;

public class PlayerHPUI : MonoBehaviour
{
    private PlayerHP playerHP;

    [SerializeField] GameObject[] playerHPImage;
    int playerCurrentHP;

    void Start()
    {
        playerHP = GetComponent<PlayerHP>();
        playerCurrentHP = playerHP.playerHP;
    }

    public void PlayerHPUIUpdate(int hp)
    {
        for(; playerCurrentHP <= hp; playerCurrentHP--)
        {
            playerHPImage[playerCurrentHP].SetActive(false);
        }
    }
}
