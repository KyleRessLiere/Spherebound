using UnityEngine;

public struct AttackInstance
{
    public Vector2Int targetCoord;
    public int damage;
    // Future: add fields like:
    // public StatusEffect statusEffect;

    public AttackInstance(Vector2Int targetCoord, int damage)
    {
        this.targetCoord = targetCoord;
        this.damage = damage;
    }
}
