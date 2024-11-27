using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance = default;
    public static Vector2[] _checkPoint = new Vector2[3];
    private void Start()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ResetPoint()
    {
        for (var i = 0; i < _checkPoint.Length; i++)
            _checkPoint[i] = Vector2.zero;
    }
}
