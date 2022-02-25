using System;
using Assets.Scripts.utils;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    public RectTransform bg;
    public RectTransform image;
    [Header("Props")]
    public int ID;
    internal TimelineIndicator TimelineIndicator;
    internal int UnitID;
    internal RectTransform Rt;

    internal void Init(TimelineIndicator timelineIndicator)
    {
        TimelineIndicator = timelineIndicator;
        UnitID = TimelineIndicator.Unit.ID;
        ID = UnitID + Timeline.ATTACKINDICATOR_ID_THRESHOLD;

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
        UnitID: {1}
        ", ID, UnitID);
    }
}
