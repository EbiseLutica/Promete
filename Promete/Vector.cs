using System;
using System.Numerics;

namespace Promete;

/// <summary>
/// 2Dのベクトルを表します。
/// </summary>
public struct Vector(float x, float y) : IEquatable<Vector>
{
    /// <summary>
    /// このベクトルのX座標を取得または設定します。
    /// </summary>
    public float X { get; set; } = x;

    /// <summary>
    /// このベクトルのY座標を取得または設定します。
    /// </summary>
    public float Y { get; set; } = y;

    /// <summary>
    /// このベクトルの長さを取得します。
    /// </summary>
    public float Magnitude => MathF.Sqrt(X * X + Y * Y);

    /// <summary>
    /// このベクトルの単位ベクトルを取得します。
    /// </summary>
    public Vector Normalized => (X / Magnitude, Y / Magnitude);

    public static Vector operator +(Vector v1, Vector v2)
    {
        return (v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
        return (v1.X - v2.X, v1.Y - v2.Y);
    }

    public static Vector operator *(Vector v1, float v2)
    {
        return (v1.X * v2, v1.Y * v2);
    }

    public static Vector operator *(Vector v1, Vector v2)
    {
        return (v1.X * v2.X, v1.Y * v2.Y);
    }

    public static Vector operator /(Vector v1, float v2)
    {
        return (v1.X / v2, v1.Y / v2);
    }

    public static Vector operator /(Vector v1, Vector v2)
    {
        return (v1.X / v2.X, v1.Y / v2.Y);
    }

    public static Vector operator -(Vector v1)
    {
        return (-v1.X, -v1.Y);
    }

    public static bool operator ==(Vector v1, Vector v2)
    {
        return v1.X == v2.X && v1.Y == v2.Y;
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
        return v1.X != v2.X || v1.Y != v2.Y;
    }

    public static explicit operator VectorInt(Vector v1)
    {
        return new VectorInt((int)v1.X, (int)v1.Y);
    }

    public static implicit operator Vector((float x, float y) v1)
    {
        return new Vector(v1.x, v1.y);
    }

    public static explicit operator Vector2(Vector v)
    {
        return new Vector2(v.X, v.Y);
    }

    /// <summary>
    /// Get angle between 2 vectors.
    /// </summary>
    /// <returns>Radian angle between 2 vectors.</returns>
    public static float Angle(Vector from, Vector to)
    {
        return MathF.Atan2(to.Y - from.Y, to.X - from.X);
    }


    /// <summary>
    /// Get the distance between 2 vectors.
    /// </summary>
    public static float Distance(Vector from, Vector to)
    {
        return MathF.Sqrt(
            MathF.Abs((to.X - from.X) * (to.X - from.X) + (to.Y - from.Y) * (to.Y - from.Y))
        );
    }

    /// <summary>
    /// Calculate a dot product.
    /// </summary>
    public static float Dot(Vector v1, Vector v2)
    {
        return v1.X * v1.Y + v2.X * v2.Y;
    }

    /// <summary>
    /// Calculate a dot product.
    /// </summary>
    public float Dot(Vector v)
    {
        return Dot(this, v);
    }

    /// <summary>
    /// Compare this object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Vector vector && Equals(vector);
    }

    /// <summary>
    /// Compare this object.
    /// </summary>
    public bool Equals(Vector other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    /// <summary>
    /// Get the hash value of this object.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <summary>
    /// Get angle of this vector.
    /// </summary>
    public float Angle()
    {
        return MathF.Atan2(Y, X);
    }


    /// <summary>
    /// Get the direction of the specified vector relative to this vector.
    /// </summary>
    public float Angle(Vector to)
    {
        return Angle(this, to);
    }

    /// <summary>
    /// Get the distance between two vectors.
    /// </summary>
    public float Distance(Vector to)
    {
        return Distance(this, to);
    }

    /// <summary>
    /// Check if this vector is in the specified range.
    /// </summary>
    public bool In(Rect rect)
    {
        var topLeft = rect.Location;
        var bottomRight = rect.Location + rect.Size - One;
        return X >= topLeft.X && X <= bottomRight.X &&
               Y >= topLeft.Y && Y <= bottomRight.Y;
    }

    /// <summary>
    /// Check if this vector is in the specified range.
    /// </summary>
    public bool In(Vector location, Vector size)
    {
        return In(new Rect(location, size));
    }

    /// <summary>
    /// Deconstructs x and y.
    /// </summary>
    public void Deconstruct(out float x, out float y)
    {
        (x, y) = (X, Y);
    }

    /// <summary>
    /// Get formatted string of this vector.
    /// </summary>
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    /// <summary>
    /// Get <c>new Vector(0, 0)</c> .
    /// </summary>
    public static readonly Vector Zero = (0, 0);

    /// <summary>
    /// Get <c>new Vector(1, 1)</c> .
    /// </summary>
    public static readonly Vector One = (1, 1);

    /// <summary>
    /// Get <c>new Vector(-1, 0)</c> .
    /// </summary>
    public static readonly Vector Left = (-1, 0);

    /// <summary>
    /// Get <c>new Vector(0, -1)</c> .
    /// </summary>
    public static readonly Vector Up = (0, -1);

    /// <summary>
    /// Get <c>new Vector(1, 0)</c> .
    /// </summary>
    public static readonly Vector Right = (1, 0);

    /// <summary>
    /// Get <c>new Vector(0, 1)</c> .
    /// </summary>
    public static readonly Vector Down = (0, 1);

    public static Vector From(Vector2 vec)
    {
        return (vec.X, vec.Y);
    }
}
