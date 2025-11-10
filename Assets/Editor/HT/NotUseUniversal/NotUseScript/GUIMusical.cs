using UnityEngine;

public class GUIMusical : MonoBehaviour
{


    public float RotationSpeed;
    public AudioSource audioSource;
    public float scaleMultiplier = 50f;
    float[] spectrum = new float[512];
    public GameObject[] Tars;
    public void Update()
    {
        //Quaternion delta = Quaternion.AngleAxis(RotationSpeed * Time.unscaledDeltaTime, Vector3.forward);

         
        //transform.rotation = transform.rotation * delta;

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

         Tars[0].transform.localScale = new Vector3(1f,1f+ GetAverage(0,128) * scaleMultiplier,1f);
         Tars[1].transform.localScale = new Vector3(1f,1f+ GetAverage(128, 128)+(GetAverage(0, 128)/2f) * scaleMultiplier,1f);
         Tars[2].transform.localScale = new Vector3(1f,1f+ GetAverage(256, 128)+(GetAverage(0, 128) / 3f) * scaleMultiplier,1f);
         Tars[3].transform.localScale = new Vector3(1f,1f + GetAverage(384, 128) + (GetAverage(0, 128) / 4f) * scaleMultiplier, 1f);
        //transform.localScale = new Vector3(1f, 1f + bass, 1f);
    }

    float GetAverage(int startIndex, int count)
    {
        float sum = 0;
        for (int i = startIndex; i < startIndex + count; i++)
        {
            sum += spectrum[i];
        }
        return sum / count;
    }
}
