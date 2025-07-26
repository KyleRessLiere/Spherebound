using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private RectTransform _fillRT;
    private Camera _cam;
    private float _maxWidth = 1.5f;

    public void Init(int currentHP, int maxHP)
    {
        _cam = Camera.main;

        // === Canvas ===
        var canvasGO = new GameObject("HealthBarCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasGO.transform.SetParent(transform, false);
        var canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.localPosition = Vector3.zero;
        canvasRT.localScale = Vector3.one * 0.5f;
        canvasRT.sizeDelta = new Vector2(_maxWidth, 0.25f);

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = _cam;
        canvas.sortingOrder = 10;

        canvasGO.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;

        // === Background ===
        var bg = new GameObject("BG", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgImg = bg.GetComponent<Image>();
        bgImg.color = Color.black;
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // === Red Fill ===
        var fill = new GameObject("Fill", typeof(Image));
        fill.transform.SetParent(bg.transform, false);
        var fillImg = fill.GetComponent<Image>();
        fillImg.color = Color.red;

        _fillRT = fill.GetComponent<RectTransform>();
        _fillRT.pivot = new Vector2(0, 0.5f); // align left
        _fillRT.anchorMin = new Vector2(0, 0.5f);
        _fillRT.anchorMax = new Vector2(0, 0.5f);
        _fillRT.anchoredPosition = Vector2.zero;
        _fillRT.sizeDelta = new Vector2(_maxWidth, 0.25f); // start full

        UpdateBar(currentHP, maxHP);
    }

    public void UpdateBar(int current, int max)
    {
        float pct = Mathf.Clamp01((float)current / max);
        _fillRT.sizeDelta = new Vector2(_maxWidth * pct, 0.25f);
    }

    public void FaceCamera()
    {
        if (_cam != null)
            transform.forward = _cam.transform.forward;
    }
}
