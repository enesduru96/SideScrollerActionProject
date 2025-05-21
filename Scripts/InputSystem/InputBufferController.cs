using System.Collections.Generic;
using UnityEngine;


public enum BufferedInputType
{
    None,
    Jump,
    Dash,
    LightAttack,
    AirLightAttack,
    HeavyAttack,
    AirHeavyAttack,
    Block
}

public struct BufferedInput
{
    public BufferedInputType inputType;
    public float timeStamp;
    public float bufferLife;

    public BufferedInput(BufferedInputType type, float time, float bufferLifeTime)
    {
        inputType = type;
        bufferLife = bufferLifeTime;
        timeStamp = time;
    }
}


public class InputBufferController : MonoBehaviour
{
    [SerializeField] private float lightAttackBufferTime = 0.2f;
    [SerializeField] private float airLightAttackBufferTime = 0.2f;
    [SerializeField] private float heavyAttackBufferTime = 0.2f;
    [SerializeField] private float airHeavyAttackBufferTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float dashBufferTime = 0.2f;

    public float LightAttackBufferTime => lightAttackBufferTime;
    public float AirLightAttackBufferTime => airLightAttackBufferTime;
    public float HeavyAttackBufferTime => heavyAttackBufferTime;
    public float AirHeavyAttackBufferTime => airHeavyAttackBufferTime;
    public float JumpBufferTime => jumpBufferTime;
    public float DashBufferTime => dashBufferTime;


    private Queue<BufferedInput> inputBuffer = new Queue<BufferedInput>();


#if UNITY_EDITOR
    [SerializeField, Tooltip("Debug Buffer (FIFO)")]
    private List<BufferedInputType> debugBuffer = new();
#endif



    private long lastFrameBuffered = -1;


    public void BufferInput(BufferedInputType inputType, float bufferLifeTime)
    {
        if (Time.frameCount == lastFrameBuffered)
            return;

        BufferedInput bufferedInput = new BufferedInput(inputType, Time.time, bufferLifeTime);
        inputBuffer.Enqueue(bufferedInput);

#if UNITY_EDITOR
        debugBuffer.Add(inputType);            // Inspector listesi
#endif

        lastFrameBuffered = Time.frameCount;
    }




    public bool TryConsumeInput(BufferedInputType inputType)
    {
        if (inputBuffer.Count == 0)
            return false;

        BufferedInput peek = inputBuffer.Peek();

        if (Time.time - peek.timeStamp > peek.bufferLife)
        {
            PopFront();
            return false;
        }


        if (peek.inputType == inputType)
        {
            PopFront();
            return true;
        }

        return false;

    }


    public bool HasBufferedInput(BufferedInputType wantedType)
    {
        // Süresi dolmuş girdileri baştan temizle
        while (inputBuffer.Count > 0)
        {
            var peek = inputBuffer.Peek();
            if (Time.time - peek.timeStamp > peek.bufferLife)
            {
                PopFront();
                continue;
            }
            return peek.inputType == wantedType;   // baştaki eleman tür eşleşiyor mu?
        }
        return false;
    }



    public void ClearBuffer()
    {
        inputBuffer.Clear();
#if UNITY_EDITOR
        debugBuffer.Clear();
#endif
    }

    /*────────────────────────────────────────────────────────────────*/
    void PopFront()
    {
        inputBuffer.Dequeue();
#if UNITY_EDITOR
        if (debugBuffer.Count > 0)
            debugBuffer.RemoveAt(0);      // aynı sırada sil
#endif
    }
}
