using UnityEngine;

/// <summary>
/// Attach this to your player to debug combat issues
/// This will tell you exactly what's working and what's not
/// </summary>
public class CombatDebugHelper : MonoBehaviour
{
    private CharacterInputController cinput;
    private CombatController combat;
    private Animator anim;

    void Start()
    {
        cinput = GetComponent<CharacterInputController>();
        combat = GetComponent<CombatController>();
        anim = GetComponent<Animator>();

        Debug.Log("=== COMBAT DEBUG HELPER STARTED ===");

        // Check components
        if (cinput == null)
            Debug.LogError("❌ CharacterInputController NOT FOUND!");
        else
            Debug.Log("✅ CharacterInputController found");

        if (combat == null)
            Debug.LogError("❌ CombatController NOT FOUND!");
        else
            Debug.Log("✅ CombatController found");

        if (anim == null)
            Debug.LogError("❌ Animator NOT FOUND!");
        else
        {
            Debug.Log("✅ Animator found");

            // Check for attack parameter
            bool hasAttackTrigger = false;
            foreach (var param in anim.parameters)
            {
                if (param.name == "attack" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasAttackTrigger = true;
                    break;
                }
            }

            if (hasAttackTrigger)
                Debug.Log("✅ 'attack' trigger parameter exists in Animator");
            else
                Debug.LogError("❌ 'attack' trigger parameter NOT FOUND in Animator!");
        }

        Debug.Log("=== END STARTUP CHECK ===");
    }

    void Update()
    {
        // Test 1: Raw mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("🖱️ [1] RAW MOUSE CLICK DETECTED");
        }

        // Test 2: Unity button input
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("🔥 [2] FIRE1 BUTTON DETECTED");
        }

        // Test 3: CharacterInputController
        if (cinput != null && cinput.enabled)
        {
            if (cinput.Attack)
            {
                Debug.Log("⚔️ [3] CharacterInputController.Attack = TRUE");
            }

            if (cinput.Action)
            {
                Debug.Log("💥 [3] CharacterInputController.Action = TRUE");
            }
        }
        else if (cinput != null && !cinput.enabled)
        {
            Debug.LogWarning("⚠️ CharacterInputController is DISABLED!");
        }

        // Test 4: CombatController status
        if (combat != null && combat.enabled)
        {
            // This will be visible in inspector
        }
        else if (combat != null && !combat.enabled)
        {
            Debug.LogWarning("⚠️ CombatController is DISABLED!");
        }

        // Test 5: Manual attack test (press T key)
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("🧪 [TEST] Manually triggering attack animation...");
            if (anim != null)
            {
                anim.SetTrigger("attack");
                Debug.Log("✅ Attack trigger set on Animator");
            }
        }
    }

    // Visual status in scene view
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Label("=== COMBAT DEBUG STATUS ===");

        GUILayout.Label($"CharacterInput: {(cinput != null && cinput.enabled ? "✅ ENABLED" : "❌ DISABLED")}");
        GUILayout.Label($"CombatController: {(combat != null && combat.enabled ? "✅ ENABLED" : "❌ DISABLED")}");
        GUILayout.Label($"Animator: {(anim != null ? "✅ EXISTS" : "❌ MISSING")}");

        if (cinput != null)
        {
            GUILayout.Label($"Attack Input: {cinput.Attack}");
            GUILayout.Label($"Action Input: {cinput.Action}");
        }

        GUILayout.Label("\nPress LEFT MOUSE to attack");
        GUILayout.Label("Press T to manually trigger animation");

        GUILayout.EndArea();
    }
}