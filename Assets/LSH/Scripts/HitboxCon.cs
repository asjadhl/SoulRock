using Unity.VisualScripting;
using UnityEngine;

//일단 해야할 것.
//1. 제거는 끝났으니 범위 또 만들어서 성공 히트박스 만들기.
//2. 큰 히트박스는 크리티컬용도 작은거는 일반데미지용도?
[RequireComponent(typeof(BoxCollider2D))]
public class HitBoxCon: MonoBehaviour
{
    [Header("타이밍 확인용도")]
    public bool goodTiming = false;
    public bool badTiming = false;

    DotBoxCon dotboxCon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dotboxCon = GetComponent<DotBoxCon>();
    }

    // Update is called once per frame
    void Update()
    {
        MakeACol();
    }
    void TimingIsNow()
    {

    }
    void Clicker(bool GoodBad)
    {
        if (Input.GetMouseButtonDown(0) && GoodBad) //좋은 타이밍 클릭
        {
            goodTiming = true;
            Debug.Log("클릭 감지");
        }
        else if (Input.GetMouseButtonDown(0) && !GoodBad) //안좋은 타이밍 클릭
        {
            badTiming = true;
            Debug.Log("안좋은 타이밍!");
        }
        else//박자 놓치기
        {
            badTiming = true;
            Debug.Log("놓쳤어요~");
        }
    }
    void MakeACol()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // RectTransform 크기 BoxCollider2D에 같게
        collider.size = rectTransform.rect.size;
        collider.offset = rectTransform.rect.center;//중심임.
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Dotbox"))
        {
            Clicker(true);
        }
        else
        {
            Clicker(false);
        }
    }
}
