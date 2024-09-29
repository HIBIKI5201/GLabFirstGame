using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Scenes _scene;
    [SerializeField] GameObject _fadePanel;
    [SerializeField] float _fadeTime;
    [SerializeField] Color _fadeColor;
    Image _image;
    private void Start()
    {
        if (_fadePanel != null)
        {
            _image = _fadePanel.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("FadePanel‚ªƒZƒbƒg‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
        }
    }
    public enum Scenes
    {
        None = -1,
        Title,
        StageSelect,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
    }
    string[] SceneName =
    {
        "Title",
        "StageSelect",
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4"
    };
    public void FadeAndLoadScene() =>DOTween.To(() => _image.color, c => _image.color = c, _fadeColor, _fadeTime).OnComplete(SceneLoad);
    void SceneLoad() => SceneManager.LoadScene(SceneName[(int)_scene]);
}
