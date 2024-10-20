using DG.Tweening;
using UnityEngine;

public class TitleImageTween : MonoBehaviour
{
    public MovePatternEnum _movePattern;

    [SerializeField] float _moveTime;
    [SerializeField, Tooltip("MoveYの移動値")] float _distance = 0.1f;
    [SerializeField, Tooltip("Scaleの拡大後の値")] float _scale = 1.03f;
    float _initializePosY;

    void Start()
    {
        switch (_movePattern)
        {
            case MovePatternEnum.MoveY:
                _initializePosY = transform.position.y;
                transform.DOMoveY(_initializePosY - _distance, _moveTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
                break;
            case MovePatternEnum.Scale:
                transform.DOScale(_scale, _moveTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }

    public enum MovePatternEnum
    {
        MoveY,
        Scale,
    }
}
