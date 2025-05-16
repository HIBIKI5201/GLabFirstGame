using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

/// <summary>
/// シーン読み込みを行うクラス
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneType _targetSceneType; // 移行したいシーンタイプ
    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _fadeTime;
    [SerializeField] private Color _fadeColor;
    public static bool IsFading { get; private set; }
    
    private void Start()
    {
        Time.timeScale = 1;
        if (_fadePanel != null)
        {
            _fadePanel.color = Color.clear;

        }
        else
        {
            Debug.LogError("FadePanelが設定されていません");
        }
    }
    
    private void SceneLoad()
    {
        AudioManager.Instance.SESource.volume = 1f;
        IsFading = false;
        SceneManager.LoadScene(Enum.GetName(typeof(SceneType), _targetSceneType));
    }
    
    public void FadeAndLoadScene()
    {
        IsFading = true;
        _fadePanel.gameObject.SetActive(true);
        DOTween.To(() => _fadePanel.color, c => _fadePanel.color = c, _fadeColor, _fadeTime).OnComplete(SceneLoad);
    }
    
    public void FadeAndLoadScene(Image image, Color fadeColor, float fadeTime, SceneType sceneType)
    {
        IsFading = true;
        DOTween.To(() => image.color, c => image.color = c, fadeColor, _fadeTime).SetUpdate(true).OnComplete(
        () =>
        {
            IsFading = false;
            SceneManager.LoadScene(Enum.GetName(typeof(SceneType), sceneType));
        });
        AudioManager.Instance.OnFading();
    }
}
