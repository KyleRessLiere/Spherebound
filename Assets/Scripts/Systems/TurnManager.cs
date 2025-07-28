using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [Header("Actors")]
    public PlayerController player;
    public EnemyManager enemyManager;

    [Header("UI")]
    public Button actionButton;
    public TMP_Text actionButtonText;

    private PlayerStats stats;

    void Start()
    {
        if (player == null || actionButton == null || actionButtonText == null)
        {
            Debug.LogError("❌ TurnManager: UI references (actionButton or actionButtonText) are not set.");
            return;
        }

        stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("❌ TurnManager: No PlayerStats found on player.");
            return;
        }

        actionButton.onClick.AddListener(OnActionButtonClicked);
        stats.OnActionsChanged += _ => RefreshButton();

        stats.ResetActions();
        RefreshButton();
    }

    void RefreshButton()
    {
        if (stats.CurrentActions > 0)
        {
            actionButton.interactable = false;
            actionButtonText.text = $"Moves Left: {stats.CurrentActions}";
        }
        else
        {
            actionButton.interactable = true;
            actionButtonText.text = "End Turn";
        }
    }

    void OnActionButtonClicked()
    {
        if (stats.CurrentActions > 0) return;

        enemyManager.TakeAllTurns();
        stats.ResetActions();
    }
}
