using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTrap : MonoBehaviour
{
    [SerializeField, Header("丸太の速さ")] private float _swingSpeed;
    private float _rotationz = 90;
    private bool _isSwingRight;
    bool _isSwing = false;

    void Update()
    {
        if (_isSwing)
        {
            //丸太の軸を振る
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
        //Z座標のローテーションを90になるまで増やす
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
        //Z座標のローテーションを-90になるまで減らす
        if (_isSwingRight == false && _rotationz > -90)
        {
            _rotationz -= Time.deltaTime + _swingSpeed / 10;
        }
        else
        {
            _isSwingRight = true;
        }

        //実際にローテーションを動かす
        transform.localEulerAngles
               = new(transform.localEulerAngles.x, transform.localEulerAngles.y, _rotationz);
    }
}
