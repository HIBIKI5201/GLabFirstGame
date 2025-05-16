using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [Header("フェードアウトにかける時間"),SerializeField] public float _fadeTime;
    Image _image;
    void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(2f, _fadeTime);
    }
}
