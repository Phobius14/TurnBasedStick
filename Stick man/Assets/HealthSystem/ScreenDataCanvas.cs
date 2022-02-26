using System.Collections;
using System.Collections.Generic;
using Assets.HeadStart.Core;
using Assets.HeadStart.CoreUi;
using UnityEngine;
using UniRx;
using Assets.Scripts.utils;
using System;

public class ScreenDataCanvas : MonoBehaviour, IUiDependency
{
    public List<HealthBarSetting> HealthBarSettings;

    private RectTransform _rt;

    void Awake()
    {
        _rt = transform as RectTransform;
        ScreenData.SetAvailable(this);
    }

    public void InitDependency(object obj)
    {
        HealthBarSetting hbSetting = obj as HealthBarSetting;

        hbSetting.HealthBar = createHealthBar(hbSetting);

        HealthChange hc = new HealthChange() { CalculatedHp = hbSetting.MaxHealth };

        hbSetting.HealthBar.CalculateHealthBar(hc, hbSetting.MaxHealth);

        if (HealthBarSettings == null)
        {
            HealthBarSettings = new List<HealthBarSetting>();
        }
        HealthBarSettings.Add(hbSetting);
    }

    public void ListenForChanges(CoreUiObservedValue obj)
    {
    }

    public void ListenForChanges(ref CoreObservedValues obj)
    {
        obj.ObserveEveryValueChanged(o => o.CoreValues)
            .Subscribe(cv =>
            {
                if (cv == null) { return; }
                // __debug.DDictionary<object>(cv.Values, "cv.Values: ");

                if (cv.Contains(ScreenHealth.HEALTH_CHANGE))
                {
                    handleHealthChange(cv.Values[(int)ScreenHealth.HEALTH_CHANGE] as HealthChange);
                }
            });
    }

    private HealthBar createHealthBar(HealthBarSetting hbSetting)
    {
        GameObject go = Instantiate(
            Resources.Load("HealthBar", typeof(GameObject)),
            Vector3.zero, Quaternion.identity, _rt
            ) as GameObject;
        var rect = go.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localEulerAngles = Vector3.zero;

        var hpBar = go.GetComponent<HealthBar>();
        Color hpColor = __gameColor.GetColorByName(
            hbSetting.TeamID == 2
            ? "Red_Maroon_Flush"
            : "Blue_Cornflower"
        );

        hpBar.HealthImage.color = hpColor;
        hpBar.Init(hbSetting.Target);
        return hpBar;
    }

    private void handleHealthChange(HealthChange healthChange)
    {
        // Debug.Log("healthChange: " + healthChange);

        var healthBarSettings = HealthBarSettings
            .Find(hbs => hbs.UnitID == healthChange.UnitID);

        if (healthChange.Kill)
        {
            var index = HealthBarSettings.FindIndex(hpb => hpb.UnitID == healthChange.UnitID);
            HealthBarSettings[index].HealthBar.Kill();
            HealthBarSettings.RemoveAt(index);

            return;
        }

        healthBarSettings.HealthBar.CalculateHealthBar(
            healthChange, healthBarSettings.MaxHealth
        );
    }

    public void UnregisterOnDestroy()
    {
        ScreenData.SetUnavailable();
    }
    void OnDestroy()
    {
        UnregisterOnDestroy();
    }
}
