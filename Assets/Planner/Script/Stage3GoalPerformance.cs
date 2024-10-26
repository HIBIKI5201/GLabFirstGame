using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// ステージ3特有のGoal演出
/// </summary>
public class Stage3GoalPerformance : MonoBehaviour
{
    [SerializeField] CanvasGroup _clearPanel, _operate, _hp;
    [SerializeField] Image _vignette, _fadePanel;
    [SerializeField] CinemachineVirtualCamera _cam;
    [SerializeField] GameObject[] _fukidasi;
    [SerializeField] Text _text;
    [SerializeField] Goal _goal;
    [SerializeField] Volume _volume;
    

    void Start()
    {
        IsClear._concealed = true;
        _vignette.gameObject.SetActive(false);
        _cam.gameObject.SetActive(false);
    }

    public IEnumerator DoPerformance(float warkTime)
    {
        yield return new WaitForSeconds(1f);

        int panelTime = 2; //パネルを表示する秒数

        _goal.Clear();

        _cam.gameObject.SetActive(true);
        _vignette.gameObject.SetActive(true);
        _vignette.DOFade(1, warkTime);
        DOTween.To(() => 8.66f, num => _cam.m_Lens.OrthographicSize = num, 6f, warkTime);
        //クリアパネル表示・歩行中・カメラズーム中

        yield return new WaitForSeconds(panelTime);

        _clearPanel.DOFade(0, 1);
        _operate.DOFade(0, 1);
        _hp.DOFade(0, 1);
        AudioManager.Instance._bgmSource.DOFade(0,4);

        yield return new WaitForSeconds(warkTime - panelTime);

        //立ち止まってセリフが開始

        _fukidasi[0].SetActive(true);
        _text.text = "";
        _text.gameObject.SetActive(true);
        _text.DOText("おばあさん。赤ずきんだよ。",2f);

        yield return new WaitForSeconds(5f);

        if (!IsClear._concealed) //BadEnd
        {
            _text.text = "";
            _text.DOText("扉を開けて。", 2f);
            AudioManager.Instance.PlaySE("knock");

            yield return new WaitForSeconds(5f);

            _text.text = "";
            _text.DOText("・・・・・・。", 3f);

            yield return new WaitForSeconds(5f);

            _text.text = "";
            _text.DOText("赤ずきんだよ。扉を開けて。", 2f);
            AudioManager.Instance.PlaySE("knock");

            yield return new WaitForSeconds(5f);

            _fukidasi[0].SetActive(false);
            _fukidasi[1].SetActive(true);
            _text.text = "";
            _text.color = Color.red;
            _text.DOText("食べたりしないからさあ、開けてよお？", 4f);

            yield return new WaitForSeconds(4f);

            _fadePanel.color = new Color(0, 0, 0, 0);
            _fadePanel.gameObject.SetActive(true);
            _fadePanel.DOFade(1, 2);

            yield return new WaitForSeconds(1f);

            AudioManager.Instance.PlaySE("openDoor");

            yield return new WaitForSeconds(4f);

            AudioManager.Instance.PlaySE("damaged");

            yield return new WaitForSeconds(2f);

            _goal.StartCoroutine(_goal.LoadScene(1f));

        }
        else //HappyEnd
        {
            _text.text = "";
            _text.DOText("扉を開けてほしいな。", 2f);

            yield return new WaitForSeconds(5f);

            _text.text = "";
            _text.DOText("大好きな、大好きな、おばあさん。", 2f);

            yield return new WaitForSeconds(3f);

            AudioManager.Instance.PlaySE("openDoor");

            yield return new WaitForSeconds(2f);

            _goal.StartCoroutine(_goal.LoadScene(1f));
        }
    }
}
