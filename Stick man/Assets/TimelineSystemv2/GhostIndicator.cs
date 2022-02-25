using System;
using Assets.Scripts.utils;
using UnityEngine;

public class GhostIndicator : MonoBehaviour
{
    public RectTransform bg;
    public RectTransform image;
    [Header("Props")]
    public int ID;
    internal int UnitID;
    internal RectTransform Rt;

    internal void Init(int unitID)
    {
        ID = unitID + Timeline.GHOSTINDICATOR_ID_THRESHOLD;
        UnitID = unitID;

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
        return String.Format(@"
        ID: {0},
        UnitID: {1}
        ", ID, UnitID);
    }
}
