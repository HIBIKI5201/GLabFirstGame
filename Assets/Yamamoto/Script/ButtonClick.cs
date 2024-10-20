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
        StartFadeOutBGM();
    }

    public void OpenCreditPanel()
    {
        AudioManager.Instance.PlaySE("determination");
        ButtonCtrl._openCredit = true;
        panel.SetActive(true);
    }

    public void CloseCredit()
    {
        AudioManager.Instance.PlaySE("back");
        ButtonCtrl._openCredit = false;
        panel.SetActive(false);
    }

    public void ReturnTitle()
    {
        AudioManager.Instance.PlaySE("determination");
    }

    public void SelectStage1()
    {
        AudioManager.Instance.PlaySE("determination");
    }
    public void SelectStage2()
    {
        AudioManager.Instance.PlaySE("determination");
    }
    public void SelectStage3()
    {
        AudioManager.Instance.PlaySE("determination");
    }
    public void SelectStage4()
    {
        AudioManager.Instance.PlaySE("determination");
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
