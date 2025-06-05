using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct GroundedRay
{
    [Header("地面と壁のレイヤーマスク設定")]
    [FormerlySerializedAs("_mask")]
    public LayerMask RaycastGroundMask;

    [FormerlySerializedAs("_sideMask")]
    public LayerMask RaycastSideMask;

    [Header("現在の位置から左と右の距離差")]
    [FormerlySerializedAs("_rightRayPos")]
    public Vector2 RightRayOriginOffset;

    [FormerlySerializedAs("_leftRayPos")]
    public Vector2 LeftRayOriginOffset;

    [Header("地面との垂直距離がこれより短いなら、接地と見なす")]
    [FormerlySerializedAs("_rayLong")]
    public float GroundCheckRayDistance;

    [Header("壁との距離がこれより短いなら、張り付いていると見なす")]
    [FormerlySerializedAs("_sideRayLong")]
    public float SideCheckRayDistance;

    [Header("壁との距離がこれより短いなら、乗り越えられる")]
    [FormerlySerializedAs("_jumpRayLong"), FormerlySerializedAs("MaxJumpableDistanceFromWall")]
    public float MaxJumpDistanceFromWall;
}
