using FixMath;
using UnityEngine;

public static partial class Extention
{
    public static Fix64 ToFix(this int v)
    {
        return new Fix64(v);
    }

    public static Fix64 ToFix(this float v)
    {
        int d = (int)(v * 1000);
        return new Fix64(d) / new Fix64(1000);
    }

    public static Vec3 ToVec3(this Vector3 vector)
    {
        return new Vec3(vector.x, vector.y, vector.z);
    }
}