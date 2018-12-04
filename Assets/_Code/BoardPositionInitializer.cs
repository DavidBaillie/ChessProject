using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPositionInitializer : MonoBehaviour {

    //
    public GameObject Pawn;
    public GameObject Rook;
    public GameObject Bishop;
    public GameObject Knight;
    public GameObject King;
    public GameObject Queen;
    //

    private const int BOARD_SIZE = 8;
    private bool colourSwitch;

    private PositionData[,] boardDataArray;

    /// <summary>
    /// Called before first frame
    /// </summary>
    private void Awake()
    {
        //Initialize the array of board positions
        boardDataArray = new PositionData[BOARD_SIZE, BOARD_SIZE];

        //Create board Base
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            colourSwitch = !colourSwitch;
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                //Create the position object -> set name, position, and colour
                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Cube);
                point.name = x.ToString() + "/" + y.ToString();
                point.transform.position = new Vector3(x, 0, y);
                //point.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                if (colourSwitch)
                    point.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                colourSwitch = !colourSwitch;

                //Now we have the position on the board, save it to an array for later access
                boardDataArray[x, y] = new PositionData(new BoardLocation(point, x, y), getPieceToSpawn(x,y));
                if (boardDataArray[x, y].currentUser != null)
                    boardDataArray[x, y].currentUser.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        }
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
            p.initialize(new Vector3(x, 0, y), PieceTypes.Pawn, 0);
            return p;
        }
        if (x == BOARD_SIZE - 2)
        {
            GameObject pawn = Instantiate(Pawn);
            Piece p = pawn.AddComponent<Piece>();
            p.initialize(new Vector3(x, 0, y), PieceTypes.Pawn, 1);
            return p;
        }

        //Otherwise determine what piece goes to what position
        if (x == 0)
        {
            return generatePiece(x, y, 0);
        }
        else if (x == BOARD_SIZE - 1)
        {
            return generatePiece(x, y, 1);
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
    private Piece generatePiece (int x, int y, int team)
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
/// Board Position data for each location on the board. Holds the BoardLocation Struct for position data
/// and the current piece using this position.
/// </summary>
class PositionData
{
    internal BoardLocation location;
    internal Piece currentUser;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="location">BoardLocation struct</param>
    /// <param name="currentUser">Piece Class attached to Piece GameObject spawned</param>
    internal PositionData(BoardLocation location, Piece currentUser)
    {
        this.location = location;
        this.currentUser = currentUser;
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