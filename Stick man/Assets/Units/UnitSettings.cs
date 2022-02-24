using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitSettings", order = 1)]
public class UnitSettings : ScriptableObject
{

    [Header("Prefabs")]
    public GameObject TimelineIndicator;
    public GameObject AttackIndicator;
    public GameObject GhostIndicator;
}
