using UnityEngine;

/// <summary>
/// シーンをまたいでクリア済みステージ情報を記録する
/// </summary>
public class GameProgressManager : MonoBehaviour
{
    // シングルトンを使ってもよかったかも？（現在正常に機能しているので、一旦放置）
    
    public static int HighestClearedStage = 0; // クリアした最高ステージ
    public static bool IsSecretModeUnlocked = false; // 隠しクリアフラグ
    public static bool IsGameCompleted = false; // ステージ全てをクリアしたか
    
    // 定数
    private const int FINAL_STAGE_INDEX = 3; // 最後のステージ数

    private void Update()
    {
        // デバッグ用のキー入力処理
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            IsGameCompleted = true;
        }
        #endif
    }

    /// <summary>
    /// クリアしたステージを記録する
    /// </summary>
    public void StageClear(int stageIndex)
    {
        // より高いステージをクリアした場合のみ記録を更新
        if (HighestClearedStage < stageIndex)
        {
            if (stageIndex == FINAL_STAGE_INDEX)
            {
                // ステージ3がクリアされたらゲームクリア
                IsGameCompleted = true;
            }
            
            HighestClearedStage = stageIndex; // 最高ステージクリア情報を書き換える
            PlayerPrefs.SetInt("nowStage", HighestClearedStage); // PlayerPrefsに保存する
        }
        
        Debug.Log($"ステージ全てをクリアしたか: {IsGameCompleted}, 隠しクリアフラグ: {IsSecretModeUnlocked}");
    }
}
