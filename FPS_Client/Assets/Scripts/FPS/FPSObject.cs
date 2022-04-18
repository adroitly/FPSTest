using FixMath;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TransformData
{
    private GameObject gameObject;
    public Fix64 speed { get; set; }
    public Vec3 position { get; private set; }
    public TransformData()
    {
        speed = 1.ToFix();
    }
    public TransformData(GameObject go)
    {
        gameObject = go;
        speed = 2.ToFix();
    }
    public void SetPosition(Vec3 vec3)
    {
        position = vec3;
        gameObject.transform.position = vec3.ToVector3;
    }
}
public interface ActionBase
{

}
public interface UpdateActionBase : ActionBase
{
    public void DoAction(FPSObject fPSObject);
}
class MoveTo : UpdateActionBase
{
    public Vec3 targetPosition;
    public Fix64 time;
    public Fix64 curTime;
    public void DoAction(FPSObject fPSObject)
    {
        curTime += TimeMgr.fixTime;
        TransformData transformData = fPSObject.transformData;
        if (curTime > time)
        {
            transformData.SetPosition(targetPosition);
            return;
        }
        var forward = (targetPosition - transformData.position).normalized;
        var pos = transformData.position + forward * transformData.speed * TimeMgr.fixTime;
        transformData.SetPosition(pos);
    }
}
public class FPSObject
{
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }
    public TransformData transformData { get; set; }
    public int id { get; set; }
    private Vec3 potision = Vec3.zero;
    private MoveTo moveTo;


    public FPSObject()
    {
        var asset = Resources.Load<GameObject>("Gos/TestFPSObject");
        gameObject = GameObject.Instantiate(asset);
        transform = gameObject.transform;
        transformData = new TransformData(gameObject);
    }

    public void DoAction()
    {
        gameObject.name = id.ToString();
        if (moveTo != null)
        {
            moveTo.DoAction(this);
        }
    }

    internal void MoveTo(Vec3 targetPosition)
    {
        moveTo = new MoveTo() {
            targetPosition = targetPosition,
            time = (targetPosition - transformData.position).distance,
        };
    }

    internal void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
