using System;
using UnityEngine;

public class GhostIndicator : MonoBehaviour
{
    public int ID;
    internal int UnitID;

    public override string ToString()
    {
        return String.Format(@"
        ID: {0},
        UnitID: {1}
        ", ID, UnitID);
    }
}
