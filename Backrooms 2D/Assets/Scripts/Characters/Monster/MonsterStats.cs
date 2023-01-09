using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Stat", menuName = "Monster/Monster Stat", order = 1)]
public class MonsterStats : ScriptableObject
{
    public float speed = 500;
    public float timeBeforeGenerateNextPath = .2f;
    public float nextWaypointDistance = .25f;
    public float linearDrag = 3f;
}