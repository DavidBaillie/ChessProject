/// <summary>
/// Wrapper class used to represent the WorldPiece class when AI is processing data.
/// </summary>
public class Piece {

    internal PieceTypes type;
    internal Team team;

    /// <summary>
    /// Constructor
    /// </summary>
    internal Piece (PieceTypes type, Team team)
    {
        this.type = type;
        this.team = team;
    }
}
