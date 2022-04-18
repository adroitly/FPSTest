using FixMath;
using UnityEngine;

public enum Command2Id
{
    FrameStart = 1,
    UserOutCmd = 2,
    InitUserIdCmd = 3,
    CreateUserCmd = 4,

    MoveToCmd = 999,
}

public static class TimeMgr
{
    public static Fix64 targetRate = new Fix64(15);
    public static Fix64 fixTime = new Fix64(1) / targetRate;
}

public static class CommandUtil
{
    public const string DataEndSplit = "        ";
    public static int GetCommandId(string data)
    {
        TempCommand obj = JsonUtility.FromJson<TempCommand>(data);
        return obj.GetCmdId();
    }
    public static CommandBase GetCommand(string data)
    {
        TempCommand obj = JsonUtility.FromJson<TempCommand>(data);
        int cmdId = obj.GetCmdId();
        Command2Id command2Id = (Command2Id)cmdId;
        switch (command2Id)
        {
            case Command2Id.MoveToCmd:
                return CommandBase.Str2Obj<MoveToCmd>(data);
            case Command2Id.FrameStart:
                return CommandBase.Str2Obj<FrameStart>(data);
            case Command2Id.InitUserIdCmd:
                return CommandBase.Str2Obj<InitUserIdCmd>(data);
            case Command2Id.UserOutCmd:
                return CommandBase.Str2Obj<UserOutCmd>(data);
            case Command2Id.CreateUserCmd:
                return CommandBase.Str2Obj<CreateUserCmd>(data);
            default:
                return default;
        }
    }
}