using UnityEngine;

public class ComboSave : MonoBehaviour
{
    public static ComboSave Instance { get; private set; }

    public MaxComboData maxComboData;

    private void Awake()
    {
        maxComboData.maxComboValue = 0;

        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void MaxcomboSaveScr()
    {
        maxComboData.maxComboValue = CircleHit.Instance.combo;
    }
}
