using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float animationSpeed;

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        Debug.Log($"HealthBar: Setting health to {health}.");
        StartCoroutine(AnimateHealthBar(health));
    }

    private IEnumerator AnimateHealthBar(int health)
    {
        while (slider.value != health)
        {
            slider.value = Mathf.MoveTowards(slider.value, health, Time.deltaTime * animationSpeed);
            yield return null;
        }
    }
}
