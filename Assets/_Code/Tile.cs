using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private BoardLocation localLocationData;
    [SerializeField]private Piece currentPiece;

    private Material material;
    private Color baseColour;
    private Color availableColour = Color.yellow;
    private float timer;

    /// <summary>
    /// Constructor
    /// </summary>
    internal void initialize(BoardLocation localLocationData, Piece currentPiece)
    {
        this.localLocationData = localLocationData;
        this.currentPiece = currentPiece;
        if (currentPiece != null)
        {
            currentPiece.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            currentPiece.gameObject.name = currentPiece.type.ToString();
        }

        material = GetComponent<Renderer>().material;
        baseColour = GetComponent<Renderer>().material.color;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (timer > 0)
        {
            material.SetColor("_Color", availableColour);
            timer -= Time.deltaTime;
        }
        else
        {
            material.SetColor("_Color", baseColour);
        }
    }

    /// <summary>
    /// Called to mark this tile as as possible send point for the selected Piece.
    /// Updates timer to mark as yellow for 5 seconds
    /// </summary>
    internal void markSelected ()
    {
        timer = 5f;
    }


    /// <summary>
    /// Returns the X position of this tile
    /// </summary>
    /// <returns></returns>
    internal int getXPosition() { return localLocationData.xPosition; }
    /// <summary>
    /// Returns the Y position of this tile
    /// </summary>
    /// <returns></returns>
    internal int getYPosition() { return localLocationData.yPosition; }
    /// <summary>
    /// Returns the piece using this tile
    /// </summary>
    /// <returns></returns>
    internal Piece getCurrentPiece () { return currentPiece; }
    /// <summary>
    /// Called to set the current Piece for this Tile
    /// </summary>
    /// <param name="p">Piece Class to assign to this tile</param>
    internal void setCurrentPiece (Piece p) { currentPiece = p; }
}
