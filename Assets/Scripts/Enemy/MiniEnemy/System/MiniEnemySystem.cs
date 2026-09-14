using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct MiniEnemySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Lấy thời gian đã trôi qua kể từ frame trước.
        var deltaTime = SystemAPI.Time.DeltaTime;

        // Tìm entity singleton đang lưu tham chiếu đến GridManager.
        if (!SystemAPI.TryGetSingleton<TargetMapComponent>(out var mapComponent))
        {
            // Ghi log nếu chưa tìm thấy GridManager trong ECS.
            Debug.Log("map component not found!");
        }

        // Lấy đối tượng GridManager từ singleton.
        GridManager gridManager = mapComponent.MapRef.Value;

        // Dictionary lưu path theo số thứ tự của graph.
        Dictionary<int, List<Vector3>> SPFAPathList = new();

        // Bắt đầu đánh số graph từ key 1.
        int i = 1;

        // Duyệt qua tất cả path graph được cấu hình trong GridManager.
        foreach (var graph in gridManager.pathGraphAsset)
        {
            // Bỏ qua graph không tồn tại hoặc không có node.
            if (graph == null || graph.Nodes == null || graph.Nodes.Count == 0)
            {
                // Ghi cảnh báo để biết graph nào không hợp lệ.
                Debug.LogWarning("Bỏ qua path graph rỗng.");
                // Tăng key để graph tiếp theo không bị trùng chỉ số.
                i++;
                continue;
            }

            // Chuyển danh sách node chỉ đọc của asset thành List<PathNode>.
            List<PathNode> nodes = graph.Nodes.ToList();

            // Tìm đường ngắn nhất từ node đầu tiên đến node cuối cùng bằng SPFA.
            SPFAPathList.Add(
                i,
                gridManager.FindPath(
                    // Danh sách node của graph hiện tại.
                    nodes,
                    // Vị trí bắt đầu là node đầu tiên.
                    nodes.First().CellPosition,
                    // Vị trí đích là node cuối cùng.
                    nodes.Last().CellPosition));

            // Tăng key cho graph tiếp theo.
            i++;
        }

        // Lấy path của graph có key 1 và kiểm tra path có waypoint hay không.
        if (!SPFAPathList.TryGetValue(1, out List<Vector3> path) || path.Count == 0)
        {
            // Dừng system nếu không có path để enemy di chuyển.
            Debug.LogWarning("Không tìm thấy path cho graph 1.");
            return;
        }

        // In số lượng waypoint của path để kiểm tra kết quả SPFA.
        Debug.Log($"Graph 1 path: {path.Count} nodes");

        // Lấy tất cả entity có LocalTransform và MimiEnemyComponent.
        foreach (var (transform, EnemyComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MimiEnemyComponent>>())
        {
            // Lấy index waypoint mà enemy đang hướng tới.
            int index = EnemyComponent.ValueRO.PathIndex;

            // Nếu đã đi hết waypoint thì không cần di chuyển nữa.
            if (index >= path.Count)
                continue;

            // Lấy vị trí waypoint tiếp theo trên path.
            float3 targetPosition = path[index];

            // Lấy vị trí hiện tại của enemy.
            float3 currentPosition =
                transform.ValueRO.Position;

            // Tính vector từ enemy đến waypoint tiếp theo.
            float3 offset = targetPosition - currentPosition;
            // Tính khoảng cách còn lại đến waypoint.
            float distance = math.length(offset);
            // Tính quãng đường enemy có thể đi trong frame hiện tại.
            float movement = EnemyComponent.ValueRO.MoveSpeed * (float)deltaTime;

            // Nếu đã đủ gần hoặc bước di chuyển vượt qua waypoint.
            if (distance <= movement || distance < 0.05f)
            {
                // Đặt enemy chính xác vào waypoint, tránh bị vượt qua.
                transform.ValueRW.Position = targetPosition;
                // Chuyển sang waypoint tiếp theo ở frame sau.
                EnemyComponent.ValueRW.PathIndex++;
                // Kết thúc xử lý enemy hiện tại.
                continue;
            }

            // Chuẩn hóa hướng rồi di chuyển enemy theo tốc độ trong frame này.
            transform.ValueRW.Position += math.normalizesafe(offset) * movement;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
