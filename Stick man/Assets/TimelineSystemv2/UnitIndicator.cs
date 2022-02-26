using System;
using Assets.Scripts.utils;
using UnityEngine;

public class UnitIndicator : MonoBehaviour, ITimelineIndicator
{
    public RectTransform bg;
    public RectTransform image;
    public RectTransform AttackIndicator;
    [Header("Props")]
    public float PercentTurn;
    public INDICATOR_TYPE Type { get; set; }
    public Unit Unit { get; set; }
    public int Level { get; set; }
    public float TurnWidth { get; set; }
    public int GhostID { get; set; }
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

        var bgSize = __percent.Find(150, Rt.sizeDelta.y);
        AttackIndicator.sizeDelta = new Vector2(bgSize, Rt.sizeDelta.y);
        AttackIndicator.anchoredPosition = new Vector2(0, -pos);

        ShowAttackIndicator(false);
    }

    public void ShowAttackIndicator(bool show = true)
    {
        bg.gameObject.SetActive(!show);
        AttackIndicator.gameObject.SetActive(show);
    }

    public override string ToString()
    {
        return "\n" + String.Format(@"Unit.ID: {0},
        TurnWidth: {1},
        Level: {2},
        PercentTurn: {3}
        GhostID: {4},
        INDICATOR_TYPE: {5}",
        Unit.ID,
        TurnWidth,
        Level,
        PercentTurn,
        GhostID,
        Type
        );
    }
}
