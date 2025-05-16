using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] Collider2D _colliders1in;
    [SerializeField] Collider2D _colliders1out;
    [SerializeField] GameObject _cameras1;
    int _act = -1;
    bool _active = false;

    private void Update()
    {
        _cameras1.SetActive(_active);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == _colliders1in || collision == _colliders1out)
        {
            _act *= -1;
            if (_act < 0)
            {
                _active = false;
            }
            else
            {
                _active = true;
            }          
        }
    }

}
