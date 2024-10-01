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

    public void SelectStage()
    {
        AudioManager.Instance.PlaySE("determination");
        SceneManager.LoadScene("InGame");
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
