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

public enum Command2Id
{
    FrameStart = 1,
    UserOutCmd = 2,
    InitUserIdCmd = 3,
    CreateUserCmd = 4,
}
public static class CommonUtil
{
    public const string DataEndSplit = "        ";
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
    }

    public int GetUserId()
    {
        return __userId__;
    }
    public void SetUserId(int id)
    {
        this.__userId__ = id;
    }

    public virtual string Obj2Str()
    {
        return JsonUtility.ToJson(this);
    }
}
