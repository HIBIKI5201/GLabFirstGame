using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 手に持っているアイテムにバラを表示します
/// </summary>
public class CurrentItemUI : MonoBehaviour
{
    [SerializeField] GameObject[] _itemUI;
    PlayerController _playerController;
    Outline[] _outline = new Outline[3];
    Vector3 _selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    Vector3 _initializeScale = Vector3.one;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();
        for (int i = 0; i < 3; i++)
        {
            _outline[i] = _itemUI[i].GetComponent<Outline>();
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
        _outline[index].enabled = true;
    }

    void ResetImage()
    {
        for (int i = 0; i < 3; i++)
        {
            _itemUI[i].transform.localScale = _initializeScale;
            _outline[i].enabled = false;
        }
    }
}
