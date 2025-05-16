using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Vector3 _bigscale;
    public Vector3 _normalScale;
    public Vector2 _newPosition = new Vector2(-15f, 0f);
    [SerializeField] Sprite [] _buttonImage;
    public Image _currentImage;

    private RectTransform rt;
    private Vector2 _originalPosition;
    // Start is called before the first frame update
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        _normalScale = rt.localScale;
        _bigscale = _normalScale * 1.1f;
        _originalPosition = rt.anchoredPosition;
    }

    public void OnSelect(BaseEventData eventData)
    {
        ChangeImage(1);
        ChangeScale(_bigscale);
        AudioManager.Instance.PlaySE("cursor");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ChangeImage(0);
        ResetScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeImage(1);
        ChangeScale(_bigscale);
        AudioManager.Instance.PlaySE("cursor");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeImage(0);
        ResetScale();
    }



    public void ChangeScale(Vector3 _big)
    {
        rt.localScale = _big;
        rt.anchoredPosition = _originalPosition +_newPosition;
    }

    public void ResetScale()
    {
        rt.localScale = _normalScale;
        rt.anchoredPosition = _originalPosition;
    }

    public void ChangeImage(int _index)
    {
        _currentImage.sprite = _buttonImage[_index];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
