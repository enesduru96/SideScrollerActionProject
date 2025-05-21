
using System.Collections;
using UnityEngine;

public class CharacterTrailHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterActor characterActor;
    [SerializeField] private CharacterLocalEventManager localEventManager;
    [SerializeField] private CharacterBodyReferences characterBodyReferences;

    [Header("Trail Related")]
    [SerializeField] private GameObject trailSpawnObject;
    [SerializeField] private Material dashTrailMaterial;


    [Header("Timers")]
    [SerializeField] private float trailSpawnMoment = 0.1f;
    [SerializeField] private float trailLifetime = 0.5f;


    private MeshRenderer spawnObjectMeshRenderer;
    private MeshFilter spawnObjectMeshFilter;
    private Mesh characterTrailMesh;

    private Coroutine meshLifetimeCoroutine;

    private void OnEnable()
    {
        localEventManager.CharacterInput.OnPlayerDashStarted += HandleDashTrail;
    }
    private void OnDisable()
    {
        localEventManager.CharacterInput.OnPlayerDashStarted -= HandleDashTrail;

        ClearCoroutine();
        ResetValues();
    }

    private void Start()
    {
        characterTrailMesh = new Mesh();
        spawnObjectMeshRenderer = trailSpawnObject.GetComponent<MeshRenderer>();
        spawnObjectMeshFilter = trailSpawnObject.GetComponent<MeshFilter>();
    }

    private void HandleDashTrail()
    {
        if(meshLifetimeCoroutine != null)
        {
            StopCoroutine(meshLifetimeCoroutine);
            meshLifetimeCoroutine = null;
        }
        meshLifetimeCoroutine = StartCoroutine(HandleTrailLifetime());
    }
    
    private IEnumerator HandleTrailLifetime()
    {
        yield return new WaitForSeconds(trailSpawnMoment);


        trailSpawnObject.transform.SetParent(null);

        if (characterActor.IsFacingRight())
            trailSpawnObject.transform.forward = Vector3.right;
        else
            trailSpawnObject.transform.forward = Vector3.left;

        characterBodyReferences.CharacterMesh.BakeMesh(characterTrailMesh);
        spawnObjectMeshFilter.mesh = characterTrailMesh;
        spawnObjectMeshRenderer.material = dashTrailMaterial;
        trailSpawnObject.SetActive(true);



        float elapsedTime = 0f;
        while (elapsedTime < trailLifetime) 
        { 
            elapsedTime += Time.deltaTime;

            float alphaValue = Mathf.Lerp(1f, 0f, elapsedTime / trailLifetime);
            Color currentColor = spawnObjectMeshRenderer.material.color;
            currentColor.a = alphaValue;
            spawnObjectMeshRenderer.material.color = currentColor;
            yield return null;
        }

        ResetValues();
    }

    private void ResetValues()
    {
        trailSpawnObject.SetActive(false);
        trailSpawnObject.transform.SetParent(transform);
        trailSpawnObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    private void ClearCoroutine()
    {
        if (meshLifetimeCoroutine != null)
        {
            StopCoroutine(meshLifetimeCoroutine);
            meshLifetimeCoroutine = null;
        }
    }
}
