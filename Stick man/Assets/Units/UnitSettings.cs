using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitSettings", order = 1)]
public class UnitSettings : ScriptableObject
{
    [Header("Stats")]
    public int MaxHealth;
    public int AttackDamage;
    public float AttackMultiplier;
    public int Haste; // TODO: Need to rethink haste ??
    [Header("Prefabs")]
    public GameObject TimelineIndicator;
    public GameObject GhostIndicator;
}
