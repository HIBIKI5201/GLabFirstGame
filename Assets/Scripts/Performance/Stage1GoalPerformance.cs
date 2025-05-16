using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージ1特有のGoal演出
/// </summary>
public class Stage1GoalPerformance : MonoBehaviour
{
    [SerializeField] Image _vignette;
    [SerializeField] CinemachineVirtualCamera _cam;

    void Start()
    {
        _vignette.gameObject.SetActive(false);
        _cam.gameObject.SetActive(false);
    }

    public void Perfomance(float warkTime)
    {
        _cam.gameObject.SetActive(true);
        _vignette.gameObject.SetActive(true);
        _vignette.DOFade(1, warkTime);
        DOTween.To(() => 8.66f, num => _cam.m_Lens.OrthographicSize = num, 6f, warkTime);
    }
}
