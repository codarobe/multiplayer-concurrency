using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScenceonclick : MonoBehaviour {

    public void LoadByIndex(int sceneIndex)
    {
        if (sceneIndex == 1 && NetworkConfiguration.isHost)
        {
            NetworkConfiguration.networkController.startGame();
            SceneManager.LoadScene(sceneIndex);
        }
        else if (sceneIndex != 1)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void killNetworking()
    {
        Debug.Log("Queue disabled!");
        NetworkConfiguration.allowConnections = false;
        NetworkConfiguration.networkController.disconnectAll();
        NetworkConfiguration.networkController = null;
    }
}
