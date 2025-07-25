using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class EnemyHealthBar : MonoBehaviour
{
    Slider slider;
    Camera cam;

    void Awake()
    {
        slider = GetComponent<Slider>();

        // Try to find a world-space Canvas first:
        var parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace && parentCanvas.worldCamera != null)
        {
            cam = parentCanvas.worldCamera;
        }
        else
        {
            // fall back to main camera
            cam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.forward = cam.transform.forward;
    }

    public void SetFraction(float frac)
    {
        slider.value = Mathf.Clamp01(frac);
    }
}
