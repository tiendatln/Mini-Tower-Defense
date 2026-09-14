using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PathGraph",
    menuName = "Mini Tower Defense/Path Graph")]
public class PathGraphAsset : ScriptableObject
{
    [SerializeField] private List<PathNode> nodes = new();

    public IReadOnlyList<PathNode> Nodes => nodes;
}
