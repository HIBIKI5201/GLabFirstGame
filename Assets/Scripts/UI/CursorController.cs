using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// キー入力を元に選択中のボタンを変更するためのクラス
/// </summary>
public class CursorController : MonoBehaviour
{
    [SerializeField] Button[] _buttons;
    [SerializeField] Button _creditButton;
    [SerializeField] GameObject credit;
    private int _index = -1;
    static public bool _openCredit = false;
    private bool _isSelected = false;

    private void SelectButton(int _index)
    {
        EventSystem.current.SetSelectedGameObject(_buttons[_index].gameObject);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (!_isSelected)
            {
                _index = 0;
                _isSelected = true;
            }

            else if (!_openCredit)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    _index--;
                    if (_index < 0)
                    {
                        _index = _buttons.Length - 1;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    _index++;
                    if (_index >= _buttons.Length)
                    {
                        _index = 0;
                    }
                }

                SelectButton(_index);
            }

            else if(_openCredit)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                {
                    EventSystem.current.SetSelectedGameObject(_creditButton.gameObject);
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    _creditButton.onClick.Invoke();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                _buttons[_index].onClick.Invoke();
            }
        }
    }
}
