using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class AbilityCooldownVisual : MonoBehaviour
{
    private UIDocument ui;
    private VisualElement overlay;
    private VisualElement icon;
    private float progress = 0f;

    private bool isPlayer1 = true;
    [SerializeField] private Sprite freezeTimeIcon0;
    [SerializeField] private Sprite freezeTimeIcon1;
    [SerializeField] private Sprite changeTimeIcon0;
    [SerializeField] private Sprite changeTimeIcon1;

    private TimeFreezeAbility freezeAbility;
    private TimeTravel timeTravelAbility;
    private bool abilitiesFound = false;

    void Start()
    {
        ui = GetComponent<UIDocument>();
        
        if (ui == null)
        {
            Debug.LogError("UIDocument component not found!");
            enabled = false;
            return;
        }

        overlay = ui.rootVisualElement.Q<VisualElement>("CooldownOverlay");
        icon = ui.rootVisualElement.Q<VisualElement>("AbilityIcon");

        if (overlay == null)
        {
            Debug.LogError("CooldownOverlay not found in UXML");
            enabled = false;
            return;
        }

        if (icon == null)
        {
            Debug.LogError("AbilityIcon not found in UXML");
            enabled = false;
            return;
        }

        Debug.Log($"=== AbilityCooldownVisual Starting on GameObject: {gameObject.name} ===");

        // Register mesh generation callback
        overlay.generateVisualContent += OnGenerateVisualContent;
        overlay.MarkDirtyRepaint();

        // Try to find player abilities
        FindPlayerAbilities();
    }

    void FindPlayerAbilities()
    {
        // Find all players in the scene
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        Debug.Log($"Found {players.Length} GameObjects with 'Player' tag");
        
        foreach (GameObject player in players)
        {
            var networkObj = player.GetComponent<NetworkObject>();
            
            Debug.Log($"Checking player: {player.name}, has NetworkObject: {networkObj != null}, IsOwner: {networkObj?.IsOwner}");
            
            if (networkObj != null && networkObj.IsOwner)
            {
                // This is the local player - get their abilities
                freezeAbility = player.GetComponent<TimeFreezeAbility>();
                timeTravelAbility = player.GetComponent<TimeTravel>();
                
                Debug.Log($"=== Found LOCAL player: {player.name} ===");
                Debug.Log($"freezeAbility found: {freezeAbility != null}, enabled: {freezeAbility?.enabled}");
                Debug.Log($"timeTravelAbility found: {timeTravelAbility != null}, enabled: {timeTravelAbility?.enabled}");
                
                abilitiesFound = (freezeAbility != null || timeTravelAbility != null);
                
                if (abilitiesFound)
                {
                    // Determine which ability is active
                    if (freezeAbility != null && freezeAbility.enabled)
                    {
                        Debug.Log(">>> This player has FREEZE ability enabled");
                    }
                    if (timeTravelAbility != null && timeTravelAbility.enabled)
                    {
                        Debug.Log(">>> This player has TIME TRAVEL ability enabled");
                    }
                    
                    UpdateIcon();
                    break;
                }
            }
        }

        if (!abilitiesFound)
        {
            Debug.LogWarning("Could not find player abilities yet. Will retry...");
        }
    }

    void Update()
    {
        // Keep trying to find abilities if not found yet
        if (!abilitiesFound)
        {
            FindPlayerAbilities();
            return;
        }

        // Update cooldown progress from whichever ability exists on this player
        UpdateCooldownProgress();
        
        // Update icon more frequently when freeze is active, otherwise throttle
        bool shouldUpdateIcon = false;
        
        if (freezeAbility != null && freezeAbility.enabled && freezeAbility.IsFreezeActive())
        {
            // Update every frame when freeze is active
            shouldUpdateIcon = true;
        }
        else if (Time.frameCount % 60 == 0)
        {
            // Update once per second otherwise
            shouldUpdateIcon = true;
        }
        
        if (shouldUpdateIcon)
        {
            UpdateIcon();
        }
    }

    void UpdateCooldownProgress()
    {
        float newProgress = 0f;

        // Check which ability is enabled and on cooldown
        if (freezeAbility != null && freezeAbility.enabled)
        {
            if (freezeAbility.IsOnCooldown())
            {
                newProgress = freezeAbility.GetCooldownProgress();
                if (Time.frameCount % 30 == 0) // Log every half second
                {
                    Debug.Log($"[FREEZE] Cooldown progress: {newProgress:F2}");
                }
            }
        }
        else if (timeTravelAbility != null && timeTravelAbility.enabled)
        {
            if (timeTravelAbility.IsOnCooldown())
            {
                newProgress = timeTravelAbility.GetCooldownProgress();
                if (Time.frameCount % 30 == 0) // Log every half second
                {
                    Debug.Log($"[TRAVEL] Cooldown progress: {newProgress:F2}");
                }
            }
        }

        // Update progress and redraw if changed
        if (Mathf.Abs(progress - newProgress) > 0.01f)
        {
            progress = newProgress;
            overlay.MarkDirtyRepaint();
        }
    }

    void UpdateIcon()
    {
        if (icon == null) return;

        Sprite selectedSprite = null;

        // Determine icon based on which ability is enabled
        if (freezeAbility != null && freezeAbility.enabled)
        {
            // Player with freeze ability
            // Use different icon based on freeze state
            if (freezeAbility.IsFreezeActive())
            {
                // While freezing, use icon1
                selectedSprite = isPlayer1 ? freezeTimeIcon1 : freezeTimeIcon0;
            }
            else
            {
                // When not freezing, use icon0
                selectedSprite = isPlayer1 ? freezeTimeIcon0 : freezeTimeIcon1;
            }
        }
        else if (timeTravelAbility != null && timeTravelAbility.enabled)
        {
            // Player with time travel ability
            bool isInOldMap = timeTravelAbility.IsPlayerInOldMap();
            selectedSprite = isInOldMap ? changeTimeIcon0 : changeTimeIcon1;
        }

        // Set the background image
        if (selectedSprite != null)
        {
            icon.style.backgroundImage = new StyleBackground(selectedSprite);
        }
        else
        {
            Debug.LogWarning($"No sprite selected! freezeAbility: {freezeAbility?.enabled}, timeTravelAbility: {timeTravelAbility?.enabled}");
        }
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
        float startAngle = -90f;
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
        isPlayer1 = isServer;
        Debug.Log($"setAbility called with isServer: {isServer}, isPlayer1 set to: {isPlayer1}");
        FindPlayerAbilities();
        UpdateIcon();
    }
}
