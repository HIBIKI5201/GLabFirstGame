using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HideGrass : MonoBehaviour
{
    private const string k_player = "Player";
    private const string k_grassIn = "grassIn";
    private const string k_grassOut = "grassOut";

    [SerializeField, ReadOnly]
    private SpriteRenderer _renderer;

    [SerializeField, Range(0, 1)]
    private float _alphaWhenPlayerIsInGrass = 0.5f;

    private void OnValidate()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(k_player))
        {
            _renderer.color = new Color(1, 1, 1, _alphaWhenPlayerIsInGrass);
            AudioManager.Instance.PlaySE(k_grassIn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(k_player))
        {
            _renderer.color = Color.white;
            AudioManager.Instance.PlaySE(k_grassOut);
        }
    }
}
