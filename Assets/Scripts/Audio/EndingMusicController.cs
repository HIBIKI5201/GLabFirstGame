using UnityEngine;

/// <summary>
/// エンディングシーンが起動した時にエンディングに応じたBGMを再生する
/// </summary>
public class EndingMusicController : MonoBehaviour
{
    [SerializeField] GameObject _badEnd, _happyEnd;
    private void Start()
    {
        if (_badEnd.activeSelf) // BadEndオブジェクトがアクティブだったら
        {
            AudioManager.Instance.PlayBGM("badend");
        }

        if (_happyEnd.activeSelf) // HappyEndオブジェクトがアクティブだったら
        {
            AudioManager.Instance.PlayBGM("happyend");
        }
    }
}
