using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathNode
{
    public int Id;
    public Vector3 CellPosition;

    // Nhập các Id node nối trực tiếp với node này trong Inspector.
    public List<int> NeighborIds = new();

    // Danh sách reference runtime được resolve từ NeighborIds.
    [NonSerialized]
    public List<PathNode> Neighbors = new();
}
