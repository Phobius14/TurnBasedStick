using System;
using Assets.Scripts.utils;
using UnityEngine;

public class UnitIndicator : MonoBehaviour, ITimelineIndicator
{
    public RectTransform bg;
    public RectTransform image;
    [Header("Props")]
    public float PercentTurn;
    public INDICATOR_TYPE Type { get { return INDICATOR_TYPE.UNIT; } }
    public Unit Unit { get; set; }
    public int Level { get; set; }
    public float TurnWidth { get; set; }
    public int GhostID { get; set; }
    public int AttackID { get; set; }
    public RectTransform Rt { get; set; }
    public GameObject Go { get { return gameObject; } }

    internal void Init(Unit unit)
    {
        Unit = unit;

        Rt = transform as RectTransform;

        if (!bg) { return; }

        var size = __percent.Find(70, Rt.sizeDelta.y);
        var pos = (Rt.sizeDelta.y - size) / 2;
        bg.sizeDelta = new Vector2(size, size);
        bg.anchoredPosition = new Vector2(pos, -pos);

        size = __percent.Find(60, Rt.sizeDelta.y);
        pos = (Rt.sizeDelta.y - size) / 2;

        image.sizeDelta = new Vector2(size, size);
        image.anchoredPosition = new Vector2(pos, -pos);
    }

    public override string ToString()
    {
        return "\n" + String.Format(@"UnitId: {0},
        TurnWidth: {1},
        Level: {2},
        PercentTurn: {3}
        GhostID: {4},
        AttackID: {5},
        INDICATOR_TYPE: {6}",
        Unit.ID,
        TurnWidth,
        Level,
        PercentTurn,
        GhostID,
        AttackID,
        Type
        );
    }
}
