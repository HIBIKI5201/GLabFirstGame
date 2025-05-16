using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeIn : MonoBehaviour
{
    public float _fadeTime = 2f;
    Image _image;
    void Start()
    {
        _image = GetComponent<Image>();
        _image.DOFade(0,_fadeTime);
        Destroy(this.gameObject,_fadeTime);
    }

    private void Delete()
    {
        Destroy(this.gameObject);
    }
}
