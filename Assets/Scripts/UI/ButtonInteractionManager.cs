using UnityEngine;

/// <summary>
/// ボタン操作後の挙動を制御するコンポーネント
/// タイトルシーンやステージ選択シーンでのボタン操作後、ボタンの無効化や効果音の設定を管理する
/// </summary>
public class ButtonInteractionManager : MonoBehaviour
{
    [SerializeField] private SelectedButton[] _selectedButtons;
    
    /// <summary>
    /// ボタン操作後の処理を実行する
    /// </summary>
    public void Mute()
    {
        Invoke(nameof(VolumeChange), 0.3f);
        
        // 全てのボタンを押せないようにする
        foreach (var button in _selectedButtons)
        {
            button.enabled = false;
        }
    }

    /// <summary>
    /// SE用のAudioSourceの音量をゼロにして絶対にSEが鳴らないようにする
    /// </summary>
    private void VolumeChange()
    {
        AudioManager.Instance.SESource.volume = 0;
    }
}
