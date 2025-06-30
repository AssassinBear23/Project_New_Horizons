using UnityEngine;
public class Shield : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Managers.GameManager.Instance.PowerUpManager.PowerDuration(duration, Managers.PowerUps.Shield);
        Destroy(gameObject);
    }
}