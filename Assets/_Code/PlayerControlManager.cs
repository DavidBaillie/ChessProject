using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour {

    private PossiblePositionManager positionsManager;

    /// <summary>
    /// Called at scene start
    /// </summary>
    private void Awake()
    {
        positionsManager = GetComponent<PossiblePositionManager>();
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        //Mouse left click event
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log(hit.collider.gameObject.GetComponent<Tile>());
                Tile current = hit.collider.gameObject.GetComponent<Tile>();
                if (current.getCurrentPiece() == null) { Debug.Log("Null Tile Clicked"); return; }
                List<Tile> options = 
                    positionsManager.getPlayerPossibleTiles(current.getXPosition(), current.getYPosition(), current.getCurrentPiece().type);
                foreach (Tile t in options)
                {
                    Debug.Log("Options: " + t.getXPosition() + "/" + t.getYPosition());
                }
            }
        }
    }
}
