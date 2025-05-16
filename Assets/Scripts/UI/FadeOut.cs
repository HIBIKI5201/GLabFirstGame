using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェードアウト処理
/// </summary>
public class FadeOut : MonoBehaviour
{
    public float _fadeTime; // フェードにかける時間
    private Image _image;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(1f, _fadeTime);
    }
}
