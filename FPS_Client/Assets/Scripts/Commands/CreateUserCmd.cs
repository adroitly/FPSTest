using UnityEngine;

public class CreateUserCmd : CommandBase
{
    public override Command2Id GetCommandId()
    {
        return Command2Id.CreateUserCmd;
    }
    public override void DoAction()
    {
        FPSMain.Instance.AddFPSObject(GetUserId());
    }
}