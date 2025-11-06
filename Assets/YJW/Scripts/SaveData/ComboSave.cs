using UnityEngine;

public class ComboSave : MonoBehaviour
{
    public static ComboSave Instance { get; private set; }

    public string playerName;
    public int combo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
}
