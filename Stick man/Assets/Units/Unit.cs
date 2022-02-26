using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public Transform HealthBarT;
    internal int Team;
    internal bool AI;
    internal int ID;
    public Unit TargetEnemy;
    public CoreObservedValues PointsObservedValues = new CoreObservedValues();
    public UnitSettings UnitSettings;
    protected CoreIdCallback _afterAttack;
    private HealthBarSetting _healthBarSetting;
    private float _maxHealth;
    private float _currentHealth;

    public virtual void Init(int unitId, int team, bool ai)
    {
        ID = unitId;
        gameObject.name = "(" + ID + ") " + Name;
        Team = team;
        AI = ai;

        _maxHealth = UnitSettings.MaxHealth;
        _currentHealth = _maxHealth;

        _healthBarSetting = new HealthBarSetting()
        {
            UnitID = ID,
            TeamID = Team,
            Target = HealthBarT,
            MaxHealth = UnitSettings.MaxHealth
        };
        ScreenData.InitDependency(_healthBarSetting);

        ScreenData.Register(ref PointsObservedValues);
    }

    public virtual void Attack1(CoreIdCallback afterAttack)
    {
        // ScreenDataSubject.Set(screenDataBlobs);
    }

    public virtual void Attack2(CoreIdCallback afterAttack = null)
    {

    }

    public virtual void Attack3(CoreIdCallback afterAttack = null)
    {
    }

    public virtual void DelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {

    }

    public virtual void DamageTargetEnemy(int multiply)
    {
        float multiplier = multiply == 0
            ? 1 : UnitSettings.AttackMultiplier * multiply;
        TargetEnemy.TakeDamage(UnitSettings.AttackDamage * multiplier);
        TargetEnemy = null;
    }

    internal void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        HealthChange hpChange = new HealthChange()
        {
            UnitID = ID,
            CalculatedHp = Mathf.CeilToInt(_currentHealth),
            Dmg = Mathf.CeilToInt(damage)
        };
        PointsObservedValues.SetValue(ScreenHealth.HEALTH_CHANGE, hpChange);
    }
}
