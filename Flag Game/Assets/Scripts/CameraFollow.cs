using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothing = 6f;

    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

 
    void FixedUpdate()
    {
        Vector3 newPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, newPosition, smoothing * Time.deltaTime);
    }
}
