using UnityEngine;

public class Roulette : MonoBehaviour
{
    private int num;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "C0")
            num = 0;
        if (other.gameObject.tag == "C1")
            num = 1;
        if (other.gameObject.tag == "C-1")
            num = -1;
        if (other.gameObject.tag == "C2")
            num = 2;
        if (other.gameObject.tag == "C3")
            num = 3;
    }

    public int GetNum()
    {
        return num;
    }
}
