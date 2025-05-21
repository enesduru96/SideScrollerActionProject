using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneUnloaded(Scene scene)
    {

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }




    private float checkInterval = 1.0f;
    private float timer = 0f;
    private int frameCount = 0;

    void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= checkInterval)
        {
            float fps = frameCount / timer;

            if (fps < 30)
            {
                Time.fixedDeltaTime = 0.04f;
            }
            else if (fps < 60)
            {
                Time.fixedDeltaTime = 0.02f;
            }
            else
            {
                Time.fixedDeltaTime = 0.016f;
            }

            frameCount = 0;
            timer = 0f;
        }
    }
}