using System;
using Assets.Scripts.utils;
using UnityEngine;

public class AttackIndicator : MonoBehaviour, ITimelineIndicator
{
    public RectTransform bg;
    public RectTransform image;
    [Header("Props")]
    public int ID;
    internal UnitIndicator TimelineIndicator;
    public INDICATOR_TYPE Type { get { return INDICATOR_TYPE.ATTACK; } }
    public Unit Unit { get; set; }
    public int Level { get; set; }
    public float TurnWidth { get; set; }
    public int GhostID { get; set; }
    public int AttackID { get { return ID; } set { ID = value; } }
    public RectTransform Rt { get; set; }
    public GameObject Go { get { return gameObject; } }

    internal void Init(UnitIndicator timelineIndicator)
    {
        TimelineIndicator = timelineIndicator;
        Unit = TimelineIndicator.Unit;
        ID = Unit.ID + Timeline.ATTACKINDICATOR_ID_THRESHOLD;

        GhostID = timelineIndicator.GhostID;

        Rt = transform as RectTransform;

        if (!bg) { return; }

        var bgSize = __percent.Find(150, Rt.sizeDelta.y);
        var size = __percent.Find(60, Rt.sizeDelta.y);
        var pos = (Rt.sizeDelta.y - size) / 2;
        bg.sizeDelta = new Vector2(bgSize, Rt.sizeDelta.y);
        bg.anchoredPosition = new Vector2(0, -pos);

        image.sizeDelta = new Vector2(size, size);
        image.anchoredPosition = new Vector2(pos, -pos);
    }

    public override string ToString()
    {
        return String.Format(@"
        ID: {0},
        Unit.ID: {1}
        ", ID, Unit.ID);
    }
}
