using DG.Tweening;
using UnityEngine;

public class TitleImageTween : MonoBehaviour
{
    [SerializeField] float _distance = 0.1f;
    float _initializePosY;

    void Start()
    {
        _initializePosY = transform.position.y;
        transform.DOMoveY(_initializePosY - _distance, 3).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }
}
