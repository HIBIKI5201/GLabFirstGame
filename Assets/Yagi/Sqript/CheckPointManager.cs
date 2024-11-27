using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static Vector2[] _checkPoint = new Vector2[3];
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ResetPoint()
    {
        for (var i = 0; i < _checkPoint.Length; i++)
            _checkPoint[i] = Vector2.zero;
    }
}
