using System;
using System.Collections.Generic;
using Assets.Scripts.utils;
using UnityEngine;

public enum ATTACK_ACTION
{
    LIGHT, MEDIUM, HARD
}

public static class TimelineUtils
{
    private static Vector2 _level1Pos;
    private static Vector2 _level2Pos;
    private static Vector2 _level3Pos;

    internal static void InitCalculations(
        ref RectTransform level1, ref RectTransform level2, ref RectTransform level3,
        float levelHeight
    )
    {
        level1.anchoredPosition = Vector2.zero;
        level1.sizeDelta = new Vector2(levelHeight, levelHeight);

        level2.anchoredPosition = new Vector2(0, -levelHeight);
        level2.sizeDelta = new Vector2(levelHeight, levelHeight);

        level3.anchoredPosition = new Vector2(0, -levelHeight * 2);
        level3.sizeDelta = new Vector2(levelHeight, levelHeight);

        _level1Pos = level1.anchoredPosition;
        _level2Pos = level2.anchoredPosition;
        _level3Pos = level3.anchoredPosition;
    }

    internal static ITimelineIndicator SolveClosestIndicator(
        List<UnitIndicator> tIndicators,
        KeyValuePair<int, float> closest, KeyValuePair<int, float> farthest
    )
    {
        Debug.Log("closest: " + closest);
        Debug.Log("farthest: " + farthest);

        var isClosestGhost = closest.Key > Timeline.GHOSTINDICATOR_ID_THRESHOLD;

        UnitIndicator closestIndicator = (isClosestGhost
            ? tIndicators.Find(ti => ti.GhostID == closest.Key)
            : tIndicators.Find(ti => ti.Unit.ID == closest.Key)
            );

        var isFarthestGhost = farthest.Key > Timeline.GHOSTINDICATOR_ID_THRESHOLD;

        var farthestIndicator = tIndicators.Find(ti =>
        {
            if (isFarthestGhost == true) { return ti.GhostID == farthest.Key; }
            return ti.Unit.ID == farthest.Key;
        });

        Debug.Log("closestIndicator: " + closestIndicator);
        Debug.Log("farthestIndicator: " + farthestIndicator);

        ITimelineIndicator indicator;
        if (closestIndicator.Unit.Haste >= farthestIndicator.Unit.Haste)
        {
            indicator = closestIndicator;
        }
        else
        {
            indicator = farthestIndicator;
        }

        return indicator;
    }

    internal static ITimelineIndicator GetClosestToTurn(
        ref float? _distanceToFirst, List<UnitIndicator> tIndicators)
    {
        ITimelineIndicator tIndicator = null;
        _distanceToFirst = 9999;
        foreach (ITimelineIndicator indicator in tIndicators)
        {
            if (indicator.Go.activeInHierarchy == false) { continue; }

            float? distance = null;
            if (indicator.Level == 1)
            {
                distance = Vector2.Distance(_level1Pos, indicator.Rt.anchoredPosition);
            }
            else if (indicator.Level == 2)
            {
                distance = Vector2.Distance(_level2Pos, indicator.Rt.anchoredPosition);
            }
            else
            {
                distance = Vector2.Distance(_level3Pos, indicator.Rt.anchoredPosition);
            }

            if (distance.HasValue == false)
            {
                continue;
            }

            if (distance < _distanceToFirst)
            {
                tIndicator = indicator;
                _distanceToFirst = distance.Value;
            }
        }
        return tIndicator;
    }
}