using System;
using Assets.HeadStart.Core;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public int Initiative;
    public int Health;
    public int Damage;
    internal int Team;
    internal bool AI;
    internal int ID;
    private CoreCallback _afterAttack;

    internal void Attack1(CoreCallback afterAttack)
    {
        _afterAttack = afterAttack;

        Debug.Log(gameObject.name + " -> <b>Attack 1</b> !!");
        __.Time.RxWait(() => {
            _afterAttack();
        }, 1f);
    }

    internal void Attack2(Action afterAttacking)
    {
        throw new NotImplementedException();
    }

    internal void Attack3(Action afterAttacking)
    {
        throw new NotImplementedException();
    }
}
