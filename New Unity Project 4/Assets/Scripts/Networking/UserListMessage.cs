using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;

public class UserListMessage
{
	private string hostID;
	private string[] identifiers;
	private string[] ipAddresses;
	private int[] ports;
	private bool[] statuses;

	public UserListMessage(string[] ids, string[] ips, int[] portNumbers, bool[] statusArray)
	{
		identifiers = ids;
		ipAddresses = ips;
		ports = portNumbers;
		statuses = statusArray;
		hostID = NetworkConfiguration.playerName;
	}

	public UserListMessage(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);

		byte[] data = new byte[1024];

		// Read id length
		ms.Read(data, 0, 4);
		int hostIDLength = System.BitConverter.ToInt32(data, 0);
		Debug.Log("Read hostid of length: " + hostIDLength);

		// Read ID
		ms.Read(data, 0, hostIDLength);
		hostID = ASCIIEncoding.ASCII.GetString(data).TrimEnd('\0');
		Debug.Log("Read host id: " + hostID);

		// Read client count
		ms.Read(data, 0, 4);
		int clientCount = System.BitConverter.ToInt32(data, 0);
		Debug.Log("Client count: " + clientCount);

		identifiers = new string[clientCount];
		ipAddresses = new string[clientCount];
		ports = new int[clientCount];
		statuses = new bool[clientCount];

		// Read in clientCount ids, ips, and ports
		for (int i = 0; i < clientCount; i++)
		{
			ms.Read(data, 0, 4);
			int idLength = System.BitConverter.ToInt32(data, 0);
			ms.Read(data, 0, idLength);
			identifiers[i] = ASCIIEncoding.ASCII.GetString(data).TrimEnd('\0');

			ms.Read(data, 0, 4);
			int addressLength = System.BitConverter.ToInt32(data, 0);
			ms.Read(data, 0, addressLength);
			ipAddresses[i] = ASCIIEncoding.ASCII.GetString(data).TrimEnd('\0');

			ms.Read(data, 0, 4);
			ports[i] = System.BitConverter.ToInt32(data, 0);

			ms.Read(data, 0, 1);
			statuses[i] = System.BitConverter.ToBoolean(data, 0);

		}
		
	}

	public byte[] toByteArray()
	{
		MemoryStream ms = new MemoryStream(1024);

		// Write messsage type
		ms.Write(System.BitConverter.GetBytes(MessageType.USER_LIST), 0, 4);
		
		// Write Host ID Length and Host ID
		byte[] idData = Encoding.ASCII.GetBytes(hostID);
		ms.Write(System.BitConverter.GetBytes(idData.Length), 0, 4);
		ms.Write(idData, 0, idData.Length);

		
		// Write number of connections
		ms.Write(System.BitConverter.GetBytes(identifiers.Length), 0, 4);

		// Write identifiers, ips, and ports
		for (int i = 0; i < identifiers.Length; i++)
		{
			Debug.Log(identifiers[i]);
			byte[] data = Encoding.ASCII.GetBytes(identifiers[i]);
			ms.Write(System.BitConverter.GetBytes(data.Length), 0, 4);
			ms.Write(data, 0, data.Length);

			data = Encoding.ASCII.GetBytes(ipAddresses[i]);
			ms.Write(System.BitConverter.GetBytes(data.Length), 0, 4);
			ms.Write(data, 0, data.Length);
			
			ms.Write(System.BitConverter.GetBytes(ports[i]), 0, 4);
			
			ms.Write(System.BitConverter.GetBytes(statuses[i]), 0, 1);
		}

		return ms.ToArray(); // gets contents regardless of 'position' location
	}

	public string[] getIDs()
	{
		return identifiers;
	}

	public string[] getIPs()
	{
		return ipAddresses;
	}

	public int[] getPorts()
	{
		return ports;
	}

	public string getHostID()
	{
		return hostID;
	}
}