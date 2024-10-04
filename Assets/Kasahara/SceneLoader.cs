using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

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
            Debug.LogError("FadePanel���Z�b�g����Ă��܂���");
        }
    }
    public enum Scenes
    {
        None = -1,
        Title,
        InGame,
        Ending,
        SelectStage,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
    }
    /// <summary>
    /// �V�[�����Ăяo�����\�b�h
    /// </summary>
    [ContextMenu("�V�[�����Ăяo�����\�b�h")]
    public void FadeAndLoadScene() => DOTween.To(() => _image.color, c => _image.color = c, _fadeColor, _fadeTime).OnComplete(SceneLoad);
    void SceneLoad() => SceneManager.LoadScene(Enum.GetName(typeof(Scenes), _scene));
}
