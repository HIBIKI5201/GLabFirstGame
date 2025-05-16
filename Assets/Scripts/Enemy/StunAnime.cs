using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[ExecuteInEditMode]
public class StunAnime : MonoBehaviour
{
    [SerializeField] List<Transform> _spritesTra;
    [SerializeField] Vector2 _a;
    [SerializeField] float _speed;
    void Start()
    {
        _spritesTra = GetComponentsInChildren<Transform>().ToList();
        _spritesTra.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        AAAAAA();
    }
    void AAAAAA()
    {
        float t;
        float rot;
        for(int i = 0; i < _spritesTra.Count; i++)
        {
            t = (float)i / _spritesTra.Count;
            rot = Mathf.Lerp(0, 360, t) + Time.time * _speed;
            rot *= Mathf.Deg2Rad;
            _spritesTra[i].localPosition = new Vector2(Mathf.Sin(rot), Mathf.Cos(rot)) * _a;
        }
    }
}
