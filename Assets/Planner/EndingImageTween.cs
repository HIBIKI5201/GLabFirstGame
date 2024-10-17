using DG.Tweening;
using UnityEngine;

public class EndingImageTween : MonoBehaviour
{
    [SerializeField] float _moveTime = 7;
    [SerializeField] GameObject _illustrations, _ui;
    CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = _ui.gameObject.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _illustrations.transform.DOMoveY(-8f, _moveTime);
        Invoke("UISet", _moveTime);
    }

    void UISet()
    {
        _canvasGroup.DOFade(1, 2);
    }
}
