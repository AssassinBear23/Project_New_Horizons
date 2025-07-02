using UnityEngine;
public class DestroyParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private void Start()
    {
        float duration = _particleSystem.startLifetime + _particleSystem.main.duration;
        Destroy(gameObject, duration);
    }
}