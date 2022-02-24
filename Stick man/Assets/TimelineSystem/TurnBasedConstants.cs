
using System;
using System.Collections.Generic;

public enum ATTACK_ACTION
{
    LIGHT, MEDIUM, HARD
}

public static class TurnBasedConstants
{

}

public class OverlappingPercentage
{
    public float Percent;
    public float Min;
    public List<TimelineIndicator> Duplicates;
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