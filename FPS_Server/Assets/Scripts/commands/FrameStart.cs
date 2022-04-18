using UnityEngine;

public class FrameStart : CommandBase
{
    public int frame;
    public override Command2Id GetCommandId()
    {
        return Command2Id.FrameStart;
    }
}