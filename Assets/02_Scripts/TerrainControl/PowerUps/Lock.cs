using UnityEngine;
public class Lock : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!Managers.GameManager.Instance.PowerUpManager.hasGoldenAcorn && !Managers.GameManager.Instance.PowerUpManager.hasLock)
            StartCoroutine(Managers.GameManager.Instance.PowerUpManager.PowerDuration(duration, Managers.PowerUps.Lock));
        
        Destroy(gameObject);
    }
}