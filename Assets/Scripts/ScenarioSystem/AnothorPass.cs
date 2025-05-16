using UnityEngine;

public class AnothorPass : MonoBehaviour
{
    [SerializeField] AnotherRoute _anotherClearCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameProgressManager.IsGameCompleted)
        {
            _anotherClearCheck._isPast = true;
        }
    }
}
