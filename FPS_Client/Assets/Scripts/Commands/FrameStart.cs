using UnityEngine;

public class FrameStart : CommandBase
{
    public override Command2Id GetCommandId()
    {
        return Command2Id.FrameStart;
    }

    public override void DoAction()
    {
        base.DoAction();
        FPSMain.Instance.AddFrame();
    }
}