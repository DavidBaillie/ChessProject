using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour {

    private PossiblePositionManager positionsManager;

    private Tile selected;
    private List<Tile> options;

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
            if (positionsManager.isPlayersTurn() == false) return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Tile current = hit.collider.gameObject.GetComponent<Tile>();

                //Case when nothing selected
                if (selected == null)
                {
                    //There is a valid piece to select
                    if (current.getCurrentPiece() != null && current.getCurrentPiece().team == 0)
                    {
                        //Save selection and get valid movement options
                        selected = current;
                        options = positionsManager.getPlayerPossibleTiles
                            (current.getXPosition(), current.getYPosition(), current.getCurrentPiece().type);                     
                    }

                    return;
                }
                
                //Otherwise we can check for movement or de-select
                if (options.Contains(current))
                {
                    positionsManager.moveToTile(selected, current, 0);
                    selected = null;
                    current = null;
                }
                //Otherwise we figure out what to do other than moving a piece
                else
                {
                    //Case where the player clicked on another one of their pieces
                    if (current.getCurrentPiece().team == 0)
                    {
                        //Change selection to new piece
                        selected = current;
                        options = positionsManager.getPlayerPossibleTiles
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
