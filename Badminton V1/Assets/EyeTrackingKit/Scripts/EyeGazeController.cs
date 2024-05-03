using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static OVRPlugin;

public class EyeGazeController : MonoBehaviour
{
    public GameObject leftEye;
    public GameObject rightEye;
    public bool debugMode = false;

    public FixationTracker fixationTracker;

    LineRenderer leftEyeLine;
    LineRenderer rightEyeLine;

    GameObject leftEyeSphere;
    GameObject rightEyeSphere;

    void Start()
    {
        leftEyeLine = InitializeLine();
        rightEyeLine = InitializeLine();

        leftEyeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEyeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        leftEyeSphere.GetComponent<SphereCollider>().enabled = false;
        rightEyeSphere.GetComponent<SphereCollider>().enabled = false;

        leftEyeSphere.layer = 2;
        rightEyeSphere.layer = 2;

        leftEyeSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        rightEyeSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        fixationTracker = new FixationTracker();
    }

    void Update()
    {
        // raycast
        Vector3? leftHitPoint = GetEyeHitPoint(leftEye);
        Vector3? rightHitPoint = GetEyeHitPoint(rightEye);

        if (leftHitPoint != null && rightHitPoint != null)
        {
            // update lines
            UpdateLinePosition(leftEyeLine, leftEye.transform.position, leftHitPoint.Value);
            UpdateLinePosition(rightEyeLine, rightEye.transform.position, rightHitPoint.Value);

            leftEyeSphere.transform.position = leftHitPoint.Value;
            rightEyeSphere.transform.position = rightHitPoint.Value;

            leftEyeSphere.GetComponent<Renderer>().material.color = Color.green;
            rightEyeSphere.GetComponent<Renderer>().material.color = Color.green;

            fixationTracker.AddGazePoint((leftHitPoint.Value + rightHitPoint.Value) * 0.5f);
        }
        else
        {
            leftEyeSphere.GetComponent<Renderer>().material.color = Color.red;
            rightEyeSphere.GetComponent<Renderer>().material.color = Color.red;
        }
        SetDebugHelpersActiveStatus(debugMode);
    }

    void SetDebugHelpersActiveStatus(bool active)
    {
        leftEyeSphere.SetActive(active); rightEyeSphere.SetActive(active);
        leftEyeLine.enabled = false; rightEyeLine.enabled = false;
    }

    Vector3? GetEyeHitPoint(GameObject eye)
    {
        Physics.Raycast(eye.transform.position, eye.transform.forward, hitInfo: out RaycastHit hitInfo, maxDistance: 10000f, layerMask: LayerMask.GetMask(new string[]{LayerMask.LayerToName(4)}));

        if (hitInfo.collider == null) 
        {
            return null;
        }

        return hitInfo.point;
    }

    LineRenderer InitializeLine()
    {
        GameObject gameObj = new GameObject("Line");
        gameObj.layer = 2;

        LineRenderer newLine = gameObj.AddComponent<LineRenderer>();
        newLine.startColor = Color.black;
        newLine.endColor = Color.black;
        newLine.startWidth = 0.05f;
        newLine.endWidth = 0.05f;
        newLine.positionCount = 2;
        newLine.useWorldSpace = true;

        return newLine;
    }

    void UpdateLinePosition(LineRenderer line, Vector3 startPos, Vector3 endPos)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }
}