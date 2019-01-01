using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour {

    private PossiblePositionManager positionsManager;

    private Tile selected;
    private Tile current;
    private List<Tile> options;
    private List<MovementData> returnedMovements;

    /// <summary>
    /// Called at scene start
    /// </summary>
    private void Awake()
    {
        positionsManager = GetComponent<PossiblePositionManager>();
        options = new List<Tile>();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Mouse left click event
        if (Input.GetMouseButtonDown(0))
        {
            //Don't let the player do anything if it's not their turn
            //if (positionsManager.isPlayersTurn() == false) return;            //TODO - Uncomment this

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                current = hit.collider.gameObject.GetComponent<Tile>();
                Debug.Log("__Selected Tile: " + current.getXPosition() + "/" + current.getYPosition());

                //Case when nothing selected
                if (selected == null)
                {
                    //There is a valid piece to select
                    if (current.getCurrentPiece() != null && current.getCurrentPiece().team == Team.Player)
                    {
                        //Save selection and get valid movement options
                        selected = current;
                        returnedMovements = positionsManager.getPlayerPossibleTiles
                            (current.getXPosition(), current.getYPosition(), current.getCurrentPiece().type);
                        Debug.Log("__Getting options for selected Tile: " + returnedMovements.Count);
                    }
                    else Debug.Log("__Selected friendly piece, doing nothing");

                    return;
                }

                //
                if (movementsContainTile(returnedMovements, current))
                {
                    Debug.Log("__Selected valid positon to move to, moving");
                    positionsManager.moveToTile(getMovementToTile(returnedMovements, current));
                    selected = null;
                    current = null;
                }
                //Otherwise we figure out what to do other than moving a piece
                else
                {
                    Debug.Log("__Selected invalid tile with previous selected");
                    //Case where the player clicked on another one of their pieces
                    if (current.getCurrentPiece().team == Team.Player)
                    {
                        Debug.Log("__Selected second friendly piece");
                        //Change selection to new piece
                        selected = current;
                        returnedMovements = positionsManager.getPlayerPossibleTiles
                            (current.getXPosition(), current.getYPosition(), current.getCurrentPiece().type);
                    }
                    //Otherwise clear selection
                    else
                    {
                        Debug.Log("__Selected emtpy/invalid Tile, resetting selection");
                        selected = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns if the tile is contained as a possible end point within the provided MovementData List
    /// </summary>
    /// <param name="movements">Movement Options</param>
    /// <param name="tile">Tile to check for presence</param>
    /// <returns>If a match exists</returns>
    private bool movementsContainTile (List<MovementData> movements, Tile tile)
    {
        foreach (MovementData move in movements)
        {
            if (move.endTile.getXPosition() == tile.getXPosition() && move.endTile.getYPosition() == tile.getYPosition()) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the MovementData associated with the provided Tile
    /// </summary>
    /// <param name="movements">Possible movements</param>
    /// <param name="tile">Tile to find for movement</param>
    /// <returns>MovementData containing the Tile</returns>
    private MovementData getMovementToTile (List<MovementData> movements, Tile tile)
    {
        foreach (MovementData move in movements)
        {
            if (move.endTile.getXPosition() == tile.getXPosition() && move.endTile.getYPosition() == tile.getYPosition()) return move;
        }

        return new MovementData();
    }
}
