using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [Header("�t�F�[�h�A�E�g�ɂ����鎞��"),SerializeField] public float _fadeTime;
    Image _image;
    void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(2f, _fadeTime);
    }
}
