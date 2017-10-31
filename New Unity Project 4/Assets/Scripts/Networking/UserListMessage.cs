using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;

public class UserListMessage
{
	private string[] identifiers;
	private string[] ipAddresses;
	private int[] ports;

	public UserListMessage(string[] ids, string[] ips, int[] portNumbers)
	{
		identifiers = ids;
		ipAddresses = ips;
		ports = portNumbers;
	}

	public UserListMessage(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);

		byte[] data = new byte[1024];

		ms.Read(data, 0, 4);

		int clientCount = System.BitConverter.ToInt32(data, 0);

		identifiers = new string[clientCount];
		ipAddresses = new string[clientCount];
		ports = new int[clientCount];

		for (int i = 0; i < clientCount; i++)
		{
			ms.Read(data, 0, 4);
			int idLength = System.BitConverter.ToInt32(data, 0);
			ms.Read(data, 0, idLength);
			identifiers[i] = ASCIIEncoding.ASCII.GetString(data);

			ms.Read(data, 0, 4);
			int addressLength = System.BitConverter.ToInt32(data, 0);
			ms.Read(data, 0, addressLength);
			ipAddresses[i] = ASCIIEncoding.ASCII.GetString(data);

			ms.Read(data, 0, 4);
			ports[i] = System.BitConverter.ToInt32(data, 0);
		}
		
	}

	public byte[] toByteArray()
	{
		MemoryStream ms = new MemoryStream(1024);

		ms.Write(System.BitConverter.GetBytes(MessageType.USER_LIST), 0, 4);
		
		ms.Write(System.BitConverter.GetBytes(identifiers.Length), 0, 4);

		for (int i = 0; i < identifiers.Length; i++)
		{
			byte[] data = Encoding.ASCII.GetBytes(identifiers[i]);
			ms.Write(System.BitConverter.GetBytes(data.Length), 0, 4);
			ms.Write(data, 0, data.Length);

			data = Encoding.ASCII.GetBytes(ipAddresses[i]);
			ms.Write(System.BitConverter.GetBytes(data.Length), 0, 4);
			ms.Write(data, 0, data.Length);
			
			ms.Write(System.BitConverter.GetBytes(ports[i]), 0, 4);
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
}