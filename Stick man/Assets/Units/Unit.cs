using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public Transform HealthBarT;
    public Transform AttackPointT;
    public PickButton PickButton;
    internal int Team;
    internal bool AI;
    internal int ID;
    internal Unit TargetEnemy;
    public CoreObservedValues PointsObservedValues = new CoreObservedValues();
    public UnitSettings UnitSettings;
    protected CoreIdCallback _afterAttack;
    protected float _attack_duration;
    private HealthBarSetting _healthBarSetting;
    private float _maxHealth;
    private float _currentHealth;
    private Vector3 _originalPos;
    private int? _moveToAttackTwId;
    private Action<Unit> _onPickedEnemy;

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

        if (PickButton != null)
        {
            PickButton.Init(() =>
            {
                if (_onPickedEnemy == null) { return; }

                _onPickedEnemy(this);
            });
            HidePickButton();
        }
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

    public virtual void DoDelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {

    }

    public virtual void SetupDelayedAttack(int actionId, CoreIdCallback afterAttack = null)
    {

    }

    internal void ActivatePickButton(Action<Unit> onPickedEnemy)
    {
        _onPickedEnemy = onPickedEnemy;
        PickButton.gameObject.SetActive(true);
    }

    internal void HidePickButton()
    {
        PickButton.gameObject.SetActive(false);
    }

    public virtual void MoveToAttack()
    {
        if (UnitSettings.IsRange)
        {
            return;
        }

        _originalPos = transform.position;

        _moveToAttackTwId = LeanTween.move(
            gameObject,
            TargetEnemy.AttackPointT.position,
            _attack_duration
        ).setEase(LeanTweenType.easeOutExpo).id;

        LeanTween.descr(_moveToAttackTwId.Value)
            .setOnComplete(() =>
            {
                _moveToAttackTwId = _moveToAttackTwId = LeanTween.move(
                    gameObject,
                    _originalPos,
                    _attack_duration
                ).setEase(LeanTweenType.easeOutExpo).id;
            });
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

        if (_currentHealth <= 0)
        {
            __.Time.RxWait(() =>
            {
                Debug.Log(
                    "<b>" + gameObject.name + "</b> " +
                    "Died (took " + damage + " damage)"
                );
                HealthChange hpChange = new HealthChange()
                {
                    UnitID = ID,
                    Kill = true
                };
                PointsObservedValues.SetValue(ScreenHealth.HEALTH_CHANGE, hpChange);
                (Main._.Game as TheGame).OnUnitDeath(this);
            }, 0.01f);
        }

        HealthChange hpChange = new HealthChange()
        {
            UnitID = ID,
            CalculatedHp = Mathf.CeilToInt(_currentHealth),
            Dmg = Mathf.CeilToInt(damage)
        };
        PointsObservedValues.SetValue(ScreenHealth.HEALTH_CHANGE, hpChange);
    }
}
