using UnityEngine;

public class ViveTriplet
{
    public Matrix4x4 Item1, Item2, Item3;
    public ViveTriplet(Matrix4x4 a, Matrix4x4 b, Matrix4x4 c)
    {
        Item1 = a; Item2 = b; Item3 = c;
    }
}

[System.Serializable]
public class ViveTripletSerializable
{
    public Matrix4x4Serializable Item1, Item2, Item3;
    public ViveTripletSerializable(Matrix4x4Serializable a, Matrix4x4Serializable b, Matrix4x4Serializable c)
    {
        Item1 = a; Item2 = b; Item3 = c;
    }
    public ViveTripletSerializable(ViveTriplet vt) : this(
        new Matrix4x4Serializable(vt.Item1), new Matrix4x4Serializable(vt.Item2), new Matrix4x4Serializable(vt.Item3)
    ) {}
}

public static class MyViveTriplet
{
    public static ViveTripletSerializable ConvertToSerializable(this ViveTriplet vt)
        => new ViveTripletSerializable(vt);
}