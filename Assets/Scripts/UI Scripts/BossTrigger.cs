using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public HealthBar enemyHealthBar;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            enemyHealthBar.gameObject.SetActive(true);
        }
    }
}
