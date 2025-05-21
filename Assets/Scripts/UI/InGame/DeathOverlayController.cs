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
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    public void OnActive()
    {
        gameObject.SetActive(true);
        _image.DOFade(1f, _fadeTime);
        Invoke(nameof(FadeOut), _fadeTime + _displayTime);
    }

    private void FadeOut()
    {
        _fadeOutObject.SetActive(true);
        Invoke(nameof(ReloadScene), _reloadDelay); // _fadeOutObjectのアニメーションを待ってからシーンを読み込みリトライ
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
