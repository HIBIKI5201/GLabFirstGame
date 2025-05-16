using UnityEngine;

/// <summary>
/// インゲームシーンで各ステージのBGMを再生するクラス
/// </summary>
public class InGameMusicController : MonoBehaviour
{
    [SerializeField] private StageType _stageType;

    private void Start()
    {
        int index = _stageType switch // switch式の書き方
        {
            StageType.Stage1 => 2,
            StageType.Stage2 => 3,
            StageType.Stage3 => 5,
            _ => 2
        };
        AudioManager.Instance.PlayBGM(AudioManager.Instance._bgmSounds[index]._name);
    }
}
