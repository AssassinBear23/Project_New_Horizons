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
    private InputManager m_inputManager;
    public UnityEvent onDestroyObstacle;
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
            if (m_inputManager.swiped)
            {
                StartSwipeCooldown();
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
                    rb.AddForce(transform.up * rb.linearVelocity.y * bounciness);
                }
            }
        }

        else if (collision.transform.CompareTag("Bird"))
        {
            StartSwipeCooldown();
            Destroy(collision.transform.parent.gameObject);
        }
    }
    private void StartSwipeCooldown()
    {
        onDestroyObstacle?.Invoke();
        StartCoroutine(m_inputManager.SwipeCooldown(swipeCooldown));
    }
}