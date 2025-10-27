using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboNumText;

    private void Update()
    {
        comboNumText.text = CircleHit.Instance.combo.ToString();   
    }

    public void RanTextColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        comboNumText.color = randomColor;
    }
}
