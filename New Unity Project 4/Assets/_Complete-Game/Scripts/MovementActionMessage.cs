using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;

public class MovementActionMessage
{
	float x;
	float y;

	int actionType;

    string identifier;

	public MovementActionMessage(float newX, float newY, int action, string id)
	{
		x = newX;
		y = newY;
		actionType = action;
        identifier = id;
	}

	public MovementActionMessage(byte[] buffer)
	{
		MemoryStream ms = new MemoryStream(buffer);

		byte[] data = new byte[1024];

		ms.Read(data, 0, 4);

		x = System.BitConverter.ToSingle(data, 0);

		ms.Read(data, 0, 4);

		y = System.BitConverter.ToSingle(data, 0);

		ms.Read(data, 0, 4);
        actionType = System.BitConverter.ToInt32(data, 0);

        ms.Read(data, 0, 4);
        int size = System.BitConverter.ToInt32(data, 0);

        ms.Read(data, 0, size);
        identifier = ASCIIEncoding.ASCII.GetString(data);
		
	}

	public byte[] toByteArray()
	{
		MemoryStream ms = new MemoryStream(1024);

		ms.Write(System.BitConverter.GetBytes(x), 0, 4);
		ms.Write(System.BitConverter.GetBytes(y), 0, 4);
		ms.Write(System.BitConverter.GetBytes(actionType), 0, 4);

        byte[] idBytes = Encoding.ASCII.GetBytes(identifier);
        int size = idBytes.Length;

        ms.Write(System.BitConverter.GetBytes(size), 0, 4);
        ms.Write(idBytes, 0, size);

		return ms.ToArray(); // gets contents regardless of 'position' location
	}

	public float getX()
	{
		return x;
	}

	public float getY()
	{
		return y;
	}

	public int getAction()
	{
		return actionType;
	}

    public string getID() {
        return identifier;
    }
}