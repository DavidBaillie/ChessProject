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

                        //Parse movement data return into Tile options
                        options = new List<Tile>();
                        for (int i = 0; i < returnedMovements.Count; i++)
                        {
                            options.Add(returnedMovements[i].endTile);
                        }
                    }

                    return;
                }
                
                //Otherwise we can check for movement or de-select
                if (options.Contains(current))
                {
                    positionsManager.moveToTile(returnedMovements[options.IndexOf(selected)]);
                    selected = null;
                    current = null;
                    options = null;
                    returnedMovements = null;
                }
                //Otherwise we figure out what to do other than moving a piece
                else
                {
                    //Case where the player clicked on another one of their pieces
                    if (current.getCurrentPiece().team == Team.Player)
                    {
                        //Change selection to new piece
                        selected = current;
                        returnedMovements = positionsManager.getPlayerPossibleTiles
                            (current.getXPosition(), current.getYPosition(), current.getCurrentPiece().type);
                    }
                    //Otherwise clear selection
                    else
                    {
                        selected = null;
                    }
                }
            }
        }
    }
}
