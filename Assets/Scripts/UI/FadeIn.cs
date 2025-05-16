using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// フェードイン処理
/// </summary>
public class FadeIn : MonoBehaviour
{
    [SerializeField] private float _fadeTime = 2f; // フェードにかける時間
    private Image _image;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(0,_fadeTime).OnComplete(() => Destroy(this)); // フェードアニメーションのあと自身を破棄する
    }
}
