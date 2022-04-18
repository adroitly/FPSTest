using UnityEngine;

public class InitUserIdCmd : CommandBase
{
    public override Command2Id GetCommandId()
    {
        return Command2Id.InitUserIdCmd;
    }

    public override void DoAction()
    {
        base.DoAction();
        FPSMain.Instance.userId = GetUserId();
    }
}