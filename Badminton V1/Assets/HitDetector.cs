using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    ShuttlecockLauncher launcher;

    private void Start()
    {
        launcher = FindObjectOfType<ShuttlecockLauncher>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Shuttlecock(Clone)")
        {
            if (name == "RacketHitBox")
            {
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                Vector3 collisionNormal = transform.position - collisionPoint;

                launcher.ShuttleHitTarget(name, transform, collisionNormal);
            }
            else
            {
                launcher.ShuttleHitTarget(name);
            }
        }
    }
}
