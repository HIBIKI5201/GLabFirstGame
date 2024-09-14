using UnityEditor.U2D.Animation;
using UnityEngine;
public class BackgroundScroll : MonoBehaviour
{
    float _length;
    float _startpos;
    /// <summary> スクロールさせる速さ</summary>
    [SerializeField] float _scrollSpeed;
    [SerializeField, Tooltip("スクロールさせる背景素材")] GameObject _background;
    SpriteRenderer _backgroundSprite;

    /// <summary>背景の複製を保存する</summary>
    SpriteRenderer _backgroundClone1;
    SpriteRenderer _backgroundClone2;

    void Start()
    {
        // 背景画像のx座標
        _startpos = transform.position.x;

        _backgroundSprite = _background.GetComponent<SpriteRenderer>();
        // 背景画像のx軸方向の幅
        _length = _backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.x;

        _backgroundClone1 = Instantiate(_backgroundSprite);
        _backgroundClone1.transform.Translate(-_length, 0, 0);
        _backgroundClone1.transform.SetParent(_backgroundSprite.transform);
        _backgroundClone2 = Instantiate(_backgroundSprite);
        _backgroundClone2.transform.Translate(_length, 0, 0);
        _backgroundClone2.transform.SetParent(_backgroundSprite.transform);

    }
    private void FixedUpdate()
    {
        // 無限スクロールに使用するパラメーター
        float temp = (Camera.main.transform.position.x * (1 - _scrollSpeed));
        // 背景の視差効果に使用するパラメーター
        float dist = (Camera.main.transform.position.x * _scrollSpeed);
        // 視差効果を与える処理
        // 背景画像のx座標をdistの分移動させる
        _background.transform.position = new Vector3(_startpos + dist, _background.transform.position.y, _background.transform.position.z);
        // 無限スクロール
        // 画面外になったら背景画像を移動させる
        if (temp > _startpos + _length) _startpos += _length;
        else if (temp < _startpos - _length) _startpos -= _length;
    }
}