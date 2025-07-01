using UnityEngine;
using UnityEngine.Events;
using Managers;
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float offScreenOffSet = 0.1f;
    [SerializeField] private float bounciness = 0.5f;
    [SerializeField] private float swipeCooldown = 10;
    public UnityEvent onDeadge;
    public Rigidbody rb;
    [SerializeField] private DestroyParticles featherParticles;
    [SerializeField] private DestroyParticles leaveParticles;
    [SerializeField] private AudioClip killBirdSound;
    [SerializeField] private AudioClip destroyBranchSound;
    private InputManager m_inputManager;
    public UnityEvent<Vector3> onDestroyBranch;
    public UnityEvent<Vector3> onDestroyBird;
    private void Start()
    {
        m_inputManager = InputManager.Instance;
    }

    /// <summary>
    /// Every frame, check if the player is off screen, if so, they die and the game ends
    /// </summary>
    private void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.y < -offScreenOffSet || pos.y > 1+offScreenOffSet) GoDie();
    }
    private void GoDie()
    {
        onDeadge?.Invoke();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Branch"))
        {
            if (m_inputManager.swiped || GameManager.Instance.PowerUpManager.hasGoldenAcorn)
            {
                if (m_inputManager.swiped) StartSwipeCooldown();
                onDestroyBranch?.Invoke(collision.transform.position);
                GameManager.Instance.SoundManager.PlaySpatialOneShotSound(destroyBranchSound, collision.transform.position);
                Destroy(collision.transform.parent.gameObject);
            }

            else
            {
                Vector3 normal = collision.contacts[0].normal;
                Transform target = collision.transform;

                float frontDot = Vector3.Dot(normal, target.forward);
                float backDot = Vector3.Dot(normal, -target.forward);
                float topDot = Vector3.Dot(normal, transform.up);

                float threshold = 0.8f; // Adjust for tolerance due to floating point inaccuracies

                if (frontDot > threshold)
                {
                    Debug.Log("Right");
                    StartCoroutine(GameManager.Instance.PlayerControls.PlayerBounce(1));
                }
                else if (backDot > threshold)
                {
                    Debug.Log("Left");
                    StartCoroutine(GameManager.Instance.PlayerControls.PlayerBounce(-1));
                }
                else if (topDot > threshold)
                {
                    Debug.Log("Top");
                    //rb.linearVelocity = transform.up * -rb.linearVelocity.y * bounciness;
                }
            }
        }

        else if (collision.transform.CompareTag("Bird") && (m_inputManager.swiped || GameManager.Instance.PowerUpManager.hasGoldenAcorn || GameManager.Instance.PowerUpManager.hasShield))
        {
            if (m_inputManager.swiped) StartSwipeCooldown();
            onDestroyBird?.Invoke(collision.transform.position);
            GameManager.Instance.SoundManager.PlaySpatialOneShotSound(killBirdSound, collision.transform.position);
            Destroy(collision.transform.parent.gameObject);
            GameManager.Instance.PowerUpManager.DisablePower(PowerUps.Shield);
        }
    }
    private void StartSwipeCooldown()
    {
        StartCoroutine(m_inputManager.SwipeCooldown(swipeCooldown));
    }
    public void CreateFeatherParticles(Vector3 position)
    {
        Instantiate(featherParticles, position, Quaternion.identity);
    }
    public void CreateLeaveParticles(Vector3 position)
    {
        position.z -= 1;
        Instantiate(leaveParticles, position, Quaternion.identity);
    }
}