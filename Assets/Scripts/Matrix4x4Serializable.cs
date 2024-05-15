using UnityEngine;

[System.Serializable]
public class Matrix4x4Serializable
{
    public float[] data = new float[16];
    public Matrix4x4Serializable() {}
    public Matrix4x4Serializable(Matrix4x4 matrix)
    {
        for (int i = 0; i < 16; i++)
            data[i] = matrix[i];
    }
}