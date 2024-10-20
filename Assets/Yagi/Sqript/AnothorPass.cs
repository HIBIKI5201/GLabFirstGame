using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnothorPass : MonoBehaviour
{
    [SerializeField] AnotherRoute _anotherClearCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _anotherClearCheck._isPast = true;
    }
}
