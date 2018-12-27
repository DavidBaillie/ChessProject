using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    internal Vector3 targetPosition;
    internal PieceTypes type;
    internal Team team;

    /// <summary>
    /// Constructor
    /// </summary>
    internal void initialize (Vector3 startPosition, PieceTypes type, Team team)
    {
        //Save data
        transform.position = startPosition + new Vector3(0, 0.5f, 0);
        this.type = type;
        targetPosition = transform.position;
        this.team = team;

        //Set colour to team
        if (team == 0)
            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        else
            GetComponent<Renderer>().material.SetColor("_Color", Color.black);
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2);
        }
    }
}
