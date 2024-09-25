using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCtrl : MonoBehaviour
{
    [SerializeField] Button[] _buttons;
    [SerializeField] Button _creditButton;
    private int _index = -1;
    private int _creIndex = -1;
    private bool _isSelected = false;

    [SerializeField] GameObject credit;

    private void SelectButton(int _index)
    {
        EventSystem.current.SetSelectedGameObject(_buttons[_index].gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (!_isSelected)
            {
                _index = 0;
                _isSelected = true;
            }

            else if (!credit.activeSelf)
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

            else
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
