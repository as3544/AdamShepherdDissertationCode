using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawGazePoint
{
    public Vector3 gazeLocation;
    public long gazeTimestamp;

    public RawGazePoint(Vector3 gazeLocation)
    {
        this.gazeLocation = gazeLocation;
        this.gazeTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public RawGazePoint(Vector3 gazeLocation, long gazeTimestamp)
    {
        this.gazeLocation = gazeLocation;
        this.gazeTimestamp = gazeTimestamp;
    }
}
