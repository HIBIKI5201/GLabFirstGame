using UnityEngine;
using DG.Tweening;

/// <summary>
/// ステージクリア時に表示するUIのアニメーション
/// </summary>
public class StageClearUIAnimator : MonoBehaviour
{
    [SerializeField] private float _duration = 0.7f;
    
    private void OnEnable()
    {
        transform.DOScale(1f, _duration).SetEase(Ease.OutBounce);
    }
}
