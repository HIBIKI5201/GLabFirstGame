using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof (BoxCollider2D))]
public class EnemyActionArea : MonoBehaviour
{
    [Header("Šm”F‚Å‚«‚½Enemy’B")]
    [SerializeField] Enemy[] _enemies;
    Transform[] _enemiesTra;
    void Start()
    {
        _enemies = GameObject.FindObjectsOfType<Enemy>();
        _enemiesTra = new Transform[_enemies.Length];
        for (int i = 0; i < _enemies.Length; i++)
        {
            _enemies[i].enabled = false;
            _enemiesTra[i] = _enemies[i].transform;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("in");
        Transform target = collision.transform;
        if (target.CompareTag("Enemy"))
        {
            Debug.Log("enemy");
            for (int i = 0; i < _enemies.Length; i++)
            {
                if (target == _enemiesTra[i])
                {
                    _enemies[i].enabled = true;
                    return;
                }
            }

        }
    }
}
