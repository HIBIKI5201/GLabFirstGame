using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayBGM("title");
    }
    // Update is called once per frame
    void Update()
    {
    }
    void DebSound()
    {
        AudioManager.Instance.PlayBGM("title");
        Debug.Log("‰¹‚È‚è‚Ü‚µ‚½");
    }
}
