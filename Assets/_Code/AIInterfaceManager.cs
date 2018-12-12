using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AIInterfaceManager : MonoBehaviour {

    private PossiblePositionManager positionManager;
    private TileData[,] cleanedBoardArray;

    /// <summary>
    /// Main thread that spins up and controls AI with interactions back to main Unity Systems
    /// </summary>
    /// <param name="parent">AIInterfaceManager for Unity Interactions</param>
    private static void main_AI_Thread(AIInterfaceManager parent)
    {
        //Continue running while game active
        while (true)
        {
            //If it's the AI's turn, spin up an AI and have it decide a turn
            if (parent.positionManager.isPlayersTurn() == false)
            {
                //Spin up AI
                parent.updateBoardStatus();
                Main m = new Main(parent);

                //Respond with choice of piece to move
                parent.finishAITurn(m.getFinalData());
            }
        }
    }

    /// <summary>
    /// Called by the PossiblePositionManager once world generation has completed.
    /// Method handles starting up and running the AI
    /// </summary>
    internal void initialize (PossiblePositionManager positionManager)
    {
        //Save positions manager
        this.positionManager = positionManager;
        updateBoardStatus();

        //Spin up thread;
        Thread t = new Thread(() => main_AI_Thread(this));
        t.IsBackground = true;
        t.Start();
    }

    /// <summary>
    /// Called by the AI thread when the decision to move a piece has been made.
    /// </summary>
    /// <param name="x1">Piece X Coordinate</param>
    /// <param name="y1">Piece Y Coordinate</param>
    /// <param name="x2">Move to X Coordinate</param>
    /// <param name="y2">Move to Y Coordinate</param>
    internal void finishAITurn (FinalData finalData)
    {
        positionManager.setIsPlayerTurn(true);

        positionManager.moveToTile(
            positionManager.getTileArray()[finalData.startXPosition, finalData.startYPosition],
            positionManager.getTileArray()[finalData.endXPosition, finalData.endYPosition], 
            1);
    }

    /// <summary>
    /// Called to set up a local copy of the game board. Assumed to be called 
    /// before the start of the AI run to make sure board data is current.
    /// </summary>
    internal void updateBoardStatus ()
    {
        cleanedBoardArray = new TileData[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //Grab full tile data
                Tile t = positionManager.getTileArray()[x, y];

                //Parse data for empty tile
                if (t.getCurrentPiece() == null)
                    cleanedBoardArray[x, y] = new TileData(t.getXPosition(), t.getYPosition());
                //Otherwise we have a piece on this tile
                else
                    cleanedBoardArray[x, y] = new TileData(t.getXPosition(), t.getYPosition(), t.getCurrentPiece().type,
                        t.getCurrentPiece().team);
            }
        }
    }

    /// <summary>
    /// Returns an array of TileData representing the current board in the game world.
    /// </summary>
    internal TileData[,] getCurrentBoard () { return cleanedBoardArray; }

    /// <summary>
    /// Returns a copy of the provided TileData array to prevent accidental pointer links when recursing.
    /// </summary>
    /// <param name="data">Array to be copied</param>
    /// <returns>2D TileData array</returns>
    internal TileData[,] getCopyOfBoard (TileData[,] data)
    {
        TileData[,] copy = new TileData[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                copy[x, y] = new TileData(data[x, y].xCoordinate, data[x, y].yCoordinate, data[x, y].type, t.getCurrentPiece().team);
            }
        }

        return copy;
    }

    /// <summary>
    /// Called to return all possible TileData positions a given unit on a TileData can move to.
    /// Hackily converts from TileData objects to Tile's and then back again to re-use previous code for
    /// determine available movement options for a piece.
    /// </summary>
    /// <param name="xCoordinate">X Coordinate of TileData</param>
    /// <param name="yCoordinate">Y Coordinate of TileData</param>
    /// <param name="inputArray">Array of TileData to be used as the current board</param>
    /// <returns></returns>
    internal List<TileData> getMovementOptions (int xCoordinate, int yCoordinate, TileData[,] inputArray)
    {
        //Convert back to Tile array
        Tile[,] convertedArray = new Tile[8, 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile t = new Tile();
                t.localLocationData = new BoardLocation(null, x, y);
                t.currentPiece = new Piece();
                t.currentPiece.type = inputArray[x, y].type;
                t.currentPiece.team = inputArray[x, y].team;

            }
        }

        //Get possible movement options
        List<Tile> options;
        if (inputArray[xCoordinate, yCoordinate].team == 0)
        {
            options = positionManager.getPlayerPossibleTiles(xCoordinate, yCoordinate, inputArray[xCoordinate, yCoordinate].type, convertedArray);
        }
        else
        {
            options = positionManager.getAIPossibleTiles(xCoordinate, yCoordinate, inputArray[xCoordinate, yCoordinate].type, convertedArray);
        }

        //Add returned Tile's to List for AI to read - convert back to TileData objects
        List<TileData> availableTiles = new List<TileData>();
        foreach (Tile t in options)
        {
            availableTiles.Add(inputArray[t.getXPosition(), t.getYPosition()]);
        }

        return availableTiles;
    }
}

/// <summary>
/// Custom class dedicated to AI class interactions with game board
/// </summary>
internal class TileData
{
    internal int xCoordinate;
    internal int yCoordinate;
    internal PieceTypes type;
    internal int team;

    /// <summary>
    /// Full Constructor
    /// </summary>
    internal TileData (int x, int y, PieceTypes type, int team)
    {
        xCoordinate = x;
        yCoordinate = y;
        this.type = type;
        this.team = team;
    }

    /// <summary>
    /// Partial Constructor
    /// </summary>
    internal TileData (int x, int y)
    {
        xCoordinate = x;
        yCoordinate = y;
        type = PieceTypes.None;
    }
}