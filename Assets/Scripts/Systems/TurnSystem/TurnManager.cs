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

    void Start()
    {
        // hook up our one click handler
        actionButton.onClick.AddListener(OnActionButtonClicked);

        // init player actions
        player.actionsPerTurn = player.maxActionsPerTurn;
        RefreshButton();
    }

    void Update()
    {
        // keep the button text & state in sync
        RefreshButton();
    }

    void RefreshButton()
    {
        if (player.actionsPerTurn > 0)
        {
            // still have moves — disable clicking
            actionButton.interactable = false;
            actionButtonText.text = $"Moves Left: {player.actionsPerTurn}";
        }
        else
        {
            // no moves left — enable End Turn
            actionButton.interactable = true;
            actionButtonText.text = "End Turn";
        }
    }

    void OnActionButtonClicked()
    {
        // only respond when it's truly "End Turn"
        if (player.actionsPerTurn > 0)
            return;

        Debug.Log("End Turn clicked!");

        // 1) Have all enemies take their turn
        enemyManager.TakeAllTurns();

        // 2) Reset the player's actions
        player.actionsPerTurn = player.maxActionsPerTurn;

        // 3) Immediately refresh UI
        RefreshButton();
    }
}
