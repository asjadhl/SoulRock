

//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using Cysharp.Threading.Tasks.Triggers;



//public class UIEDITOR
//{
//  public  RectTransform m_rect;

//    public UIEDITOR()
//    { }
//    public UIEDITOR(RectTransform rect)
//    {
//        m_rect = rect;
//    }

//    public void SetRendered()
//    {
//        m_rect.offsetMin = Vector3.zero;
//        m_rect.offsetMax = Vector3.zero;
//    }

//    public void SetScale(Vector3 scale)
//    {
//        m_rect.localScale = scale;

//    }
//}


//public class Tower
//{
//    public string name;
//    public string description;

//    public string ID_GameObject;

//    public GameObject TowerObj;
//    public Vector3 WorldSpace;
//    public float Damage;
//    public float Range;

//    public float FireRate;


//    public void SetObj(GameObject obj)
//    {
//        TowerObj = obj;
//    }

//   public void LookAt(Vector3 pos)
//    {
//        Vector3 head = new Vector3(0, 0, 0);

//        Vector3 dir = (pos - head).normalized;





//    }
//}



using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class test : MonoBehaviour
{



  public GameObject prefab;
  public GameObject current;
  public void Start()
  {
    current = Instantiate(prefab,new Vector3(Random.Range(-5f,5f),0,Random.Range(-5f,5f)),Quaternion.identity);

    Pop().Forget();
  }
 
  public void Update()
  {
     if(current == null)
     {

     }
  }

  public async UniTaskVoid Pop()
  {
   var token = current.GetCancellationTokenOnDestroy();

    while(!token.IsCancellationRequested)
    {
      Destroy(current);
      Debug.Log(current.name);
      current.transform.position = Vector3.zero;
      Debug.Log(current.transform);
      await UniTask.Yield();
    }

    if (token.IsCancellationRequested)
      Debug.Log("SAFE");
  }
}

//public class test : MonoBehaviour
//{
//    public GameObject Head;
//    public GameObject Player;
//    public GameObject Shoot;
//    public GameObject FirePos;
//    public LineRenderer m_LineRenderer;


//    float firerate = 5f;
//    float time = 0;
//    public float RecoilMin;
//    public float RecoilMax;
//    public float RecoilFireJourneyDuration;
//    public float RecoilFireBackHomeDuration;
//    private void Start()
//    {

//        Player = GameObject.FindWithTag("Player");
//        if(Player == null)
//            gameObject.SetActive(false);

//        m_LineRenderer = GetComponent<LineRenderer>();

//        if (m_LineRenderer == null)
//            gameObject.SetActive(false);




//    }

//    void FireUpdate()
//    {

//        if (Head.transform.eulerAngles.x >= -225f && Head.transform.eulerAngles.x <= 45f)
//        {
//            time += Time.deltaTime;

//            if (time >= firerate)
//            {
//                StartCoroutine(Fire());
//                time = 0;
//            }
//            m_LineRenderer.SetPosition(0, FirePos.transform.position);
//            m_LineRenderer.SetPosition(1, Player.transform.position);
//        }
//        else
//            time = 0;
//    }



//    private void Update()
//    {
//        Head.transform.LookAt(Player.transform.position);
//        Debug.Log(Head.transform.eulerAngles);
//        FireUpdate();
//        Head.transform.eulerAngles = new Vector3(Mathf.Clamp(Head.transform.eulerAngles.x, -225, 45f), Head.transform.eulerAngles.y, Head.transform.eulerAngles.z); 

//    }

//    IEnumerator Fire()
//    {
//        Debug.Log("Fired!");

//       StartCoroutine(Recoil());
//        m_LineRenderer.startWidth = 0.095f;
//        yield return new WaitForSeconds(0.2f);
//        m_LineRenderer.startWidth = 0;
//    }

//    IEnumerator Recoil()
//    {
//        float t = 0;

//        while (true)
//        {   
//              t+= Time.deltaTime;
//               Shoot.transform.localPosition = new Vector3(Shoot.transform.localPosition.x, Shoot.transform.localPosition.y,
//                   Mathf.Lerp(Shoot.transform.localPosition.z, RecoilMin,t/ RecoilFireJourneyDuration));

//            if (t >= 1f)
//                break;
//            yield return new WaitForFixedUpdate();
//        }

//        StartCoroutine(GoBackNormal());

//    }

//    IEnumerator GoBackNormal()
//    {
//        float t = 0;

//        while (true)
//        {
//            t += Time.deltaTime;
//            Shoot.transform.localPosition = new Vector3(Shoot.transform.localPosition.x, Shoot.transform.localPosition.y,
//                Mathf.Lerp(Shoot.transform.localPosition.z, RecoilMax, t / RecoilFireBackHomeDuration));

//            if (t >= 1f)
//                break;
//            yield return new WaitForFixedUpdate();
//        }



//    }
//}



