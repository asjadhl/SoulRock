using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] string stageName;

    [SerializeField] bool isLocked;
    [SerializeField] GameObject squareImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(stageName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked == false)
            squareImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        squareImage.SetActive(false);
    }

}
