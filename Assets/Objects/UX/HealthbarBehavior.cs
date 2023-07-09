using UnityEngine;
using UnityEngine.UI;

public class HealthbarBehavior : MonoBehaviour
{
    public Slider healthSlider;
    public Color low;
    public Color high;
    public Vector3 offset;

    public void SetHealth(float health, float maxHealth)
    {
        healthSlider.gameObject.SetActive(health < maxHealth);
        healthSlider.value = health;
        healthSlider.maxValue = maxHealth;

        healthSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, healthSlider.normalizedValue);
    }

    void Update()
    {
        healthSlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
}
