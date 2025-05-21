
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class WorldObjectPooler : MonoBehaviour
{
    private static WorldObjectPooler instance;
    public static WorldObjectPooler Instance => instance;


    public Dictionary<Transform, LadderObject> ActiveLadders = new Dictionary<Transform, LadderObject>();
    public Dictionary<Transform, VaultObject> ActiveVaults = new Dictionary<Transform, VaultObject>();
    public Dictionary<Transform, PositionMarker> ActivePositionMarkers = new Dictionary<Transform, PositionMarker>();

    [SerializeField] private int ladderCount = 0;
    [SerializeField] private int vaultCount = 0;
    [SerializeField] private int markerCount = 0;

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
        ClearAll();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearAll();
    }

    private void ClearAll()
    {
        ActiveLadders.Clear();
        ladderCount = 0;

        ActiveVaults.Clear();
        vaultCount = 0;

        ActivePositionMarkers.Clear();
        markerCount = 0;
    }

    public void RegisterLadder(LadderObject ladder)
    {
        if (!ActiveLadders.ContainsKey(ladder.transform))
        {
            ActiveLadders.Add(ladder.transform, ladder);
            ladderCount = ActiveLadders.Count;
        }
    }

    public void UnregisterLadder(LadderObject ladder)
    {
        if (ActiveLadders.ContainsKey(ladder.transform))
        {
            ActiveLadders.Remove(ladder.transform);
            ladderCount = ActiveLadders.Count;

        }
    }

    public void RegisterVault(VaultObject vault)
    {
        if (!ActiveVaults.ContainsKey(vault.transform))
        {
            ActiveVaults.Add(vault.transform, vault);
            vaultCount = ActiveVaults.Count;

        }
    }

    public void UnregisterVault(VaultObject vault)
    {
        if (ActiveVaults.ContainsKey(vault.transform))
        {
            ActiveVaults.Remove(vault.transform);
            vaultCount = ActiveVaults.Count;

        }
    }


    public void RegisterPositionMarker(PositionMarker marker)
    {
        if (!ActivePositionMarkers.ContainsKey(marker.transform))
        {
            ActivePositionMarkers.Add(marker.transform, marker);
            markerCount = ActivePositionMarkers.Count;

        }
    }

    public void UnregisterPositionMarker(PositionMarker marker)
    {
        if (ActivePositionMarkers.ContainsKey(marker.transform))
        {
            ActivePositionMarkers.Remove(marker.transform);
            markerCount = ActivePositionMarkers.Count;

        }
    }
}
