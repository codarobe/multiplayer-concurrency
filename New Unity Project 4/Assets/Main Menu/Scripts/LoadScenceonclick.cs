using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScenceonclick : MonoBehaviour {

    public void LoadByIndex(int sceneIndex)
    {
        if (sceneIndex == 1) {
            string ip = GameObject.Find("Multiplayer/SettingsWindow/IPInput").GetComponent<InputField>().text;
            if (ip != "") {
                NetworkConfiguration.ipAddress = ip;
            }
            string portInput = GameObject.Find("Multiplayer/SettingsWindow/RemotePortNum").GetComponent<InputField>().text;
            int port;
            if (Int32.TryParse(portInput, out port)) {
                NetworkConfiguration.localPort = port;
            }
            portInput = GameObject.Find("Multiplayer/SettingsWindow/LocalPortNum").GetComponent<InputField>().text;
            if (Int32.TryParse(portInput, out port)) {
                NetworkConfiguration.remotePort = port;
            }
            Debug.Log("ip: " + ip);
            Debug.Log("port: " + port);
        }
        SceneManager.LoadScene(sceneIndex);
    }
}
