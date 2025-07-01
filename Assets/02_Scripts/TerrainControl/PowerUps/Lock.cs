using UnityEngine;
public class Lock : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private AudioClip pickUpSound;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!Managers.GameManager.Instance.PowerUpManager.hasGoldenAcorn && !Managers.GameManager.Instance.PowerUpManager.hasLock)
            Managers.GameManager.Instance.StartCoroutine(Managers.GameManager.Instance.PowerUpManager.PowerDuration(duration, Managers.PowerUps.Lock));

        Managers.GameManager.Instance.SoundManager.PlaySpatialOneShotSound(pickUpSound, transform.position);

        Destroy(gameObject);
    }
}