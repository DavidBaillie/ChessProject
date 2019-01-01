using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossiblePositionManager : MonoBehaviour {

    private AIInterfaceManager AI_Interface;

    private bool playersTurn = true;            //Tracks if it is the AI or players turn
    private Tile[,] boardTileArray;            //Array of Tile classes representing game board

    private Tile currentSelectedTile;           //Data saved when the player selects a Piece/Tile to move to a new Tile
    private List<Tile> possibleTiles;           //Possible tiles to move towards based on the currentSelectedTile

    private MovementData lastMove;              //Previous move made before current turn

    /// <summary>
    /// Called when the BoardPositionInitializer finishes spawning all tiles with pieces
    /// </summary>
    /// <param name="data">Array of all tiles on board</param>
    internal void  setTileDataArray (Tile[,] data)
    {
        boardTileArray = data;


        AI_Interface = GetComponent<AIInterfaceManager>();
        AI_Interface.initialize(this);
    }

    /// <summary>
    /// Called to instruct a a Piece to move from one Tile to another
    /// </summary>
    /// <param name="start">Tile the Piece is starting on</param>
    /// <param name="end">Tile for the Piece to move to</param>
    internal void moveToTile(MovementData data)
    {
        //Update who's turn it is
        if (data.startTile.getCurrentPiece().team == Team.Player) playersTurn = false;
        else playersTurn = true;

        lastMove = data;

        //Move pieces according to parameter data
        Tile start, end, startTwo, endTwo;
        switch (data.movementType)
        {
            //Standard Movement cases
            case StateChange.StandardMovement:
            case StateChange.StandardTaken:
                //Remove piece taken if any and grab local tiles being referenced
                removePiece(data.endTile);
                start = boardTileArray[data.startTile.getXPosition(), data.startTile.getYPosition()];
                end = boardTileArray[data.endTile.getXPosition(), data.endTile.getYPosition()];

                //Move the Piece to the new Tile
                start.getCurrentPiece().targetPosition = end.gameObject.transform.position + (Vector3.up / 2);
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
                removePiece(startTwo);

                //Move my pawn
                start.getCurrentPiece().targetPosition = end.gameObject.transform.position + (Vector3.up / 2);
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
                start.getCurrentPiece().targetPosition = end.gameObject.transform.position + (Vector3.up / 2);
                end.setCurrentPiece(start.getCurrentPiece());
                start.setCurrentPiece(null);

                //Move Rook
                startTwo.getCurrentPiece().targetPosition =
                    endTwo.gameObject.transform.position + (Vector3.up / 2);
                endTwo.setCurrentPiece(startTwo.getCurrentPiece());
                startTwo.setCurrentPiece(null);
                break;
        }
    }

    /// <summary>
    /// Called when a Piece beeds to be removed from the board after being "taken"
    /// </summary>
    /// <param name="tile">Tile holding Piece class to remove</param>
    private void removePiece (Tile tile)
    {
        if (tile.getCurrentPiece() == null) return;

        //TODO - Move piece off board instead of killing it !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Destroy(tile.getCurrentPiece().gameObject, 0.5f);
        tile.setCurrentPiece(null);
    }

    #region Player Turn
    /// <summary>
    /// Returns if it is the players turn currently
    /// </summary>
    /// <returns></returns>
    internal bool isPlayersTurn () { return playersTurn; }

    /// <summary>
    /// Called to set the playersTurn boolean
    /// </summary>
    /// <param name="turn">Boolean to set playersTurn to</param>
    internal void setIsPlayerTurn (bool turn) { playersTurn = turn; }
    #endregion

    #region Internal Getter Methods
    /// <summary>
    /// Called to get the current array of tiles on the board
    /// </summary>
    /// <returns></returns>
    internal Tile[,] getTileArray() { return boardTileArray; }

    /// <summary>
    /// Returns the possible Tiles the provided Piece can move to using the current world Tiles
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Type of Piece on Tile</param>
    /// <returns></returns>
    internal List<MovementData> getPlayerPossibleTiles(int x, int y, PieceTypes type)
    {
        switch (type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(boardTileArray[x,y], boardTileArray);
            case PieceTypes.Rook:
                return getRookTiles(boardTileArray[x, y], boardTileArray);
            case PieceTypes.Knight:
                return getKnightTiles(boardTileArray[x, y], boardTileArray);
            case PieceTypes.Bishop:
                return getBishopTiles(boardTileArray[x, y], boardTileArray);
            case PieceTypes.Queen:
                return getQueenTiles(boardTileArray[x, y], boardTileArray);
            case PieceTypes.King:
                return getKingTiles(boardTileArray[x, y], boardTileArray);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + type + " :: " + gameObject.name);
                return new List<MovementData>();
        }
    }

    /// <summary>
    /// Returns the possible Tiles the provided Piece can move to using a provided tile array
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Type of Piece on Tile</param>
    /// <returns></returns>
    internal List<MovementData> getPlayerPossibleTiles (Tile tile, Tile[,] currentTiles)
    {
        switch (tile.getCurrentPiece().type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(tile, currentTiles);
            case PieceTypes.Rook:
                return getRookTiles(tile, currentTiles);
            case PieceTypes.Knight:
                return getKnightTiles(tile, currentTiles);
            case PieceTypes.Bishop:
                return getBishopTiles(tile, currentTiles);
            case PieceTypes.Queen:
                return getQueenTiles(tile, currentTiles);
            case PieceTypes.King:
                return getKingTiles(tile, currentTiles);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + tile.getCurrentPiece().type + " :: " + gameObject.name);
                return new List<MovementData>();
        }
    }

    /// <summary>
    /// Returns the possible Tiles the provided Tile Piece can move to.
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Piece Type on Tile</param>
    /// <returns></returns>
    internal List<MovementData> getAIPossibleTiles (Tile tile, Tile[,] currentTiles)
    {
        switch (tile.getCurrentPiece().type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(tile, currentTiles);
            case PieceTypes.Rook:
                return getRookTiles(tile, currentTiles);
            case PieceTypes.Knight:
                return getKnightTiles(tile, currentTiles);
            case PieceTypes.Bishop:
                return getBishopTiles(tile, currentTiles);
            case PieceTypes.Queen:
                return getQueenTiles(tile, currentTiles);
            case PieceTypes.King:
                return getKingTiles(tile, currentTiles);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + tile.getCurrentPiece().type + " :: " + gameObject.name);
                return new List<MovementData>();
        }
    }
    #endregion

    #region Get Piece Options
    /// <summary>
    /// Given the pawn's team, this method will find all valid movement positions for the piece
    /// </summary>
    /// <param name="x">X Coordinate of Tile</param>
    /// <param name="y">Y Coordinate of Tile</param>
    /// <param name="team">Piece Team</param>
    /// <returns>List of possible Tiles to move to</returns>
    private List<MovementData> getPawnTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        Team team = tile.getCurrentPiece().team;
        int x = tile.getXPosition();
        int y = tile.getYPosition();


        //Player pieces
        if (team == Team.Player)
        {
            //Pawn still at starting position
            if (x == 1)
            {
                if (tileDataArray[1, y].getCurrentPiece() == null) Debug.Log("Started Null");
                else Debug.Log("Started with " + tileDataArray[1,y].getCurrentPiece().type.ToString());
                if (tileDataArray[2, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[2, y], tileDataArray), team) == false)
                    {
                        if (tileDataArray[1, y].getCurrentPiece() == null) Debug.Log("Ended Null");
                        else Debug.Log("Ended with " + tileDataArray[1, y].getCurrentPiece().type.ToString());
                        options.Add(new MovementData(tile, tileDataArray[2, y], StateChange.StandardMovement));
                    }
                if (tileDataArray[2, y].getCurrentPiece() == null && tileDataArray[3, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[3, y], tileDataArray), team) == false)
                    {
                        options.Add(new MovementData(tile, tileDataArray[3, y], StateChange.StandardMovement));
                    }
            }
            //Pawn is out on the board, check for forwards movement
            else
            {
                if (tileDataArray[x + 1, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x + 1, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardMovement));
            }
            

                //Check for side motion
            if (x < 7)
            {
                //Check if we can attack on the right
                if (y < 7 && tileDataArray[x + 1, y + 1].getCurrentPiece() != null && tileDataArray[x + 1, y + 1].getCurrentPiece().team == Team.AI)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x + 1, y + 1], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardTaken));
                //Check if we can attack on the left
                if (y > 0 && tileDataArray[x + 1, y - 1].getCurrentPiece() != null && tileDataArray[x + 1, y - 1].getCurrentPiece().team == Team.AI)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x + 1, y - 1], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardTaken)); ;
            }

            //Check for en passen
            if (pawnDoubleMoveLastTurn()                                                    //Last move enemy pawn double moved from start
                && tile.getXPosition() == lastMove.endTile.getXPosition()                   //Make sure pawns are on the same row
                && Mathf.Abs(tile.getYPosition() - lastMove.endTile.getYPosition()) == 1)    //Make sure pawns are beside each other
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[tile.getXPosition() + 1, lastMove.endTile.getYPosition()], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, 
                        tileDataArray[tile.getXPosition() + 1, lastMove.endTile.getYPosition()],
                        StateChange.EnPassen, 
                        tileDataArray[lastMove.endTile.getXPosition(), lastMove.endTile.getYPosition()]));
            }
        }
        //Moving AI pieces
        else
        {
            //Pawn at starting position
            if (x == 6)
            {
                if (tileDataArray[5, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[5, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[5, y], StateChange.StandardMovement));
                if (tileDataArray[5, y].getCurrentPiece() == null && tileDataArray[4, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[4, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[4, y], StateChange.StandardMovement));
            }
            //Otherwise pawn out on board
            else
            {
                if (tileDataArray[x - 1, y].getCurrentPiece() == null)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x - 1, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardMovement));
            }

            //Check for side motion
            if (x > 0)
            {
                //Check if we can attack on the left
                if (y < 7 && tileDataArray[x - 1, y + 1].getCurrentPiece() != null && tileDataArray[x - 1, y + 1].getCurrentPiece().team == 0)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x - 1, y + 1], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardTaken));
                //Check if we can attack on the right
                if (y > 0 && tileDataArray[x - 1, y - 1].getCurrentPiece() != null && tileDataArray[x - 1, y - 1].getCurrentPiece().team == 0)
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x - 1, y - 1], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardTaken));
            }

            //Check for en passen
            if (pawnDoubleMoveLastTurn()
                && tile.getXPosition() == lastMove.endTile.getXPosition()
                && Mathf.Abs(tile.getYPosition() - lastMove.endTile.getYPosition()) == 1)
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[tile.getXPosition() - 1, lastMove.endTile.getYPosition()], tileDataArray), team) == false)
                    options.Add(new MovementData(tile,
                        tileDataArray[tile.getXPosition() - 1, lastMove.endTile.getYPosition()],
                        StateChange.EnPassen,
                        tileDataArray[lastMove.endTile.getXPosition(), lastMove.endTile.getYPosition()]));
            }
        }
        foreach (MovementData data in options) printMovementData(data);
        return options;
    }

    internal void printMovementData (MovementData data)
    {
        string output = "";

        output += "Printing MovementData: \n";
        output += "Starting Tile: " + data.startTile.getXPosition() + "/" + data.startTile.getYPosition();
        if (data.startTile.getCurrentPiece() == null) output += "(Null) ";
        else output += "(" + data.startTile.getCurrentPiece().type.ToString() + ") ";
        output += "  -->  ";
        output += "End Tile: " + data.endTile.getXPosition() + "/" + data.endTile.getYPosition();
        if (data.endTile.getCurrentPiece() == null) output += "(Null) ";
        else output += "(" + data.endTile.getCurrentPiece().type.ToString() + ") ";
        output += "\n Movement Type: " + data.movementType.ToString();

        Debug.Log(output);
    }

    /// <summary>
    /// Given a rook's team and position, this method returns all possible Tiles the piece can move to
    /// </summary>
    /// <param name="x">X Coordinate of Rook</param>
    /// <param name="y">Y Coordinate of Rook</param>
    /// <param name="team">Team of Rook</param>
    /// <returns>List of possible Tiles to move to</returns>
    private List<MovementData> getRookTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Iterate along positive X
        for (int i = x + 1; i < 8; i++)
        {
            //If the next in order is empty
            if (tileDataArray[i, y].getCurrentPiece() == null)
            {
                //Add it
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, y], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along negative X
        for (int i = x - 1; i >= 0; i--)
        {            
            //If the next in order is empty
            if (tileDataArray[i, y].getCurrentPiece() == null)
            {
                //Add it
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, y], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, y], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along positive Y
        for (int i = y + 1; i < 8; i++)
        {
            //If the next in order is empty
            if (tileDataArray[x, i].getCurrentPiece() == null)
            {
                //Add it
                if (kingIsInCheck(movePiece(tile, tileDataArray[x, i], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x, i], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along negative Y
        for (int i = y - 1; i >= 0; i--)
        {
            //If the next in order is empty
            if (tileDataArray[x, i].getCurrentPiece() == null)
            {
                //Add it
                if (kingIsInCheck(movePiece(tile, tileDataArray[x, i], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    if (kingIsInCheck(movePiece(tile, tileDataArray[x, i], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        return options;
    }

    /// <summary>
    /// Give a Knight's position the method will return a List of all Tiles the knight can move to.
    /// </summary>
    /// <param name="x">X Coordinate of Knight</param>
    /// <param name="y">Y Coordinate of Knight</param>
    /// <param name="team">Team of Knight</param>
    /// <returns>List of Tiles</returns>
    private List<MovementData> getKnightTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        Tile t;

        //Check knight position 2/1
        t = positionHelper(x + 2, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x + 2, y + 1], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x + 2, y + 1], StateChange.StandardTaken));
        //Check knight position 2/-1
        t = positionHelper(x + 2, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x + 2, y - 1], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x + 2, y - 1], StateChange.StandardTaken));

        //Check knight position 1/2
        t = positionHelper(x + 1, y + 2, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x + 1, y + 2], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x + 1, y + 2], StateChange.StandardTaken));
        //Check knight position -1/2
        t = positionHelper(x - 1, y + 2, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x - 1, y + 2], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x - 1, y + 2], StateChange.StandardTaken));

        //Check knight position -2/1
        t = positionHelper(x - 2, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x - 2, y + 1], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x - 2, y + 1], StateChange.StandardTaken));
        //Check knight position -2/-1
        t = positionHelper(x -2, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x - 2, y - 1], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x - 2, y - 1], StateChange.StandardTaken));

        //Check knight position 1/-2
        t = positionHelper(x + 1, y - 2, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x + 2, y - 2], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x + 1, y - 2], StateChange.StandardTaken));
        //Check knight position -1/-2
        t = positionHelper(x - 1, y - 2, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, tileDataArray[x - 1, y - 2], tileDataArray), team) == false)
                options.Add(new MovementData(tile, tileDataArray[x - 1, y - 2], StateChange.StandardTaken));

        return options;
    }

    /// <summary>
    /// Given a Bishops position the method will return a list of all Tiles the Bishop can move to
    /// </summary>
    /// <param name="x">X Coordinate of Bishop</param>
    /// <param name="y">Y Coordinate of Bishop</param>
    /// <param name="team">Team of Bishop</param>
    /// <returns>List of Tiles the Bishop can move to</returns>
    private List<MovementData> getBishopTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Check x and y in +/+ direction
        for (int i = x + 1, j = y + 1; i < 8 && j < 8; i++, j++)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in -/- direction
        for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in +/- direction
        for (int i = x + 1, j = y - 1; i < 8 && j >= 0; i++, j--)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in -/+ direction
        for (int i = x - 1, j = y + 1; i >= 0 && j < 8; i--, j++)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    if (kingIsInCheck(movePiece(tile, tileDataArray[i, j], tileDataArray), team) == false)
                        options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        return options;
    }

    /// <summary>
    /// Given a Queen's position the method will return a list of all Tiles the piece can move to.
    /// </summary>
    /// <param name="x">X Coordinate of Queen</param>
    /// <param name="y">Y Coordinate of Queen</param>
    /// <param name="team">Team of Queen</param>
    /// <returns>List of Tile</returns>
    private List<MovementData> getQueenTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        //Get rook movement tiles
        options = getRookTiles(tile, tileDataArray);
        //Add bishop movement tiles
        options.AddRange(getBishopTiles(tile, tileDataArray));

        return options;
    }

    /// <summary>
    /// Given a King's position the method will return a list of all tiles the King can move to
    /// </summary>
    /// <param name="x">X Coordinate of King</param>
    /// <param name="y">Y Coordinate of King</param>
    /// <param name="team">Team of King</param>
    /// <returns>List of Tiles</returns>
    private List<MovementData> getKingTiles (Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        Tile t;

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Check position 1/1
        t = positionHelper(x + 1, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardTaken));
            }

        //Check position 0/1
        t = positionHelper(x, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x, y + 1], StateChange.StandardTaken));
            }

        //Check position 1/0
        t = positionHelper(x + 1, y, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardTaken));
            }

        //Check position -1/1
        t = positionHelper(x - 1, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardTaken));
            }

        //Check position 1/-1
        t = positionHelper(x + 1, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardTaken));
            }

        //Check position -1/-1
        t = positionHelper(x - 1, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardTaken));
            }

        //Check position -1/0
        t = positionHelper(x - 1, y, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardTaken));
            }

        //Check position 0/-1
        t = positionHelper(x, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x, y - 1], StateChange.StandardTaken));
            }

        return options;
    }


    /// <summary>
    /// Simple position check helper. Makes sure Tile is valid and within range.
    /// </summary>
    /// <param name="x">X Coordinate of Piece</param>
    /// <param name="y">Y Coordinate of Piece</param>
    /// <param name="team">Team of Piece</param>
    /// <returns>Null if invalid, Tile if valid</returns>
    private Tile positionHelper(int x, int y, Team team, Tile[,] tileDataArray)
    {
        if (x > 7 || x < 0 || y > 7 || y < 0) return null;

        if (tileDataArray[x, y].getCurrentPiece() == null || tileDataArray[x, y].getCurrentPiece().team != team)
            return tileDataArray[x, y];
        else
            return null;
    }

    /// <summary>
    /// Returns if the last move made was by a pawn who moved two spaces
    /// </summary>
    /// <returns>Bool of case</returns>
    private bool pawnDoubleMoveLastTurn ()
    {
        //Make sure last move exists - edge case for first move
        if (lastMove.startTile == null || lastMove.endTile == null) return false;

        //Make sure we're checking a pawn
        if (lastMove.movementType == StateChange.EnPassen) return false;
        if (lastMove.startTile.getCurrentPiece().type != PieceTypes.Pawn) return false;

        return Mathf.Abs(lastMove.startTile.getXPosition() - lastMove.endTile.getXPosition()) == 2;
    }

    /// <summary>
    /// Returns if the King for the given team is in check for the provided board
    /// </summary>
    /// <param name="gameBoard">Board to check</param>
    /// <param name="team">King's team</param>
    /// <returns>If King is in check</returns>
    private bool kingIsInCheck (Tile[,] gameBoard, Team team)
    {
        //Check king against enemy pieces
        foreach (Tile tile in gameBoard)
        {
            //Only run on hostile pieces
            if (tile.getCurrentPiece() == null) continue;
            if (tile.getCurrentPiece().team == team) continue;

            //Get Tiles the piece can move to
            List<MovementData> options = new List<MovementData>();
            options = getPossibleTilesRaw(tile, gameBoard);

            //Check for all pieces that can be taken, if one is a king then it's in check
            foreach (MovementData movement in options)
            {
                if (movement.movementType == StateChange.StandardTaken)
                {
                    if (movement.endTile.getCurrentPiece().type == PieceTypes.King)
                        return true;
                }
            }
        }

        //Default return if no pieces can attack king
        return false;
    }

    /// <summary>
    /// Creates a new copy of the provided board with the specified piece moved to a new tile
    /// </summary>
    /// <param name="start">Start Tile of Piece</param>
    /// <param name="end">End Tile of Piece</param>
    /// <param name="tileDataArray">Source Game Board</param>
    /// <returns>New board with piece moved</returns>
    private Tile[,] movePiece (Tile start, Tile end, Tile[,] tileDataArray)
    {
        Tile[,] newBoard = AI_Interface.AC_getCopyOfBoard(tileDataArray);

        newBoard[end.getXPosition(), end.getYPosition()].setCurrentPiece(newBoard[start.getXPosition(), start.getYPosition()].getCurrentPiece());
        newBoard[start.getXPosition(), start.getYPosition()].setCurrentPiece(null);

        return newBoard;
    }
    #endregion

    #region Duplicate GetPieceTiles - Used to avoid infinite looping for checks
    /// <summary>
    /// Returns the possible Tiles the provided Tile Piece can move to.
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Piece Type on Tile</param>
    /// <returns></returns>
    internal List<MovementData> getPossibleTilesRaw(Tile tile, Tile[,] currentTiles)
    {
        switch (tile.getCurrentPiece().type)
        {
            case PieceTypes.Pawn:
                return getPawnTilesRaw(tile, currentTiles);
            case PieceTypes.Rook:
                return getRookTilesRaw(tile, currentTiles);
            case PieceTypes.Knight:
                return getKnightTilesRaw(tile, currentTiles);
            case PieceTypes.Bishop:
                return getBishopTilesRaw(tile, currentTiles);
            case PieceTypes.Queen:
                return getQueenTilesRaw(tile, currentTiles);
            case PieceTypes.King:
                return getKingTilesRaw(tile, currentTiles);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + tile.getCurrentPiece().type + " :: " + gameObject.name);
                return new List<MovementData>();
        }
    }

    /// <summary>
    /// Given the pawn's team, this method will find all valid movement positions for the piece
    /// </summary>
    /// <param name="x">X Coordinate of Tile</param>
    /// <param name="y">Y Coordinate of Tile</param>
    /// <param name="team">Piece Team</param>
    /// <returns>List of possible Tiles to move to</returns>
    private List<MovementData> getPawnTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        Team team = tile.getCurrentPiece().team;
        int x = tile.getXPosition();
        int y = tile.getYPosition();


        //Player pieces
        if (team == Team.Player)
        {
            //Pawn still at starting position
            if (x == 1)
            {
                if (tileDataArray[2, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[2, y], StateChange.StandardMovement));
                if (tileDataArray[2, y].getCurrentPiece() == null && tileDataArray[3, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[3, y], StateChange.StandardMovement));
            }
            //Pawn is out on the board, check for forwards movement
            else
            {
                if (tileDataArray[x + 1, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardMovement));
            }

            //Check for side motion
            if (x < 7)
            {
                //Check if we can attack on the right
                if (y < 7 && tileDataArray[x + 1, y + 1].getCurrentPiece() != null && tileDataArray[x + 1, y + 1].getCurrentPiece().team == Team.AI)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardTaken));
                //Check if we can attack on the left
                if (y > 0 && tileDataArray[x + 1, y - 1].getCurrentPiece() != null && tileDataArray[x + 1, y - 1].getCurrentPiece().team == Team.AI)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardTaken)); ;
            }

            //Check for en passen
            if (pawnDoubleMoveLastTurn()                                                    //Last move enemy pawn double moved from start
                && tile.getXPosition() == lastMove.endTile.getXPosition()                   //Make sure pawns are on the same row
                && Mathf.Abs(tile.getYPosition() - lastMove.endTile.getYPosition()) == 1)    //Make sure pawns are beside each other
            {
                options.Add(new MovementData(tile,
                    tileDataArray[tile.getXPosition() + 1, lastMove.endTile.getYPosition()],
                    StateChange.EnPassen,
                    tileDataArray[lastMove.endTile.getXPosition(), lastMove.endTile.getYPosition()]));
            }
        }
        //Moving AI pieces
        else
        {
            //Pawn at starting position
            if (x == 6)
            {
                if (tileDataArray[5, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[5, y], StateChange.StandardMovement));
                if (tileDataArray[5, y].getCurrentPiece() == null && tileDataArray[4, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[4, y], StateChange.StandardMovement));
            }
            //Otherwise pawn out on board
            else
            {
                if (tileDataArray[x - 1, y].getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardMovement));
            }

            //Check for side motion
            if (x > 0)
            {
                //Check if we can attack on the left
                if (y < 7 && tileDataArray[x - 1, y + 1].getCurrentPiece() != null && tileDataArray[x - 1, y + 1].getCurrentPiece().team == 0)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardTaken));
                //Check if we can attack on the right
                if (y > 0 && tileDataArray[x - 1, y - 1].getCurrentPiece() != null && tileDataArray[x - 1, y - 1].getCurrentPiece().team == 0)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardTaken));
            }

            //Check for en passen
            if (pawnDoubleMoveLastTurn()
                && tile.getXPosition() == lastMove.endTile.getXPosition()
                && Mathf.Abs(tile.getYPosition() - lastMove.endTile.getYPosition()) == 1)
            {
                options.Add(new MovementData(tile,
                    tileDataArray[tile.getXPosition() - 1, lastMove.endTile.getYPosition()],
                    StateChange.EnPassen,
                    tileDataArray[lastMove.endTile.getXPosition(), lastMove.endTile.getYPosition()]));
            }
        }

        return options;
    }

    /// <summary>
    /// Given a rook's team and position, this method returns all possible Tiles the piece can move to
    /// </summary>
    /// <param name="x">X Coordinate of Rook</param>
    /// <param name="y">Y Coordinate of Rook</param>
    /// <param name="team">Team of Rook</param>
    /// <returns>List of possible Tiles to move to</returns>
    private List<MovementData> getRookTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Iterate along positive X
        for (int i = x + 1; i < 8; i++)
        {
            //If the next in order is empty
            if (tileDataArray[i, y].getCurrentPiece() == null)
            {
                //Add it
                options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along negative X
        for (int i = x - 1; i >= 0; i--)
        {
            //If the next in order is empty
            if (tileDataArray[i, y].getCurrentPiece() == null)
            {
                //Add it
                options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(new MovementData(tile, tileDataArray[i, y], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along positive Y
        for (int i = y + 1; i < 8; i++)
        {
            //If the next in order is empty
            if (tileDataArray[x, i].getCurrentPiece() == null)
            {
                //Add it
                options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        //Iterate along negative Y
        for (int i = y - 1; i >= 0; i--)
        {
            //If the next in order is empty
            if (tileDataArray[x, i].getCurrentPiece() == null)
            {
                //Add it
                options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardMovement));
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(new MovementData(tile, tileDataArray[x, i], StateChange.StandardTaken));
                }

                //Can't grab Tiles past another unit
                break;
            }
        }

        return options;
    }

    /// <summary>
    /// Give a Knight's position the method will return a List of all Tiles the knight can move to.
    /// </summary>
    /// <param name="x">X Coordinate of Knight</param>
    /// <param name="y">Y Coordinate of Knight</param>
    /// <param name="team">Team of Knight</param>
    /// <returns>List of Tiles</returns>
    private List<MovementData> getKnightTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        Tile t;

        //Check knight position 2/1
        t = positionHelper(x + 2, y + 1, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x + 2, y + 1], StateChange.StandardTaken));
        //Check knight position 2/-1
        t = positionHelper(x + 2, y - 1, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x + 2, y - 1], StateChange.StandardTaken));

        //Check knight position 1/2
        t = positionHelper(x + 1, y + 2, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x + 1, y + 2], StateChange.StandardTaken));
        //Check knight position -1/2
        t = positionHelper(x - 1, y + 2, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x - 1, y + 2], StateChange.StandardTaken));

        //Check knight position -2/1
        t = positionHelper(x - 2, y + 1, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x - 2, y + 1], StateChange.StandardTaken));
        //Check knight position -2/-1
        t = positionHelper(x - 2, y - 1, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x - 2, y - 1], StateChange.StandardTaken));

        //Check knight position 1/-2
        t = positionHelper(x + 1, y - 2, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x + 1, y - 2], StateChange.StandardTaken));
        //Check knight position -1/-2
        t = positionHelper(x - 1, y - 2, team, tileDataArray);
        if (t != null)
            options.Add(new MovementData(tile, tileDataArray[x - 1, y - 2], StateChange.StandardTaken));

        return options;
    }

    /// <summary>
    /// Given a Bishops position the method will return a list of all Tiles the Bishop can move to
    /// </summary>
    /// <param name="x">X Coordinate of Bishop</param>
    /// <param name="y">Y Coordinate of Bishop</param>
    /// <param name="team">Team of Bishop</param>
    /// <returns>List of Tiles the Bishop can move to</returns>
    private List<MovementData> getBishopTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Check x and y in +/+ direction
        for (int i = x + 1, j = y + 1; i < 8 && j < 8; i++, j++)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in -/- direction
        for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in +/- direction
        for (int i = x + 1, j = y - 1; i < 8 && j >= 0; i++, j--)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        //Check x and y in -/+ direction
        for (int i = x - 1, j = y + 1; i >= 0 && j < 8; i--, j++)
        {
            //Empty space to move to
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardMovement));
            }
            //otherwise there's a piece in the way
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(new MovementData(tile, tileDataArray[i, j], StateChange.StandardTaken));
                }

                break;
            }
        }

        return options;
    }

    /// <summary>
    /// Given a Queen's position the method will return a list of all Tiles the piece can move to.
    /// </summary>
    /// <param name="x">X Coordinate of Queen</param>
    /// <param name="y">Y Coordinate of Queen</param>
    /// <param name="team">Team of Queen</param>
    /// <returns>List of Tile</returns>
    private List<MovementData> getQueenTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();

        //Get rook movement tiles
        options = getRookTiles(tile, tileDataArray);
        //Add bishop movement tiles
        options.AddRange(getBishopTiles(tile, tileDataArray));

        return options;
    }

    /// <summary>
    /// Given a King's position the method will return a list of all tiles the King can move to
    /// </summary>
    /// <param name="x">X Coordinate of King</param>
    /// <param name="y">Y Coordinate of King</param>
    /// <param name="team">Team of King</param>
    /// <returns>List of Tiles</returns>
    private List<MovementData> getKingTilesRaw(Tile tile, Tile[,] tileDataArray)
    {
        List<MovementData> options = new List<MovementData>();
        Tile t;

        int x = tile.getXPosition();
        int y = tile.getYPosition();
        Team team = tile.getCurrentPiece().team;

        //Check position 1/1
        t = positionHelper(x + 1, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y + 1], StateChange.StandardTaken));
            }

        //Check position 0/1
        t = positionHelper(x, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x, y + 1], StateChange.StandardTaken));
            }

        //Check position 1/0
        t = positionHelper(x + 1, y, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y], StateChange.StandardTaken));
            }

        //Check position -1/1
        t = positionHelper(x - 1, y + 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y + 1], StateChange.StandardTaken));
            }

        //Check position 1/-1
        t = positionHelper(x + 1, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x + 1, y - 1], StateChange.StandardTaken));
            }

        //Check position -1/-1
        t = positionHelper(x - 1, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y - 1], StateChange.StandardTaken));
            }

        //Check position -1/0
        t = positionHelper(x - 1, y, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x - 1, y], StateChange.StandardTaken));
            }

        //Check position 0/-1
        t = positionHelper(x, y - 1, team, tileDataArray);
        if (t != null)
            if (kingIsInCheck(movePiece(tile, t, tileDataArray), team) == false)
            {
                if (t.getCurrentPiece() == null)
                    options.Add(new MovementData(tile, tileDataArray[x, y - 1], StateChange.StandardMovement));
                else
                    options.Add(new MovementData(tile, tileDataArray[x, y - 1], StateChange.StandardTaken));
            }

        return options;
    }
    #endregion
}

