using UnityEngine;

/// <summary>
/// Attach to Character 1 to diagnose input issues
/// This will show you EXACTLY what's happening with inputs
/// </summary>
public class InputDiagnostic : MonoBehaviour
{
    private CharacterInputController cinput;
    private CombatController combat;
    private Animator anim;

    void Start()
    {
        cinput = GetComponent<CharacterInputController>();
        combat = GetComponent<CombatController>();
        anim = GetComponent<Animator>();

        Debug.Log("=== INPUT DIAGNOSTIC STARTED ===");
        Debug.Log($"CharacterInputController: {(cinput != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"CharacterInputController Enabled: {(cinput != null ? cinput.enabled.ToString() : "N/A")}");
        Debug.Log($"CombatController: {(combat != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"CombatController Enabled: {(combat != null ? combat.enabled.ToString() : "N/A")}");
        Debug.Log($"Animator: {(anim != null ? "✅ Found" : "❌ Missing")}");
    }

    void Update()
    {
        // Level 1: Raw Unity Input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("🖱️ [RAW] Mouse button 0 DOWN detected!");
        }

        if (Input.GetMouseButton(0))
        {
            Debug.Log("🖱️ [RAW] Mouse button 0 HELD");
        }

        // Level 2: Input Manager
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("🔥 [INPUT MANAGER] Fire1 button DOWN!");
        }

        try
        {
            if (Input.GetButtonDown("Attack"))
            {
                Debug.Log("⚔️ [INPUT MANAGER] Attack button DOWN!");
            }
        }
        catch
        {
            // Attack button might not be defined in Input Manager
        }

        // Level 3: CharacterInputController
        if (cinput != null)
        {
            if (!cinput.enabled)
            {
                Debug.LogWarning("⚠️ CharacterInputController is DISABLED!");
            }
            else
            {
                if (cinput.Attack)
                {
                    Debug.Log($"⚔️ [CharacterInputController] Attack = TRUE (Frame {Time.frameCount})");
                }

                // Log all inputs every frame
                if (Time.frameCount % 60 == 0) // Every 60 frames
                {
                    Debug.Log($"📊 Input Status: Attack={cinput.Attack}, Action={cinput.Action}");
                }
            }
        }

        // Level 4: Combat Controller
        if (combat != null && !combat.enabled)
        {
            Debug.LogWarning("⚠️ CombatController is DISABLED!");
        }

        // Manual test with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("🧪 [TEST] T key pressed - manually triggering attack animation");
            if (anim != null)
            {
                anim.SetTrigger("attack");
                Debug.Log("✅ Attack trigger SET on animator");
            }
            else
            {
                Debug.LogError("❌ No animator found!");
            }
        }

        // Manual combat test with Y key
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("🧪 [TEST] Y key pressed - calling CombatController.ForceAttack()");
            if (combat != null)
            {
                combat.ForceAttack();
            }
            else
            {
                Debug.LogError("❌ No CombatController found!");
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 500, 400));
        GUILayout.Label("=== INPUT DIAGNOSTIC ===", GUI.skin.box);

        // Component status
        GUILayout.Label($"CharacterInputController: {(cinput != null && cinput.enabled ? "✅ ENABLED" : "❌ DISABLED/MISSING")}");
        GUILayout.Label($"CombatController: {(combat != null && combat.enabled ? "✅ ENABLED" : "❌ DISABLED/MISSING")}");
        GUILayout.Label($"Animator: {(anim != null ? "✅ EXISTS" : "❌ MISSING")}");

        GUILayout.Space(10);

        // Input status
        if (cinput != null && cinput.enabled)
        {
            GUILayout.Label($"Attack Input: {(cinput.Attack ? "🟢 TRUE" : "⚪ FALSE")}");
            GUILayout.Label($"Action Input: {(cinput.Action ? "🟢 TRUE" : "⚪ FALSE")}");
        }
        else
        {
            GUILayout.Label("⚠️ Cannot read inputs - CharacterInputController disabled");
        }

        GUILayout.Space(10);

        // Raw input
        GUILayout.Label($"Raw Mouse Button 0: {(Input.GetMouseButton(0) ? "🟢 PRESSED" : "⚪ NOT PRESSED")}");

        GUILayout.Space(10);

        // Instructions
        GUILayout.Label("=== TESTS ===", GUI.skin.box);
        GUILayout.Label("• Click LEFT MOUSE to test attack");
        GUILayout.Label("• Press T to force animation trigger");
        GUILayout.Label("• Press Y to force combat attack");

        GUILayout.EndArea();
    }
}