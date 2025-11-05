using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float smoothSpeed = 10f;

    private float targetValue;
    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        targetValue = maxHealth;
    }

    public void SetHealth(float health)
    {
        targetValue = health;
    }

    void Update()
    {
        // Smoothly move toward target value
        if (Mathf.Abs(slider.value - targetValue) > 0.01f)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }
}