/// <summary>
/// Data container struct for holding all relevant information about a piece moving
/// on the chess board.
/// </summary>
internal struct MovementData
{
    internal Tile startTile;                //Tile provided for intial movement
    internal Tile endTile;                  //Tile option presented in the set

    internal StateChange movementType;      //Possible types of move (special and standard)
    internal Tile secondaryChangeStart;     //Secondary piece start position
    internal Tile secondaryChangeEnd;       //Secondary piece end position

    internal bool pawnUpgraded;             //Was the pawn upgraded?
    internal Piece newPiece;                //What unit is the pawn now?

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="s1">Start Tile of Piece to Move</param>
    /// <param name="e1">End Tile of Piece to move</param>
    /// <param name="state">Type of movement being represented</param>
    /// <param name="s2">Secondary start Tile of piece to move</param>
    /// <param name="e2">Secondary end Tile of piece to move</param>
    internal MovementData (Tile s1, Tile e1, StateChange state, Tile s2, Tile e2, bool upgraded, Piece p)
    {
        startTile = s1; endTile = e1;
        movementType = state; secondaryChangeStart = s2; secondaryChangeEnd = e2;
        pawnUpgraded = upgraded; newPiece = p;
    }

    /// <summary>
    /// Standard Movement Constructor
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="e1"></param>
    /// <param name="state"></param>
    internal MovementData (Tile s1, Tile e1, StateChange state)
    {
        startTile = s1; endTile = e1;
        movementType = state;
        secondaryChangeStart = null; secondaryChangeEnd = null;
        pawnUpgraded = false; newPiece = null;
    }

