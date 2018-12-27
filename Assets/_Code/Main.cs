using System.Collections;
using System.Collections.Generic;
using System;

public class Main {

	//Class used to interface with Unity Code
	private AIInterfaceManager unityInterface;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="unityInterface">Interface Class for Unity</param>
	public Main(AIInterfaceManager unityInterface){
		this.unityInterface = unityInterface;
		Tile[,] startingBoard = unityInterface.AC_getCurrentBoard();
		Tile[,] boardCopy = unityInterface.AC_getCopyOfBoard(startingBoard);

		searchAlphaBeta(boardCopy); // chooses an action
	}

	// Methods ------------------------------------------------------------

	private void searchAlphaBeta(Tile[,] boardCopy) {
		//int value = maxValue(boardCopy, int.MinValue, int.MaxValue)
		//needs to choose an action in successor state with value above ... ?
	}

	// This method returns the max value of the next state
	// may need a "current level" counter parameter 
	// I should create a method which calls this method to start.  Calling method will iterate over every peice currently on AI-team board and call)
	private int maxValue(Tile[,] boardCopy, int alpha, int beta, int cutOff){
		if (cutOff == 12){
			return unityInterface.AC_getScoreOfBoard(boardCopy);
		}

		int currentValue = int.MinValue; // Setting current value to negative inifinity

		foreach (Tile tiles in piecesList(boardCopy)) {
			if (tiles.getCurrentPiece().team == 0) continue;

			//if (tiles.type == PieceTypes.Pawn && tiles.xCoordinate == 0) { // if piece is a pawn and at other end, promote it. May need to place this check elsewhere
			//	// need to explore 2 options (knight or queen) method call!
			//	//TODO: create a wrapper class and store promotion, coordinates of piece, coordinates where going and value returned. Not necessarily at this spot in code.
			//	//TODO tiles.type =    this is where I choose the promotion type. 
			//}

			List<Tile> options = unityInterface.getMovementOptions(tiles.xCoordinate, tiles.yCoordinate, boardCopy);
			foreach (Tile choice in options) {
				Tile[,] newBoard = unityInterface.getCopyOfBoard(boardCopy); // creating a new board
				newBoard[choice.xCoordinate, choice.yCoordinate] = newBoard[tiles.xCoordinate, tiles.yCoordinate]; // moving piece to new position
				newBoard[tiles.xCoordinate, tiles.yCoordinate] = new Tile(tiles.xCoordinate, tiles.yCoordinate); // resetting tile data to remove old piece

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

	// turning everything into a new 1D list to make everything easier to read
	private List<Tile> piecesList(Tile[,] arr) {
		List<Tile> betterList = new List<Tile>();
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
	private int minValue(Tile[,] boardCopy, int alpha, int beta, int cutOff) {
		if (cutOff == 12) return unityInterface.AC_getScoreOfBoard(boardCopy);
		int currentValue = int.MaxValue;
		for each choice in nextStates(boardCopy){
			if (tiles.getCurrentPiece().team == 1) continue;
			//must define a choice object of some form to pass as a parameter
			int nextVal = MaxValue(choice, alpha, beta, cutOff++);
			if (nextVal < currentValue) currentValue = nextVal;
			if (nextVal <= alpha) return currentValue;
			if (nextVal < beta) beta = nextVal;
		}
		return currentValue;
	}



	///// <summary>
	///// Called once the Main class constructor has finished running, ie the AI has run it's 
	///// course and has made a decision. Will return the piece position data to be used.
	///// </summary>
	///// <returns>FinalData struct</returns>
	//internal FinalData getFinalData () { return new FinalData(startXPosition, startYPosition, endXPosition, endYPosition, test); }
}

///// <summary>
///// Data struct used to return all needed data from the AI when a decision has been made
///// </summary>
//internal struct FinalData {
//	internal int startXPosition;
//	internal int startYPosition;
//	internal int endXPosition;
//	internal int endYPosition;

//	internal Promotion newUnit;

///// <summary>
///// Constructor
///// </summary>
//internal FinalData(int x1, int y1, int x2, int y2, Promotion e) {
//		startXPosition = x1;
//		startYPosition = y1;
//		endXPosition = x2;
//		endYPosition = y2;
//		newUnit = e;
//	}
//}

class Choice {
	internal int bestMoveValue;
	internal MovementData bestMove;

	Choice(int value, MovementData move) {
		bestMove = move;
		bestMoveValue = value;
	}
	
}