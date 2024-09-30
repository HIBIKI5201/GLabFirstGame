using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RedImage : MonoBehaviour
{
    [Header("赤くする時間"), SerializeField] public float _fadeTime;
    [Header("フェードアウトするイメージオブジェクト"), SerializeField] GameObject _fadeOutObject;
    Image _image;

    private void Awake()
    {
        this.gameObject.SetActive(false);
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
        Invoke("Relode", 2f);
    }

    private void Relode()
    {
        Scene _scnen = SceneManager.GetActiveScene();
        SceneManager.LoadScene(_scnen.name);
    }
}
