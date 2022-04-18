using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMain
{
    public static FPSMain Instance { get; } = new FPSMain();
    public static int frameMaxCount = -1;
    public int frame { get; private set; }
    List<string> reciveDatas = new List<string>();

    internal void RemoveFPSObject(int uid)
    {
        FPSObject fPSObject = GetFPSObject(uid);
        if (fPSObject != null)
        {
            fPSObject.Destroy();
            fPSObjects[uid] = null;
        }
    }

    List<FPSObject> fPSObjects = new List<FPSObject>();
    public int userId { get; set; } = -1;
    public FPSObject GetFPSObject(int uid)
    {
        if (fPSObjects.Count > uid)
        {
            return fPSObjects[uid];
        }
        else
        {
            return null;
        }
    }
    public void InitFPS()
    {
        fPSObjects.Clear();
        frame = 0;
    }

    public void AddFPSObject(int id)
    {
        while(fPSObjects.Count <= id)
        {
            fPSObjects.Add(null);
        }
        if (fPSObjects[id] == null)
        {
            fPSObjects[id] = new FPSObject() { id = id, };
        }
    }

    public void DoActions()
    {
        while(true)
        {
            string data = GameManager.TcpClient.GetRecvStr();
            if (data == null)
            {
                break;
            }
            reciveDatas.Add(data);
            if (frameMaxCount > 0)
            {
                if (reciveDatas.Count >= frameMaxCount)
                {
                    break;
                }
            }

        }
        for (int i = 0; i < reciveDatas.Count; i ++)
        {
            try
            {
                CommandBase commandBase = CommandUtil.GetCommand(reciveDatas[i]);
                if (commandBase != null)
                {
                    commandBase.DoAction();
                }
                else
                {
                    Debug.LogWarning("unkonw cmd Id" + CommandUtil.GetCommandId(reciveDatas[i]));
                }
            }catch(Exception e)
            {
                Debug.LogError(reciveDatas[i]);
                Debug.LogError(e);
            }
        }
        reciveDatas.Clear();
    }

    public void AddFrame()
    {
        for (int i = 0; i < fPSObjects.Count; i ++)
        {
            if (fPSObjects[i] != null)
            {
                fPSObjects[i].DoAction();
            }
        }
        frame++;
        Debug.Log(frame);
    }
    
    public static void SendCommand(CommandBase commandBase)
    {
        commandBase.SetUserId(FPSMain.Instance.userId);
        GameManager.TcpClient.SocketSend(commandBase.Obj2Str());
    }
}
