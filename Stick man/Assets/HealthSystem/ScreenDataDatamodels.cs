using System;
using UnityEngine;

public class HealthBarSetting
{
    public int UnitID;
    public int MaxHealth;
    public int TeamID;
    public Transform Target;
    public HealthBar HealthBar;

    public override string ToString()
    {
        return String.Format(@"{{
    UnitID: {0},
    MaxHealth: {1}
    }}", UnitID,
        MaxHealth
        );
    }
}

public class HealthChange
{
    public int UnitID;
    public int Heal;
    public int Shield;
    public int Dmg;
    public int CalculatedHp;
    public bool Kill;

    public override string ToString()
    {
        return String.Format(@"{{
    UnitID: {0},
    Heal: {1},
    Shield: {2},
    Damage: {3}
    }}", UnitID,
        Heal,
        Shield,
        Dmg
        );
    }
}
