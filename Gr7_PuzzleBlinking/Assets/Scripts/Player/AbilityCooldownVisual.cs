using UnityEngine;
using UnityEngine.UIElements;

public class AbilityCooldownVisual : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 5.0f;
    private UIDocument ui;
    private VisualElement overlay;
    private float timer = 0.0f;
    private bool isOnCooldown = false;
    private float progress = 0f;

    private bool isPlayer1 = true;
    [SerializeField] private Sprite freezeTimeIcon0;
    [SerializeField] private Sprite freezeTimeIcon1;
    [SerializeField] private Sprite changeTimeIcon0;
    [SerializeField] private Sprite changeTimeIcon1;

    void Start()
    {
        ui = GetComponent<UIDocument>();
        overlay = ui.rootVisualElement.Q<VisualElement>("CooldownOverlay");
        
        if (overlay == null)
        {
            Debug.LogError("CooldownOverlay not found in UXML");
            enabled = false;
            return;
        }

        // Register mesh generation callback
        overlay.generateVisualContent += OnGenerateVisualContent;
        overlay.MarkDirtyRepaint();
    }

    void Update()
    {
        // Input checks belong in Update, not FixedUpdate
        if (Input.GetKeyDown(KeyCode.Q) && !isOnCooldown)
            StartCooldown();

        if (isOnCooldown)
        {
            timer -= Time.deltaTime; // now matches Update
            progress = Mathf.Clamp01(timer / cooldownTime);
            overlay.MarkDirtyRepaint(); // trigger redraw
            
            if (timer <= 0)
            {
                isOnCooldown = false;
                progress = 0f;
                overlay.MarkDirtyRepaint();
            }
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        timer = cooldownTime;
        progress = 1f;
        overlay.MarkDirtyRepaint(); // force immediate redraw
    }

    void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        // Only draw if progress > 0
        if (progress <= 0f)
            return;

        // Draw radial cooldown using painter API
        var painter2D = ctx.painter2D;
        var rect = overlay.contentRect;
        var center = rect.center;
        var radius = Mathf.Min(rect.width, rect.height) * 0.5f;

        painter2D.fillColor = new Color(0, 0, 0, 0.8f);
        painter2D.BeginPath();
        painter2D.MoveTo(center);
        
        // Draw arc from top (270°) clockwise based on progress
        float startAngle = -90f; // start at top
        float sweepAngle = progress * 360f;
        
        painter2D.Arc(center, radius, startAngle, startAngle + sweepAngle);
        painter2D.LineTo(center);
        painter2D.ClosePath();
        painter2D.Fill();
    }

    void OnDestroy()
    {
        if (overlay != null)
            overlay.generateVisualContent -= OnGenerateVisualContent;
    }

    public void setAbility(bool isServer)
    {
        if (!isServer)
        {
            isPlayer1 = false;
        }
    }

    private void changeIcon(int state)
    {
        if (isPlayer1)
        {
            if(state == 0)
            {
                return;
            }
        }
    }
}
