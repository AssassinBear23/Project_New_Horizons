using UnityEngine;
using UnityEngine.Events;
using Managers;
public class PlayerDeath : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float offScreenOffSet = 0.1f;
    [SerializeField] private float scoreMultiplier = 0.01f;
    [SerializeField] private float swipeCooldown = 10;

    [Header("References")]
    public Rigidbody rb;
    [SerializeField] private DestroyParticles featherParticles;
    [SerializeField] private DestroyParticles leaveParticles;
    [SerializeField] private AudioClip killBirdSound;
    [SerializeField] private AudioClip destroyBranchSound;
    

    [Header("Evenets")]
    public UnityEvent onDeadge;
    public UnityEvent<Vector3> onDestroyBranch;
    public UnityEvent<Vector3> onDestroyBird;
    public UnityEvent UsedSwipePower;
    public UnityEvent<float> screenShake;

    private Vector3 lastPos;
    private InputManager m_inputManager;
    private void Start()
    {
        m_inputManager = InputManager.Instance;
        lastPos = transform.position;
    }

    /// <summary>
    /// Every frame, check if the player is off screen, if so, they die and the game ends
    /// </summary>
    private void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.y < -offScreenOffSet || pos.y > 1+offScreenOffSet) GoDie();
    }
    private void FixedUpdate()
    {
        float diff = transform.position.y - lastPos.y;
        Managers.GameManager.Instance.AddToCurrentScore(-1f * diff * scoreMultiplier);
        lastPos = transform.position;
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
                screenShake?.Invoke(rb.linearVelocity.y);
            }

            else
            {
                Vector3 normal = collision.contacts[0].normal;
                Transform target = collision.transform;

                float rightDot = Vector3.Dot(normal, target.forward);
                float leftDot = Vector3.Dot(normal, -target.forward);
                float topDot = Vector3.Dot(normal, transform.up);

                float threshold = 0.65f; // Adjust for tolerance due to floating point inaccuracies

                if (rightDot > threshold)
                {
                    StartCoroutine(GameManager.Instance.PlayerControls.PlayerBounce(1));
                }
                else if (leftDot > threshold)
                {
                    StartCoroutine(GameManager.Instance.PlayerControls.PlayerBounce(-1));
                }
                else if (topDot > threshold)
                {
                    screenShake?.Invoke(rb.linearVelocity.y);
                }
            }

            rb.AddForce(-1f * rb.GetAccumulatedForce());
            rb.linearVelocity = Vector3.zero;
        }

        else if (collision.transform.CompareTag("Bird"))
        {
            if (m_inputManager.swiped || GameManager.Instance.PowerUpManager.hasGoldenAcorn || GameManager.Instance.PowerUpManager.hasShield)
            {
                if (m_inputManager.swiped) StartSwipeCooldown();
                onDestroyBird?.Invoke(collision.transform.position);
                GameManager.Instance.SoundManager.PlaySpatialOneShotSound(killBirdSound, collision.transform.position);
                Destroy(collision.transform.parent.gameObject);
                GameManager.Instance.PowerUpManager.DisablePower(PowerUps.Shield);
            }

            screenShake?.Invoke(rb.linearVelocity.y);

            rb.AddForce(-1f * rb.GetAccumulatedForce());
            rb.linearVelocity = Vector3.zero;
        }
    }
    private void StartSwipeCooldown()
    {
        UsedSwipePower?.Invoke();
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
    public void ResetPlayerForce()
    {
        rb.AddForce(-1f * rb.GetAccumulatedForce());
        rb.linearVelocity = Vector3.zero;
    }
}