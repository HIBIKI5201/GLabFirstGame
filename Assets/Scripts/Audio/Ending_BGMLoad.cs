using UnityEngine;

public class Ending_BGMLoad : MonoBehaviour
{
    [SerializeField] GameObject _badEnd, _happyEnd;
    void Start()
    {
        if(_badEnd.activeSelf) AudioManager.Instance.PlayBGM("badend");
        if(_happyEnd.activeSelf) AudioManager.Instance.PlayBGM("happyend");
    }
}
