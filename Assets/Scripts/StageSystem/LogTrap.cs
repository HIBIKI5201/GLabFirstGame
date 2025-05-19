using UnityEngine;

/// <summary>
/// 丸太トラップを制御するクラス
/// </summary>
public class LogTrap : MonoBehaviour
{
    [SerializeField] private float _swingSpeed; // 振り子のスピード
    private float _currentAngle = 90; // 現在の覚悟
    private bool _isSwinginRight; // 右側に振れている状態か
    bool _isActive = false; // トラップが作動中か

    private void Update()
    {
        if (_isActive)
        {
            UpdatePendulumMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isActive = true; // プレイヤーがコライダーに触れたら動作を開始する
        }
    }

    /// <summary>
    /// 振り子の動きを更新する
    /// </summary>
    private void UpdatePendulumMovement()
    {
        float swingAmount = Time.deltaTime * _swingSpeed;
        
        if (_isSwinginRight && _currentAngle < 90)
        {
            _currentAngle += swingAmount / 10; // 右方向への揺れ
        }
        else
        {
            if (_currentAngle >= 90 && _isSwinginRight)
            {
                _isActive = false;
            }
            _isSwinginRight = false;　// 最大角度に達したら方向転換
        }
        
        if (!_isSwinginRight && _currentAngle > -90)
        {
            _currentAngle -= swingAmount / 10; // 左方向への揺れ
        }
        else
        {
            _isSwinginRight = true; // 最小角度に達したら方向転換
        }

        // オブジェクトの回転を変更する
        transform.localEulerAngles
               = new(transform.localEulerAngles.x, transform.localEulerAngles.y, _currentAngle);
    }
}
