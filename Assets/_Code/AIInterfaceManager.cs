using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AIInterfaceManager : MonoBehaviour {

    private PossiblePositionManager positionManager;
    private MovementData finalChoice;

    /// <summary>
    /// Main thread that spins up and controls AI with interactions back to main Unity Systems
    /// </summary>
    /// <param name="parent">AIInterfaceManager for Unity Interactions</param>
    private static void main_AI_Thread(AIInterfaceManager parent)
    {
        /*
        //Continue running while game active
        while (true)
        {
            //If it's the AI's turn, spin up an AI and have it decide a turn
            if (parent.positionManager.isPlayersTurn() == false)
            {
                //Spin up AI
                Main m = new Main(parent);

                parent.finishAITurn();
            }
        }
        */
    }

    /// <summary>
    /// Called by the PossiblePositionManager once world generation has completed.
    /// Method handles starting up and running the AI
    /// </summary>
    internal void initialize (PossiblePositionManager positionManager)
    {
        //Save positions manager
        this.positionManager = positionManager;
        //updateBoardStatus();

        //Spin up thread;
        //Thread t = new Thread(() => main_AI_Thread(this));
        //t.IsBackground = true;
        //t.Start();
    }

    /// <summary>
    /// Called by the AI thread when the decision to move a piece has been made.
    /// </summary>
    /// <param name="x1">Piece X Coordinate</param>
    /// <param name="y1">Piece Y Coordinate</param>
    /// <param name="x2">Move to X Coordinate</param>
    /// <param name="y2">Move to Y Coordinate</param>
    internal void finishAITurn ()
    {
        //TODO - Uncomment this to make AI functional again
        //positionManager.moveToTile(finalChoice);
    }

    /// <summary>
    /// Returns the current Tile[,] representing the board the player is seeing. Provided board is
    /// a unique object array separate from the one tracked by the PossiblePositionManager.
    /// </summary>
    internal Tile[,] AC_getCurrentBoard () { return AC_getCopyOfBoard(positionManager.getTileArray()); }

    /// <summary>
    /// Returns a copy of the provided 2D Tile array. Used to make sure a "board" can be passed
    /// between recursive steps without sharing a memory address. 
    /// </summary>
    /// <param name="data">2D Tile array to be copied</param>
    /// <returns>New 2D Tile array with a new memory address.</returns>
    internal Tile[,] AC_getCopyOfBoard (Tile[,] data)
    {
        Tile[,] copy = new Tile[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile t = new Tile();
                t.initialize(data[x, y].getCurrentPiece(), data[x, y].getXPosition(), 
                    data[x, y].getYPosition(), data[x, y].getTileObject());
                copy[x, y] = t;
            }
        }

        return copy;
    }

    /// <summary>
    /// Used by the AI. Provided a version of the game board, the method will return a new
    /// copy of the board with the pieces moved accoridng to the MovementData struct.
    /// </summary>
    /// <param name="data">MovementData holding movement instructions</param>
    /// <param name="arrayIn">Current version game board to be modified</param>
    /// <returns></returns>
    internal Tile[,] AC_getBoardAfterMovement (MovementData data, Tile[,] arrayIn)
    {
        //Build copy of board to prevent overwrite
        Tile[,] boardTileArray = AC_getCopyOfBoard(arrayIn);

        //Move pieces according to parameter data
        Tile start, end, startTwo, endTwo;
        switch (data.movementType)
        {
            //Standard Movement cases
            case StateChange.StandardMovement:
            case StateChange.StandardTaken:
                //Remove piece taken if any and grab local tiles being referenced
                start = boardTileArray[data.startTile.getXPosition(), data.startTile.getYPosition()];
                end = boardTileArray[data.endTile.getXPosition(), data.endTile.getYPosition()];

                //Move the Piece to the new Tile
                end.setCurrentPiece(start.getCurrentPiece());
                start.setCurrentPiece(null);
                break;
            //En Passen movement 
            case StateChange.EnPassen:
                //Grab start and end data
                start = boardTileArray[data.startTile.getXPosition(), data.startTile.getYPosition()];
                end = boardTileArray[data.endTile.getXPosition(), data.endTile.getYPosition()];
                startTwo = boardTileArray[data.secondaryChangeStart.getXPosition(),
                    data.secondaryChangeStart.getYPosition()];

                //Remove other pawn
                startTwo.setCurrentPiece(null);

                //Move my pawn
                end.setCurrentPiece(start.getCurrentPiece());
                start.setCurrentPiece(null);
                break;
            //Castling movement
            case StateChange.Castling:
                //Grab start and end data
                start = boardTileArray[data.startTile.getXPosition(), data.startTile.getYPosition()];
                end = boardTileArray[data.endTile.getXPosition(), data.endTile.getYPosition()];
                startTwo = boardTileArray[data.secondaryChangeStart.getXPosition(),
                    data.secondaryChangeStart.getYPosition()];
                endTwo = boardTileArray[data.secondaryChangeEnd.getXPosition(),
                    data.secondaryChangeEnd.getYPosition()];

                //Move King
                end.setCurrentPiece(start.getCurrentPiece());
                start.setCurrentPiece(null);

                //Move Rook
                endTwo.setCurrentPiece(startTwo.getCurrentPiece());
                startTwo.setCurrentPiece(null);
                break;
        }

        //Return new board with pieces moved
        return boardTileArray;
    }

    /// <summary>
    /// Provided a Tile to move and the current version of the chess board, this 
    /// method will return a List of all Tile's the Piece on the provided Tile can move to.
    /// </summary>
    /// <param name="tile">Tile housing the Piece to move</param>
    /// <param name="inputArray">2D Tile array of the current board configuration</param>
    /// <returns>List<Tile> of all Tiles the piece can move to</Tile></returns>
    internal List<MovementData> AC_getMovementOptions (Tile tile, Tile[,] inputArray)
    {
        //Get possible movement options
        List<MovementData> options;
        if (tile.getCurrentPiece().team == Team.Player)
        {
            options = positionManager.getPlayerPossibleTiles(tile, inputArray);
        }
        else
        {
            options = positionManager.getAIPossibleTiles(tile, inputArray);
        }

        return options;
    }

    /// <summary>
    /// Returns the score of a board provided a version of the board
    /// </summary>
    /// <param name="inputArray">2D Tile array to score</param>
    /// <returns>Integer score of board</returns>
    internal int AC_getScoreOfBoard (Tile[,] inputArray)
    {
        return 0;
    }

    /// <summary>
    /// Called by the AI when a choice of movement has been made
    /// </summary>
    /// <param name="data">MovementData struct holding choice</param>
    internal void AC_submitChoice (MovementData data)
    {
        finalChoice = data;
    }
}

/*
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
*/  //Old Code