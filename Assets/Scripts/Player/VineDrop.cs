using UnityEngine;

public class VineDrop : MonoBehaviour
{
    public GameObject vinePrefab;          // �c�^�̃v���n�u
    public Transform vineSpawnPoint;       // main_2��Transform

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter�L�[
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
