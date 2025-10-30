using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
public class BlindEffect : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float duration = 2f;

    private async UniTaskVoid Start()
    {
        await FadeTextAlpha(0f, 1f, duration); 
        await UniTask.Delay(1000);
        await FadeTextAlpha(1f, 0f, duration); 
    }

    private async UniTask FadeTextAlpha(float start, float end, float time)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            text.color = color;
            await UniTask.Yield();
        }

        color.a = end;
        text.color = color;
    }
}
