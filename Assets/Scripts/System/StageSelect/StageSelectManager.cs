using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージ選択画面の管理を行うコンポーネント
/// ゲームの進行状況に応じてステージボタンと達成マークの表示を制御する
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Button[] _stage = { };
    [SerializeField] private GameObject[] _clearObj = new GameObject[3];

    private void Start()
    {
        UpdateStageSelectionUI();
    }

    /// <summary>
    /// ゲームの進行状況に基づいてステージボタンと達成マークの表示を更新する
    /// </summary>
    private void UpdateStageSelectionUI()
    {
        // 全てのステージボタンを初期状態（無効化・グレー表示）にする
        foreach (var stage in _stage)
        {
            stage.enabled = false;
            stage.image.color = Color.gray;
        }
        
        // 全ての達成マークを非表示にする
        foreach (var clearObj in _clearObj)
        {
            clearObj.SetActive(false);
        }
        
        // クリア済みのステージとその次のステージのボタンを有効化する
        for (var i = 0; i <= GameProgressManager.HighestClearedStage; i++)
        {
            if(GameProgressManager.HighestClearedStage == 3) break;
            _stage[i].enabled = true;
            _stage[i].image.color = Color.white;
        }

        // クリア済みのステージに達成マークを表示する
        for (var i = 1; i <= GameProgressManager.HighestClearedStage; i++)
        {
            _clearObj[i - 1].SetActive(true);
        }
    }

    /// <summary>
    /// ゲームの進行状況をリセットし、UI表示を初期状態に戻す
    /// </summary>
    public void ResetGame()
    {
        // クリアステージ情報・隠しルート解放フラグをリセットする
        GameProgressManager.IsSecretModeUnlocked = false;
        GameProgressManager.HighestClearedStage = 0;
        PlayerPrefs.SetInt("nowStage", GameProgressManager.HighestClearedStage);
        
        // UI表示を変更する
        UpdateStageSelectionUI();
    }
}
