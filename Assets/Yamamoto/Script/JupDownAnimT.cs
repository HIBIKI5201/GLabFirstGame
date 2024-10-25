using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JupDownAnimT : MonoBehaviour
{
    Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator  = GetComponent<Animator>();   
    }

    void IsJumpFalseTiming()
    {
        _animator.SetBool("isJump",false);
    }
    // Update is called once per frame
    void Update()
    {
        if (_animator.GetBool("Jump"))
        {
            Invoke("IsJumpFalseTiming", 0.6f);
        }
    }
}
