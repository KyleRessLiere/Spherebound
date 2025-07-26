using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HealthBarUI : MonoBehaviour
{
    PlayerStats _playerStats;
    Image _fillImage;

    void Awake()
    {
        // 1) grab the stats
        _playerStats = FindObjectOfType<PlayerStats>();
        if (_playerStats == null)
        {
            Debug.LogError("HealthBarUI: no PlayerStats in scene");
            enabled = false;
            return;
        }

        // 2) build a Screen‑Space Overlay canvas
        var canvasGO = new GameObject("HealthBarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Fix: stretch canvas RectTransform to full screen
        var canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.anchorMin = Vector2.zero;
        canvasRT.anchorMax = Vector2.one;
        canvasRT.offsetMin = Vector2.zero;
        canvasRT.offsetMax = Vector2.zero;

        // 3) background container (anchored top-right)
        var bg = new GameObject("HealthBarBG", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(1, 1);  // top-right
        bgRT.anchorMax = new Vector2(1, 1);  // top-right
        bgRT.pivot = new Vector2(1, 1);      // pivot also top-right
        bgRT.anchoredPosition = new Vector2(-10, -10); // offset from top-right
        bgRT.sizeDelta = new Vector2(200, 20);
        bg.GetComponent<Image>().color = Color.gray;

        // 4) fill bar
        var fg = new GameObject("HealthBarFill", typeof(RectTransform), typeof(Image));
        fg.transform.SetParent(bg.transform, false);
        var fgRT = fg.GetComponent<RectTransform>();
        fgRT.anchorMin = Vector2.zero;
        fgRT.anchorMax = Vector2.one;
        fgRT.offsetMin = fgRT.offsetMax = Vector2.zero;
        _fillImage = fg.GetComponent<Image>();
        _fillImage.type = Image.Type.Filled;
        _fillImage.fillMethod = Image.FillMethod.Horizontal;
        _fillImage.color = Color.red;

        // 5) subscribe & init
        _playerStats.OnHealthChanged += UpdateBar;
        UpdateBar(_playerStats.CurrentHealth);
    }

    void UpdateBar(int hp)
    {
        float pct = (float)hp / _playerStats.maxHealth;
        _fillImage.fillAmount = Mathf.Clamp01(pct);
    }
}
