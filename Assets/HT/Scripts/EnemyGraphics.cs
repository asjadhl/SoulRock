using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
 
public enum Cts
{
    normal,master
}
public enum AnimationState
{
    Underground, Spawn, Idle, Forward, Attack, Die, DamageDone, Null
}
public class EnemyGraphics : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private Animator m_anim;
    [SerializeField]
    public AnimationState My_State;
    [Space(2)]
    public List<string> ListClipName;
    public Dictionary<string, AnimationClip> m_eventDic;
    [SerializeField]
    List<CancellationTokenSource> listcts;
    private int activetask = 0;
    
   
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
        listcts = new()
        {
            new CancellationTokenSource(),
            new CancellationTokenSource()
        };



        My_State = AnimationState.Null;

      
    }



    public void ResetNow()
    {
        My_State = AnimationState.Null;
    }
    public  void SetRandomColor()
    {

        var mat = GetComponentInChildren<SkinnedMeshRenderer>();
        mat.material.SetColor("_MainColor", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

    }
    private void OnDestroy()
    {
        listcts[(int)Cts.normal].Cancel();
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

    public async UniTaskVoid AnimationManager(AnimationState _newState, Cts cts, System.Action callback = null, float sample = 0)
    {

        if (My_State == _newState || My_State == AnimationState.Die)
            return;
        else
            My_State = _newState;


        if (activetask >= 1)
        {

            listcts[0].Cancel();
        }
        switch (My_State)
        {
            case AnimationState.Underground:
                m_anim.Play(ListClipName[0]);
                break;
            case AnimationState.Spawn:
                m_anim.Play(ListClipName[1]);
                break;

            case AnimationState.Idle:
                m_anim.Play(ListClipName[2]);
                break;

            case AnimationState.Forward:
                m_anim.Play(ListClipName[3]);
                break;
            case AnimationState.Attack:
                m_anim.Play(ListClipName[4]);
                break;
            case AnimationState.Die:
                m_anim.Play(ListClipName[5]);
                break;
            case AnimationState.DamageDone:
                m_anim.Play(ListClipName[6]);
                break;
               
        }

        if (sample > 0)
        {
            float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate; ;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: listcts[(int)cts].Token);
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

    public async UniTaskVoid UniPlayOneShot(AnimationState _newState, Cts cts, AnimationState returnState)
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
                AnimationManager(returnState, cts).Forget();
            }

        }
    }


    public async UniTaskVoid UniTriggerAtSample(AnimationState _newState, int sample, Cts cts, bool IsMaster, System.Action callback = null)
    {
        float waitTime = sample / m_eventDic[ListClipName[(int)_newState]].frameRate;

        bool canceled = false;

        try
        {
            if (!IsMaster)
                activetask++;
            await UniTask.WaitForSeconds(waitTime, cancellationToken: listcts[(int)cts].Token);
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

    public async UniTaskVoid UniScaleChangeOverTime()
    {
        float scales = 1;
        Origin = transform.localScale;
        while (true)
        {
            scales -= Time.deltaTime*2f;
            if (scales <= 0)
                break;
            transform.localScale = Origin * scales;
            await UniTask.WaitForSeconds(Time.deltaTime, cancellationToken: listcts[0].Token);
        }
    }



    #endregion
}
