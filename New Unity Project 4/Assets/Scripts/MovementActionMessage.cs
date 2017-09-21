using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class MovementActionMessage {
	float x;
	float y;

	int actionType;

	public MovementActionMessage(float newX, float newY, int action) {
		x = newX;
		y = newY;
		actionType = action;
	}

	public MovementActionMessage(byte[] buffer) {
        MemoryStream ms = new MemoryStream(buffer);

        byte[] data = new byte[4];

        ms.Read(data, 0, 4);

        x = System.BitConverter.ToSingle(data, 0);

        ms.Read(data, 0, 4);

        y = System.BitConverter.ToSingle(data, 0);

        ms.Read(data, 0, 4);

        actionType = System.BitConverter.ToInt32(data, 0);
	}

	public byte[] toByteArray() {
        MemoryStream ms = new MemoryStream(1024);

        ms.Write(System.BitConverter.GetBytes(x), 0, 4);
        ms.Write(System.BitConverter.GetBytes(y), 0, 4);
        ms.Write(System.BitConverter.GetBytes(actionType), 0, 4);

        return ms.ToArray(); // gets contents regardless of 'position' location
	}

	public float getX() {
		return x;
	}

	public float getY() {
		return y;
	}

	public float getAction() {
		return actionType;
	}
}
