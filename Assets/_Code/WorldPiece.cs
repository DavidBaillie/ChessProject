using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPiece : MonoBehaviour {

    internal Vector3 targetPosition;
    internal PieceTypes type;
    internal Team team;

    internal float movementSpeed;

    /// <summary>
    /// Called when the WorldPiece is created to assign needed data
    /// </summary>
    /// <param name="targetPosition">Position for the Piece to be at</param>
    /// <param name="type">Type of Piece this is</param>
    /// <param name="team">Team the Piece is on</param>
    /// <param name="movementSpeed">How fast the Piece moves</param>
    internal void instanciate (Vector3 targetPosition, PieceTypes type, Team team, float movementSpeed)
    {
        this.targetPosition = targetPosition;
        this.type = type;
        this.team = team;
        this.movementSpeed = movementSpeed;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);
        }
    }
}
