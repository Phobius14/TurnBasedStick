
using System;
using System.Collections.Generic;
using UnityEngine;

public enum INDICATOR_TYPE
{
    UNIT, ATTACK
}

public interface ITimelineIndicator
{
    INDICATOR_TYPE Type { get; }
    Unit Unit { get; set; }
    int Level { get; set; }
    RectTransform Rt { get; set; }
    GameObject Go { get; }
    float TurnWidth { get; set; }
    int GhostID { get; set; }

    void ShowAttackIndicator(bool show = true);
}

public class OverlappingPercentage
{
    public float Percent;
    public float Min;
    public List<ITimelineIndicator> Duplicates;
    public float Max;

    public override string ToString()
    {
        string duplicateString = "";
        foreach (var d in Duplicates)
        {
            duplicateString += d.Unit.ID + ", ";
        }

        return "\n" + String.Format(@"
        Percent: {0},
        Duplicates: {1},
        Min: {2},
        Max: {3}
        ",
        Percent,
        duplicateString,
        Min,
        Max
        );
    }
}

public class IndicatorMove
{
    public int IndicatorIndex;
    public int UnitID;
    public float OffsetPercent;
    internal bool MoveLeft;

    public override string ToString()
    {
        return "\n" + String.Format(@"
        UnitID: {0},
        IndicatorIndex: {1},
        OffsetPercent: {2},
        MoveLeft: {3}
        ",
        UnitID,
        IndicatorIndex,
        OffsetPercent,
        MoveLeft
        );
    }
}