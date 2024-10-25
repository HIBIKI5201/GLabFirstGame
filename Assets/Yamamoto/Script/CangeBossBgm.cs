using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CangeBossBgm : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BossArea"))
        {
            AudioManager.Instance.FadeBGM();
            if (AudioManager._isFinish)
            {
                AudioManager.Instance.FadeInBGM();
            }
        }
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
