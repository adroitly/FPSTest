using UnityEngine;

public class FrameStart : CommandBase
{
    public int logFrame = 0;
    public override Command2Id GetCommandId()
    {
        return Command2Id.FrameStart;
    }
}