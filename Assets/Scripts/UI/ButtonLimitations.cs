using UnityEngine;

/// <summary>
/// ボタンを押した後のボタンの動きを制御します
/// </summary>
public class ButtonLimitations : MonoBehaviour
{
    [SerializeField] SelectedButton[] _selectedButtons;
    public void Mute()
    {
        Invoke("VolumeChange", 0.3f);
        foreach (var button in _selectedButtons)
        {
            button.enabled = false;
        }
    }

    void VolumeChange()
    {
        AudioManager.Instance.SESource.volume = 0;
    }
}
