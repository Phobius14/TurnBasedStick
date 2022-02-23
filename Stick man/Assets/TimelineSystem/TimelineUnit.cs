using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineUnit
{
    public int TIndex;
    public bool IsOccupied;
    public Unit Unit;
    internal RectTransform CircleRt;

    public TimelineUnit()
    {
        initBaseValues();
    }

    public TimelineUnit(int time)
    {
        initBaseValues();
        TIndex = time;
    }

    private void initBaseValues()
    {
        IsOccupied = false;
    }

    public override string ToString()
    {
        return "\n{ " +
            "T: " + TIndex +
            ", Occupied: " + IsOccupied +
            (IsOccupied ? ", Unit: " + Unit.gameObject.name : "") +
        " }";
    }

    internal void Clear()
    {
        Unit = null;
        CircleRt = null;
        IsOccupied = false;
    }
}
