using UnityEngine;

/// <summary>
/// 現在選択中のアイテムをUI上で視覚的に強調表示するクラス
/// </summary>
public class CurrentItemUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _itemUI; // 左上に表示しているアイテムイラスト
    [SerializeField] private Vector3 _selectedScale = new Vector3(1.05f, 1.05f, 1.05f); // 選択中の時のScale
    [SerializeField] private Vector3 _selectedOffset = new Vector3(40f, -10f, 0f); // 選択中の時の位置オフセット
    private PlayerController _playerController;
    private Vector3 _initialScale = Vector3.one; // 初期Scale
    private Vector3[] _initialPositions = new Vector3[3]; // アイコンの初期位置

    private void Start()
    {
        // プレイヤーを探してPlayerControllerの参照を取得する
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerController = player.GetComponent<PlayerController>();
        
        // アイテムのイラストの初期座標を配列に保存しておく
        for (int i = 0; i < 3; i++)
        {
            _initialPositions[i] = _itemUI[i].transform.position;
        }
    }

    private void Update()
    {
        // アイテムが選択されたら
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateItemHighlight();
        }

        // アイテムが
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ResetAllItems();
        }
    }
    
    /// <summary>
    /// 現在手に持っているアイテムをUIで強調表示する
    /// </summary>
    private void UpdateItemHighlight()
    {
        ResetAllItems(); // まずすべてのアイテムUIを初期状態に戻す

        int selectedIndex = GetSelectedItemIndex();
        if (selectedIndex < 0 || selectedIndex >= _itemUI.Length) return; // 有効なインデックスでない場合は何もしない

        // 選択中のアイテムUIを強調表示
        _itemUI[selectedIndex].transform.localScale = _selectedScale;
        _itemUI[selectedIndex].transform.position = _initialPositions[selectedIndex] + _selectedOffset;
    }

    /// <summary>
    /// 現在のプレイヤーステータスに対応するアイテムインデックスを取得
    /// </summary>
    /// <returns>選択中のアイテムインデックス。該当なしの場合は-1</returns>
    private int GetSelectedItemIndex()
    {
        if (_playerController == null)
        {
            return -1;
        }
        
        return _playerController._playerStatus switch
        {
            PlayerStatusType.Rock => 0,
            PlayerStatusType.Bottle => 1,
            PlayerStatusType.Meat => 2,
            _ => -1,
        };
    }

    /// <summary>
    /// すべてのアイテムUIを初期状態にリセットする
    /// </summary>
    private void ResetAllItems()
    {
        for (int i = 0; i < 3; i++)
        {
            _itemUI[i].transform.localScale = _initialScale;
            _itemUI[i].transform.position = _initialPositions[i];
        }
    }
}
