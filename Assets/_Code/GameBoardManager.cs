using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardManager : MonoBehaviour {

    //Inspector Vars////
    public float movementSpeed;
    public float pieceScale;
    public InputField depthField;
    public Button standardButton;
    public Button customButton;
    [Space(5)]
    public GameObject Pawn;
    public GameObject Rook;
    public GameObject Bishop;
    public GameObject Knight;
    public GameObject King;
    public GameObject Queen;
    [Space(5)]
    public int pawnValue;
    public int rookValue;
    public int bishopValue;
    public int knightValue;
    public int kingValue;
    public int queenValue;
    //Inspector Vars////

    private WorldTile[,] gameBoard;
    private PossiblePositionManager positionManager;
    private AIInterfaceManager AIManager;
    private CanvasManager canvasManager;


    /// <summary>
    /// Called on object creation
    /// </summary>
    private void Awake()
    {
        positionManager = GetComponent<PossiblePositionManager>();
        AIManager = GetComponent<AIInterfaceManager>();
        canvasManager = GetComponent<CanvasManager>();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        int ignore = 0;
        if (Int32.TryParse(depthField.text, out ignore))
        {
            standardButton.interactable = true;
            customButton.interactable = true;
        }
        else
        {
            standardButton.interactable = false;
            customButton.interactable = false;
        }
    }



    /// <summary>
    /// Called by the CanvasManager to create a standard game
    /// </summary>
	internal void createStandardGame ()
    {
        //Create board and add pieces
        createBoard();
        addStandardPieces();
        AIManager.setAIDepth(Int32.Parse(depthField.text));

        //Initialize the PossiblePositionManager to kick off the core game loop
        positionManager.construct(getTileCopyOfGameBoard());
    }

    /// <summary>
    /// Called by the CanvasManager to create a custom game
    /// </summary>
    internal void createCustomGame ()
    {
        createBoard();

        AIManager.setAIDepth(Int32.Parse(depthField.text));
    }



    /// <summary>
    /// Called when the game board needs to be created for use by the player in both modes.
    /// </summary>
    private void createBoard ()
    {
        //Create array to hold board data
        gameBoard = new WorldTile[8, 8];

        bool colourSwitch = true;

        //Spin up parent to keep hierarchy clean
        GameObject boardParent = new GameObject("Board Parent");
        Transform parentTransform = boardParent.transform;

        //Nested for loop to create all tiles at needed coordinates
        for (int x = 0; x < 8; x++)
        {
            colourSwitch = !colourSwitch;
            for (int y = 0; y < 8; y++)
            {
                //Set up GameObject for use
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = "Tile (" + x + "/" + y + ")"; 
                tile.transform.position = new Vector3(x, 0, y);
                tile.transform.parent = parentTransform;

                //Set up WorldTile code
                WorldTile worldTile = tile.AddComponent<WorldTile>();
                worldTile.instanciate(x, y, colourSwitch);

                //Save Component for later accessing/use
                gameBoard[x, y] = worldTile;

                //Flip boolean to alternate colours
                colourSwitch = !colourSwitch;
            }
        }
    }

    /// <summary>
    /// Called when creating a standard game board to add Pieces to standard starting positions
    /// </summary>
    private void addStandardPieces ()
    {
        //Create pawns for both teams
        for (int i = 0; i < 8; i++)
        {
            //Create GameObject for White Pawn and set to correct position
            GameObject whitePawn = Instantiate(Pawn);
            whitePawn.transform.position = new Vector3(1, 0.5f, i);
            whitePawn.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

            //Set up Player Pawn
            WorldPiece whitePiece = whitePawn.AddComponent<WorldPiece>();
            whitePiece.instanciate(new Vector3(1, 0.5f, i), PieceTypes.Pawn, Team.Player, movementSpeed);

            gameBoard[1, i].currentPiece = whitePiece;


            //Create GameObject for Black Pawn and set to correct position
            GameObject blackPawn = Instantiate(Pawn);
            blackPawn.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            blackPawn.transform.position = new Vector3(6, 0.5f, i);
            blackPawn.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

            //Set up AI Pawn
            WorldPiece blackPiece = blackPawn.AddComponent<WorldPiece>();
            blackPiece.instanciate(new Vector3(6, 0.5f, i), PieceTypes.Pawn, Team.AI, movementSpeed);

            gameBoard[6, i].currentPiece = blackPiece;
        }

        //Create all non-pawn Pieces
        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                //ROOK
                case 0:
                case 7:
                    ////Player
                    //Set up GameObject
                    GameObject PlayerRook = Instantiate(Rook);
                    PlayerRook.transform.position = new Vector3(0, 0.5f, i);
                    PlayerRook.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece pRookPiece = PlayerRook.AddComponent<WorldPiece>();
                    pRookPiece.instanciate(new Vector3(0, 0.5f, i), PieceTypes.Rook, Team.Player, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[0, i].currentPiece = pRookPiece;

                    ////AI
                    //Set up GameObject
                    GameObject AIRook = Instantiate(Rook);
                    AIRook.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    AIRook.transform.position = new Vector3(7, 0.5f, i);
                    AIRook.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece aRookPiece = AIRook.AddComponent<WorldPiece>();
                    aRookPiece.instanciate(new Vector3(7, 0.5f, i), PieceTypes.Rook, Team.AI, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[7, i].currentPiece = aRookPiece;
                    break;
                
                //KNIGHT
                case 1:
                case 6:
                    ////Player
                    //Set up GameObject
                    GameObject PlayerKnight = Instantiate(Knight);
                    PlayerKnight.transform.position = new Vector3(0, 0.5f, i);
                    PlayerKnight.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece pKnightPiece = PlayerKnight.AddComponent<WorldPiece>();
                    pKnightPiece.instanciate(new Vector3(0, 0.5f, i), PieceTypes.Knight, Team.Player, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[0, i].currentPiece = pKnightPiece;

                    ////AI
                    //Set up GameObject
                    GameObject AIKnight = Instantiate(Knight);
                    AIKnight.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    AIKnight.transform.position = new Vector3(7, 0.5f, i);
                    AIKnight.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece aKnightPiece = AIKnight.AddComponent<WorldPiece>();
                    aKnightPiece.instanciate(new Vector3(7, 0.5f, i), PieceTypes.Knight, Team.AI, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[7, i].currentPiece = aKnightPiece;
                    break;

                //BISHOP
                case 2:
                case 5:
                    ////Player
                    //Set up GameObject
                    GameObject PlayerBishop = Instantiate(Bishop);
                    PlayerBishop.transform.position = new Vector3(0, 0.5f, i);
                    PlayerBishop.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece pBishopPiece = PlayerBishop.AddComponent<WorldPiece>();
                    pBishopPiece.instanciate(new Vector3(0, 0.5f, i), PieceTypes.Bishop, Team.Player, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[0, i].currentPiece = pBishopPiece;

                    ////AI
                    //Set up GameObject
                    GameObject AIBishop = Instantiate(Bishop);
                    AIBishop.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    AIBishop.transform.position = new Vector3(7, 0.5f, i);
                    AIBishop.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece aBishopPiece = AIBishop.AddComponent<WorldPiece>();
                    aBishopPiece.instanciate(new Vector3(7, 0.5f, i), PieceTypes.Bishop, Team.AI, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[7, i].currentPiece = aBishopPiece;
                    break;

                //QUEEN
                case 3:
                    ////Player
                    //Set up GameObject
                    GameObject PlayerQueen = Instantiate(Queen);
                    PlayerQueen.transform.position = new Vector3(0, 0.5f, i);
                    PlayerQueen.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece pQueenPiece = PlayerQueen.AddComponent<WorldPiece>();
                    pQueenPiece.instanciate(new Vector3(0, 0.5f, i), PieceTypes.Queen, Team.Player, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[0, i].currentPiece = pQueenPiece;

                    ////AI
                    //Set up GameObject
                    GameObject AIQueen = Instantiate(Queen);
                    AIQueen.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    AIQueen.transform.position = new Vector3(7, 0.5f, i);
                    AIQueen.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece aQueenPiece = AIQueen.AddComponent<WorldPiece>();
                    aQueenPiece.instanciate(new Vector3(7, 0.5f, i), PieceTypes.Queen, Team.AI, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[7, i].currentPiece = aQueenPiece;
                    break;

                //KING
                case 4:
                    ////Player
                    //Set up GameObject
                    GameObject PlayerKing = Instantiate(King);
                    PlayerKing.transform.position = new Vector3(0, 0.5f, i);
                    PlayerKing.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece pKingPiece = PlayerKing.AddComponent<WorldPiece>();
                    pKingPiece.instanciate(new Vector3(0, 0.5f, i), PieceTypes.King, Team.Player, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[0, i].currentPiece = pKingPiece;

                    ////AI
                    //Set up GameObject
                    GameObject AIKing = Instantiate(King);
                    AIKing.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    AIKing.transform.position = new Vector3(7, 0.5f, i);
                    AIKing.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

                    //Set up WorldPiece
                    WorldPiece aKingPiece = AIKing.AddComponent<WorldPiece>();
                    aKingPiece.instanciate(new Vector3(7, 0.5f, i), PieceTypes.King, Team.AI, movementSpeed);

                    //Assign WorldPiece to Tile
                    gameBoard[7, i].currentPiece = aKingPiece;
                    break;
            }
        }
    }



    /// <summary>
    /// Called to get a copy of the game board in the for of the Tile class. 
    /// </summary>
    /// <returns>2D Tile array representing the game board</returns>
    internal Tile[,] getTileCopyOfGameBoard ()
    {
        Tile[,] copy = new Tile[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //Built Piece class as either null or having data
                Piece piece = null;
                if (gameBoard[x,y].currentPiece != null)
                {
                    piece = new Piece(gameBoard[x, y].currentPiece.type, gameBoard[x, y].currentPiece.team);
                }

                //Save Tile with correct Piece attached 
                copy[x, y] = new Tile(piece, x, y);
            }
        }

        return copy;
    }

    /// <summary>
    /// Returns the value of the provided PieceType
    /// </summary>
    /// <param name="type">PieceType to return value of</param>
    /// <returns>float representing Piece value</returns>
    internal int getPieceScore(PieceTypes type)
    {
        switch (type)
        {
            case PieceTypes.Pawn:
                return pawnValue;
            case PieceTypes.Rook:
                return rookValue;
            case PieceTypes.Knight:
                return knightValue;
            case PieceTypes.Bishop:
                return bishopValue;
            case PieceTypes.King:
                return kingValue;
            case PieceTypes.Queen:
                return queenValue;
        }

        //Default return, should never run
        return 0;
    }



    /// <summary>
    /// Removes the piece at the provided coordinates from the board
    /// </summary>
    /// <param name="x">X Coordinate of WorldTile</param>
    /// <param name="y">Y Coordinate of WorldTIle</param>
    internal void removeUnit (int x, int y)
    {
        //Don't try accessing things if there's no WorldPiece
        if (gameBoard[x, y].currentPiece == null) return;

        //TODO - Move unit off board instead of destroying it
        Destroy(gameBoard[x, y].currentPiece.gameObject, 0.5f);
        gameBoard[x, y].currentPiece = null;
    }

    /// <summary>
    /// Moves the Piece at WorldTile (x1,y1) to WorldTile (x2,y2)
    /// </summary>
    /// <param name="x1">X Coordinate of Start WorldTile</param>
    /// <param name="y1">Y Coordinate of Start WorldTile</param>
    /// <param name="x2">X Coordinate of End WorldTile</param>
    /// <param name="y2">Y Coordinate of End WorldTile</param>
    internal void moveUnit (int x1, int y1, int x2, int y2)
    {
        //Save references
        WorldTile start = gameBoard[x1, y1];
        WorldTile end = gameBoard[x2, y2];

        //Move Piece GameObject to new position
        start.currentPiece.targetPosition = end.gameObject.transform.position + (Vector3.up / 2);

        //Assign WorldPiece to new WorldTile 
        end.currentPiece = start.currentPiece;
        start.currentPiece = null;
    }

    /// <summary>
    /// Will change the PieceType of the WorldPiece at WorldTile (x,y) to the new specified PieceType
    /// </summary>
    /// <param name="x">X Coordinate of WorldTile</param>
    /// <param name="y">Y Coordinate of WorldTile</param>
    /// <param name="newType">PieceType the WorldPiece will become</param>
    internal void upgradeUnit (int x, int y, PieceTypes newType)
    {
        Team team = gameBoard[x, y].currentPiece.team;
        Destroy(gameBoard[x, y].currentPiece.gameObject);

        switch (newType)
        {
            case PieceTypes.Queen:
                GameObject newQueen = Instantiate(Queen);
                if (team == Team.AI) newQueen.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                newQueen.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
                newQueen.transform.position = new Vector3(x, 0.5f, y);

                WorldPiece queenPiece = newQueen.AddComponent<WorldPiece>();
                queenPiece.instanciate(newQueen.transform.position, PieceTypes.Queen, team, movementSpeed);

                gameBoard[x, y].currentPiece = queenPiece;
                break;
            case PieceTypes.Knight:
                GameObject newKnight = Instantiate(Knight);
                if (team == Team.AI) newKnight.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                newKnight.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
                newKnight.transform.position = new Vector3(x, 0.5f, y);

                WorldPiece knightPiece = newKnight.AddComponent<WorldPiece>();
                knightPiece.instanciate(newKnight.transform.position, PieceTypes.Knight, team, movementSpeed);

                gameBoard[x, y].currentPiece = knightPiece;
                break;
            default:
                Debug.Log("Unexpected parameter provided when upgrading Pawn to a new Type! (" + newType + ")");
                break;
        }

        Debug.Log("Finished upgrading unit");
    }



    /// <summary>
    /// Called when CheckMate occurrs to end the game
    /// </summary>
    /// <param name="team">Winning Team</param>
    internal void gameWin (Team team)
    {
        destroyBoard();
        canvasManager.showWinCanvas(team);
    }

    /// <summary>
    /// Called when a StaleMate occurrs
    /// </summary>
    internal void tieGame ()
    {
        destroyBoard();
        canvasManager.showTieCanvas();
    }

    /// <summary>
    /// Destroys all GameObjects related to the game Board
    /// </summary>
    private void destroyBoard ()
    {
        AIManager.running = false;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (gameBoard[x, y].currentPiece != null) Destroy(gameBoard[x, y].currentPiece.gameObject);
                Destroy(gameBoard[x, y].gameObject);
            }
        }

        gameBoard = null;
    }
}