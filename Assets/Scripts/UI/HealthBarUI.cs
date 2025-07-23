// HealthBarUI.cs
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
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // 3) background
        var bg = new GameObject("HealthBarBG", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = bgRT.anchorMax = new Vector2(1, 1);
        bgRT.pivot = new Vector2(1, 1);
        bgRT.anchoredPosition = new Vector2(-10, -10);
        bgRT.sizeDelta = new Vector2(200, 20);
        bg.GetComponent<Image>().color = Color.gray;

        // 4) fill
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
        Debug.Log($"[HealthBarUI] setting fill to {pct:0.00}");
        _fillImage.fillAmount = Mathf.Clamp01(pct);
    }
}
