using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] GameObject panel;
    public void StartGame()
    {
        SceneManager.LoadScene("");
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
