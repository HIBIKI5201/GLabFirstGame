using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxisRaw("Horizontal");
        _animator.SetFloat("MoveX", a);

        if (Input.GetButtonDown("Jump"))
        {
            _animator.SetBool("Jump", true);
            Invoke("JumpReset", 0.5f);
        }
    }

    void JumpReset()
    {
        _animator.SetBool("Jump", false);
    }
}
