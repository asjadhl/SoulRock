using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public enum States
{
    Underground, Spawn, Idle, Forward, Attack, Die, SpawnR, Null
}
public class EnemyGhostGraphics : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private Animator m_anim;
    [SerializeField]
    public State My_State;
    [SerializeField]
    private int AttackIndex;
    [Space(2)]
    public List<string> ListClipName;
    public Dictionary<string, AnimationClip> m_eventDic;
    [SerializeField]
    List<CancellationTokenSource> listcts;
    private int activetask = 0;
    [SerializeField]
    float Attackrate = 2f;
    [SerializeField]
    float timer = 0;
    Vector3 Origin;


    private void Awake()
    {
        if (m_anim == null)
            m_anim = GetComponent<Animator>();
        if (m_anim == null)
        {
            Debug.LogError($"GameObject :{this.name}'s Animator is missing");
            gameObject.SetActive(false);

        }


        //Prototype
        int i = 0;
        m_eventDic = new Dictionary<string, AnimationClip>();
        if (m_eventDic != null && ListClipName != null)
        {
            foreach (var child in ListClipName)
            {
                if (child != "")
                    m_eventDic.Add(child, GetClipByName(child));
                else
                    m_eventDic.Add(i++.ToString(), null);
            }
        }
        listcts = new();
        listcts.Add(new CancellationTokenSource());
        listcts.Add(new CancellationTokenSource());



        My_State = State.Null;

        SetRandomColor();
    }
















    public  void SetRandomColor()
    {

        var mat = GetComponentInChildren<SkinnedMeshRenderer>();
        mat.material.SetColor("_MainColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

    }
    private void OnDestroy()
    {
        listcts[0].Cancel();
    }
    #region Animation_System

    public async UniTaskVoid UniLaterCall(float time, CancellationToken token, System.Action callback = null)
    {

        bool canceled = false;

        try
        {
            await UniTask.WaitForSeconds(time, cancellationToken: token);
        }
        catch (System.OperationCanceledException)
        {
            canceled = true;
        }
        finally
        {
            if (!canceled)
                callback?.Invoke();
        }

    }

    public async UniTaskVoid AnimationManager(State _newState, int tokenIndex, System.Action callback = null, float sample = 0)
    {

        if (My_State == _newState || My_State == State.Die)
            return;
        else
            My_State = _newState;


        if (activetask >= 1)
        {

            listcts[0].Cancel();
        }
        switch (My_State)
        {
            case State.Underground:
                m_anim.Play(ListClipName[0]);
                break;
            case State.Spawn:
                m_anim.Play(ListClipName[1]);
                break;

            case State.Idle:
                m_anim.Play(ListClipName[2]);
                break;

            case State.Forward:
                m_anim.Play(ListClipName[3]);
                break;
            case State.Attack:
                m_anim.Play(ListClipName[4]);
                break;
            case State.Die:
                m_anim.Play(ListClipName[5]);
                break;
            case State.SpawnR:
                m_anim.Play(ListClipName[6]);
                break;
        }

        if (sample > 0)
        {
            float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate; ;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: listcts[0].Token);
        }
        else
            await UniTask.Yield();

        callback?.Invoke();
    }
    public  AnimationClip GetClipByName(string clipName)
    {
        foreach (var clip in m_anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip;
        }
        return null;
    }

    public async UniTaskVoid UniPlayOneShot(State _newState,int tokenIndex, State returnState)
    {
        bool canceled = false;

        try
        {
            m_anim.Play(ListClipName[(int)_newState]);
            if (m_eventDic[ListClipName[(int)_newState]])
            {
                await UniTask.WaitForSeconds(m_eventDic[ListClipName[(int)_newState]].length, cancellationToken: listcts[0].Token);
            }
        }
        catch (System.OperationCanceledException)
        {
            canceled = true;
            Debug.Log("UniPlayOneShot-Cancel");
        }
        finally
        {
            if (!canceled)
            {
                AnimationManager(returnState, tokenIndex).Forget();
            }

        }
    }


    public async UniTaskVoid UniTriggerAtSample(State _newState, int sample, CancellationToken token, bool IsMaster, System.Action callback = null)
    {
        float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate;

        bool canceled = false;

        try
        {
            if (!IsMaster)
                activetask++;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: token);
        }
        catch (System.OperationCanceledException)
        {
            canceled = true;
            Debug.Log("UniTrig-Cancel");
        }
        finally
        {
            if (!IsMaster)
                activetask--;
            if (!canceled)
                callback?.Invoke();
        }

    }

    public async UniTaskVoid UniScaleChangeOverTime( int tokenIndex)
    {
        float scales = 1;
        Origin = transform.localScale;
        while (true)
        {
            scales -= Time.deltaTime;
            if (scales <= 0)
                break;
            transform.localScale = Origin * scales;
            await UniTask.WaitForSeconds(Time.deltaTime, cancellationToken: listcts[0].Token);
        }
    }



    #endregion
}
