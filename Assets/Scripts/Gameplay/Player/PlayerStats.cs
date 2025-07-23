// PlayerStats.cs
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 10;
    public int maxActionsPerTurn = 2;

    public int CurrentHealth { get; private set; }
    public int CurrentActions { get; private set; }

    public event Action<int> OnHealthChanged;
    public event Action<int> OnActionsChanged;

    void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentActions = maxActionsPerTurn;
    }

    public void ChangeHealth(int delta)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + delta, 0, maxHealth);
        Debug.Log($"[PlayerStats] Health now {CurrentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public bool TryUseAction(int cost = 1)
    {
        if (CurrentActions < cost) return false;
        CurrentActions -= cost;
        OnActionsChanged?.Invoke(CurrentActions);
        return true;
    }

    public void ResetActions()
    {
        CurrentActions = maxActionsPerTurn;
        OnActionsChanged?.Invoke(CurrentActions);
    }
}
