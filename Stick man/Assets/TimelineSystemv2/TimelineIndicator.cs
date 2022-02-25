using System;
using Assets.Scripts.utils;
using UnityEngine;

public class TimelineIndicator : MonoBehaviour
{
    public RectTransform bg;
    public RectTransform image;
    [Header("Props")]
    public float TurnWidth;
    public int Level;
    public float PercentTurn;
    internal Unit Unit;
    internal int GhostID;
    internal int AttackID;
    private RectTransform _rt;

    internal void Init(Unit unit)
    {
        Unit = unit;

        _rt = transform as RectTransform;

        if (!bg) { return; }

        var size = __percent.Find(70, _rt.sizeDelta.y);
        var pos = (_rt.sizeDelta.y - size) / 2;
        bg.sizeDelta = new Vector2(size, size);
        bg.anchoredPosition = new Vector2(pos, -pos);

        size = __percent.Find(60, _rt.sizeDelta.y);
        pos = (_rt.sizeDelta.y - size) / 2;

        image.sizeDelta = new Vector2(size, size);
        image.anchoredPosition = new Vector2(pos, -pos);
    }

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
