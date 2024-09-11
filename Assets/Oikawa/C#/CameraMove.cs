using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform _target;
    [Header("カメラの追跡速度")]
    [SerializeField] float _speed;
    [Header("カメラの移動できる範囲")]
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

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 targetPos = _target.position;
        targetPos.x = Mathf.Clamp(targetPos.x, _minClamp.x, _maxClamp.x);
        targetPos.y = Mathf.Clamp(targetPos.y, _minClamp.y, _maxClamp.y);
        _outPos = Vector3.Lerp(_outPos, targetPos, Time.deltaTime * _speed);

        Vector2 blur = new(
            Mathf.Sin(Time.unscaledTime * 5 * _shakeSpeed) * _shakePower,
            Mathf.Sin(Time.unscaledTime * 6 * _shakeSpeed) * _shakePower);

        Vector3 outPos = _outPos + blur;
        outPos.z = -20;

        _myTra.position = outPos;

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
        Debug.DrawLine(new Vector2(min.x,p.y), new Vector2(max.x, p.y));
        Debug.DrawLine(new Vector2(p.x,min.y), new Vector2(p.x, max.y));
    }
}
