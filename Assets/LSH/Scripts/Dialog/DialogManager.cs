using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private string[] Dialog;
    [SerializeField] Text text;
    bool isDialog = true;
    void Start()
    {
        Dialog = new string[]
        {
            "크하하하핳",
            "여기까지 오다니 제법이구나!",
            "시네~!!!",
            "우효WWWWWWW"
        };
    }

    // Update is called once per frame
    void Update()
    {
        MainDialog();
    }

    void MainDialog()
    {
        if (isDialog)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Dialog.Length > 0)
                {
                    text.text = Dialog[0];
                    var temp = new string[Dialog.Length - 1];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = Dialog[i + 1];
                    }
                    Dialog = temp;
                }
                else
                {
                    text.text = "";
                    isDialog = false;
                }
            }

        }
    }
}
