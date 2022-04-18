using UnityEngine;

public class UserOutCmd : CommandBase
{
    public override Command2Id GetCommandId()
    {
        return Command2Id.UserOutCmd;
    }
}