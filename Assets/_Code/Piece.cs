using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    internal Vector3 targetPosition;
    internal PieceTypes type;
    internal int team;

    /// <summary>
    /// Constructor
    /// </summary>
    internal void initialize (Vector3 startPosition, PieceTypes type, int team)
    {
        transform.position = startPosition + new Vector3(0, 0.5f, 0);
        this.type = type;
        targetPosition = transform.position;
        this.team = team;
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0)
        {
            transform.Translate(targetPosition - transform.position * Time.deltaTime);
        }
    }
}
