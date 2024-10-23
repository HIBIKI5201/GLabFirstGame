using UnityEngine;

/// <summary>
/// ステージのBGMを読み込むスクリプトです
/// </summary>

[DisallowMultipleComponent]
public class Stage_BGMLoad : MonoBehaviour
{
    public StageNum _stageNum;

    void Start()
    {
        int index = _stageNum switch
        {
            StageNum.Stage1 => 2,
            StageNum.Stage2 => 3,
            StageNum.Stage3 => 5,
            _ => 2
        };
        AudioManager.Instance.PlayBGM(AudioManager.Instance._bgmSounds[index]._name);
    }

    public enum StageNum
    {
        Stage1,
        Stage2,
        Stage3,
    }
}
