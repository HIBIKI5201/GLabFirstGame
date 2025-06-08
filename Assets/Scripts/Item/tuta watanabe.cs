using UnityEngine;

public class VineDropper : MonoBehaviour
{
    public GameObject vinePrefab; // �c�^�̃v���n�u
    public Transform dropPoint;   // �c�^�𗎂Ƃ��J�n�ʒu�i��: �v���C���[�̉��j

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // �G���^�[�L�[����������
        {
            DropVine();
        }
    }

    void DropVine()
    {
        GameObject vine = Instantiate(vinePrefab, dropPoint.position, Quaternion.identity);

        // Rigidbody2D �����݂���ꍇ�A�^���ɗ͂�������i�C�Ӂj
        Rigidbody2D rb = vine.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.down * 5f; // �^���ɃX�s�[�h��^����i�����j
        }
    }
}