    /// <summary>
    /// Pawn Upgrade Constructor
    /// </summary>
    /// <param name="s1">Tile pawn started on</param>
    /// <param name="e1">Tile pawn upgrading on</param>
    /// <param name="upgraded">If the pawn upgraded</param>
    /// <param name="p">Piece the pawn will become</param>
    internal MovementData (Tile s1, Tile e1, bool upgraded, Piece p)
    {
        startTile = s1; endTile = e1; movementType = StateChange.Upgrade;
        secondaryChangeStart = null; secondaryChangeEnd = null;
        pawnUpgraded = upgraded; newPiece = p;
    }

    /// <summary>
    /// En Passen Constructor
    /// </summary>
    /// <param name="s1">Attack pawn start tile</param>
    /// <param name="e1">Attack pawm end tile</param>
    /// <param name="change">Statechange type</param>
    /// <param name="s2">pawn to remove</param>
    internal MovementData (Tile s1, Tile e1, StateChange change, Tile s2)
    {
        startTile = s1; endTile = e1; movementType = change;
        secondaryChangeStart = s2; secondaryChangeEnd = null;
        pawnUpgraded = false; newPiece = null;
    }

    /// <summary>
    /// Castling Constructor
    /// </summary>
    /// <param name="s1">King start tile</param>
    /// <param name="e1">King end tile</param>
    /// <param name="change">Castling Change</param>
    /// <param name="s2">Rook start tile</param>
    /// <param name="e2">Rook end tile</param>
    internal MovementData (Tile s1, Tile e1, StateChange change, Tile s2, Tile e2)
    {
        startTile = s1; endTile = e1; movementType = change;
        secondaryChangeStart = s2; secondaryChangeEnd = e2;
        pawnUpgraded = false; newPiece = null;
    }
}