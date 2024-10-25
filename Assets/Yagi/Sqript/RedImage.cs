using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RedImage : MonoBehaviour
{
    [SerializeField, Header("�Ԃ����鎞��")] public float _fadeTime;
    [SerializeField, Header("�t�F�[�h�A�E�g����C���[�W�I�u�W�F�N�g")] GameObject _fadeOutObject;
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
        Invoke("Relode", 2f);
    }

    private void Reload()
    {
        Scene scnen = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scnen.name);
    }
}
