using System;
using UnityEngine;

[Serializable]
public struct GroundedRay
{
    public LayerMask _mask;
    public LayerMask _sideMask;
    public Vector2 _sideRayPos;
    public Vector2 _rightRayPos;
    public Vector2 _leftRayPos;
    public float _rayLong;
    public float _sideRayLong;
    public float _jumpRayLong;
}
