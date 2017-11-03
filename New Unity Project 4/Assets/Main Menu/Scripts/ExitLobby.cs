using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExitLobby {

	public static void exitLobby()
	{
		NetworkConfiguration.allowConnections = false;
		NetworkConfiguration.networkManager.disconnectAll();
		NetworkConfiguration.networkManager = null;
	}
}
