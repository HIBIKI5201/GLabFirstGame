using UnityEngine;
using DG.Tweening;

public class ClearUI : MonoBehaviour
{
    void OnEnable()
    {
        transform.DOScale(1f, 0.7f).SetEase(Ease.OutBounce);
    }
}
