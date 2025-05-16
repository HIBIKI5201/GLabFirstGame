using UnityEngine;

/// <summary>
/// カメラの動きを制御するクラス
/// </summary>
[ExecuteInEditMode]
public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Header("カメラの追跡速度")]
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _offset;

    [Header("カメラの移動できる範囲")]
    [Tooltip("カメラの移動範囲を制限するか")] [SerializeField] private bool _clampEnabled;
    [SerializeField] private Vector2 _minClamp;
    [SerializeField] private Vector2 _maxClamp;
    private Vector2 _outPos;

    [Header("手ブレ")]
    [SerializeField] private bool _canShake;
    [SerializeField] private float _shakeSpeed = 0.05f;
    [SerializeField] private float _shakePower = 0.4f;

    private void Start()
    {
        _outPos = transform.position;
        
        if (_target == null)
        {
            // ターゲットが設定されていなかったらPlayerを探す
            _target = FindAnyObjectByType<PlayerController>().transform;
        }
        
        transform.position = _target.position;
        _outPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 targetPos = _offset + (Vector2)_target.position;
        
        if (_clampEnabled)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, _minClamp.x, _maxClamp.x);
            targetPos.y = Mathf.Clamp(targetPos.y, _minClamp.y, _maxClamp.y);
        }
        
        _outPos = Vector3.Lerp(_outPos, targetPos, Time.deltaTime * _speed);

        Vector2 blur = Vector2.zero;
        
        if (_canShake)
        {
            blur = new(
            Mathf.Sin(Time.unscaledTime * 5 * _shakeSpeed) * _shakePower,
            Mathf.Sin(Time.unscaledTime * 6 * _shakeSpeed) * _shakePower);
        }

        Vector3 outPos = _outPos + blur;
        outPos.z = -20;

        transform.position = outPos;
    }
    
    /// <summary>
    /// ギズモを描写する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!_clampEnabled) return;
        
        Vector2 min = _minClamp;
        Vector2 max = _maxClamp;
        Gizmos.DrawLine(min, new Vector2(min.x, max.y));
        Gizmos.DrawLine(min, new Vector2(max.x, min.y));
        Gizmos.DrawLine(new Vector2(max.x, min.y),max);
        Gizmos.DrawLine(new Vector2(min.x, max.y),max);

        Vector2 p = transform.position;
        Vector2 clampP = p;
        clampP.x = Mathf.Clamp(clampP.x, min.x, max.x);
        clampP.y = Mathf.Clamp(clampP.y, min.y, max.y);
        Gizmos.DrawLine(new Vector2(min.x,clampP.y), p);
        Gizmos.DrawLine(new Vector2(max.x,clampP.y), p);
        Gizmos.DrawLine(new Vector2(clampP.x,min.y), p);
        Gizmos.DrawLine(new Vector2(clampP.x,max.y), p);
    }
}
