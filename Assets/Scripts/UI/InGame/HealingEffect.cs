using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealingEffect : MonoBehaviour
{
    [SerializeField] private Image _vignette; // 回復時のビネット
    [SerializeField] private float _healingEffectDuration = 0.3f; // 回復エフェクトの再生時間
    private PlayerController _playerController;
    private DamageEffect _damageEffect;
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _damageEffect = GetComponent<DamageEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 回復エフェクトを再生する
    /// </summary>
    public void PlayHealingEffect()
    {
        HpIconAnimation();
        HealingVignetteAnimation();
        if(_playerController.CurrentHp > 1)
        {
            _damageEffect.StopDyingEffect();
        }
    }
    /// <summary>
    /// HPアイコンのアニメーション。黒からFadeIn
    /// </summary>
    private void HpIconAnimation()
    {
        for (int i = 0; i < _playerController.MaxHp; i++)
        {
            Image rose = _playerController._rose[i].GetComponent<Image>();
            if (i < _playerController.CurrentHp)
            {
                if (!rose.gameObject.activeInHierarchy)
                {
                    rose.gameObject.SetActive(true);
                    rose.color = new Color(0f, 0f, 0f, 0f);
                    rose.DOColor(new Color(1f, 1f, 1f), _healingEffectDuration);
                    rose.DOFade(1, _healingEffectDuration).SetEase(Ease.InQuart);
                }
            }
        }
    }
    /// <summary>
    /// 回復ビネットのアニメーション
    /// </summary>
    private void HealingVignetteAnimation()
    {
        _vignette.gameObject.SetActive(true); // 回復ビネットを表示する
        _vignette.color = new Color(0f, 1f, 0f, 0.25f); // 緑に設定する
        _vignette.DOFade(0, _healingEffectDuration); // フェードアウト
    }
}
