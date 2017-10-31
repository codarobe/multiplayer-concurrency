using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using UnityEngine;


public class StateUpdateMessage
{

    public const int START_GAME = 0;
    public const int END_GAME = 1;
    public const int READY = 2;
    
    string identifier;

    private int updateType;

    public StateUpdateMessage(int type)
    {
        updateType = type;
    }

    public StateUpdateMessage(byte[] buffer)
    {
        MemoryStream ms = new MemoryStream(buffer);

        byte[] data = new byte[1024];

        ms.Read(data, 0, 4);

        int identifierLength = System.BitConverter.ToInt32(data, 0);

        ms.Read(data, 0, identifierLength);

        identifier = ASCIIEncoding.ASCII.GetString(data);

        ms.Read(data, 0, 4);
        updateType = System.BitConverter.ToInt32(data, 0);
    }

    public byte[] toByteArray()
    {
        MemoryStream ms = new MemoryStream(1024);

        ms.Write(System.BitConverter.GetBytes(MessageType.STATE_UPDATE), 0, 4);

        byte[] idBytes = Encoding.ASCII.GetBytes(identifier);
        int size = idBytes.Length;
        
        ms.Write(System.BitConverter.GetBytes(size), 0, 4);
        ms.Write(idBytes, 0, size);

        return ms.ToArray(); // gets contents regardless of 'position' location
    }

    public string getID()
    {
        return identifier;
    }

    public int getUpdateType()
    {
        return updateType;
    }
}
