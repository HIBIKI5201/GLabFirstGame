using UnityEngine;
using Cinemachine;

public class DamageCamera : MonoBehaviour
{
    CinemachineImpulseSource _impulse;
    private void Start()
    {
        _impulse = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// ƒJƒƒ‰‚ğ—h‚ç‚·
    /// </summary>
    public void Shake()
    {
        _impulse.GenerateImpulse();
    }
}
