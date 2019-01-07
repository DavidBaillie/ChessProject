using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour {

    private PossiblePositionManager positionsManager;
    private CanvasManager canvasManager;

    private WorldTile selected;
    private WorldTile current;
    private List<MovementData> returnedMovements;

    /// <summary>
    /// Called at scene start
    /// </summary>
    private void Awake()
    {
        positionsManager = GetComponent<PossiblePositionManager>();
        canvasManager = GetComponent<CanvasManager>();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Mouse left click event
        if (Input.GetMouseButtonDown(0) && positionsManager.isPlayersTurn())
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                registerPlayerAction(hit.collider.gameObject.GetComponent<WorldTile>());
            }
        }

        //Check for pawn promotion state
        if (selected != null && selected.currentPiece.type == PieceTypes.Pawn && selected.x == 7)
        {
            canvasManager.togglePawnPromotionDisplay(true);
        }
        else
        {
            canvasManager.togglePawnPromotionDisplay(false);
        }
    }

    /// <summary>
    /// If there is valid user input we take their selection and determine an action
    /// </summary>
    /// <param name="hit">RaycastHit of selection</param>
    private void registerPlayerAction (WorldTile c)
    {
        current = c;
		if (current == null) { Debug.Log("Selected null WorldTile"); return; }

        //Case where nothing is currently selected
        if (selected == null)
        {
            //If the player has selected one of thier own pieces
            if (current.currentPiece != null && current.currentPiece.team == Team.Player)
            {
                Debug.Log("Player is making a new piece selection --> " + current.currentPiece.type.ToString());
                //Save selection and get valid movement options
                selected = current;
                returnedMovements = positionsManager.getPlayerPossibleTiles(current.x, current.y, current.currentPiece.type);
            }
            
            return;
        }

        //Case where there is already a Piece selected and a valid movement was chosen
        if (movementsContainTile(returnedMovements, current))
        {
            Debug.Log("Player made a valid movement choice, moving piece");
            positionsManager.moveToTile(getMovementToTile(returnedMovements, current));
            selected = null;
            current = null;
        }
        //Otherwise we received an invalid movement and must decide another action
        else
        {
            //Case where the player clicked on another one of their own pieces
            if (current.currentPiece.team == Team.Player)
            {
                Debug.Log("Player has changed their selection --> " + current.currentPiece.type.ToString());
                //Change selection to new piece
                selected = current;
                returnedMovements = positionsManager.getPlayerPossibleTiles(current.x, current.y, current.currentPiece.type);
            }
            //Otherwise clear selection
            else
            {
                Debug.Log("Player selected an invalid tile, resetting selection");
                selected = null;
            }
        }
    }

    /// <summary>
    /// Called by the pawnPromotionCanvas buttons when the player has chosen to promote a pawn
    /// </summary>
    /// <param name="newType">New PieceType the pawn will become</param>
    public void submitPromotion (int n)
    {
        PieceTypes newType = PieceTypes.Queen;

        //Determine what the selected piece is
        switch (n)
        {
            case 0:
                newType = PieceTypes.Rook;
                break;
            case 1:
                newType = PieceTypes.Knight;
                break;
            case 2:
                newType = PieceTypes.Bishop;
                break;
            case 3:
                newType = PieceTypes.Queen;
                break;
        }

        int y = selected.y;

        selected = null;
        current = null;

        //Apply the promotion
        positionsManager.moveToTile(new MovementData(
            new Tile(new Piece(PieceTypes.Pawn, Team.Player), 7, y), true, new Piece(newType, Team.Player)));
    }

    /// <summary>
    /// Returns if the tile is contained as a possible end point within the provided MovementData List
    /// </summary>
    /// <param name="movements">Movement Options</param>
    /// <param name="tile">Tile to check for presence</param>
    /// <returns>If a match exists</returns>
    private bool movementsContainTile (List<MovementData> movements, WorldTile tile)
    {
        foreach (MovementData move in movements)
        {
            if (move.endTile.x == tile.x && move.endTile.y == tile.y) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the MovementData associated with the provided Tile
    /// </summary>
    /// <param name="movements">Possible movements</param>
    /// <param name="tile">Tile to find for movement</param>
    /// <returns>MovementData containing the Tile</returns>
    private MovementData getMovementToTile (List<MovementData> movements, WorldTile tile)
    {
        foreach (MovementData move in movements)
        {
            if (move.endTile.x == tile.x && move.endTile.y == tile.y) return move;
        }

        return new MovementData();
    }
}
