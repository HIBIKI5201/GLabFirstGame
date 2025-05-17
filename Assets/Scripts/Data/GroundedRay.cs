using System;
using UnityEngine;

[Serializable]
public struct GroundedRay
{
    [Tooltip("Ray����������LayerMask")]
    public LayerMask _mask;

    [Tooltip("����Ray����������LayerMask")]
    public LayerMask _sideMask;

    [Tooltip("�ǂ�G�A�v���C���[�𔻒f����")]
    public Vector2 _sideRayPos;

    [Tooltip("�E���̊R�𔻒f����Ray�̒��S")]
    public Vector2 _rightRayPos;

    [Tooltip("�����̊R�𔻒f����Ray�̒��S")]
    public Vector2 _leftRayPos;

    [Space, Tooltip("�R�𔻒f����Ray�̒���")]
    public float _rayLong;

    [Tooltip("�ǂ𔻒f����Ray�̒���")]
    public float _sideRayLong;

    [Space, Tooltip("��Ԃ̂𔻒f����Ray�̒���")]
    public float _jumpRayLong;
}
