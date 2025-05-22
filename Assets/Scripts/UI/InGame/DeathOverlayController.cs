using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 死亡時の赤いオーバーレイパネルのアニメーション
/// </summary>
public class DeathOverlayController : MonoBehaviour
{
    [SerializeField] private float _fadeTime; // フェードインにかける時間
    [SerializeField] private float _displayTime = 1f; // 表示し続ける時間
    [SerializeField] private float _reloadDelay = 2f; // リロードまでの待機時間
    [SerializeField] private GameObject _fadeOutObject;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        
    }
    
    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    public async UniTask OnActive()
    {
        _image.DOFade(1f, _fadeTime);
        
        await UniTask.Delay(TimeSpan.FromSeconds(_fadeTime + _displayTime));
        
        _fadeOutObject.SetActive(true);
     
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadDelay));
        
        ReloadScene();
    }

    /// <summary>
    /// 同じシーンをもう一度読み込み、ゲームをリトライする
    /// </summary>
    private void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
