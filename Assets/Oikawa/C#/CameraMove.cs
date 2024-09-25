using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform _target;

    [Header("�J�����̒ǐՑ��x")]
    [SerializeField] float _speed;

    [Space]
    [SerializeField] Vector2 _offset;

    [Header("�J�����̈ړ��ł���͈�")]
    [SerializeField] bool _clampEnabled;
    [SerializeField] Vector2 _minClamp;
    [SerializeField] Vector2 _maxClamp;
    Vector2 _outPos;

    [Header("��u��")]
    [SerializeField] float _shakeSpeed;
    [SerializeField] float _shakePower;

    Transform _myTra;
    void Start()
    {
        _myTra = transform;
        _outPos = _myTra.position;
        if(_target == null)
            _target = GameObject.FindAnyObjectByType<PlayerController>().transform;
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
    }
    private void OnDrawGizmosSelected()
    {
        if (!_clampEnabled)
            return;
        Vector2 min = _minClamp;
        Vector2 max = _maxClamp;
        Gizmos.DrawLine(min, new Vector2(min.x, max.y));
        Gizmos.DrawLine(min, new Vector2(max.x, min.y));
        Gizmos.DrawLine(new Vector2(max.x, min.y),max);
        Gizmos.DrawLine(new Vector2(min.x, max.y),max);

        Vector2 p = _myTra.position;
        Vector2 clampP = p;
        clampP.x = Mathf.Clamp(clampP.x, min.x, max.x);
        clampP.y = Mathf.Clamp(clampP.y, min.y, max.y);
        Gizmos.DrawLine(new Vector2(min.x,clampP.y), p);
        Gizmos.DrawLine(new Vector2(max.x,clampP.y), p);
        Gizmos.DrawLine(new Vector2(clampP.x,min.y), p);
        Gizmos.DrawLine(new Vector2(clampP.x,max.y), p);
    }
}
