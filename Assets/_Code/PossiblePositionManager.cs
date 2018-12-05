using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossiblePositionManager : MonoBehaviour {

    private bool playersTurn;           //Tracks if it is the AI or players turn
    private Tile[,] tileDataArray;      //Array of Tile classes representing game board

    private Tile currentSelectedTile;   //Data saved when the player selects a Piece/Tile to move to a new Tile
    private List<Tile> possibleTiles;   //Possible tiles to move towards based on the currentSelectedTile

    /// <summary>
    /// Called when the BoardPositionInitializer finishes spawning all tiles with pieces
    /// </summary>
    /// <param name="data">Array of all tiles on board</param>
    internal void  setTileDataArray (Tile[,] data)
    {
        tileDataArray = data;
    }

    /// <summary>
    /// Returns if it is the players turn currently
    /// </summary>
    /// <returns></returns>
    internal bool isPlayersTurn () { return playersTurn; }

    /// <summary>
    /// Called by the PlayerControlManager when the player clicks on a tile to select a piece to move.
    /// </summary>
    /// <param name="selected">Tile class attached to the GameObject clicked on</param>
    /// <returns>Boolean indicating result of selection</returns>
    internal bool playerSelectedTile (Tile selected)
    {
        //Do nothing on AI turn
        if (playersTurn == false) return false;
        
        //If the player has no previous selection
        if (currentSelectedTile == null)
        {
            //New tile selection has no piece, there's nothing to save
            if (selected.getCurrentPiece() == null) return false;
            //No selecting the AI pieces
            if (selected.getCurrentPiece().team == 1) return false;

            //Save the tile as our starting point and register possible movement options
            currentSelectedTile = selected;
            possibleTiles = getPossibleTileOptions(currentSelectedTile);
        }
        //Otherwise they're looking to move the selected piece
        else
        {
            //If the player clicked on an allowable tile to move to 
            if (possibleTiles.Contains(selected))
            {
                //Move the piece to the new tile
                moveToTile(currentSelectedTile, selected);
                playersTurn = false;
                currentSelectedTile = null;
                possibleTiles = null;
                return true;
            }
            //Otherwise it was a non-moveable tile
            else
            {
                //Player clicked on an open tile or an AI piece, reset selection
                if (selected.getCurrentPiece() == null || selected.getCurrentPiece().team == 1)
                {
                    currentSelectedTile = null;
                    possibleTiles = null;
                    return false;
                }
                //If the player clicked on another of their own pieces, switch selection to that piece
                if (selected.getCurrentPiece().team == 0)
                {
                    currentSelectedTile = selected;
                    possibleTiles = getPossibleTileOptions(currentSelectedTile);
                    return false;
                }
            }
        }

        //Default return for some weird edge case I haven't thought of
        return false;
    }

    /// <summary>
    /// Called to instruct a a Piece to move from one Tile to another
    /// </summary>
    /// <param name="start">Tile the Piece is starting on</param>
    /// <param name="end">Tile for the Piece to move to</param>
    private void moveToTile (Tile start, Tile end)
    {
        start.getCurrentPiece().targetPosition = end.gameObject.transform.position + Vector3.up;
        end.setCurrentPiece(start.getCurrentPiece());
        start.setCurrentPiece(null);
    }

    /// <summary>
    /// Given a tile the method will return all possible tiles the associated Piece can move to
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    internal List<Tile> getPossibleTileOptions (Tile tile)
    {
        //Return the math for player movements
        if (tile.getCurrentPiece().team == 0)
            return getPlayerPossibleTiles(tile.getXPosition(), tile.getYPosition(), tile.getCurrentPiece().type);
        //Otherwise do the math for AI movement
        else
            return getAIPossibleTiles(tile.getXPosition(), tile.getYPosition(), tile.getCurrentPiece().type);
    }

    /// <summary>
    /// Returns the possible Tiles the provided Piece can move to.
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Type of Piece on Tile</param>
    /// <returns></returns>
    internal List<Tile> getPlayerPossibleTiles (int x, int y, PieceTypes type)
    {
        List<Tile> options = new List<Tile>();

        //Possible moves for a pawn
        if (type == PieceTypes.Pawn)
        {
            //Pawn still at starting position
            if (x == 1)
            {
                if (tileDataArray[2,y].getCurrentPiece() == null)
                    options.Add(tileDataArray[2, y]);
                if (tileDataArray[2, y].getCurrentPiece() == null && tileDataArray[3, y].getCurrentPiece() == null)
                    options.Add(tileDataArray[3, y]);
            }
            //Pawn is out on the board, check for forwards movement
            else
            {
                if (tileDataArray[x + 1, y].getCurrentPiece() == null)
                    options.Add(tileDataArray[x + 1, y]);
            }

            //Check for side motion
            if (x < 7)
            {
                //Check if we can attack on the right
                if (y < 7 && tileDataArray[x+1,y+1].getCurrentPiece() != null && tileDataArray[x+1,y+1].getCurrentPiece().team == 1)
                    options.Add(tileDataArray[x + 1, y + 1]);
                //Check if we can attack on the left
                if (y > 0 && tileDataArray[x + 1, y - 1].getCurrentPiece() != null && tileDataArray[x + 1, y - 1].getCurrentPiece().team == 1)
                    options.Add(tileDataArray[x + 1, y - 1]);
            }
        }
        //Possible moves for Rook
        else if (type == PieceTypes.Rook)
        {

        }
    }

    /// <summary>
    /// Returns the possible Tiles the provided Tile Piece can move to.
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Piece Type on Tile</param>
    /// <returns></returns>
    internal List<Tile> getAIPossibleTiles (int x, int y, PieceTypes type)
    {

    }
}
