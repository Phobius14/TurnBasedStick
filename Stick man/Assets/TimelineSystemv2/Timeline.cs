using System.Collections.Generic;
using System.Linq;
using Assets.HeadStart.Core;
using Assets.Scripts.utils;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    public RectTransform MaskRt;
    public Vector2 TimelineCanvas;
    public float TimelineOneTurnWidth;
    public float OneUnitPercentage;
    public RectTransform Level1;
    public RectTransform Level2;
    public RectTransform Level3;
    public List<TimelineIndicator> TimelineIndicators;
    public List<GhostIndicator> GhostIndicators;
    public static float LevelHeight;
    private List<Unit> _allUnits;
    private GameObject _valueTweenGo;
    private CoreObjCallback _setNextToDoTurn;
    private int? _pullAllTwId;
    private float? _distanceToFirst;
    private float _distanceSoFar;
    private TimelineIndicator _currentTimelineIndicator;
    private int? _pushOneTwId;
    private CoreCallback _endingTurnCallback;
    // 
    private readonly float MOVE_INDICATORS_TIME = 0.3f;

    public void Init(List<Unit> team1, List<Unit> team2)
    {
        var canasRt = Main._.CoreCamera.Canvas.GetComponent<RectTransform>();
        float w = canasRt.rect.width;
        float h = canasRt.rect.height;
        TimelineCanvas = new Vector2(w * 0.75f, h * 0.2f);
        TimelineOneTurnWidth = __percent.Find(80, TimelineCanvas.x);
        LevelHeight = TimelineCanvas.y / 3;
        OneUnitPercentage = __percent.What(LevelHeight, TimelineOneTurnWidth) / 2;
        Debug.Log("OneUnitPercentage: " + OneUnitPercentage);

        Level1.anchoredPosition = Vector2.zero;
        Level1.sizeDelta = new Vector2(LevelHeight, LevelHeight);
        Level2.anchoredPosition = new Vector2(0, -LevelHeight);
        Level2.sizeDelta = new Vector2(LevelHeight, LevelHeight);
        Level3.anchoredPosition = new Vector2(0, -LevelHeight * 2);
        Level3.sizeDelta = new Vector2(LevelHeight, LevelHeight);

        _allUnits = new List<Unit>();
        _allUnits.AddRange(team1);
        _allUnits.AddRange(team2);

        foreach (var unit in _allUnits)
        {
            CreateUnitIndicator(unit);
        }

        checkPercentageDistribution();

        foreach (var indicator in TimelineIndicators)
        {
            indicator.TurnWidth = __percent.Find(indicator.PercentTurn, TimelineOneTurnWidth);
            indicator.Level = 1;

            var iRt = (indicator.transform as RectTransform);
            iRt.anchoredPosition = new Vector2(
                indicator.TurnWidth,
                getLevelYPos(indicator.Level)
            );

            var giIndex = GhostIndicators.FindIndex(gi => gi.UnitID == indicator.Unit.ID);
            var giRt = (GhostIndicators[giIndex].transform as RectTransform);
            giRt.anchoredPosition = new Vector2(
                indicator.TurnWidth * 2,
                getLevelYPos(indicator.Level)
            );
        }

        _valueTweenGo = new GameObject();
    }

    public TimelineIndicator GetClosestToTurn()
    {
        TimelineIndicator tIndicator = null;
        _distanceToFirst = 9999;
        foreach (var indicator in TimelineIndicators)
        {
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

        // Debug.Log("_distanceToFirst: " + _distanceToFirst);
        return tIndicator;
    }

    public void DragAllCloserToTurn(CoreObjCallback setNextToDoTurn)
    {
        _currentTimelineIndicator = GetClosestToTurn();

        _setNextToDoTurn = setNextToDoTurn;
        _distanceSoFar = _distanceToFirst.Value;
        var fromPos = _distanceToFirst.Value;
        var toPos = 0;

        _pullAllTwId = LeanTween.value(
            fromPos,
            toPos,
            MOVE_INDICATORS_TIME
        ).id;

        LeanTween.descr(_pullAllTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_pullAllTwId.Value)
            .setOnUpdate((float newPos) =>
            {
                var diff = _distanceSoFar - newPos;
                _distanceSoFar = newPos;

                foreach (var indicator in TimelineIndicators)
                {
                    var iRt = (indicator.transform as RectTransform);
                    iRt.anchoredPosition = new Vector2(
                        iRt.anchoredPosition.x - diff,
                        iRt.anchoredPosition.y
                    );
                }

                foreach (var gi in GhostIndicators)
                {
                    var giRt = (gi.transform as RectTransform);
                    giRt.anchoredPosition = new Vector2(
                        giRt.anchoredPosition.x - diff,
                        giRt.anchoredPosition.y
                    );
                }
            });

        if (_setNextToDoTurn != null)
        {
            LeanTween.descr(_pullAllTwId.Value)
                .setOnComplete(() =>
                {
                    _setNextToDoTurn(_currentTimelineIndicator);
                });
        }
    }

    public void MoveCurrentIndicatorToHisNextTurn(CoreCallback endingTurnCallback)
    {
        _endingTurnCallback = endingTurnCallback;

        _distanceSoFar = 0;
        var fromPos = 0;
        var toPos = _currentTimelineIndicator.TurnWidth;

        _pushOneTwId = LeanTween.value(
            fromPos,
            toPos,
            MOVE_INDICATORS_TIME
        ).id;
        LeanTween.descr(_pushOneTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_pushOneTwId.Value)
            .setOnUpdate((float newPos) =>
            {
                var diff = newPos - _distanceSoFar;
                _distanceSoFar = newPos;

                var iRt = (_currentTimelineIndicator.transform as RectTransform);
                iRt.anchoredPosition = new Vector2(
                    iRt.anchoredPosition.x + diff,
                    iRt.anchoredPosition.y
                );

                var giIndex = GhostIndicators.FindIndex(gi => gi.UnitID == _currentTimelineIndicator.Unit.ID);
                var giRt = (GhostIndicators[giIndex].transform as RectTransform);
                giRt.anchoredPosition = new Vector2(
                    giRt.anchoredPosition.x + diff,
                    giRt.anchoredPosition.y
                );
            });

        if (_endingTurnCallback != null)
        {
            LeanTween.descr(_pushOneTwId.Value)
                .setOnComplete(() =>
                {
                    _distanceToFirst = null;
                    _setNextToDoTurn = null;
                    _currentTimelineIndicator = null;

                    if (checkIfIndicatorsOverlap())
                    {
                        return;
                    }

                    _endingTurnCallback();
                });
        }
    }

    private void CreateUnitIndicator(Unit unit)
    {
        var go = Instantiate(
            unit.UnitSettings.TimelineIndicator,
            Vector2.zero, Quaternion.identity, MaskRt);

        var unitIndicatorRt = (go.transform as RectTransform);
        unitIndicatorRt.anchoredPosition = Vector2.zero;
        unitIndicatorRt.sizeDelta = new Vector2(LevelHeight, LevelHeight);

        var indicator = unitIndicatorRt.GetComponent<TimelineIndicator>();
        indicator.Unit = unit;
        go.name = "(" + unit.ID + ")" + unit.Name;

        // Create Ghost Indicator
        go = Instantiate(
            unit.UnitSettings.GhostIndicator,
            Vector2.zero, Quaternion.identity, MaskRt);

        unitIndicatorRt = (go.transform as RectTransform);
        unitIndicatorRt.anchoredPosition = Vector2.zero;
        unitIndicatorRt.sizeDelta = new Vector2(LevelHeight, LevelHeight);

        var ghostIndicator = unitIndicatorRt.GetComponent<GhostIndicator>();
        ghostIndicator.ID = unit.ID + 1000;
        go.name = "(" + ghostIndicator.ID + ") GHOST" + unit.Name;
        ghostIndicator.UnitID = unit.ID;
        indicator.GhostID = ghostIndicator.ID;

        TimelineIndicators.Add(indicator);
        GhostIndicators.Add(ghostIndicator);
    }

    private bool checkIfIndicatorsOverlap()
    {
        var unsorted = new Dictionary<int, float>();

        TimelineIndicators.ForEach(ti =>
        {
            unsorted.Add(
                ti.Unit.ID,
                Mathf.Ceil(__percent.What(
                    ti.gameObject.GetComponent<RectTransform>().anchoredPosition.x,
                    TimelineOneTurnWidth
                ))
            );
        });
        GhostIndicators.ForEach(gi =>
        {
            unsorted.Add(
                gi.ID,
                Mathf.Ceil(__percent.What(
                    gi.gameObject.GetComponent<RectTransform>().anchoredPosition.x,
                    TimelineOneTurnWidth
                ))
            );
        });

        // __debug.DDictionary<float>(unsorted, "unsorted: ");

        var allRectsXP = new Dictionary<int, float>();
        foreach (KeyValuePair<int, float> author in unsorted.OrderBy(key => key.Value))
        {
            allRectsXP.Add(author.Key, author.Value);
        }

        // __debug.DDictionary<float>(allRectsXP, "allRectsXP: ");

        var allPercentages = allRectsXP.Select(pair => pair.Value).Distinct().ToList();
        // __debug.DList<float>(allPercentages, "allPercentages: ");

        var duplicatePercentages = new List<OverlappingPercentage>();

        bool hasTooCloseNeighbors = false;
        for (var i = 0; i < (allRectsXP.Count - 1); i++)
        {
            var closest = allRectsXP.ElementAt(i);
            var farthest = allRectsXP.ElementAt(i + 1);
            var offset = (OneUnitPercentage / 2);
            // Debug.Log("offset: " + offset);
            var isNeighborClose = closest.Value > (farthest.Value - offset);
            if (isNeighborClose)
            {
                hasTooCloseNeighbors = true;

                // Debug.Log(
                //     "(" + closest.Key + ")A: " + closest.Value + ", (" + farthest.Key + ")B: " + farthest.Value +
                //     " => " + closest.Value + " > " + (farthest.Value - offset)
                // );

                // Solve concurency issue
                var isClosestGhost = closest.Key > 1000;
                var closestIndicator = isClosestGhost
                    ? TimelineIndicators.Find(ti => ti.GhostID == closest.Key)
                    : TimelineIndicators.Find(ti => ti.Unit.ID == closest.Key);

                var isFarthestGhost = farthest.Key > 1000;
                var farthestIndicator = isFarthestGhost
                    ? TimelineIndicators.Find(ti => ti.GhostID == farthest.Key)
                    : TimelineIndicators.Find(ti => ti.Unit.ID == farthest.Key);

                var indicator = closestIndicator.Unit.Haste >= farthestIndicator.Unit.Haste
                    ? closestIndicator : farthestIndicator;

                var currentPercentageOfIndicator = allRectsXP[indicator.Unit.ID];
                var pIndex = allPercentages.FindIndex(ap => ap == currentPercentageOfIndicator);
                float min = (pIndex == 0
                    ? allPercentages[pIndex]
                    : allPercentages[pIndex - 1]);
                float max = (pIndex == (allPercentages.Count - 1)
                    ? allPercentages[pIndex]
                    : allPercentages[pIndex + 1]);

                var existingIndex = duplicatePercentages
                    .FindIndex(dp => dp.Percent == currentPercentageOfIndicator);
                if (existingIndex >= 0)
                {
                    duplicatePercentages[existingIndex].Duplicates.Add(indicator);
                }
                else
                {
                    duplicatePercentages.Add(new OverlappingPercentage()
                    {
                        Min = min,
                        Max = max,
                        Percent = currentPercentageOfIndicator,
                        Duplicates = new List<TimelineIndicator> { indicator }
                    });
                }
            }
        }
        // __debug.DList<OverlappingPercentage>(duplicatePercentages, "duplicatePercentages: ");

        var indicatorsToMove = new List<IndicatorMove>();

        foreach (var dp in duplicatePercentages)
        {
            bool moveLeft = (dp.Percent - dp.Min) > (dp.Max - dp.Percent);
            float space = moveLeft
                ? dp.Percent - dp.Min
                : dp.Max - dp.Percent;

            float howMuchWeOffset = space / (dp.Duplicates.Count + 1);
            // Debug.Log("howMuchWeOffset: " + howMuchWeOffset);

            // Debug.Log("OneUnitPercentage: " + OneUnitPercentage);
            if (howMuchWeOffset > OneUnitPercentage)
            {
                howMuchWeOffset = OneUnitPercentage;
            }

            int i = 1;
            foreach (var dpTi in dp.Duplicates)
            {
                indicatorsToMove.Add(new IndicatorMove()
                {
                    IndicatorIndex = TimelineIndicators.FindIndex(ti => ti.Unit.ID == dpTi.Unit.ID),
                    UnitID = dpTi.Unit.ID,
                    MoveLeft = moveLeft,
                    OffsetPercent = (i * howMuchWeOffset)
                });
                i++;
            }
        }

        // __debug.DList<IndicatorMove>(indicatorsToMove, "indicatorsToMove: ");

        foreach (var im in indicatorsToMove)
        {
            moveToCorrectedPosition(im);
        }

        if (hasTooCloseNeighbors)
        {
            __.Time.RxWait(() => { if (_endingTurnCallback != null) _endingTurnCallback(); }, MOVE_INDICATORS_TIME);
        }

        return hasTooCloseNeighbors;
    }

    private void moveToCorrectedPosition(IndicatorMove im)
    {
        float distanceSoFar = 0;
        float fromPos = 0;
        float toPos = __percent.Find(im.OffsetPercent, TimelineOneTurnWidth);
        if (im.MoveLeft)
        {
            fromPos = __percent.Find(im.OffsetPercent, TimelineOneTurnWidth);
            distanceSoFar = fromPos;
            toPos = 0;
        }
        // Debug.Log("fromPos: " + fromPos);
        // Debug.Log("toPos: " + toPos);

        var pushTwId = LeanTween.value(
            fromPos,
            toPos,
            MOVE_INDICATORS_TIME / 2
        ).id;
        LeanTween.descr(pushTwId).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(pushTwId)
            .setOnUpdate((float newPos) =>
            {
                // Debug.Log("newPos: " + newPos);
                var diff = newPos - distanceSoFar;
                distanceSoFar = newPos;
                // Debug.Log("diff: " + diff);

                var tiRt = (TimelineIndicators[im.IndicatorIndex].transform as RectTransform);
                var giRt = GhostIndicators
                    .Find(gi => gi.ID == TimelineIndicators[im.IndicatorIndex].GhostID)
                    .transform as RectTransform;

                tiRt.anchoredPosition = tiRt.anchoredPosition + new Vector2(diff, 0);
                giRt.anchoredPosition = giRt.anchoredPosition + new Vector2(diff, 0);
            });
    }

    private void checkPercentageDistribution()
    {
        var duplicatePercentages = new List<OverlappingPercentage>();

        var highestHaste = TimelineIndicators.Max(ti => ti.Unit.Haste);
        foreach (var indicator in TimelineIndicators)
        {
            var percentHaste = __percent.What(indicator.Unit.Haste, highestHaste);
            indicator.PercentTurn = percentHaste;
        }
        // __debug.DebugList<TimelineIndicator>(TimelineIndicators, "TimelineIndicators: ");

        var sorted = TimelineIndicators.OrderBy(o => o.PercentTurn).Reverse().ToList();
        // __debug.DebugList<TimelineIndicator>(sorted, "sorted: ");

        var allPercentages = sorted.Select(s => s.PercentTurn).Distinct().ToList();
        // __debug.DebugList<float>(allPercentages, "allPercentages: ");

        for (int i = 0; i < sorted.Count; i++)
        {
            var duplicates = sorted.FindAll(s => s.PercentTurn == sorted[i].PercentTurn);

            if (duplicates.Count <= 1) { continue; }

            var existsDuplicates = duplicatePercentages.Exists(dp => dp.Percent == sorted[i].PercentTurn);

            if (existsDuplicates) { continue; }

            var pIndex = allPercentages.FindIndex(ap => ap == sorted[i].PercentTurn);
            float min = (pIndex == 0
                ? allPercentages[pIndex]
                : allPercentages[pIndex - 1]);
            float max = (pIndex == (allPercentages.Count - 1)
                ? allPercentages[pIndex]
                : allPercentages[pIndex + 1]);

            duplicatePercentages.Add(new OverlappingPercentage()
            {
                Min = min,
                Max = max,
                Percent = sorted[i].PercentTurn,
                Duplicates = duplicates
            });
        }

        // __debug.DebugList<OverlappingPercentage>(duplicatePercentages, "duplicatePercentages: ");

        foreach (var dp in duplicatePercentages)
        {
            float howMuchWeOffset = (dp.Min - dp.Max) / dp.Duplicates.Count;
            // Debug.Log("howMuchWeOffset: " + howMuchWeOffset);

            if (howMuchWeOffset > OneUnitPercentage)
            {
                howMuchWeOffset = OneUnitPercentage;
            }

            int i = 0;
            foreach (var dpTi in dp.Duplicates)
            {
                var tiIndex = TimelineIndicators.FindIndex(ti => ti.Unit.ID == dpTi.Unit.ID);
                TimelineIndicators[tiIndex].PercentTurn = dp.Percent - (i * howMuchWeOffset);
                i++;
            }
        }

        // __debug.DebugList<TimelineIndicator>(TimelineIndicators, "TimelineIndicators: ");

        // Debug.Break();
    }

    private float getLevelYPos(int level)
    {
        return Mathf.CeilToInt(LevelHeight - (level * LevelHeight));
    }

    private bool almostEqual(float a, float b, float diff)
    {
        return (b - diff <= a && a <= b + diff);
    }
}
