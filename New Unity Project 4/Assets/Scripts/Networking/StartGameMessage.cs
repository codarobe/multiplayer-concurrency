

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;

public class StartGameMessage {
	private string[] identifiers;
	private int[] spawns;

	private int hostSpawn;

	public StartGameMessage(string[] ids, int[] spawnPoints, int hostSpawnID)
	{
		identifiers = ids;
		spawns = spawnPoints;
		hostSpawn = hostSpawnID;
	}

	public StartGameMessage(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);

		byte[] data = new byte[2048];

		// Read host spawn
		ms.Read(data, 0, 4);
		hostSpawn = System.BitConverter.ToInt32(data, 0);

		// Read ID Count
		ms.Read(data, 0, 4);
		int idCount = System.BitConverter.ToInt32(data, 0);

		identifiers = new string[idCount];
		spawns = new int[idCount];

		for (int i = 0; i < idCount; i++)
		{
			ms.Read(data, 0, 4);
			int idLength = System.BitConverter.ToInt32(data, 0);
			ms.Read(data, 0, idLength);
			identifiers[i] = ASCIIEncoding.ASCII.GetString(data).TrimEnd('\0');

			ms.Read(data, 0, 4);
			spawns[i] = System.BitConverter.ToInt32(data, 0);
			
		}
		
	}

	public byte[] toByteArray()
	{
		MemoryStream ms = new MemoryStream(2048);

		// Write message type
		ms.Write(System.BitConverter.GetBytes(MessageType.START_GAME), 0, 4);
		
		// Write host spawn
		ms.Write(System.BitConverter.GetBytes(hostSpawn), 0, 4);
		
		// Write identifier type
		ms.Write(System.BitConverter.GetBytes(identifiers.Length), 0, 4);

		for (int i = 0; i < identifiers.Length; i++)
		{
			// Write id length and id
			byte[] data = Encoding.ASCII.GetBytes(identifiers[i]);
			ms.Write(System.BitConverter.GetBytes(data.Length), 0, 4);
			ms.Write(data, 0, data.Length);

			// Write spawn
			ms.Write(System.BitConverter.GetBytes(spawns[i]), 0, 4);
			
		}

		// return the byte array
		return ms.ToArray();
	}

	public string[] getIDs()
	{
		return identifiers;
	}



	public int[] getSpawns()
	{
		return spawns;
	}

	public int getHostSpawn()
	{
		return hostSpawn;
	}

}


