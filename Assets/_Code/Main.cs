using System.Collections;
using System.Collections.Generic;
using System;

public class Main {

	//Class used to interface with Unity Code
	private AIInterfaceManager unityInterface;

	//CAMERON - Store the coordinates for your decision here!
	//Used for storing the Tiles to move
	private int startXPosition;
	private int startYPosition;
	private int endXPosition;
	private int endYPosition;

	private Promotion test;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="unityInterface">Interface Class for Unity</param>
	public Main(AIInterfaceManager unityInterface){
		this.unityInterface = unityInterface;
		TileData[,] startingBoard = unityInterface.getCurrentBoard();

		//Cameron Code Here
		TileData[,] boardCopy = unityInterface.getCopyOfBoard(startingBoard);
		searchAlphaBeta(boardCopy); // returns an action
		//get piece and store start and end positions

	}

	// Methods ------------------------------------------------------------

	private void searchAlphaBeta(TileData[,] boardCopy) {
		int value = maxValue(boardCopy, int.MinValue, int.MaxValue)
		//needs to return an action in successor state with value above ... ?
	}

	// This method returns the max value of the next state
	// may need a "current level" counter parameter 
	// I should create a method which calls this method to start.  Calling method will iterate over every peice currently on AI-team board and call)
	private int maxValue(TileData[,] boardCopy, int alpha, int beta, int cutOff){
		if (cutOff == 12){
			return unityInterface.getBoardScore(boardCopy);
		}
		int currentValue = int.MinValue;

		foreach (TileData tiles in piecesList(boardCopy)) {
			if (tiles.team == 0) continue;
			if (tiles.type == PieceTypes.Pawn && tiles.xCoordinate == 0) { // if piece is a pawn and at other end, promote it. May need to place this check elsewhere
				// need to explore 2 options (knight or queen) method call!
				//TODO: create a wrapper class and store promotion, coordinates of piece, coordinates where going and value returned. Not necessarily at this spot in code.
				//TODO tiles.type =    this is where I choose the promotion type. 
			}
			List<TileData> options = unityInterface.getMovementOptions(tiles.xCoordinate, tiles.yCoordinate, boardCopy);
			foreach (TileData choice in options) {
				TileData[,] newBoard = unityInterface.getCopyOfBoard(boardCopy); // creating a new board
				newBoard[choice.xCoordinate, choice.yCoordinate] = newBoard[tiles.xCoordinate, tiles.yCoordinate]; // moving piece to new position
				newBoard[tiles.xCoordinate, tiles.yCoordinate] = new TileData(tiles.xCoordinate, tiles.yCoordinate); // resetting tile data to remove old piece

				int nextVal = minValue(newBoard, alpha, beta, cutOff++);
				if (nextVal > currentValue) currentValue = nextVal;
				if (nextVal >= beta) return currentValue;
				if (nextVal > alpha) alpha = nextVal;
			}
		}

		//foreach ( choice in nextStates(boardCopy)){ // nextStates method should iterate over all possible choices and return an array of those choices
		//	//must define a choice object of some form to pass as a parameter
		//	int nextVal = minValue(choice, alpha, beta, cutOff++);
		//	if (nextVal > currentValue) currentValue = nextVal;
		//	if (nextVal >= beta) return currentValue;
		//	if (nextVal > alpha) alpha = nextVal;
		//}
		return currentValue;
	}

	private List<TileData> piecesList(TileData[,] arr) {
		List<TileData> betterList = new List<TileData>();
		for (int i = 0; i < arr.GetLength(0); i++) {
			for (int k = 0; k < arr.GetLength(1); k++) {
				if (arr[i,k].type != PieceTypes.None) {
					betterList.Add(arr[i, k]);
				}
			}
		}
		return betterList;
	}

	// This method returns the minimum value of the next state
	private int minValue(TileData[,] boardCopy, int alpha, int beta, int cutOff) {
		if (cutOff == 12) {
			return unityInterface.getBoardScore(boardCopy);
		}
		int currentValue = int.MaxValue;
		for each choice in nextStates(boardCopy){
			//must define a choice object of some form to pass as a parameter
			int nextVal = MaxValue(choice, alpha, beta, cutOff++);
			if (nextVal < currentValue) currentValue = nextVal;
			if (nextVal <= alpha) return currentValue;
			if (nextVal < beta) beta = nextVal;
		}
		return currentValue;
	}



	/// <summary>
	/// Called once the Main class constructor has finished running, ie the AI has run it's 
	/// course and has made a decision. Will return the piece position data to be used.
	/// </summary>
	/// <returns>FinalData struct</returns>
	internal FinalData getFinalData () { return new FinalData(startXPosition, startYPosition, endXPosition, endYPosition, test); }
}

public enum Promotion { None, Queen, Knight }

/// <summary>
/// Data struct used to return all needed data from the AI when a decision has been made
/// </summary>
internal struct FinalData{
	internal int startXPosition;
	internal int startYPosition;
	internal int endXPosition;
	internal int endYPosition;

	internal Promotion newUnit;

	/// <summary>
	/// Constructor
	/// </summary>
	internal FinalData(int x1, int y1, int x2, int y2, Promotion e)	{
		startXPosition = x1;
		startYPosition = y1;
		endXPosition = x2;
		endYPosition = y2;
		newUnit = e;
	}
}