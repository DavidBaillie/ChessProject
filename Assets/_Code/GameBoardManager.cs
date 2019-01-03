using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour {

    //Inspector Vars////
    public float movementSpeed;
    public float pieceScale;
    [Space(5)]
    public GameObject Pawn;
    public GameObject Rook;
    public GameObject Bishop;
    public GameObject Knight;
    public GameObject King;
    public GameObject Queen;
    //Inspector Vars////

    private WorldTile[,] gameBoard;


    /// <summary>
    /// Called by the CanvasManager to create a standard game
    /// </summary>
	internal void createStandardGame ()
    {
        createBoard();
        addStandardPieces();
    }

    /// <summary>
    /// Called by the CanvasManager to create a custom game
    /// </summary>
    internal void createCustomGame ()
    {
        createBoard();
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



            //Create GameObject for Black Pawn and set to correct position
            GameObject blackPawn = Instantiate(Pawn);
            blackPawn.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            blackPawn.transform.position = new Vector3(6, 0.5f, i);
            blackPawn.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);

            //Set up AI Pawn
            WorldPiece blackPiece = blackPawn.AddComponent<WorldPiece>();
            blackPiece.instanciate(new Vector3(6, 0.5f, i), PieceTypes.Pawn, Team.AI, movementSpeed);
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
}
