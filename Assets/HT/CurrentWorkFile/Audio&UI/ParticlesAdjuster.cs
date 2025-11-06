using System;
using UnityEngine;

public class ParticlesAdjuster : MonoBehaviour
{
  public Canvas MainCanvas;
  ParticleSystem myparticleSystem;
  UnityEngine.ParticleSystem.ShapeModule myshape;
  ParticleSystemRenderer myparticleSystemRenderer;
  float newScale = 0;
  float oldScale = 0;
  Func<float,float, float, float> cal;
    void Start()
    {
    myparticleSystem = GetComponent<ParticleSystem>();  
    myparticleSystem.GetComponent<ParticleSystemRenderer>();
    myparticleSystemRenderer = myparticleSystem.GetComponent<ParticleSystemRenderer>();
    myshape = myparticleSystem.shape;
    newScale = MainCanvas.scaleFactor;
    cal = (s1,d1, d2) => {

      float s2;

      s2  = s1 * (d1 /d2); 
     
      return s2; };
  }

  private void Update()
  {
    newScale = cal(myparticleSystemRenderer.maxParticleSize, newScale, MainCanvas.scaleFactor);
    Debug.Log(myparticleSystemRenderer.maxParticleSize);
    ChangeShapeScale();
    ChangeRendererScale();
  }


  private void ChangeShapeScale()
     {
          if (myparticleSystem != null) {
               myshape.scale = new Vector3(newScale,newScale,newScale);
           }
     }

  private void ChangeRendererScale()
  {
    if (myparticleSystemRenderer != null)
    {
      myparticleSystemRenderer.maxParticleSize = newScale;
    }
  }
}
