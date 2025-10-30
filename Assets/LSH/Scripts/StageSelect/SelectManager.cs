using UnityEngine;

public class SelectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject clownLight;
    [SerializeField] GameObject skullLight;
    [SerializeField] BoxCollider clownCol;
    [SerializeField] BoxCollider skullCol;
    bool isAllCol = false;
    void Start()
    {
        skullLight.SetActive(false);
        clownLight.SetActive(true);
        clownCol.enabled = true;
        skullCol.enabled = false;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckClearLight();
    }

    void CheckClearLight()
    {
        // 모든 클리어
        if (BossState.isBoss2Dead && BossState.isBoss2Dead)
        {
            Debug.Log("보스2 사망");
            clownLight.SetActive(false);
            skullLight.SetActive(false);
            clownCol.enabled = false;
            skullCol.enabled = false;
            return;
        }
        // 보스1 사망 + 2보스 대사 중
        if (BossState.isBoss1Dead && DialogueLineTrueORFalse.stage2True && !BossState.isBoss2Dead)
        {
            clownCol.enabled = false;
            skullCol.enabled = false;
            clownLight.SetActive(false);
            skullLight.SetActive(true);
            return;
        }

        // 보스1 사망 + 대사 끝
        if (BossState.isBoss1Dead && !DialogueLineTrueORFalse.stage2True && !BossState.isBoss2Dead)
        {
            clownCol.enabled = false;
            skullCol.enabled = true;
            clownLight.SetActive(false);
            skullLight.SetActive(true);
            return;
        }

        // 1보스 대사 중
        if (DialogueLineTrueORFalse.stage1True && !BossState.isBoss1Dead)
        {
            clownCol.enabled = false;
            skullCol.enabled = false;
            return;
        }

        // 1보스 대사 끝
        if (!DialogueLineTrueORFalse.stage1True && !BossState.isBoss1Dead)
        {
            clownCol.enabled = false;
            skullCol.enabled = false;
            return;
        }
    }
}
