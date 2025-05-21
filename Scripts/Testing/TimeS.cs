using UnityEngine;

public class TimeS : MonoBehaviour
{
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = speed;

    }
}
