using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform _target;

    [Header("カメラの追跡速度")]
    [SerializeField] float _speed;

    [Space]
    [SerializeField] Vector2 _offset;

    [Header("カメラの移動できる範囲")]
    [SerializeField] bool _clampEnabled;
    [SerializeField] Vector2 _minClamp;
    [SerializeField] Vector2 _maxClamp;
    Vector2 _outPos;

    [Header("手ブレ")]
    [SerializeField] float _shakeSpeed;
    [SerializeField] float _shakePower;

    Transform _myTra;
    void Start()
    {
        _myTra = transform;
        _outPos = _myTra.position;
    }

    void FixedUpdate()
    {
        Vector2 targetPos = _offset + (Vector2)_target.position;
        if (_clampEnabled)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, _minClamp.x, _maxClamp.x);
            targetPos.y = Mathf.Clamp(targetPos.y, _minClamp.y, _maxClamp.y);
        }
        _outPos = Vector3.Lerp(_outPos, targetPos, Time.deltaTime * _speed);

        Vector2 blur = new(
            Mathf.Sin(Time.unscaledTime * 5 * _shakeSpeed) * _shakePower,
            Mathf.Sin(Time.unscaledTime * 6 * _shakeSpeed) * _shakePower);

        Vector3 outPos = _outPos + blur;
        outPos.z = -20;

        _myTra.position = outPos;

        if (_clampEnabled)
            DebugClamp();
    }
    void DebugClamp()
    {
        Vector2 min = _minClamp;
        Vector2 max = _maxClamp;
        Debug.DrawLine(min, new Vector2(min.x, max.y));
        Debug.DrawLine(min, new Vector2(max.x, min.y));
        Debug.DrawLine(new Vector2(max.x, min.y),max);
        Debug.DrawLine(new Vector2(min.x, max.y),max);

        Vector2 p = _myTra.position;
        Vector2 clampP = p;
        clampP.x = Mathf.Clamp(clampP.x, min.x, max.x);
        clampP.y = Mathf.Clamp(clampP.y, min.y, max.y);
        Debug.DrawLine(new Vector2(min.x,clampP.y), p);
        Debug.DrawLine(new Vector2(max.x,clampP.y), p);
        Debug.DrawLine(new Vector2(clampP.x,min.y), p);
        Debug.DrawLine(new Vector2(clampP.x,max.y), p);
    }
}
