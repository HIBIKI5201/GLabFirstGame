using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーがダメージを受けた時の演出を管理するクラス
/// </summary>
public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Image _redImage; // ダメージ時に表示される赤い画面オーバーレイ
    [SerializeField] private Image _vignette; // プレイヤーが瀕死状態のときに表示されるビネットエフェクト
    [SerializeField] private float _damageEffectDuration = 0.3f; // ダメージエフェクトの点滅間隔
    private bool _isDying; // プレイヤーが現在瀕死状態かどうかのフラグ
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(GameManager.instance.stateType == GameStateType.StageClear)
        {
            // ステージクリア時はビネットエフェクトを非表示にする
            _vignette.gameObject.SetActive(false);
        }

        if(!_isDying && _playerController.CurrentHp == 1) 
        {
            // HPが1になった時、まだ瀕死状態になっていなければ瀕死エフェクトを開始
            Dying();
        }
    }

    /// <summary>
    /// ダメージエフェクトを再生する
    /// </summary>
    public void PlayDamageEffect()
    {
        HpIconAnimation();
        RedImageAnimation();
    }

    /// <summary>
    /// HPアイコンのアニメーション
    /// </summary>
    private void HpIconAnimation()
    {
        Image hpIcon = _playerController._rose[0].GetComponent<Image>(); // 参照を取得する
        
        // 黒色に変化させつつフェードアウトし、破棄
        hpIcon.DOColor(new Color(0, 0, 0), _damageEffectDuration);
        hpIcon.DOFade(0, _damageEffectDuration).SetEase(Ease.InQuart);
        Destroy(_playerController._rose[0], _damageEffectDuration);
    }
    
    /// <summary>
    /// 最前面に出てくる赤いパネルのアニメーション
    /// </summary>
    private void RedImageAnimation()
    {
        _redImage.gameObject.SetActive(true); // 赤いパネルを表示する
        _redImage.color = new Color(255, 0, 0, 0.25f);
        _redImage.DOFade(0, _damageEffectDuration); // フェードアウト
    }

    /// <summary>
    /// 瀕死のときの演出
    /// </summary>
    private void Dying()
    {
        _isDying = true;
        _vignette.gameObject.SetActive(true); // ビネットを表示
        _vignette.DOFade(0, 1f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo); // 明滅するアニメーション
    }
}
