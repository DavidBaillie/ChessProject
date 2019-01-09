using System.Collections.Generic;
using UnityEngine;

/// This class is responsible for the AI portion of the code.
/// It considers future optins using tree-based alpha-beta search with pruning
/// with the depth being chosen by the user.
public class Main {
	// Class used to interface with Unity Code
	private AIInterfaceManager unityInterface;
	// depth for search tree to check against (user specified)
    private int depth;

	/// <summary>
	/// Constructor. Sets the depth of the search, obtains a copy of the current board
	/// and calls the alpha-beta search. Then it submits the yielded choice.
	/// </summary>
	/// <param name="unityInterface">Interface Class for Unity</param>
	public Main(AIInterfaceManager unityInterface){
		// setting unity interface class to reference
		this.unityInterface = unityInterface;
		// setting user-chosen depth for search
		depth = unityInterface.AIDepth;
		// obtaining copy of board
		Tile[,] boardCopy = unityInterface.AC_getCurrentBoard();
		// choose an action
		MovementData bestChoice = searchAlphaBeta(boardCopy);
		// submits the choice
		unityInterface.AC_submitChoice(bestChoice); 
	}

	// Methods ------------------------------------------------------------

	/// <summary>
	/// This method is responsible for looking at all the pieces for the AI in the current board 
	/// and calling the min-max seach for each.
	/// </summary>
	/// <param name="boardCopy">the current state of the board as a tile array</param>
	/// <returns>the best choice object</returns>
	private MovementData searchAlphaBeta(Tile[,] boardCopy) {
		// create a list of moves with their associated values for each piece on the board
		List<Choice> allChoices = new List<Choice>(); 
		// method call in foreach loop returns a list of all pieces
		foreach (Tile atile in piecesList(boardCopy)) {
			// if player team, skip and iterate again. Basically only iterate over AI team pieces
			if (atile.currentPiece.team == Team.Player) continue;
			//make a list of all movements possible for this piece
			List<MovementData> options = unityInterface.AC_getMovementOptions(atile, boardCopy);
			// for each movement option
			foreach (MovementData move in options) {
				// create a new board with movement of piece
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(move, boardCopy);
				// starting cutoff 1 layer down (due to already starting at the first option)
				int value = maxValue(newBoard, int.MinValue, int.MaxValue, 1);
				// create wrapper class holding score and decision
				Choice choice = new Choice(value, move);
				// add wrapper class to a list
				allChoices.Add(choice); 
            }
		}
		// return the index of the best-valued option
		int bestValIndex = findBest(allChoices);
		// get the best choice
		MovementData bestChoice = allChoices[bestValIndex].bestMove;
		//return choice with highest value
		return bestChoice; 
	}

	/// <summary>
	/// This method is responsible for finding the index of the best option in the provided list 
	/// of choices
	/// </summary>
	/// <param name="allChoices">list of choice objects</param>
	/// <returns>integer index</returns>
	private int findBest(List<Choice> allChoices) {
		// index value to return (default = 0)
		int index = 0;
		// best value to return (default = first choice's move value)
		int bestVal = allChoices[0].bestMoveValue;
		// counter variable to indicate what index we are at
		int counter = 0;
		// for each option in choices, find best value and set index
		foreach (Choice option in allChoices) {
			int value = option.bestMoveValue;
			if (value > bestVal) {
				bestVal = value;
				index = counter;
			}
			// need to increment counter after, otherwise not 0 indexing
			counter++; 
		}
		return index;
	}

