using UnityEngine;
public class GoldenAcorn : MonoBehaviour
{
    [SerializeField] private float duration;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!Managers.GameManager.Instance.PowerUpManager.hasGoldenAcorn)
        {
            Managers.GameManager.Instance.StartCoroutine(Managers.GameManager.Instance.PowerUpManager.PowerDuration(duration, Managers.PowerUps.GoldenAcorn));

            if (Managers.GameManager.Instance.PowerUpManager.hasLock)
            {
                Managers.GameManager.Instance.PowerUpManager.DisablePower(Managers.PowerUps.Lock);
            }
        }
        
        Destroy(gameObject);
    }
}