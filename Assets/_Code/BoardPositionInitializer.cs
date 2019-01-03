using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handles the initialization and creation of all tiles and pieces for the start of the board. 
/// Class instanciates both tiles and pieces at the correct location, sets team and type data, then passes 
/// any persistent data over to the PossiblePositionManager for later usage.
/// </summary>
public class BoardPositionInitializer : MonoBehaviour {

    //Inspector Vars////
    public GameObject Pawn;
    public GameObject Rook;
    public GameObject Bishop;
    public GameObject Knight;
    public GameObject King;
    public GameObject Queen;
    //Inspector Vars////


    private const int BOARD_SIZE = 8;       //Size of board
    private bool colourSwitch;              //Boolean for rotating colours of board tiles

    private Tile[,] boardTileArray;         //Array of all tiles on board - sent to PossiblePositionManager for storage

    /// <summary>
    /// Called before first frame
    /// </summary>
    private void Awake()
    {
        /*
        //Initialize the array of board positions
        boardTileArray = new Tile[BOARD_SIZE, BOARD_SIZE];

        GameObject boardParent = new GameObject();

        //Create board Base with pieces
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            colourSwitch = !colourSwitch;
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                //Create the position object -> set name, position, and colour
                GameObject tilePoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tilePoint.name = x.ToString() + "/" + y.ToString();
                tilePoint.transform.position = new Vector3(x, 0, y);
                tilePoint.transform.parent = boardParent.transform;

                //Set the correct colour for the tiles
                if (colourSwitch)
                    tilePoint.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                //Give the tilePoint a Tile class for later usage
                Tile t = tilePoint.AddComponent<Tile>();
                t.initialize(getPieceToSpawn(x, y), x, y, tilePoint);
                boardTileArray[x, y] = t;
                                
                colourSwitch = !colourSwitch;
            }
        }

		//Save persistent data over on the position manager
		GetComponent<PossiblePositionManager>().setTileDataArray(boardTileArray);
        */
    }


    /// <summary>
    /// Returns a Piece Object for initial spawning of the board
    /// </summary>
    /// <param name="x">X Position</param>
    /// <param name="y">Y Position</param>
    /// <returns>Piece Struct</returns>
    private Piece getPieceToSpawn(int x, int y)
    {
        //Return null for empty board positions
        if (x > 1 && x < BOARD_SIZE - 2) return null;

        //Return pawns with the right team data
        if (x == 1)
        {
            GameObject pawn = Instantiate(Pawn);
            Piece p = pawn.AddComponent<Piece>();
            p.initialize(new Vector3(x, 0, y), PieceTypes.Pawn, Team.Player);
            return p;
        }
        if (x == BOARD_SIZE - 2)
        {
            GameObject pawn = Instantiate(Pawn);
            Piece p = pawn.AddComponent<Piece>();
            p.initialize(new Vector3(x, 0, y), PieceTypes.Pawn, Team.AI);
            return p;
        }

        //Otherwise determine what piece goes to what position
        if (x == 0)
        {
            return generatePiece(x, y, Team.Player);
        }
        else if (x == BOARD_SIZE - 1)
        {
            return generatePiece(x, y, Team.AI);
        }
        else
        {
            Debug.Log
                ("CODE ERROR - Unexpected Input - BoardPositionInitializer failed to identify the X position provided when generating a new" +
                "board piece -> " + x.ToString() + " :: " + gameObject.name);
            return null;
        }
    }

    /// <summary>
    /// Generates a Piece Type with the present team provided
    /// </summary>
    /// <param name="y">Y Position of the Piece</param>
    /// <param name="team">Team number to return with</param>
    /// <returns>Piece Class</returns>
    private Piece generatePiece (int x, int y, Team team)
    {
        switch (y)
        {
            //Case for Rook
            case 0:
            case 7:
                GameObject rook = Instantiate(Rook);
                Piece p = rook.AddComponent<Piece>();
                p.initialize(new Vector3(x, 0, y), PieceTypes.Rook, team);
                return p;
            //Case for Knights
            case 1:
            case 6:
                GameObject knight = Instantiate(Knight);
                p = knight.AddComponent<Piece>();
                p.initialize(new Vector3(x, 0, y), PieceTypes.Knight, team);
                return p;
            //Case for Bishops
            case 2:
            case 5:
                GameObject bishop = Instantiate(Bishop);
                p = bishop.AddComponent<Piece>();
                p.initialize(new Vector3(x, 0, y), PieceTypes.Bishop, team);
                return p;
            //Case for Queen
            case 3:
                GameObject queen = Instantiate(Queen);
                p = queen.AddComponent<Piece>();
                p.initialize(new Vector3(x, 0, y), PieceTypes.Queen, team);
                return p;
            //Case for Kings
            case 4:
                GameObject king = Instantiate(King);
                p = king.AddComponent<Piece>();
                p.initialize(new Vector3(x, 0, y), PieceTypes.King, team);
                return p;
            //Should never reach this state - debug if we do
            default:
                Debug.LogError
                    ("CODE ERROR - Invalid Position - Failed to generate a piece fir provided position " + y.ToString() +
                    " on team " + team.ToString() + " for BoardPositionInitializer :: " + gameObject.name);
                return null;
        }
    }
}



/// <summary>
/// Struct designed to hold basic constant data about each position of the board.
/// Contains reference to world GameObject and position data for the position
/// </summary>
struct BoardLocation
{
    internal GameObject positionBase;
    internal int xPosition;
    internal int yPosition;

    public BoardLocation (GameObject positionBase, int x, int y)
    {
        this.positionBase = positionBase;
        this.xPosition = x;
        this.yPosition = y;
    }
}