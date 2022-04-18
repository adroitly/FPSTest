using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class TempCommand
{
    [SerializeField]
    private int __cmdId__;
    public int GetCmdId()
    {
        return __cmdId__;
    }
}
public abstract class CommandBase
{
    [SerializeField]
    private int __userId__;
    [SerializeField]
    private int __cmdId__;
    public abstract Command2Id GetCommandId();
    public CommandBase()
    {
        __cmdId__ = (int)GetCommandId();
        __userId__ = FPSMain.Instance.userId;
    }

    public int GetUserId()
    {
        return __userId__;
    }
    public void SetUserId(int id)
    {
        this.__userId__ = id;
    }
    public static CommandBase Str2Obj<T>(string json) where T : CommandBase
    {
        return JsonUtility.FromJson<T>(json);
        //return TinyJson.JSONParser.FromJson<T>(json);
    }

    public virtual string Obj2Str()
    {
        return JsonUtility.ToJson(this);
        //return TinyJson.JSONWriter.ToJson(this);
    }

    public FPSObject GetFPSObject()
    {
        return FPSMain.Instance.GetFPSObject(GetUserId());
    }

    public virtual void DoAction()
    {

    }
}
