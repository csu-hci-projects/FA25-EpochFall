using UnityEngine;
using System.Collections;
public class TriggerFall : MonoBehaviour
{
    public Rigidbody2D[] objectsToDrop;
    public float delayBetweenFalls = 0.05f;
    private bool hasFallen = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasFallen) return; //prevents retrigger
        if (other.CompareTag("Player"))
        {
            hasFallen = true;
            StartCoroutine(DropObjectsSequentally());
        }
    }
    private IEnumerator DropObjectsSequentally()
    {
        foreach (Rigidbody2D rb in objectsToDrop)
        {
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(delayBetweenFalls);
        }
    }
}
