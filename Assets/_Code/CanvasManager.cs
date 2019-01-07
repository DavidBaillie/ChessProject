using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    public GameObject menuCanvas;
    public GameObject winCanvas;
    public Text winText;
    public GameObject tieCanvas;
    public GameObject pawnPromotionCanvas;
    public GameObject customCeationCanvas;

    private GameBoardManager boardManager;

    /// <summary>
    /// Called before first frame
    /// </summary>
    private void Awake()
    {
        boardManager = GetComponent<GameBoardManager>();

        menuCanvas.SetActive(true);
        winCanvas.SetActive(false);
        tieCanvas.SetActive(false);
        pawnPromotionCanvas.SetActive(false);
        customCeationCanvas.SetActive(false);
    }

    /// <summary>
    /// Called when user presses "Standard Game" Button
    /// </summary>
    public void standardGame ()
    {
        Debug.Log("Standard");
        menuCanvas.SetActive(false);
        boardManager.createStandardGame();
    }

    /// <summary>
    /// Called when user presses "Custom Scenario" Button
    /// </summary>
    public void customScenario ()
    {
        Debug.Log("Custom");
        boardManager.createCustomGame();
        menuCanvas.SetActive(false);
        customCeationCanvas.SetActive(true);
    }

    /// <summary>
    /// Called when the game ends with one team winning
    /// </summary>
    /// <param name="team">Winning team</param>
    public void showWinCanvas (Team team)
    {
        winCanvas.SetActive(true);

        if (team == Team.AI)
        {
            winText.text = "You Lose!";
        }
    }

    /// <summary>
    /// Called when the game ends in a tie
    /// </summary>
    public void showTieCanvas ()
    {
        tieCanvas.SetActive(true);
    }

    /// <summary>
    /// Toggles if the Pawn Promotion Display is active
    /// </summary>
    /// <param name="activity">New state for toggle</param>
    internal void togglePawnPromotionDisplay(bool activity)
    {
        pawnPromotionCanvas.SetActive(activity);
    }
}
