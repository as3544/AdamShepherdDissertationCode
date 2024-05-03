using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ObjectSpeed : MonoBehaviour
{
    private Vector3 newPosition = new Vector3(0f, 0f, 0f);
    private Vector3 previousPosition = new Vector3(0f, 0f, 0f);
    public float objectSpeed;

    // Update is called once per frame
    void Update()
    {
        previousPosition = newPosition;
        newPosition = transform.position;
        objectSpeed = Vector3.Distance(newPosition, previousPosition) / Time.deltaTime;
    }
}
