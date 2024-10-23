using UnityEngine;

/// <summary>
/// 手に持っているアイテムにバラを表示します
/// </summary>
public class CurrentItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] _uiArea;
    [SerializeField] GameObject _rose;
    PlayerController _playerController;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();

        _rose.SetActive(false);
    }

    void Update()
    {
        if (_playerController._playerStatus == PlayerController.PlayerStatus.Rock)
        {
            ActiveCheck();
            _rose.transform.position = _uiArea[0].transform.position;
        }
        else if (_playerController._playerStatus == PlayerController.PlayerStatus.Bottle)
        {
            ActiveCheck();
            _rose.transform.position = _uiArea[1].transform.position;
        }
        else if (_playerController._playerStatus == PlayerController.PlayerStatus.Meat)
        {
            ActiveCheck();
            _rose.transform.position = _uiArea[2].transform.position;
        }
        else
            _rose.SetActive(false);
    }

    void ActiveCheck()
    {
        if(!_rose.activeSelf) _rose.SetActive(true);
    }
}
