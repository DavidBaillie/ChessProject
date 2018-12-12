using System.Collections;
using System.Collections.Generic;

public class Main {

    //Class used to interface with Unity Code
    private AIInterfaceManager unityInterface;

    //CAMERON - Store the coordinates for your decision here!
    //Used for storing the Tiles to move
    private int startXPosition;
    private int startYPosition;
    private int endXPosition;
    private int endYPosition;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="unityInterface">Interface Class for Unity</param>
	public Main (AIInterfaceManager unityInterface)
    {
        this.unityInterface = unityInterface;
        TileData[,] startingBoard = unityInterface.getCurrentBoard();

        //Cameron Code Here


    }

    /// <summary>
    /// Called once the Main class constructor has finished running, ie the AI has run it's 
    /// course and has made a decision. Will return the piece position data to be used.
    /// </summary>
    /// <returns>FinalData struct</returns>
    internal FinalData getFinalData () { return new FinalData(startXPosition, startYPosition, endXPosition, endYPosition); }
}

/// <summary>
/// Data struct used to return all needed data from the AI when a decision has been made
/// </summary>
internal struct FinalData
{
    internal int startXPosition;
    internal int startYPosition;
    internal int endXPosition;
    internal int endYPosition;

    /// <summary>
    /// Constructor
    /// </summary>
    internal FinalData (int x1, int y1, int x2, int y2)
    {
        startXPosition = x1;
        startYPosition = y1;
        endXPosition = x2;
        endYPosition = y2;
    }
}