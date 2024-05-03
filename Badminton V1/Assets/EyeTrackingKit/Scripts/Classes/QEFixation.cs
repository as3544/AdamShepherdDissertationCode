using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QEFixation
{
    public Fixation fixation;
    public long relativeHitPointTimeStamp;

    public long offset
    {
        get
        {
            return fixation.endTimeStamp - relativeHitPointTimeStamp;
        }
    }
    public long onset
    {
        get
        {
            return relativeHitPointTimeStamp - fixation.startTimeStamp;
        }
    }
    public long duration
    {
        get
        {
            return offset + onset;
        }
    }

    public QEFixation(Fixation fixation, long relativeHitPointTimeStamp)
    {
        this.fixation = fixation;
        this.relativeHitPointTimeStamp = relativeHitPointTimeStamp;
    }
}
