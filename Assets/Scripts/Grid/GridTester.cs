using System.Text;
using UnityEngine;

public class GridTester : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private void Start()
    {
        StringBuilder grid = new StringBuilder();

        for (int x = -6; x <= 6; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileType tileType = gridManager.GetTileType(position);

                string tileName = tileType.ToString();
                grid.Append(string.IsNullOrEmpty(tileName) ? '?' : tileName[0]);
            }

            grid.AppendLine();
        }

        Debug.Log($"Grid:\n{grid}");
    }
}
