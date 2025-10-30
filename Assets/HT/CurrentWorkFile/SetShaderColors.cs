using UnityEngine;
using UnityEngine.UI;

public class SetShaderColors : MonoBehaviour
{
  public string prop;
  public Color color;
  public int multiply;
  private Material mat;
  private void Start()
  {
    var image = GetComponent<Image>();
    if (image == null)
      return;
    mat = image.material;
    if (mat == null)
      return;

    
    mat = Object.Instantiate(mat);
    image.material = mat;
    mat.SetColor(prop, color* multiply);
  }
}
