using DG.Tweening;
using DG.Tweening.Core;
using UniRx;
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
    private CompositeDisposable _stateDisposable = new CompositeDisposable();
    public TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _dyingVignette = null; // 瀕死ビネットのTweenerオブジェクト

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        GameManager.Instance.CurrentStateProp
            .Where(state => state == GameStateType.StageClear) // ステートがStageClearになったら実行する
            .Subscribe(_ =>
            {
                // ステージクリア時はビネットエフェクトを非表示にする
                _vignette.gameObject.SetActive(false);
            })
            .AddTo(_stateDisposable);
    }

    private void Update()
    {
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
        for(int i = 0; i < _playerController.MaxHp; i++)
        {
            Image rose = _playerController._rose[i].GetComponent<Image>();
            if (i >= _playerController.CurrentHp)
            {
                if (rose.gameObject.activeInHierarchy)
                {
                    rose.DOColor(new Color(0f, 0f, 0f), _damageEffectDuration);
                    rose.DOFade(1, _damageEffectDuration).SetEase(Ease.InQuart).OnComplete(() => { rose.gameObject.SetActive(false); });
                }
            }
        }
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
        _dyingVignette = _vignette.DOFade(0, 1f).SetEase(Ease.OutQuart).SetLoops(-1, LoopType.Yoyo); // 明滅するアニメーション
    }

    /// <summary>
    /// 瀕死状態ビネットの点滅を停止
    /// </summary>
    public void StopDyingEffect()
    {
        _isDying = false;
        if(_dyingVignette != null)
        {
            _dyingVignette.Kill();
            _dyingVignette = null;
        }
    }

    private void OnDestroy()
    {
        _stateDisposable?.Dispose();
    }
}
