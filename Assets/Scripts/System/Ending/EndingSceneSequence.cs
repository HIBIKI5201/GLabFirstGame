using DG.Tweening;
using UnityEngine;

/// <summary>
/// エンディングシーンの演出を制御するクラス
/// </summary>
public class EndingSceneSequence : MonoBehaviour
{
    [SerializeField] private float _moveTime = 7f; // イラストが移動する時間
    [SerializeField] private float _showDuration = 2f; // UIパネルが表示されるフェード時間
    [SerializeField] private Transform _illustrations; // 移動するイラストのオブジェクト
    [SerializeField] private CanvasGroup _canvasGroup;　// 表示するUI

    private void Start()
    {
        if (_canvasGroup == null)
        {
            // CanvasGroupが設定されていなかったら警告を出して、あとの処理は行わないようにする
            Debug.LogAssertion($"{gameObject.name}: CanvasGroupが設定されていません");
            return;
        }
        
        // CanvasGroupは最初は反応しないようにしておく（gameObject.SetActive(false)よりもこちらのほうが軽量）
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        
        _illustrations.DOMoveY(-8f, _moveTime).OnComplete(ShowUI); // アニメーションが終わったらUISet()を呼ぶ
    }

    private void ShowUI()
    {
        // 反応できるようにする
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1, _showDuration); // フェードしながら表示する
    }
}
