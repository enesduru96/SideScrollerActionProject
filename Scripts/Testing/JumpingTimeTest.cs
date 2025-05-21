
using ShadowFort.Utilities;
using UnityEngine;

public class JumpingTimeTest : MonoBehaviour
{

    [SerializeField] private CharacterActor characterActor;


    [SerializeField] private float ascendingDuration = 0f;
    [SerializeField] private float peakDuration = 0f;
    [SerializeField] private float descendingDuration = 0f;


    private float startTimer = 2f;
    private float startElapsedTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterActor = this.GetComponentInBranch<CharacterActor>();
        
    }

    // Update is called once per frame
    void Update()
    {
        startElapsedTime += Time.deltaTime;
        if(startElapsedTime < startTimer)
            return;
        
        if (characterActor.IsAscending)
        {
            ascendingDuration += Time.deltaTime;
        }

        if (characterActor.IsFalling)
        {
            descendingDuration += Time.deltaTime;
        }

        if(!characterActor.IsFalling && !characterActor.IsAscending && !characterActor.IsGrounded)
        {
            peakDuration += Time.deltaTime;
        }
    }
}
