using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health Bar Layout")]
    public Vector2 size = new Vector2(200, 20);
    public Vector2 offset = new Vector2(-10, -10);

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TileHighlighter tileHighlighter;

    private Image _fillImage;
    private Sprite _rectSprite;

    void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("❌ UIManager: PlayerStats not assigned in Inspector.");
            return;
        }

        // 1×1 white texture sprite
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _rectSprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f,
            0,
            SpriteMeshType.FullRect
        );

        // Create canvas
        var canvasGO = new GameObject("HealthBarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Background
        var bgGO = new GameObject("HealthBarBG", typeof(RectTransform), typeof(Image));
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = bgRT.anchorMax = new Vector2(1, 1);
        bgRT.pivot = new Vector2(1, 1);
        bgRT.anchoredPosition = offset;
        bgRT.sizeDelta = size;
        var bgImage = bgGO.GetComponent<Image>();
        bgImage.sprite = _rectSprite;
        bgImage.type = Image.Type.Simple;
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);

        // Fill
        var fillGO = new GameObject("HealthBarFill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(bgGO.transform, false);
        var fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        _fillImage = fillGO.GetComponent<Image>();
        _fillImage.sprite = _rectSprite;
        _fillImage.type = Image.Type.Filled;
        _fillImage.fillMethod = Image.FillMethod.Horizontal;
        _fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        _fillImage.color = Color.green;

        // Subscribe to health updates
        playerStats.OnHealthChanged += UpdateBar;
        UpdateBar(playerStats.CurrentHealth);
    }

    private void UpdateBar(int current)
    {
        float pct = (float)current / playerStats.maxHealth;
        _fillImage.fillAmount = Mathf.Clamp01(pct);
    }
}
