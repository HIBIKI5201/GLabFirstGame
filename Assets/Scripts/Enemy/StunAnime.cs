using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 石アイテムが当たった時のスタンアニメーション
/// </summary>
[ExecuteInEditMode]
public class StunAnime : MonoBehaviour
{
    [SerializeField] List<Transform> _spritesTra;
    [SerializeField] Vector2 _a;
    [SerializeField] float _speed;
    
    private void Start()
    {
        _spritesTra = GetComponentsInChildren<Transform>().ToList();
        _spritesTra.RemoveAt(0);
    }

    private void Update()
    {
        Animation();
    }
    
    private void Animation()
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
