using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUIManager : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    [SerializeField] private GameObject speechBubble;        // 말풍선 패널
    [SerializeField] private GameObject speakerImageObject;  // 유령 이미지 (캔버스 안)
    [SerializeField] private TextMeshProUGUI dialogueText;   // 대사 텍스트

    [Header("폰트 (SDF 폰트 에셋)")]
    [SerializeField] private TMP_FontAsset sdfFontAsset;

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("사운드 설정")]
    [SerializeField] private AudioSource typingAudio;
    [SerializeField] private bool randomPitch = true;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private Coroutine typingCoroutine;

    void Start()
    {
        //  이 부분이 문제! (삭제하거나 주석 처리)
        // if (speechBubble != null)
        //     speechBubble.SetActive(false);
        // if (speakerImageObject != null)
        //     speakerImageObject.SetActive(false);

        //  폰트만 설정
        if (sdfFontAsset != null && dialogueText != null)
            dialogueText.font = sdfFontAsset;
    }


    /// <summary>
    /// 말풍선 + 유령 이미지 켜기/끄기
    /// </summary>
    public void ShowDialogueUI(bool isOn)
    {
        if (speechBubble != null)
            speechBubble.SetActive(isOn);

        if (speakerImageObject != null)
            speakerImageObject.SetActive(isOn);

        if (dialogueText != null)
            dialogueText.gameObject.SetActive(isOn);
    }

    /// <summary>
    /// 대사 출력 시작 (타이핑)
    /// </summary>
    public void ShowDialogueText(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;

            if (typingAudio != null)
            {
                if (randomPitch)
                    typingAudio.pitch = Random.Range(pitchRange.x, pitchRange.y);
                typingAudio.PlayOneShot(typingAudio.clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
