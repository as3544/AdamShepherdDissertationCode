using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FixationTracker
{
    List<RawGazePoint> gazePoints = new List<RawGazePoint>();
    List<Fixation> fixations = new List<Fixation>();

    public float maximumDistanceForGaze = 0.5f;
    public int minimumTimeForGaze = 100;
    bool isCurrentlyFixating = false;
#nullable enable
    Fixation? currentFixation;
#nullable disable

    public void AddGazePoint(Vector3 gazeLocation)
    {
        gazePoints.Add(new RawGazePoint(gazeLocation));
        CheckForFixation();
    }

#nullable enable
    public void CheckForFixation()
    {
        RawGazePoint recentGazePoint = gazePoints.Last();

        if (!isCurrentlyFixating)
        {
            if (gazePoints.Count > 1)
            {

                bool pointOutOfTimeRangeFound = false;
                bool allGazeWithinRange = true;
                RawGazePoint? previousGazePoint;
                int gazePointIndex = gazePoints.Count - 2;

                do
                {
                    previousGazePoint = gazePoints.ElementAtOrDefault(gazePointIndex);

                    // if prev exists, and within time we're looking for
                    if (previousGazePoint != null && VectorsWithinRange(recentGazePoint.gazeLocation, previousGazePoint.gazeLocation, maximumDistanceForGaze))
                    {
                        if (recentGazePoint.gazeTimestamp - previousGazePoint.gazeTimestamp > minimumTimeForGaze)
                        {
                            pointOutOfTimeRangeFound = true;
                        }
                    }
                    else
                    {
                        allGazeWithinRange = false;
                        break;
                    }
                    gazePointIndex--;

                } while (pointOutOfTimeRangeFound == false);

                if (allGazeWithinRange && previousGazePoint != null)
                {
                    // start fixation
                    currentFixation = new Fixation();
                    currentFixation.startTimeStamp = previousGazePoint.gazeTimestamp;
                    currentFixation.gazeLocation = recentGazePoint.gazeLocation;

                    isCurrentlyFixating = true;
                }
            }
        }
        else
        {
            // check if new gaze point is out of range of fixation
            if (currentFixation != null && !VectorsWithinRange(currentFixation.gazeLocation, recentGazePoint.gazeLocation, maximumDistanceForGaze))
            {
                currentFixation.endTimeStamp = recentGazePoint.gazeTimestamp;
                fixations.Add(currentFixation);

                GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                debugSphere.transform.position = currentFixation.gazeLocation;
                debugSphere.GetComponent<Renderer>().material.color = Color.blue;
                debugSphere.GetComponent<SphereCollider>().enabled = false;
                debugSphere.SetActive(false); // turn off for getting rid of debug

                currentFixation = null;
                isCurrentlyFixating = false;
            }
        }
    }

    public Fixation? GetFixationStartingClosestToBeforeTime(long time)
    {
        for (int i = fixations.Count - 1; i >= 0; i--)
        {
            if (fixations[i].startTimeStamp < time)
            {
                return fixations[i];
            }
        }

        return null;
    }
#nullable disable

    bool VectorsWithinRange(Vector3 v1, Vector3 v2, float range)
    {
        return Vector3.Distance(v1, v2) < range;
    }
}
