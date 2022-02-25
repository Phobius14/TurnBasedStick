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
    public static RectTransform Level1;
    public static RectTransform Level2;
    public static RectTransform Level3;

    internal static void InitCalculations(float levelHeight)
    {
        Level1.anchoredPosition = Vector2.zero;
        Level1.sizeDelta = new Vector2(levelHeight, levelHeight);
        Level2.anchoredPosition = new Vector2(0, -levelHeight);
        Level2.sizeDelta = new Vector2(levelHeight, levelHeight);
        Level3.anchoredPosition = new Vector2(0, -levelHeight * 2);
        Level3.sizeDelta = new Vector2(levelHeight, levelHeight);
    }

    internal static TimelineIndicator SolveClosestIndicator(
        List<TimelineIndicator> tIndicators,
        KeyValuePair<int, float> closest, KeyValuePair<int, float> farthest
    )
    {
        var isClosestGhost = closest.Key > Timeline.GHOSTINDICATOR_ID_THRESHOLD
            && closest.Key < Timeline.ATTACKINDICATOR_ID_THRESHOLD;
        var isClosestAttack = closest.Key > Timeline.ATTACKINDICATOR_ID_THRESHOLD;

        TimelineIndicator closestIndicator = (isClosestGhost
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

    internal static TimelineIndicator GetClosestToTurn(
        ref float? _distanceToFirst,
        List<TimelineIndicator> tIndicators,
        List<AttackIndicator> aIndicators
    )
    {
        TimelineIndicator tIndicator = null;
        _distanceToFirst = 9999;
        foreach (var indicator in tIndicators)
        {
            if (indicator.gameObject.activeInHierarchy == false) { continue; }

            var iRt = (indicator.transform as RectTransform);
            float? distance = null;
            if (indicator.Level == 1)
            {
                distance = Vector2.Distance(Level1.anchoredPosition, iRt.anchoredPosition);
            }
            else if (indicator.Level == 2)
            {
                distance = Vector2.Distance(Level2.anchoredPosition, iRt.anchoredPosition);
            }
            else
            {
                distance = Vector2.Distance(Level3.anchoredPosition, iRt.anchoredPosition);
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

        foreach (var ai in aIndicators)
        {
            if (ai.gameObject.activeInHierarchy == false) { continue; }

            var iRt = (ai.transform as RectTransform);
            float? distance = null;
            if (ai.TimelineIndicator.Level == 1)
            {
                distance = Vector2.Distance(Level1.anchoredPosition, iRt.anchoredPosition);
            }
            else if (ai.TimelineIndicator.Level == 2)
            {
                distance = Vector2.Distance(Level2.anchoredPosition, iRt.anchoredPosition);
            }
            else
            {
                distance = Vector2.Distance(Level3.anchoredPosition, iRt.anchoredPosition);
            }

            if (distance.HasValue == false)
            {
                continue;
            }

            if (distance < _distanceToFirst)
            {
                // TODO: return an interface
                // tIndicator = ai;
                _distanceToFirst = distance.Value;
            }
        }
        return tIndicator;
    }
}