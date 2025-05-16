using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ボタンのホバー時などのアニメーションをまとめたクラス
/// </summary>
public class ButtonAnimationController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Vector2 _newPosition = new Vector2(-15f, 0f);
    [SerializeField] Sprite [] _buttonImage;
    [SerializeField] private Image _currentImage;
    
    private RectTransform _rt;
    private Vector3 _enlargedScale; // 拡大時のScale
    private Vector3 _defaultScale; // デフォルトのScale
    private Vector2 _defaultPosition; // デフォルトのアンカーポジション
    
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _defaultScale = _rt.localScale;
        _enlargedScale = _defaultScale * 1.1f;
        _defaultPosition = _rt.anchoredPosition;
    }

    /// <summary>
    /// ボタンが選択された時
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        ChangeSprite(1);
        ChangeScale(_enlargedScale);
        AudioManager.Instance.PlaySE("cursor");
    }

    /// <summary>
    /// ボタンの選択が解除された時
    /// </summary>
    public void OnDeselect(BaseEventData eventData)
    {
        ChangeSprite(0);
        ResetScale();
    }

    /// <summary>
    /// ポインター（マウスなど）がボタン領域に入った時
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeSprite(1);
        ChangeScale(_enlargedScale);
        AudioManager.Instance.PlaySE("cursor");
    }

    /// <summary>
    /// ポインター（マウスなど）がボタン領域から出た時
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeSprite(0);
        ResetScale();
    }
    
    private void ChangeScale(Vector3 scale)
    {
        _rt.localScale = scale;
        _rt.anchoredPosition = _defaultPosition + _newPosition;
    }

    private void ResetScale()
    {
        _rt.localScale = _defaultScale;
        _rt.anchoredPosition = _defaultPosition;
    }

    /// <summary>
    /// ボタンの画像を変更する
    /// </summary>
    private void ChangeSprite(int index)
    {
        _currentImage.sprite = _buttonImage[index];
    }
}
