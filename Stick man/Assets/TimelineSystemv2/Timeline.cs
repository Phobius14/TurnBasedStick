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
    public List<UnitIndicator> UnitIndicators;
    public List<GhostIndicator> GhostIndicators;
    public RectTransform Level1;
    public RectTransform Level2;
    public RectTransform Level3;
    public static float LevelHeight;
    private List<Unit> _allUnits;
    private GameObject _valueTweenGo;
    private CoreObjCallback _setNextToDoTurn;
    private int? _pullAllTwId;
    private float? _distanceToFirst;
    private float _distanceSoFar;
    private ITimelineIndicator _currentTimelineIndicator;
    private int? _pushOneTwId;
    private int? _moveAttackTwId;
    private CoreCallback _endingTurnCallback;
    // 
    public static readonly int GHOSTINDICATOR_ID_THRESHOLD = 1000;
    public static readonly float MOVE_INDICATORS_TIME = 0.3f;

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

        TimelineUtils.InitCalculations(ref Level1, ref Level2, ref Level3, LevelHeight);

        _allUnits = new List<Unit>();
        _allUnits.AddRange(team1);
        _allUnits.AddRange(team2);

        foreach (var unit in _allUnits)
        {
            CreateUnitIndicator(unit);
        }

        checkPercentageDistribution();

        foreach (var indicator in UnitIndicators)
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

    internal void RemoveIndicator(Unit unit)
    {
        int index = UnitIndicators.FindIndex(ui => ui.Unit.ID == unit.ID);
        UnitIndicators[index].RemoveYourself();
        Destroy(UnitIndicators[index].gameObject);

        UnitIndicators.RemoveAt(index);

        index = GhostIndicators.FindIndex(gi => gi.UnitID == unit.ID);
        Destroy(GhostIndicators[index].gameObject);

        GhostIndicators.RemoveAt(index);
    }

    public void DragAllCloserToTurn(CoreObjCallback setNextToDoTurn)
    {
        _currentTimelineIndicator = TimelineUtils.GetClosestToTurn(
            ref _distanceToFirst, UnitIndicators
        );

        _setNextToDoTurn = setNextToDoTurn;
        _distanceSoFar = _distanceToFirst.Value;
        var fromPos = _distanceToFirst.Value;
        var toPos = 0;

        _pullAllTwId = LeanTween.value(
            fromPos,
            toPos,
            MOVE_INDICATORS_TIME * TheGame.TIME_m
        ).id;

        LeanTween.descr(_pullAllTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_pullAllTwId.Value)
            .setOnUpdate((float newPos) =>
            {
                var diff = _distanceSoFar - newPos;
                _distanceSoFar = newPos;

                foreach (var indicator in UnitIndicators)
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

    internal void ShowDelayedAttack(ATTACK_ACTION action)
    {
        _currentTimelineIndicator.ShowAttackIndicator();

        Vector2 fromPos = _currentTimelineIndicator.Rt.anchoredPosition;
        Vector2 toPos = Vector2.zero;
        switch (action)
        {
            case ATTACK_ACTION.HARD:
                toPos = new Vector2(
                    _currentTimelineIndicator.TurnWidth / 1.33f,
                    getLevelYPos(3)
                );
                break;
            case ATTACK_ACTION.MEDIUM:
                toPos = new Vector2(
                    _currentTimelineIndicator.TurnWidth / 3,
                    getLevelYPos(2)
                );
                break;
            case ATTACK_ACTION.LIGHT:
            default:
                toPos = new Vector2(0, getLevelYPos(1));
                // _currentTimelineIndicator.Rt.anchoredPosition = toPos;
                break;
        }

        if (_moveAttackTwId.HasValue)
        {
            LeanTween.cancel(_moveAttackTwId.Value);
            _moveAttackTwId = null;
        }

        Vector2 distanceSoFar = Vector2.zero;

        _moveAttackTwId = LeanTween.value(
            _valueTweenGo,
            fromPos,
            toPos,
            MOVE_INDICATORS_TIME * TheGame.TIME_m
        ).id;
        LeanTween.descr(_moveAttackTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_moveAttackTwId.Value)
            .setOnUpdate((Vector2 newPos) =>
            {
                var diff = newPos - distanceSoFar;
                distanceSoFar = newPos;

                _currentTimelineIndicator.Rt.anchoredPosition = new Vector2(newPos.x, newPos.y);
                var giRt = GhostIndicators.Find(gi => gi.UnitID == _currentTimelineIndicator.Unit.ID).Rt;
                giRt.anchoredPosition = new Vector2(
                    newPos.x + _currentTimelineIndicator.TurnWidth,
                    newPos.y
                );
            });

        LeanTween.descr(_moveAttackTwId.Value)
            .setOnComplete(() =>
            {
                if (action == ATTACK_ACTION.LIGHT)
                {
                    _currentTimelineIndicator.ShowAttackIndicator(false);
                }
            });
    }

    internal void DoDelayedAttack(ITimelineIndicator ti, CoreCallback endingTurnCallback)
    {
        _currentTimelineIndicator.Rt.anchoredPosition = ti.Rt.anchoredPosition;

        _currentTimelineIndicator.ShowAttackIndicator(false);

        MoveCurrentIndicatorToHisNextTurn(endingTurnCallback);
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
            MOVE_INDICATORS_TIME * TheGame.TIME_m
        ).id;
        LeanTween.descr(_pushOneTwId.Value).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(_pushOneTwId.Value)
            .setOnUpdate((float newPos) =>
            {
                var diff = newPos - _distanceSoFar;
                _distanceSoFar = newPos;

                var iRt = _currentTimelineIndicator.Rt;
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

        if (_endingTurnCallback == null) { return; }

        LeanTween.descr(_pushOneTwId.Value)
            .setOnComplete(() =>
            {
                ResetTimeline();
                if (CheckIfIndicatorsOverlap())
                {
                    return;
                }

                _endingTurnCallback();
            });
    }

    public void SetLevelToCurrent(int actionLevel)
    {
        _currentTimelineIndicator.Level = actionLevel;
    }

    public void SetCurrentIndicatorType(INDICATOR_TYPE type)
    {
        _currentTimelineIndicator.Type = type;
    }

    public void ResetTimeline()
    {
        _distanceToFirst = null;
        _setNextToDoTurn = null;
        _currentTimelineIndicator = null;
    }

    private void CreateUnitIndicator(Unit unit)
    {
        var go = Instantiate(
            unit.UnitSettings.TimelineIndicator,
            Vector2.zero, Quaternion.identity, MaskRt);

        var indicatorRt = (go.transform as RectTransform);
        indicatorRt.anchoredPosition = Vector2.zero;
        indicatorRt.sizeDelta = new Vector2(LevelHeight, LevelHeight);

        var indicator = indicatorRt.GetComponent<UnitIndicator>();
        indicator.Init(unit);
        go.name = "(" + unit.ID + ")" + unit.Name;

        // Create Ghost Indicator
        go = Instantiate(
            unit.UnitSettings.GhostIndicator,
            Vector2.zero, Quaternion.identity, MaskRt);

        indicatorRt = (go.transform as RectTransform);
        indicatorRt.anchoredPosition = Vector2.zero;
        indicatorRt.sizeDelta = new Vector2(LevelHeight, LevelHeight);

        var ghostIndicator = indicatorRt.GetComponent<GhostIndicator>();
        ghostIndicator.Init(unit.ID);
        go.name = "(" + ghostIndicator.ID + ") GHOST" + unit.Name;

        indicator.GhostID = ghostIndicator.ID;

        UnitIndicators.Add(indicator);
        GhostIndicators.Add(ghostIndicator);
    }

    public bool CheckIfIndicatorsOverlap(CoreCallback endingTurnCallback = null)
    {
        if (endingTurnCallback != null)
        {
            _endingTurnCallback = endingTurnCallback;
        }
        var unsorted = new Dictionary<int, float>();

        UnitIndicators.ForEach(ti =>
        {
            if (ti.gameObject.activeInHierarchy == false) { return; }
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
        foreach (KeyValuePair<int, float> indctr in unsorted.OrderBy(key => key.Value))
        {
            allRectsXP.Add(indctr.Key, indctr.Value);
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

                var indicator = TimelineUtils.SolveClosestIndicator(
                    UnitIndicators,
                    closest, farthest
                );

                float currentPercentageOfIndicator = allRectsXP[indicator.Unit.ID];

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
                        Duplicates = new List<ITimelineIndicator> { indicator }
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
                    IndicatorIndex = UnitIndicators.FindIndex(ti => ti.Unit.ID == dpTi.Unit.ID),
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
            (MOVE_INDICATORS_TIME / 2) * TheGame.TIME_m
        ).id;
        LeanTween.descr(pushTwId).setEase(LeanTweenType.easeOutSine);
        LeanTween.descr(pushTwId)
            .setOnUpdate((float newPos) =>
            {
                // Debug.Log("newPos: " + newPos);
                var diff = newPos - distanceSoFar;
                distanceSoFar = newPos;
                // Debug.Log("diff: " + diff);

                var tiRt = (UnitIndicators[im.IndicatorIndex].transform as RectTransform);
                var giRt = GhostIndicators
                    .Find(gi => gi.ID == UnitIndicators[im.IndicatorIndex].GhostID)
                    .transform as RectTransform;

                tiRt.anchoredPosition = tiRt.anchoredPosition + new Vector2(diff, 0);
                giRt.anchoredPosition = giRt.anchoredPosition + new Vector2(diff, 0);
            });
    }

    private void checkPercentageDistribution()
    {
        var duplicatePercentages = new List<OverlappingPercentage>();

        var highestHaste = UnitIndicators.Max(ti => ti.Unit.UnitSettings.Haste);
        foreach (var indicator in UnitIndicators)
        {
            var percentHaste = __percent.What(indicator.Unit.UnitSettings.Haste, highestHaste);
            indicator.PercentTurn = percentHaste;
        }
        // __debug.DebugList<TimelineIndicator>(TimelineIndicators, "TimelineIndicators: ");

        var sorted = UnitIndicators.OrderBy(o => o.PercentTurn).Reverse().ToList();
        // __debug.DebugList<TimelineIndicator>(sorted, "sorted: ");

        var allPercentages = sorted.Select(s => s.PercentTurn).Distinct().ToList();
        // __debug.DebugList<float>(allPercentages, "allPercentages: ");

        for (int i = 0; i < sorted.Count; i++)
        {
            List<ITimelineIndicator> duplicates = sorted
                .FindAll(s => s.PercentTurn == sorted[i].PercentTurn)
                .Select(s => s as ITimelineIndicator).ToList();

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
                var tiIndex = UnitIndicators.FindIndex(ti => ti.Unit.ID == dpTi.Unit.ID);
                UnitIndicators[tiIndex].PercentTurn = dp.Percent - (i * howMuchWeOffset);
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
