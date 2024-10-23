using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダメージを受けた時の演出
/// </summary>
public class DamageEffect : MonoBehaviour
{
    [SerializeField] Image _redImage;
    PlayerController _ctrl;

    void Start()
    {
        _ctrl = GetComponent<PlayerController>();
    }

    public void DamageEffectPlay()
    {
        Image roseImage = _ctrl._rose[0].GetComponent<Image>();
        roseImage.DOColor(new Color(0, 0, 0), 0.3f);
        roseImage.DOFade(0, 0.3f).SetEase(Ease.InQuart);
        Destroy(_ctrl._rose[0], 0.3f);

        _redImage.gameObject.SetActive(true);
        _redImage.color = new Color(255, 0, 0, 0.25f);
        _redImage.DOFade(0, 0.3f);
    }
}
