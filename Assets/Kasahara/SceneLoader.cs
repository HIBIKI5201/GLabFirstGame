using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Scenes _scene;
    [SerializeField] GameObject _fadePanel;
    [SerializeField] float _fadeTime;
    [SerializeField] Color _fadeColor;
    public static Image Image;
    public static bool IsFading { get; private set; }
    private void Start()
    {
        Time.timeScale = 1;
        if (_fadePanel != null)
        {
            if (Image == null)
            {
                Image = _fadePanel.GetComponent<Image>();
                Image.color = Color.clear;
            }
        }
        else
        {
            Debug.LogError("FadePanelがセットされていません");
        }
    }
    public enum Scenes
    {
        None = -1,
        Title,
        InGame,
        Ending,
        SelectStage,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
    }
    /// <summary>
    /// シーンを呼び出すメソッド
    /// </summary>
    [ContextMenu("シーンを呼び出すメソッド")]
    public void FadeAndLoadScene()
    {
        IsFading = true;
        Image = _fadePanel.GetComponent<Image>();
        Image.gameObject.SetActive(true);
        DOTween.To(() => Image.color, c => Image.color = c, _fadeColor, _fadeTime).OnComplete(SceneLoad);
    }
    void SceneLoad()
    {
        AudioManager.Instance._seSource.volume = 1f;
        IsFading = false;
        Image = null;
        //Debug.Log("LoadScene");
        SceneManager.LoadScene(Enum.GetName(typeof(Scenes), _scene));
    }
    /// <summary>
    /// シーンを呼び出すメソッド
    /// </summary>
    [ContextMenu("シーンを呼び出すメソッド")]
    public void FadeAndLoadScene(Image image, Color fadeColor, float fadeTime, Scenes scene)
    {
        IsFading = true;
        DOTween.To(() => image.color, c => image.color = c, fadeColor, _fadeTime).SetUpdate(true).OnComplete(
        () =>
        {
            IsFading = false;
            SceneManager.LoadScene(Enum.GetName(typeof(Scenes), scene));
        });
    }
}
