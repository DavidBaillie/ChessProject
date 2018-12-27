using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossiblePositionManager : MonoBehaviour {

    private bool playersTurn = true;            //Tracks if it is the AI or players turn
    private Tile[,] boardTileArray;            //Array of Tile classes representing game board

    private Tile currentSelectedTile;           //Data saved when the player selects a Piece/Tile to move to a new Tile
    private List<Tile> possibleTiles;           //Possible tiles to move towards based on the currentSelectedTile

    /// <summary>
    /// Called when the BoardPositionInitializer finishes spawning all tiles with pieces
    /// </summary>
    /// <param name="data">Array of all tiles on board</param>
    internal void  setTileDataArray (Tile[,] data)
    {
        boardTileArray = data;

        GetComponent<AIInterfaceManager>().initialize(this);
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
    internal List<Tile> getPlayerPossibleTiles(int x, int y, PieceTypes type)
    {
        switch (type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(x, y, 0, boardTileArray);
            case PieceTypes.Rook:
                return getRookTiles(x, y, 0, boardTileArray);
            case PieceTypes.Knight:
                return getKnightTiles(x, y, 0, boardTileArray);
            case PieceTypes.Bishop:
                return getBishopTiles(x, y, 0, boardTileArray);
            case PieceTypes.Queen:
                return getQueenTiles(x, y, 0, boardTileArray);
            case PieceTypes.King:
                return getKingTiles(x, y, 0, boardTileArray);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + type + " :: " + gameObject.name);
                return new List<Tile>();
        }
    }

    /// <summary>
    /// Returns the possible Tiles the provided Piece can move to using a provided tile array
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Type of Piece on Tile</param>
    /// <returns></returns>
    internal List<Tile> getPlayerPossibleTiles (int x, int y, PieceTypes type, Tile[,] currentTiles)
    {
        switch (type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(x, y, 0, currentTiles);
            case PieceTypes.Rook:
                return getRookTiles(x, y, 0, currentTiles);
            case PieceTypes.Knight:
                return getKnightTiles(x, y, 0, currentTiles);
            case PieceTypes.Bishop:
                return getBishopTiles(x, y, 0, currentTiles);
            case PieceTypes.Queen:
                return getQueenTiles(x, y, 0, currentTiles);
            case PieceTypes.King:
                return getKingTiles(x, y, 0, currentTiles);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + type + " :: " + gameObject.name);
                return new List<Tile>();
        }
    }

    /// <summary>
    /// Returns the possible Tiles the provided Tile Piece can move to.
    /// </summary>
    /// <param name="x">X position of Tile</param>
    /// <param name="y">Y position of Tile</param>
    /// <param name="type">Piece Type on Tile</param>
    /// <returns></returns>
    internal List<Tile> getAIPossibleTiles (int x, int y, PieceTypes type, Tile[,] currentTiles)
    {
        switch (type)
        {
            case PieceTypes.Pawn:
                return getPawnTiles(x, y, 1, currentTiles);
            case PieceTypes.Rook:
                return getRookTiles(x, y, 1, currentTiles);
            case PieceTypes.Knight:
                return getKnightTiles(x, y, 1, currentTiles);
            case PieceTypes.Bishop:
                return getBishopTiles(x, y, 1, currentTiles);
            case PieceTypes.Queen:
                return getQueenTiles(x, y, 1, currentTiles);
            case PieceTypes.King:
                return getKingTiles(x, y, 1, currentTiles);
            default:
                Debug.LogError("CODE ERROR - Failed Check - PossiblePositionManager failed to match the PiecesType ENUM when returning possible positions" +
                    "for unit " + type + " :: " + gameObject.name);
                return new List<Tile>();
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
    private List<Tile> getPawnTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running Pawn Check!");
        List<Tile> options = new List<Tile>();

        //Player pieces
        if (team == 0)
        {
            //Pawn still at starting position
            if (x == 1)
            {
                if (tileDataArray[2, y].getCurrentPiece() == null)
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
                if (y < 7 && tileDataArray[x + 1, y + 1].getCurrentPiece() != null && tileDataArray[x + 1, y + 1].getCurrentPiece().team == 1)
                    options.Add(tileDataArray[x + 1, y + 1]);
                //Check if we can attack on the left
                if (y > 0 && tileDataArray[x + 1, y - 1].getCurrentPiece() != null && tileDataArray[x + 1, y - 1].getCurrentPiece().team == 1)
                    options.Add(tileDataArray[x + 1, y - 1]);
            }
        }
        //Moving AI pieces
        else
        {
            Debug.Log("ON AI TEAM");
            //Pawn at starting position
            if (x == 6)
            {
                if (tileDataArray[5, y].getCurrentPiece() == null)
                    options.Add(tileDataArray[5, y]);
                if (tileDataArray[5, y].getCurrentPiece() == null && tileDataArray[4, y].getCurrentPiece() == null)
                    options.Add(tileDataArray[4, y]);
            }
            //Otherwise pawn out on board
            else
            {
                if (tileDataArray[x - 1, y].getCurrentPiece() == null)
                    options.Add(tileDataArray[x - 1, y]);
            }

            //Check for side motion
            if (x > 0)
            {
                //Check if we can attack on the left
                if (y < 7 && tileDataArray[x - 1, y + 1].getCurrentPiece() != null && tileDataArray[x - 1, y + 1].getCurrentPiece().team == 0)
                    options.Add(tileDataArray[x - 1, y + 1]);
                //Check if we can attack on the right
                if (y > 0 && tileDataArray[x - 1, y - 1].getCurrentPiece() != null && tileDataArray[x - 1, y - 1].getCurrentPiece().team == 0)
                    options.Add(tileDataArray[x - 1, y - 1]);
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
    private List<Tile> getRookTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running Rook Check!");
        List<Tile> options = new List<Tile>();

        //Iterate along positive X
        for (int i = x + 1; i < 8; i++)
        {
            //If the next in order is empty
            if (tileDataArray[i, y].getCurrentPiece() == null)
            {
                //Add it
                options.Add(tileDataArray[i, y]);
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(tileDataArray[i, y]);
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
                options.Add(tileDataArray[i, y]);
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[i, y].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(tileDataArray[i, y]);
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
                options.Add(tileDataArray[x, i]);
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(tileDataArray[x, i]);
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
                options.Add(tileDataArray[x, i]);
            }
            //Otherwise we found a tile
            else
            {
                //If unit found is on other team
                if (tileDataArray[x, i].getCurrentPiece().team != team)
                {
                    //We can take that unit
                    options.Add(tileDataArray[x, i]);
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
    private List<Tile> getKnightTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running Knight Check!");
        List<Tile> options = new List<Tile>();
        Tile t;

        //Check knight position 2/1
        t = positionHelper(x + 2, y + 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x+2,y+1]);
        //Check knight position 2/-1
        t = positionHelper(x + 2, y - 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 2, y - 1]);

        //Check knight position 1/2
        t = positionHelper(x + 1, y + 2, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 1, y + 2]);
        //Check knight position -1/2
        t = positionHelper(x - 1, y + 2, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 1, y + 2]);

        //Check knight position -2/1
        t = positionHelper(x - 2, y + 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 2, y + 1]);
        //Check knight position -2/-1
        t = positionHelper(x -2, y - 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 2, y - 1]);

        //Check knight position 1/-2
        t = positionHelper(x + 1, y - 2, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 1, y - 2]);
        //Check knight position -1/-2
        t = positionHelper(x - 1, y - 2, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 1, y - 2]);

        return options;
    }

    /// <summary>
    /// Given a Bishops position the method will return a list of all Tiles the Bishop can move to
    /// </summary>
    /// <param name="x">X Coordinate of Bishop</param>
    /// <param name="y">Y Coordinate of Bishop</param>
    /// <param name="team">Team of Bishop</param>
    /// <returns>List of Tiles the Bishop can move to</returns>
    private List<Tile> getBishopTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running Bishop Check!");
        List<Tile> options = new List<Tile>();

        //Check x and y in +/+ direction
        for (int i = x + 1, j = y + 1; i < 8 && j < 8; i++, j++)
        {
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(tileDataArray[i, j]);
            }
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(tileDataArray[i, j]);
                }

                break;
            }
        }

        //Check x and y in -/- direction
        for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(tileDataArray[i, j]);
            }
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(tileDataArray[i, j]);
                }

                break;
            }
        }

        //Check x and y in +/- direction
        for (int i = x + 1, j = y - 1; i < 8 && j >= 0; i++, j--)
        {
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(tileDataArray[i, j]);
            }
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(tileDataArray[i, j]);
                }

                break;
            }
        }

        //Check x and y in -/+ direction
        for (int i = x - 1, j = y + 1; i >= 0 && j < 8; i--, j++)
        {
            if (tileDataArray[i, j].getCurrentPiece() == null)
            {
                options.Add(tileDataArray[i, j]);
            }
            else
            {
                if (tileDataArray[i, j].getCurrentPiece().team != team)
                {
                    options.Add(tileDataArray[i, j]);
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
    private List<Tile> getQueenTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running Queen Check!");
        List<Tile> options = new List<Tile>();

        //Get rook movement tiles
        options = getRookTiles(x, y, team, tileDataArray);
        //Add bishop movement tiles
        options.AddRange(getBishopTiles(x, y, team, tileDataArray));

        return options;
    }

    /// <summary>
    /// Given a King's position the method will return a list of all tiles the King can move to
    /// </summary>
    /// <param name="x">X Coordinate of King</param>
    /// <param name="y">Y Coordinate of King</param>
    /// <param name="team">Team of King</param>
    /// <returns>List of Tiles</returns>
    private List<Tile> getKingTiles (int x, int y, int team, Tile[,] tileDataArray)
    {
        Debug.Log("Running King Check!");
        List<Tile> options = new List<Tile>();
        Tile t;

        //Check position 1/1
        t = positionHelper(x + 1, y + 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 1, y + 1]);

        //Check position 0/1
        t = positionHelper(x, y + 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x, y + 1]);

        //Check position 1/0
        t = positionHelper(x + 1, y, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 1, y]);

        //Check position -1/1
        t = positionHelper(x - 1, y + 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 1, y + 1]);

        //Check position 1/-1
        t = positionHelper(x + 1, y - 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x + 1, y - 1]);

        //Check position -1/-1
        t = positionHelper(x - 1, y - 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 1, y - 1]);

        //Check position -1/0
        t = positionHelper(x - 1, y, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x - 1, y]);

        //Check position 0/-1
        t = positionHelper(x, y - 1, team, tileDataArray);
        if (t != null) options.Add(tileDataArray[x, y - 1]);

        return options;
    }


    /// <summary>
    /// Simple position check helper. Makes sure Tile is valid and within range.
    /// </summary>
    /// <param name="x">X Coordinate of Piece</param>
    /// <param name="y">Y Coordinate of Piece</param>
    /// <param name="team">Team of Piece</param>
    /// <returns>Null if invalid, Tile if valid</returns>
    private Tile positionHelper(int x, int y, int team, Tile[,] tileDataArray)
    {
        if (x > 7 || x < 0 || y > 7 || y < 0) return null;

        if (tileDataArray[x, y].getCurrentPiece() == null || tileDataArray[x, y].getCurrentPiece().team != team)
            return tileDataArray[x, y];
        else
            return null;
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

    internal StateChange movementType;       //Possible types of move (special and standard)
    internal Tile secondaryChangeStart;     //Secondary piece start position
    internal Tile secondaryChangeEnd;       //Secondary piece end position

    /// <summary>
    /// Full Constructor
    /// </summary>
    /// <param name="s1">Start Tile of Piece to Move</param>
    /// <param name="e1">End Tile of Piece to move</param>
    /// <param name="state">Type of movement being represented</param>
    /// <param name="s2">Secondary start Tile of piece to move</param>
    /// <param name="e2">Secondary end Tile of piece to move</param>
    internal MovementData (Tile s1, Tile e1, StateChange state, Tile s2, Tile e2)
    {
        startTile = s1; endTile = e1;
        movementType = state; secondaryChangeStart = s2; secondaryChangeEnd = e2;
    }

    /// <summary>
    /// Partial Constructor - Used only for standard moves
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="e1"></param>
    /// <param name="state"></param>
    internal MovementData (Tile s1, Tile e1, StateChange state)
    {
        startTile = s1; endTile = e1;
        movementType = state;
        secondaryChangeStart = null; secondaryChangeEnd = null;
    }
}