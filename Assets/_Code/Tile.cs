using UnityEngine;

/// <summary>
/// Class attached to Unity GameObject representing a Tile on the Game Board. 
/// Tile class tracks where it is, what visual indicators to display, and what Piece can be found 
/// at this location/Tile.
/// </summary>
public class Tile : MonoBehaviour {

    //Board Location Data    
    private GameObject tileObject;
    private int xPosition;
    private int yPosition;

    //Piece being housed on this Tile
    [SerializeField]internal Piece currentPiece;

    //Unity facing stuff
    private Material material;
    private Color baseColour;
    private Color availableColour = Color.yellow;
    private float timer;

    /// <summary>
    /// Constructor
    /// </summary>
    internal void initialize(Piece currentPiece, int xPosition, int yPosition, GameObject tileObject)
    {
        //Save Tile position data
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        this.tileObject = tileObject;

        //Save Piece data for piece on tile
        this.currentPiece = currentPiece;

        //Unity set up for the Piece on this tile - only runs once at start
        if (currentPiece != null)
        {
            currentPiece.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            currentPiece.gameObject.name = currentPiece.type.ToString();
        }

        /*
        //Set Material component data
        material = GetComponent<Renderer>().material;
        if (material == null)
        {
            Debug.Log("CODE ERROR - Null Reference - GetComponent failed to find Renderer on GameObject " + this.gameObject.name);
            return;
        }
        baseColour = GetComponent<Renderer>().material.color;
        */
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        /*
        if (timer > 0)
        {
            material.SetColor("_Color", availableColour);
            timer -= Time.deltaTime;
        }
        else
        {
            material.SetColor("_Color", baseColour);
        }
        */
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
    internal int getXPosition() { return xPosition; }

    /// <summary>
    /// Returns the Y position of this tile
    /// </summary>
    /// <returns></returns>
    internal int getYPosition() { return yPosition; }

    /// <summary>
    /// Returns the GameObject for this Tile
    /// </summary>
    /// <returns>GameObject</returns>
    internal GameObject getTileObject() { return tileObject; }


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
