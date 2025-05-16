using UnityEngine;

public class AnothorPass : MonoBehaviour
{
    [SerializeField] AnotherRoute _anotherClearCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsClear._gameAllClear)
        {
            _anotherClearCheck._isPast = true;
        }
    }
}
