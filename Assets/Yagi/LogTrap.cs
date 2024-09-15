using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTrap : MonoBehaviour
{
    [SerializeField] private float _swingSpeed;
    private float _rotationz;
    private bool _isSwingRight;
    void Update()
    {
        Swing();
    }

    void Swing()
    {
        if (_isSwingRight && _rotationz < 90)
        {
            _rotationz += Time.deltaTime + _swingSpeed;
        }
        else
        {
            _isSwingRight = false;
        }

        if (_isSwingRight == false && _rotationz > -90)
        {
            _rotationz -= Time.deltaTime + _swingSpeed;
        }
        else
        {
            _isSwingRight = true;
        }
        transform.localEulerAngles
               = new(transform.localEulerAngles.x, transform.localEulerAngles.y, _rotationz);
    }
}
