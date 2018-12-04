using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossiblePositionManager : MonoBehaviour {

    private Tile[,] tileDataArray;

    /// <summary>
    /// Called when the BoardPositionInitializer finishes spawning all tiles with pieces
    /// </summary>
    /// <param name="data">Array of all tiles on board</param>
    internal void  setTileDataArray (Tile[,] data)
    {
        tileDataArray = data;
    }
}
