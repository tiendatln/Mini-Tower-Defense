using UnityEngine;

public class GridPoint : MonoBehaviour
{
    [SerializeField] private Vector3Int cellPosition;

    public Vector3Int CellPosition => cellPosition;

    public Vector3 WorldPosition { get; private set; }

    private void Start()
    {
        Grid grid = GetComponentInParent<Grid>();

        if (grid == null)
        {
            Debug.LogError(
                $"GridPoint '{gameObject.name}' không tìm thấy Grid component trong parent!"
            );

            return;
        }

        WorldPosition = grid.GetCellCenterWorld(cellPosition);
        transform.position = WorldPosition;
    }
}
