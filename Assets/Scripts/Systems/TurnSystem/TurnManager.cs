// TurnManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [Header("Actors")]
    public PlayerController player;
    public EnemyManager enemyManager;

    [Header("UI")]
    [Tooltip("The single button that shows moves or ends the turn")]
    public Button actionButton;
    [Tooltip("The TextMeshPro label inside the button")]
    public TMP_Text actionButtonText;

    private PlayerStats stats;

    void Start()
    {
        stats = player.GetComponent<PlayerStats>();

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
