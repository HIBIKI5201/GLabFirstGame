using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] GameObject panel;
    public void StartGame()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("SelectStage");
    }

    public void OpenCreditPanel()
    {
        AudioManager.Instance.PlaySE("determination");
        panel.SetActive(true);
    }

    public void CloseCredit()
    {
        AudioManager.Instance.PlaySE("back");
        panel.SetActive(false);
    }

    public void ReturnTitle()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("Title");
    }

    public void SelectStage1()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("Stage1");
    }
    public void SelectStage2()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("Stage2");
    }
    public void SelectStage3()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("Stage3");
    }
    public void SelectStage4()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("Stage4");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
