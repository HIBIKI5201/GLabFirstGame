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
                Image.color = _fadeColor;
                Debug.Log(Image.color);
                IsFading = true;
                DOTween.To(() => Image.color, c => Image.color = c, Color.clear, _fadeTime).OnComplete(
                    () =>
                    {
                        Image.gameObject.SetActive(false);
                        IsFading = false;
                    });
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
        Image.gameObject.SetActive(true);
        DOTween.To(() => Image.color, c => Image.color = c, _fadeColor, _fadeTime).OnComplete(SceneLoad);
    }
    void SceneLoad()
    {
        IsFading = false;
        Image = null;
        SceneManager.LoadScene(Enum.GetName(typeof(Scenes), _scene));
    }
    /// <summary>
    /// シーンを呼び出すメソッド
    /// </summary>
    [ContextMenu("シーンを呼び出すメソッド")]
    public void FadeAndLoadScene(Image image, Color fadeColor, float fadeTime, Scenes scene)
    {
        IsFading = true;
        Image.gameObject.SetActive(true);
        DOTween.To(() => image.color, c => image.color = c, fadeColor, _fadeTime).SetUpdate(true).OnComplete(
        () =>
        {
            IsFading = false;
            Image = null;
            SceneManager.LoadScene(Enum.GetName(typeof(Scenes), scene));
        });
    }
}
