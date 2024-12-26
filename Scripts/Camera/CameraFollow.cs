using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Tham chiếu đến nhân vật
    public float smoothSpeed = 0.125f; // Tốc độ mượt mà khi di chuyển camera
    public Vector3 offset; // Khoảng cách giữa camera và nhân vật

    private void LateUpdate()
    {
        if (player != null)
        {
            // Tính toán vị trí mục tiêu của camera
            Vector3 desiredPosition = new Vector3(player.position.x + offset.x, transform.position.y, player.position.z + offset.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Có thể thêm code để quay camera theo hướng nhân vật nếu cần
            transform.LookAt(player);
        }
    }
}
