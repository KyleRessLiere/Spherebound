using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera cam;

    [Header("Camera Framing")]

    [Tooltip("Shift right (+) or left (–) in world X")]
    [Range(-20f, 20f)]
    public float xOffset = 6.5f;

    [Tooltip("Camera height above the board")]
    [Range(1f, 30f)]
    public float height = 16f;

    [Tooltip("How far back the camera is pulled")]
    [Range(1f, 30f)]
    public float backDistance = 14f;

    [Tooltip("Yaw (left/right rotation)")]
    [Range(-180f, 180f)]
    public float yaw = -45f;

    [Tooltip("Pitch (up/down tilt)")]
    [Range(-90f, 90f)]
    public float pitch = 30f;

    [Tooltip("Field of view (FOV)")]
    [Range(10f, 100f)]
    public float fov = 38f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void OnValidate()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
        Apply();
    }

    void Apply()
    {
        // 🔒 Force perspective
        cam.orthographic = false;
        cam.fieldOfView = fov;
        Debug.Log("Camera forced to Perspective Mode");

        // Set camera position and rotation
        transform.position = new Vector3(xOffset, height, -backDistance);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
