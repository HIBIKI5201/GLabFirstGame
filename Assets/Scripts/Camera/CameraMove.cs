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
    [Tooltip("�J�����̈ړ��͈͂̐��������邩")]
    [SerializeField] bool _clampEnabled;

    [Tooltip("�ړ��͈͂̍����[�̈ʒu")]
    [SerializeField] Vector2 _minClamp;

    [Tooltip("�ړ��͈͂̉E��̈ʒu")]
    [SerializeField] Vector2 _maxClamp;
    Vector2 _outPos;

    [Header("��u��")]
    [Tooltip("��u���ł��邩")]
    [SerializeField] bool _canShake;
    [Tooltip("��u�����x")]
    [SerializeField] float _shakeSpeed = 0.05f;
    [Tooltip("��u���̋���")]
    [SerializeField] float _shakePower = 0.4f;

    Transform _myTra;
    void Start()
    {
        _myTra = transform;
        _outPos = _myTra.position;
        if(_target == null)
            _target = GameObject.FindAnyObjectByType<PlayerController>().transform;
        _myTra.position = _target.position;
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

        Vector2 blur = Vector2.zero;
        if (_canShake)
        {
            blur = new(
            Mathf.Sin(Time.unscaledTime * 5 * _shakeSpeed) * _shakePower,
            Mathf.Sin(Time.unscaledTime * 6 * _shakeSpeed) * _shakePower);
        }

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
