using UnityEngine;

public class PlayerMenuUI : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset = new Vector3(1f, 1.5f, 0f); // adjust as needed

    void LateUpdate()
    {
        if (playerTransform == null || Camera.main == null)
            return;

        // Position relative to player
        transform.position = playerTransform.position + offset;

        // Face the camera
        Vector3 dir = transform.position - Camera.main.transform.position;
        dir.y = 0; // Optional: keep it upright
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
