using FixMath;
using System;
using UnityEngine;

[Serializable]
public struct Vec3
{
    public Fix64 x;
    public Fix64 y;
    public Fix64 z;
    public static readonly Vec3 zero = new Vec3(0, 0, 0);
    public Fix64 distance
    {
        get
        {
            return Fix64.Sqrt(x * x + y * y + z * z);
        }
    }

    public Vec3 normalized
    {
        get
        {
            Fix64 len = Fix64.Sqrt(x * x + y * y + z * z);
            if (len == Fix64.Zero)
            {
                return Vec3.one;
            }
            return new Vec3(x / len, y / len, z / len);
        }
    }

    public Vec3(Fix64 x, Fix64 y, Fix64 z)
    {
        this.x = x;
        this.z = z;
        this.y = y;
    }
    public Vec3(Vector3 vector)
    {
        this.x = vector.x.ToFix();
        this.y = vector.y.ToFix();
        this.z = vector.z.ToFix();
    }

    public Vec3(float x = 0, float y = 0, float z = 0)
    {
        this.x = x.ToFix();
        this.z = z.ToFix();
        this.y = y.ToFix();
    }

    public static Vec3 operator +(Vec3 vec1, Vec3 vec2)
    {
        return new Vec3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
    }
    public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
    {
        return new Vec3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
    }

    public static Vec3 operator *(Vec3 vec1, Fix64 r)
    {
        return new Vec3(vec1.x * r, vec1.y * r, vec1.z * r);
    }

    public static Vec3 operator /(Vec3 vec1, Fix64 r)
    {
        return new Vec3(vec1.x / r, vec1.y / r, vec1.z / r);
    }

    public static Vec3 operator *(Vec3 vec1, int r)
    {
        Fix64 fix = r.ToFix();
        return new Vec3(vec1.x * fix, vec1.y * fix, vec1.z * fix);
    }

    public static Vec3 operator /(Vec3 vec1, int r)
    {
        Fix64 fix = r.ToFix();
        return new Vec3(vec1.x / fix, vec1.y / fix, vec1.z / fix);
    }

    public override string ToString()
    {
        return string.Format("x:{0}, y:{1}, z:{2}", (float)x, (float)y, (float)z);
    }

    public Vector3 ToVector3
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public static Vec3 up
    {
        get
        {
            return new Vec3(0, 1, 0);
        }
    }

    public static Vec3 down
    {
        get
        {
            return new Vec3(0, -1, 0);
        }
    }

    public static Vec3 left
    {
        get
        {
            return new Vec3(-1, 0, 0);
        }
    }

    public static Vec3 one 
    {
        get
        {
            return new Vec3(1, 1, 1);
        }
    }
    public static Vec3 right
    {
        get
        {
            return new Vec3(1, 0, 0);
        }
    }
}