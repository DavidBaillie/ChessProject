using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject menuCanvas;
    public GameObject winCanvas;
    public GameObject tieCanvas;

    private GameBoardManager boardManager;

    /// <summary>
    /// Called before first frame
    /// </summary>
    private void Awake()
    {
        boardManager = GetComponent<GameBoardManager>();

        winCanvas.SetActive(false);
        tieCanvas.SetActive(false);
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
    }

    /// <summary>
    /// Called when the game ends with one team winning
    /// </summary>
    /// <param name="team">Winning team</param>
    public void showWinCanvas (Team team)
    {
        winCanvas.SetActive(true);
    }

    /// <summary>
    /// Called when the game ends in a tie
    /// </summary>
    public void showTieCanvas ()
    {
        tieCanvas.SetActive(true);
    }
}
