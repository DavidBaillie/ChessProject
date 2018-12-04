using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private BoardLocation localLocationData;

    [SerializeField]
    private Piece currentPiece;

	/// <summary>
    /// Constructor
    /// </summary>
    internal void initialize (BoardLocation localLocationData, Piece currentPiece)
    {
        this.localLocationData = localLocationData;
        this.currentPiece = currentPiece;
        if (currentPiece != null)
        {
            currentPiece.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            currentPiece.gameObject.name = currentPiece.type.ToString();
        }            
    }
}