	/// <summary>
	/// This method returns the max value of the next state
	/// </summary>
	/// <param name="boardCopy">current state of the board as a tile array</param>
	/// <param name="alpha">best alternative for value for max as an int</param>
	/// <param name="beta">best alternative for value for min as an int</param>
	/// <param name="cutOff">current depth to check as an int</param>
	/// <returns>highest min value integer as provided by minValue method</returns>
	private int maxValue(Tile[,] boardCopy, int alpha, int beta, int cutOff){
		// if we have reached the desired depth, return this state's score
        if (cutOff == depth) return unityInterface.AC_getScoreOfBoard(boardCopy);
		// Setting current value to negative inifinity
		int currentValue = int.MinValue;
		// method call in foreach loop returns a list of all pieces
		foreach (Tile atile in piecesList(boardCopy)) {
			// if player team, skip and iterate again. Basically only iterate over AI team pieces
			if (atile.currentPiece.team == Team.Player) continue;
			// make a list of all movements possible for this piece
			List<MovementData> options = unityInterface.AC_getMovementOptions(atile, boardCopy); 
			// foreach movement option
			foreach (MovementData choice in options) {
				// creating a new board after piece move
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(choice, boardCopy); 
				// get the next stat's minimum value 1 level deeper than current
				int nextVal = minValue(newBoard, alpha, beta, cutOff + 1);
				// if this min value is greater than the current value we have, update current value
				if (nextVal > currentValue) currentValue = nextVal;
				// if this min value is greater than or equal to the beta parameter (min), prune. Do not explore.
				if (nextVal >= beta) return currentValue;
				// if this min value is greater than the alpha parameter, update alpha.
				if (nextVal > alpha) alpha = nextVal;
			}
		}
		// if all else fails or we reach the end of all iterating, return the current value
		return currentValue;
	}

	/// <summary>
	/// This method returns the minimum value of the next state
	/// </summary>
	/// <param name="boardCopy">current state of the board as a tile array</param>
	/// <param name="alpha">best alternative for value for max as an int</param>
	/// <param name="beta">best alternative for value for min as an int</param>
	/// <param name="cutOff">current depth to check as an int</param>
	/// <returns>lowest max value integer as provided by maxValue method</returns>
	private int minValue(Tile[,] boardCopy, int alpha, int beta, int cutOff) {
		// if we have reached the desired depth, return this state's score
		if (cutOff == depth) return unityInterface.AC_getScoreOfBoard(boardCopy);
		// Setting current value to positive inifinity
		int currentValue = int.MaxValue;
		// method call in foreach loop returns a list of all pieces
		foreach (Tile tiles in piecesList(boardCopy)) {
			// if AI team, skip and iterate again. Basically only iterate over player team pieces
			if (tiles.currentPiece.team == Team.AI) continue;
			// make a list of all movements possible for this piece
			List<MovementData> options = unityInterface.AC_getMovementOptions(tiles, boardCopy);
			// foreach movement option
			foreach (MovementData choice in options) {
				// creating a new board after piece move
				Tile[,] newBoard = unityInterface.AC_getBoardAfterMovement(choice, boardCopy);
				// get the next stat's maximum value 1 level deeper than current
				int nextVal = maxValue(newBoard, alpha, beta, cutOff+1);
				// if this max value is less than the current value we have, update current value
				if (nextVal < currentValue) currentValue = nextVal;
				// if this max value is less than or equal to the alpha parameter (max), prune. Do not explore.
				if (nextVal <= alpha) return currentValue;
				// if this max value is less than the beta parameter, update beta.
				if (nextVal < beta) beta = nextVal;
			}
		}
		// if all else fails or we reach the end of all iterating, return the current value
		return currentValue;
	}

	/// <summary>
	/// This method is responsible for turning a board (2D tile array) into a new 1D list to 
	/// make it easier to read from
	/// </summary>
	/// <param name="arr">current board layout (2D tile array)</param>
	/// <returns></returns>
	private List<Tile> piecesList(Tile[,] arr) {
		// list to hold higher valued pieces
		List<Tile> highValList = new List<Tile>();
		// list to hold pawns (low valued pieces)
		List<Tile> lowValList = new List<Tile>();
		// iterating over everything and adding pawns to low list, rest to high list
		for (int i = 0; i < 8; i++) {
			for (int k = 0; k < 8; k++) {
				if (arr[i,k].currentPiece != null) {
					if(arr[i,k].currentPiece.type == PieceTypes.Pawn) {
						lowValList.Add(arr[i, k]);
					}
					else {
						highValList.Add(arr[i, k]);
					}
				} 
			}
		}
		// append low list to end of high list (yields pseudo sorted list)
		highValList.AddRange(lowValList);
		// return the list
		return highValList;
	}

	
	
}

/// <summary>
/// This is a wrapper class meant to hold the best a single movement data and its 
/// associated score value.
/// </summary>
class Choice {
	internal int bestMoveValue;
	internal MovementData bestMove;

	internal Choice(int value, MovementData move) {
		bestMove = move;
		bestMoveValue = value;
	}
}