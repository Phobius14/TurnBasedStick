using System;
using UnityEngine;

public class TimelineIndicator : MonoBehaviour
{
    public Unit Unit;
    public float TurnWidth;
    public int Level;
    public float PercentTurn;
    internal int GhostID;

    public override string ToString()
    {
        return "\n" + String.Format(@"
        UnitId: {0},
        TurnWidth: {1},
        Level: {2},
        PercentTurn: {3}",
        Unit.ID,
        TurnWidth,
        Level,
        PercentTurn
        );
    }
}
