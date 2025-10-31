using Unity.VisualScripting;
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
    public enum scenestate
    {
        allclear, firststageclearTalk, firststageclearNoTalk, secondStageClearTalk, secondStageClearNoTalk
    };

    scenestate mystate;


    // Update is called once per frame
    void Update()
    {
        CheckClearLight();
        switch (mystate)
        {
            case scenestate.allclear:
                Debug.Log("보스2 사망");
                clownLight.SetActive(false);
                skullLight.SetActive(false);
                clownCol.enabled = false;
                skullCol.enabled = false;
                break;
            case scenestate.firststageclearTalk:
                clownCol.enabled = false;
                skullCol.enabled = false;
                break;
            case scenestate.firststageclearNoTalk:
                clownCol.enabled = true;
                skullCol.enabled = false;
                break;
            case scenestate.secondStageClearTalk:
                clownCol.enabled = false;
                skullCol.enabled = false;
                clownLight.SetActive(false);
                skullLight.SetActive(true);
                break;
            case scenestate.secondStageClearNoTalk:
                clownCol.enabled = false;
                skullCol.enabled = true;
                clownLight.SetActive(false);
                skullLight.SetActive(true);
                break;
                default: break;
        }

        void CheckClearLight()
        {
            // 모든 클리어
            if (BossState.isBoss2Dead && BossState.isBoss1Dead)
            {
				Debug.Log(5);
				mystate = scenestate.allclear;
                return;
            }
            // 보스1 사망 + 2보스 대사 중
            if (BossState.isBoss1Dead && TalkState.isTalking && !BossState.isBoss2Dead)
            {
				Debug.Log(3);
				mystate = scenestate.secondStageClearTalk;

                return;
            }

            // 보스1 사망 + 대사 끝
            if (BossState.isBoss1Dead && !TalkState.isTalking && !BossState.isBoss2Dead)
            {
				Debug.Log(4);
				mystate = scenestate.secondStageClearNoTalk;

                return;
            }

            // 1보스 대사 중
            if (TalkState.isTalking && !BossState.isBoss1Dead && !BossState.isBoss2Dead)
            {
                Debug.Log(1);
                mystate = scenestate.firststageclearTalk;
                clownCol.enabled = false;
                skullCol.enabled = false;
                return;
            }

            // 1보스 대사 끝
            if (!TalkState.isTalking && !BossState.isBoss1Dead&&!BossState.isBoss2Dead)
            {
				Debug.Log(2);
				mystate = scenestate.firststageclearNoTalk;
                clownCol.enabled = true;
                skullCol.enabled = false;
                return;
            }
        }
    }
}
