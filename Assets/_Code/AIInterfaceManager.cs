using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AIInterfaceManager : MonoBehaviour {

    private PossiblePositionManager positionManager;

	/// <summary>
    /// Called by the PossiblePositionManager once world generation has completed.
    /// Method handles starting up and running the AI
    /// </summary>
    internal void initialize (PossiblePositionManager positionManager)
    {
        //Save positions manager
        this.positionManager = positionManager;

        //Spin up thread;
        Thread t = new Thread(() => main_AI_Thread(this));
        t.IsBackground = true;
        t.Start();
    }

    /// <summary>
    /// Main thread that spins up and controls AI with interactions back to main Unity Systems
    /// </summary>
    /// <param name="parent">AIInterfaceManager for Unity Interactions</param>
    private static void main_AI_Thread (AIInterfaceManager parent)
    {
        
    }
}


