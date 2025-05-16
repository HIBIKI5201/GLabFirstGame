using DG.Tweening;
using UnityEngine;

/// <summary>
/// UI要素に対して単純なアニメーション効果を適用するクラス
/// </summary>
public class UIAnimationHelper : MonoBehaviour
{
    [SerializeField] private UIAnimationType _movePattern; // 動き方のパターン
    [SerializeField] private float _moveTime; // アニメーションにかける時間
    [SerializeField, Tooltip("MoveYの移動値")] float _distance = 0.1f;
    [SerializeField, Tooltip("Scaleの拡大後の値")] float _scale = 1.03f;
    private float _initializePosY; // 初期のY座標

    private void Start()
    {
        switch (_movePattern)
        {
            case UIAnimationType.MoveY:
                _initializePosY = transform.position.y; // 初期値を保存
                transform.DOMoveY(_initializePosY - _distance, _moveTime)
                    .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo); // Y軸方向に移動するアニメーション
                break;
            
            case UIAnimationType.Scale:
                transform.DOScale(_scale, _moveTime)
                    .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo); // 拡大アニメーション
                break;
        }
    }
}
