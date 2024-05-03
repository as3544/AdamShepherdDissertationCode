using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixation
{
    public long startTimeStamp;
    public long endTimeStamp;
    public Vector3 gazeLocation;


    public long duration
    {
        get
        {
            return endTimeStamp - startTimeStamp;
        }
    }
}
