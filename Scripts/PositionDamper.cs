using UnityEngine;

public class PositionDamper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 100);
    }
}
