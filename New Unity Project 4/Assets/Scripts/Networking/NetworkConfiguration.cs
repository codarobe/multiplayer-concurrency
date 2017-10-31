using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkConfiguration {
	public static int localPort = -1;
	public static int remotePort = -1;
	public static string ipAddress = "";
	public static bool isConnected = false;

	public static string playerName = "Luke";

	public static NetworkManager networkManager = null;
}
