using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Vector3 _bigscale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 _normalScale = new Vector3(1f,1f, 1f);
    public Vector3 _newPosition = new Vector3(-15f,0f,0f);

    private RectTransform rt;
    private Vector3 _original;
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        _original = rt.localPosition;
    }

    public void OnSelect(BaseEventData eventData)
    {
        ChangeScale();
        Debug.Log("選択中");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetScale();
        Debug.Log("選択はずれました");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeScale();
        Debug.Log("選択中");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetScale();
        Debug.Log("選択はずれました");
    }



    public void ChangeScale()
    {
        rt.localScale = _bigscale;
        rt.localPosition = _original + _newPosition;
    }

    public void ResetScale()
    {
        rt.localScale = _normalScale;
        rt.localPosition = _original;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
