using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGeneratorController : MonoBehaviour {

    private PlayerControlManager controlManager;
    private GameBoardManager boardManager;
    private CanvasManager canvasManager;

    private PieceTypes selectedType;
    private Team selectedTeam;

    /// <summary>
    /// Called when object created
    /// </summary>
    private void Awake()
    {
        selectedTeam = Team.Player;
        selectedType = PieceTypes.Pawn;

        boardManager = GetComponent<GameBoardManager>();
        controlManager = GetComponent<PlayerControlManager>();
        canvasManager = GetComponent<CanvasManager>();
        controlManager.enabled = false;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Mouse left click event
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                WorldTile selected = hit.collider.gameObject.GetComponent<WorldTile>();
                if (selected != null)
                {
                    switch (selectedType)
                    {
                        //ROOK
                        case PieceTypes.Rook:
                            //Set up GameObject
                            GameObject PlayerRook = Instantiate(boardManager.Rook);
                            PlayerRook.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerRook.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerRook.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pRookPiece = PlayerRook.AddComponent<WorldPiece>();
                            pRookPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.Rook, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pRookPiece;
                            break;

                        //KNIGHT
                        case PieceTypes.Knight:
                            //Set up GameObject
                            GameObject PlayerKnight = Instantiate(boardManager.Knight);
                            PlayerKnight.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerKnight.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerKnight.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pKnightPiece = PlayerKnight.AddComponent<WorldPiece>();
                            pKnightPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.Knight, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pKnightPiece;
                            break;

                        //BISHOP
                        case PieceTypes.Bishop:
                            //Set up GameObject
                            GameObject PlayerBishop = Instantiate(boardManager.Bishop);
                            PlayerBishop.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerBishop.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerBishop.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pBishopPiece = PlayerBishop.AddComponent<WorldPiece>();
                            pBishopPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.Bishop, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pBishopPiece;
                            break;

                        //QUEEN
                        case PieceTypes.Queen:
                            //Set up GameObject
                            GameObject PlayerQueen = Instantiate(boardManager.Queen);
                            PlayerQueen.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerQueen.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerQueen.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pQueenPiece = PlayerQueen.AddComponent<WorldPiece>();
                            pQueenPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.Queen, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pQueenPiece;
                            break;

                        case PieceTypes.King:
                            //Set up GameObject
                            GameObject PlayerKing = Instantiate(boardManager.King);
                            PlayerKing.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerKing.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerKing.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pKingPiece = PlayerKing.AddComponent<WorldPiece>();
                            pKingPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.King, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pKingPiece;
                            break;

                        case PieceTypes.Pawn:
                            //Set up GameObject
                            GameObject PlayerPawn = Instantiate(boardManager.Pawn);
                            PlayerPawn.transform.position = new Vector3(selected.x, 0.5f, selected.y);
                            PlayerPawn.transform.localScale = new Vector3(boardManager.pieceScale, boardManager.pieceScale, boardManager.pieceScale);
                            if (selectedTeam == Team.AI) PlayerPawn.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                            //Set up WorldPiece
                            WorldPiece pPawnPiece = PlayerPawn.AddComponent<WorldPiece>();
                            pPawnPiece.instanciate(new Vector3(selected.x, 0.5f, selected.y), PieceTypes.Pawn, selectedTeam, boardManager.movementSpeed);

                            //Assign WorldPiece to Tile
                            selected.currentPiece = pPawnPiece;
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called by the Canvas when the user is done creating a board
    /// </summary>
    public void completeMode ()
    {
        GetComponent<PossiblePositionManager>().construct(boardManager.getTileCopyOfGameBoard());
        canvasManager.customCeationCanvas.SetActive(false);
        controlManager.enabled = true;
        this.enabled = false;
    }
    
    /// <summary>
    /// Called by the Canvas to change the selection
    /// </summary>
    /// <param name="type">Piece Type of selection</param>
    public void selectPiecePlayer (int type)
    {
        selectedTeam = Team.Player;

        switch (type)
        {
            case 0:
                selectedType = PieceTypes.Pawn;
                break;
            case 1:
                selectedType = PieceTypes.Rook;
                break;
            case 2:
                selectedType = PieceTypes.Knight;
                break;
            case 3:
                selectedType = PieceTypes.Bishop;
                break;
            case 4:
                selectedType = PieceTypes.Queen;
                break;
            case 5:
                selectedType = PieceTypes.King;
                break;
        }
    }

    /// <summary>
    /// Called by the Canvas to change the selection
    /// </summary>
    /// <param name="type">Piece Type of selection</param>
    public void selectPieceAI(int type)
    {
        selectedTeam = Team.AI;

        switch (type)
        {
            case 0:
                selectedType = PieceTypes.Pawn;
                break;
            case 1:
                selectedType = PieceTypes.Rook;
                break;
            case 2:
                selectedType = PieceTypes.Knight;
                break;
            case 3:
                selectedType = PieceTypes.Bishop;
                break;
            case 4:
                selectedType = PieceTypes.Queen;
                break;
            case 5:
                selectedType = PieceTypes.King;
                break;
        }
    }
}
