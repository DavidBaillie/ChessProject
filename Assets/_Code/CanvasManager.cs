using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

    public GameObject menuCanvas;

    private GameBoardManager boardManager;

    /// <summary>
    /// Called before first frame
    /// </summary>
    private void Awake()
    {
        boardManager = GetComponent<GameBoardManager>();
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
}
