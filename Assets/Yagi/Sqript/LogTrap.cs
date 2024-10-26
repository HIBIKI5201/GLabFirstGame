using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTrap : MonoBehaviour
{
    [SerializeField, Header("�ۑ��̑���")] private float _swingSpeed;
    private float _rotationz = 90;
    private bool _isSwingRight;
    bool _isSwing = false;

    void Update()
    {
        if (_isSwing)
        {
            //�ۑ��̎���U��
            Swing();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _isSwing = true;
        }
    }

    void Swing()
    {
        //Z���W�̃��[�e�[�V������90�ɂȂ�܂ő��₷
        if (_isSwingRight && _rotationz < 90)
        {
            _rotationz += Time.deltaTime + _swingSpeed / 10;
        }
        else
        {
            if (_rotationz >= 90 && _isSwingRight == true)
            {
                _isSwing = false;
            }
            _isSwingRight = false;
        }
        //Z���W�̃��[�e�[�V������-90�ɂȂ�܂Ō��炷
        if (_isSwingRight == false && _rotationz > -90)
        {
            _rotationz -= Time.deltaTime + _swingSpeed / 10;
        }
        else
        {
            _isSwingRight = true;
        }

        //���ۂɃ��[�e�[�V�����𓮂���
        transform.localEulerAngles
               = new(transform.localEulerAngles.x, transform.localEulerAngles.y, _rotationz);
    }
}
