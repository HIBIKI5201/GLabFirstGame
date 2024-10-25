using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RedImage : MonoBehaviour
{
    [SerializeField, Header("赤くする時間")] public float _fadeTime;
    [SerializeField, Header("フェードアウトするイメージオブジェクト")] GameObject _fadeOutObject;
    Image _image;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(2f, _fadeTime);
        Invoke("FadeOut", _fadeTime + 1);
    }

    private void FadeOut()
    {
        _fadeOutObject.SetActive(true);
        Invoke("Reload", 2f);
    }

    private void Reload()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
