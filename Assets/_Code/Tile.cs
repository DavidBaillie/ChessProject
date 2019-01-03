/// <summary>
/// Wrapper type class to act as a representation of a WorldTile when processing data 
/// using the AI. 
/// </summary>
public class Tile {

    //Board Location Data    
    internal int x;
    internal int y;
    //Piece being housed on this Tile
    internal Piece currentPiece;

    /// <summary>
    /// Constructor
    /// </summary>
    public Tile (Piece currentPiece, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.currentPiece = currentPiece;
    }
}
