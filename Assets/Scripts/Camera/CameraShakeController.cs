using UnityEngine;
using Cinemachine;

/// <summary>
/// Cinemachineを使用してカメラシェイクエフェクトを制御するクラス
/// </summary>
public class CameraShakeController : MonoBehaviour
{
    private CinemachineImpulseSource _impulse;
    
    private void Start()
    {
        _impulse = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// カメラシェイクを実行する
    /// </summary>
    public void TriggerShake()
    {
        _impulse.GenerateImpulse();
    }
}
