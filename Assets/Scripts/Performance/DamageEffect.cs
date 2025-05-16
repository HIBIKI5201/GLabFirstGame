using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダメージを受けた時の演出
/// </summary>
public class DamageEffect : MonoBehaviour
{
    [SerializeField] Image _redImage;
    [SerializeField] Image _vignette;
    bool _dying;
    PlayerController _ctrl;

    void Start()
    {
        _ctrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        if(GameManager.instance._state == GameManager.GameState.StageClear)
        {
            _vignette.gameObject.SetActive(false);
        }

        if(!_dying && _ctrl.CurrentHp == 1) 
        {
            Dying();
            _dying = true;
        }
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

    /// <summary>
    /// 瀕死のときの演出
    /// </summary>
    void Dying()
    {
        _vignette.gameObject.SetActive(true);
        _vignette.DOFade(0, 1f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo);
    }
}
