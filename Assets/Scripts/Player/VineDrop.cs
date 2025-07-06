using UnityEngine;

public class VineDrop : MonoBehaviour
{
    public GameObject vinePrefab;          // ツタのプレハブ
    public Transform vineSpawnPoint;       // main_2のTransform

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))// Enterキー
        {
            DropVine();
        }
    }

    void DropVine()
    {
        if (vinePrefab != null && vineSpawnPoint != null)
        {
            Instantiate(vinePrefab, vineSpawnPoint.position, Quaternion.identity);
        }
    }
}
