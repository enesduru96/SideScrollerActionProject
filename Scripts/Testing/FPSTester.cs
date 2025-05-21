using UnityEngine;
using TMPro;

public class FPSTester : MonoBehaviour
{
    [Header("FPS Control")]
    [SerializeField] private int targetFPS = 60;
    [SerializeField] private TMP_Text fpsDisplay;

    private float deltaTime = 0.0f;

    void Start()
    {
        SetTargetFPS(targetFPS);
    }

    void Update()
    {
        // FPS hesaplama
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // FPS deðerini ekranda göster
        if (fpsDisplay != null)
            fpsDisplay.text = "FPS: " + Mathf.Ceil(fps).ToString();

        // FPS deðerini deðiþtirme
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            targetFPS += 10;
            SetTargetFPS(targetFPS);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            targetFPS = Mathf.Max(10, targetFPS - 10);
            SetTargetFPS(targetFPS);
        }
    }

    private void SetTargetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        Debug.Log("Target FPS set to: " + fps);
    }
}