using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour {

    internal int x;
    internal int y;
    [SerializeField]internal WorldPiece currentPiece;

    private Material material;

    /// <summary>
    /// Called to instanciate class data for later use
    /// </summary>
    /// <param name="x">X Coordinate of Tile</param>
    /// <param name="y">Y Coordinate of Tile</param>
    internal void instanciate (int x, int y, bool isBlack)
    {
        //Save coordinates
        this.x = x;
        this.y = y;

        //Save material and set colour
        material = GetComponent<Renderer>().material;
        if (isBlack) material.SetColor("_Color", Color.black);
    }
}
