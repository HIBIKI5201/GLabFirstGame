using UnityEngine;

/// <summary>
/// ステージ選択をしたタイミングで、ステージに初めて入るフラグをリセットする
/// </summary>
public class ResetStageEntryHandler : MonoBehaviour
{
    /// <summary>
    /// ステージが選択された際に呼び出され、初回進入フラグをリセットする
    /// </summary>
    public void ResetStageEntryState()
    {
        InGameStartSequence._isFirstEntry = true;
    }
}
