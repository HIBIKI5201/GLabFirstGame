using UnityEngine;

/// <summary>
/// 背景スクロールを行うクラス
/// </summary>
public class BackgroundScroll : MonoBehaviour
{
    private float _length; // SpriteRendererの横軸の長さ（背景画像の幅）
    private float _startpos; // 背景の初期位置を保存するための変数
    
    [Header("基本設定")]
    [SerializeField] private float _scrollSpeed;
    [SerializeField] private GameObject _background;
    private SpriteRenderer _backgroundSprite; // メイン背景のSpriteRendererコンポーネント
    private SpriteRenderer _backgroundClone1; // 左側のクローン背景
    private SpriteRenderer _backgroundClone2; // 右側のクローン背景

    private void Start()
    {
        _scrollSpeed *= -1; 
        _startpos = _background.transform.position.x;

        _backgroundSprite = _background.GetComponent<SpriteRenderer>();
        _length = _backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.x; // 背景画像の幅を取得

        // 左側のクローン背景を生成（メイン背景の左側に配置）
        _backgroundClone1 = Instantiate(_backgroundSprite, _backgroundSprite.transform, true);
        _backgroundClone1.transform.Translate(-_length, 0, 0);

        // 右側のクローン背景を生成（メイン背景の右側に配置）
        _backgroundClone2 = Instantiate(_backgroundSprite, _backgroundSprite.transform, true);
        _backgroundClone2.transform.Translate(_length, 0, 0);
    }
    
    private void FixedUpdate()
    {
        // カメラの動きに合わせて背景をスクロールさせ、必要に応じて位置をリセットする
        
        float temp = Camera.main.transform.position.x * (1 - _scrollSpeed); // カメラの動きとスクロール速度に基づく補正値の計算
        float dist = Camera.main.transform.position.x * _scrollSpeed; // スクロール距離の計算
        
        // メイン背景の位置を更新（Y, Z座標は変更しない）
        _background.transform.position = new Vector3(_startpos + dist, _background.transform.position.y, _background.transform.position.z);

        // カメラが一定距離移動したら背景の起点位置をリセット
        if (temp > _startpos + _length)
        {
            _startpos += _length; 
        }
        else if (temp < _startpos - _length)
        {
            _startpos -= _length;
        }
    }
}