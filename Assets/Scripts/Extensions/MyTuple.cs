using System;
using UnityEngine;
public static class MyTuple
{
    public static Tuple<Matrix4x4Serializable, Matrix4x4Serializable, Matrix4x4Serializable> ConvertToSerializable(
        this Tuple<Matrix4x4, Matrix4x4, Matrix4x4> orig)
        => new Tuple<Matrix4x4Serializable, Matrix4x4Serializable, Matrix4x4Serializable>
                (   new Matrix4x4Serializable(orig.Item1),
                    new Matrix4x4Serializable(orig.Item2),
                    new Matrix4x4Serializable(orig.Item3) );
}