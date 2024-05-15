using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Matrix4x4SerializableList
{
    public List<Matrix4x4Serializable> matrixList = new List<Matrix4x4Serializable>();
    public Matrix4x4SerializableList() {}

    public Matrix4x4SerializableList(List<Matrix4x4> list)
    {
        foreach (Matrix4x4 matrix in list)
            matrixList.Add(new Matrix4x4Serializable(matrix));
    }
}