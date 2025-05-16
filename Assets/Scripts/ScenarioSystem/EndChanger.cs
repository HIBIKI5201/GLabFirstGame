using UnityEngine;

public class EndChanger : MonoBehaviour
{
    [SerializeField] GameObject _badEnd;
    [SerializeField] GameObject _happyEnd;
    private void Start()
    {
        if ( GameProgressManager.IsSecretModeUnlocked == false )
        {
            _happyEnd.SetActive(false);
            _badEnd.SetActive(true);
        }
        else
        {
            _badEnd.SetActive(false);
            _happyEnd.SetActive(true);
        }
    }
}
