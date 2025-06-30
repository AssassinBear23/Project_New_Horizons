using UnityEngine;
public class Lock : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(Managers.GameManager.Instance.PowerUpManager.PowerDuration(duration, Managers.PowerUps.Lock));
        Destroy(gameObject);
    }
}