using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Camera cam;

    [Header("Camera Mode")]
    public bool useOrthographic = true;
    [Tooltip("Size of the orthographic projection (half-height).")]
    public float orthoSize = 5f;

    [Header("Camera Framing")]
    [Tooltip("Shift right (+) or left (–) in world X")]
    public float xOffset = 5f;
    [Tooltip("Height above the board center")]
    public float height = 6f;
    [Tooltip("Backwards offset along world –Z")]
    public float backDistance = 4f;
    [Tooltip("Yaw around Y-axis (which edge faces you)")]
    public float yaw = -45f;
    [Tooltip("Pitch down (degrees)")]
    public float pitch = 35f;

    [Header("Perspective Lens (if not ortho)")]
    [Tooltip("Field of view when not in orthographic mode")]
    public float fov = 40f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
        Apply();
    }

    void Apply()
    {
        // 1) Set camera projection
        cam.orthographic = useOrthographic;
        if (useOrthographic)
            cam.orthographicSize = orthoSize;
        else
            cam.fieldOfView = fov;

        // 2) Position in world-space
        //    X = xOffset, Y = height, Z = –backDistance
        transform.position = new Vector3(xOffset, height, -backDistance);

        // 3) Rotate by pitch then yaw
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
