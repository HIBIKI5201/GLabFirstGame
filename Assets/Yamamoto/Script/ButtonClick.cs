using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] GameObject panel;
    public void StartGame()
    {
        SceneManager.LoadScene("SelectStage");
    }

    public void OpenCreditPanel()
    {
        panel.SetActive(true);
    }

    public void CloseCredit()
    {
        panel.SetActive(false);
        Debug.Log("‰Ÿ‚³‚ê‚Ü‚µ‚½");
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void SelectStage()
    {
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
