using UnityEngine;

public class MoveToCmd : CommandBase
{
    public Vec3 targetPosition;
    public override Command2Id GetCommandId()
    {
        return Command2Id.MoveToCmd;
    }

    public override void DoAction()
    {
        base.DoAction();
        GetFPSObject().MoveTo(targetPosition);
    }
}