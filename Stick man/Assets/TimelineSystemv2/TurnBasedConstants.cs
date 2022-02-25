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

    internal static UnitIndicator SolveClosestIndicator(
        List<UnitIndicator> tIndicators,
        KeyValuePair<int, float> closest, KeyValuePair<int, float> farthest
    )
    {
        var isClosestGhost = closest.Key > Timeline.GHOSTINDICATOR_ID_THRESHOLD
            && closest.Key < Timeline.ATTACKINDICATOR_ID_THRESHOLD;
        var isClosestAttack = closest.Key > Timeline.ATTACKINDICATOR_ID_THRESHOLD;

        UnitIndicator closestIndicator = (isClosestGhost
            ? tIndicators.Find(ti => ti.GhostID == closest.Key)
            : (isClosestAttack
                ? tIndicators.Find(ti => ti.AttackID == closest.Key)
                : tIndicators.Find(ti => ti.Unit.ID == closest.Key)
                )
            );

        var isFarthestGhost = farthest.Key > Timeline.GHOSTINDICATOR_ID_THRESHOLD
            && farthest.Key < Timeline.ATTACKINDICATOR_ID_THRESHOLD;
        var isFarthestAttack = farthest.Key > Timeline.ATTACKINDICATOR_ID_THRESHOLD;

        var farthestIndicator = tIndicators.Find(ti =>
        {
            if (isFarthestGhost == true) { return ti.GhostID == farthest.Key; }
            if (isFarthestAttack == true) { return ti.AttackID == farthest.Key; }
            return ti.Unit.ID == farthest.Key;
        });

        var indicator =
            closestIndicator.Unit.Haste >= farthestIndicator.Unit.Haste
            ? closestIndicator
            : farthestIndicator;

        return indicator;
    }

    internal static ITimelineIndicator GetClosestToTurn(
        ref float? _distanceToFirst,
        List<UnitIndicator> tIndicators,
        List<AttackIndicator> aIndicators
    )
    {
        List<ITimelineIndicator> combined = new List<ITimelineIndicator>();
        combined.AddRange(tIndicators);
        combined.AddRange(aIndicators);

        ITimelineIndicator tIndicator = null;
        _distanceToFirst = 9999;
        foreach (ITimelineIndicator indicator in combined)
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

        // Debug.Log("_distanceToFirst: " + _distanceToFirst);

        // foreach (var ai in aIndicators)
        // {
        //     if (ai.gameObject.activeInHierarchy == false) { continue; }

        //     var iRt = (ai.transform as RectTransform);
        //     float? distance = null;
        //     if (ai.TimelineIndicator.Level == 1)
        //     {
        //         distance = Vector2.Distance(_level1Pos, iRt.anchoredPosition);
        //     }
        //     else if (ai.TimelineIndicator.Level == 2)
        //     {
        //         distance = Vector2.Distance(_level2Pos, iRt.anchoredPosition);
        //     }
        //     else
        //     {
        //         distance = Vector2.Distance(_level3Pos, iRt.anchoredPosition);
        //     }

        //     if (distance.HasValue == false)
        //     {
        //         continue;
        //     }

        //     if (distance < _distanceToFirst)
        //     {
        //         tIndicator = ai;
        //         _distanceToFirst = distance.Value;
        //     }
        // }
        Debug.Log("tIndicator: " + tIndicator);
        Debug.Log("_distanceToFirst: " + _distanceToFirst);
        return tIndicator;
    }
}