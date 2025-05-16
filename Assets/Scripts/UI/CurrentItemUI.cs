using UnityEngine;

/// <summary>
/// 手に持っているアイテムにバラを表示します
/// </summary>
public class CurrentItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] _itemUI;
    PlayerController _playerController;
    Vector3 _selectedScale = new Vector3(1.05f, 1.05f, 1.05f);
    Vector3 _initializeScale = Vector3.one;
    Vector3[] _pos = new Vector3[3];

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();
        for (int i = 0; i < 3; i++)
        {
            _pos[i] = _itemUI[i].transform.position;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            ItemUsageState();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetImage();
        }
    }


    /// <summary>
    /// 手に持っているアイテムを強調します
    /// </summary>
    void ItemUsageState()
    {
        ResetImage();

        int index = _playerController._playerStatus switch
        {
            PlayerController.PlayerStatus.Rock => 0,
            PlayerController.PlayerStatus.Bottle => 1,
            PlayerController.PlayerStatus.Meat => 2,
            _ => 3,
        };

        if (index == 3) return;

        _itemUI[index].transform.localScale = _selectedScale;
        Vector3 pos = _pos[index];  
        pos.x += 40;
        pos.y -= 10;
        _itemUI[index].transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    void ResetImage()
    {
        for (int i = 0; i < 3; i++)
        {
            _itemUI[i].transform.localScale = _initializeScale;
            _itemUI[i].transform.position = _pos[i];
        }
    }
}
