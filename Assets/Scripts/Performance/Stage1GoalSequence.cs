using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージ1特有のゴール演出
/// </summary>
public class Stage1GoalSequence : MonoBehaviour
{
    [SerializeField] private Image _vignette;
    [SerializeField] private CinemachineVirtualCamera _cam;
    [SerializeField] private float _startOrthographicSize = 8.66f; // カメラズーム開始時の視野サイズ
    [SerializeField] private float _endOrthographicSize = 6f; // カメラズーム終了時の視野サイズ 
    
    private void Start()
    {
        _vignette.gameObject.SetActive(false);
        _cam.gameObject.SetActive(false);
    }

    /// <summary>
    /// 演出を開始する
    /// </summary>
    public void StartSequence(float walkTime)
    {
        _cam.gameObject.SetActive(true);
        _vignette.gameObject.SetActive(true);
        _vignette.DOFade(1, walkTime);
        DOTween.To(() => _startOrthographicSize, num => _cam.m_Lens.OrthographicSize = num,
            _endOrthographicSize, walkTime);
    }
}
