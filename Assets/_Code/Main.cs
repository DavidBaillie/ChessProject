using System.Collections;
using System.Collections.Generic;
using System;

public class Main {

	//Class used to interface with Unity Code
	private AIInterfaceManager unityInterface;
	private int depth = 12; //TODO: this is hardcoded right now, but should be user chosen ... user chosen parameter of depth to check against in Min & Max 


	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="unityInterface">Interface Class for Unity</param>
	public Main(AIInterfaceManager unityInterface){
		this.unityInterface = unityInterface;
		Tile[,] startingBoard = unityInterface.AC_getCurrentBoard();
		Tile[,] boardCopy = unityInterface.AC_getCopyOfBoard(startingBoard);

		MovementData bestChoice = searchAlphaBeta(boardCopy); // chooses an action
		unityInterface.AC_submitChoice(bestChoice);
	}

	// Methods ------------------------------------------------------------

	private MovementData searchAlphaBeta(Tile[,] boardCopy) {
		List<Choice> allChoices = new List<Choice>(); // create a list of moves with their associated values
		// for each piece on the board
		foreach (Tile tiles in piecesList(boardCopy)) { //method call returns a list of all pieces
			if (tiles.getCurrentPiece().team == Team.Player) continue; // if player team, skip and iterate again. Basically only iterate over AI team pieces
			List<MovementData> options = unityInterface.AC_getMovementOptions(tiles, boardCopy); //make a list of all movements possible for this piece
			foreach (MovementData move in options) { // for each movement option
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(move, boardCopy); // create a new board with movement of piece
				int value = maxValue(newBoard, int.MinValue, int.MaxValue, 1); // starting cutoff 1 layer down (due to making 1 decision)
				Choice choice = new Choice(value, move); // create wrapper class holding score and decision
				allChoices.Add(choice); // add wrapper class to a list
			}
		}
		int bestValIndex = findBest(allChoices);// should return the index of the best-valued option
		MovementData bestChoice = allChoices[bestValIndex].bestMove; // just getting the best choice
		return bestChoice; //return choice with highest value
	}

	private int findBest(List<Choice> allChoices) {
		int index = 0;
		int bestVal = allChoices[0].bestMoveValue; // TODO: what if empty list. Liam may need to check for this (empty list = check mate)
		int counter = 0;
		foreach (Choice option in allChoices) {
			int value = option.bestMoveValue;
			if (value > bestVal) {
				bestVal = value;
				index = counter;
			}
			counter++; // need this to happen after, otherwise not 0 indexing
		}
		return index; //TODO: there is an issue here. If the list is empty, this method will return index 0, which will yield an array out of bounds. Is an empty list even possible as this means there are no more AI Pieces (which means no king which is after checkmate)?
	}

	// This method returns the max value of the next state
	private int maxValue(Tile[,] boardCopy, int alpha, int beta, int cutOff){
		if (cutOff == depth) return unityInterface.AC_getScoreOfBoard(boardCopy); //TODO:What if we hit the bottom of the tree (i.e. there are no more objects to search/checkmate, or even check) and we have not reached cutoff yet. HOWEVER, that is likely where we return a value and evaluate pruning (return current val at end)
		int currentValue = int.MinValue; // Setting current value to negative inifinity
		foreach (Tile tiles in piecesList(boardCopy)) {
			if (tiles.getCurrentPiece().team == Team.Player) continue; // if player team, skip and iterate again
			List<MovementData> options = unityInterface.AC_getMovementOptions(tiles, boardCopy); //make a list of all movements possible for this piece
			foreach (MovementData choice in options) {	
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(choice, boardCopy); // creating a new board after piece move
				int nextVal = minValue(newBoard, alpha, beta, cutOff++);
				if (nextVal > currentValue) currentValue = nextVal;
				if (nextVal >= beta) return currentValue; // pruning 
				if (nextVal > alpha) alpha = nextVal;
			}
		}
		return currentValue;
	}

	// This method returns the minimum value of the next state
	private int minValue(Tile[,] boardCopy, int alpha, int beta, int cutOff) {
		if (cutOff == depth) return unityInterface.AC_getScoreOfBoard(boardCopy); //TODO:What if we hit the bottom of the tree i.e. there are no more objects to search/checkmate, or even check (same issue as in max)
		int currentValue = int.MaxValue; // Setting current value to positive inifinity
		foreach (Tile tiles in piecesList(boardCopy)) {
			if (tiles.getCurrentPiece().team == Team.AI) continue; // if AI team, skip and iterate again
			List<MovementData> options = unityInterface.AC_getMovementOptions(tiles, boardCopy); //make a list of all movements possible for this piece
			foreach (MovementData choice in options) {
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(choice, boardCopy); // creating a new board
				int nextVal = maxValue(newBoard, alpha, beta, cutOff++);
				if (nextVal < currentValue) currentValue = nextVal;
				if (nextVal <= alpha) return currentValue; // pruning
				if (nextVal < beta) beta = nextVal;
			}
		}
		return currentValue;
	}

	// turning everything into a new 1D list to make it easier to read from
	private List<Tile> piecesList(Tile[,] arr) {
		List<Tile> betterList = new List<Tile>();
		for (int i = 0; i < arr.GetLength(0); i++) {
			for (int k = 0; k < arr.GetLength(1); k++) {
				if (arr[i,k].getCurrentPiece() != null) {
					betterList.Add(arr[i, k]);
				}
			}
		}
		return betterList;
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

	internal Choice(int value, MovementData move) {
		bestMove = move;
		bestMoveValue = value;
	}
}