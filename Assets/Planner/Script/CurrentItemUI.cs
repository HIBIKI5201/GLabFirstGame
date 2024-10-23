using UnityEngine;

/// <summary>
/// 手に持っているアイテムにバラを表示します
/// </summary>
public class CurrentItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] _itemUI;
    PlayerController _playerController;
    Vector3 _selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    Vector3 _initializeScale = Vector3.one;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (_playerController._playerStatus == PlayerController.PlayerStatus.Rock)
        {
            _itemUI[0].transform.localScale = _selectedScale;
            _itemUI[1].transform.localScale = _initializeScale;
            _itemUI[2].transform.localScale = _initializeScale;
        }
        else if (_playerController._playerStatus == PlayerController.PlayerStatus.Bottle)
        {
            _itemUI[1].transform.localScale = _selectedScale;
            _itemUI[0].transform.localScale = _initializeScale;
            _itemUI[2].transform.localScale = _initializeScale;
        }
        else if (_playerController._playerStatus == PlayerController.PlayerStatus.Meat)
        {
            _itemUI[2].transform.localScale = _selectedScale;
            _itemUI[0].transform.localScale = _initializeScale;
            _itemUI[1].transform.localScale = _initializeScale;
        }
    }
}
