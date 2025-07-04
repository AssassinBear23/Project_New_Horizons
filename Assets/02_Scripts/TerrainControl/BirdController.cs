using Managers;
using System.Collections;
using UnityEngine;
public enum Directions { Clockwise, Counterclockwise }
public enum OnBirdTouchEvent { MovesToTopOfScreen, KillsPlayer }
public class BirdController : MonoBehaviour
{
    public Directions moveDirection = Directions.Clockwise;
    [SerializeField] private OnBirdTouchEvent onBirdTouchEvent = OnBirdTouchEvent.KillsPlayer;
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField] private float playerTakingSpeed = 2.5f;
    [SerializeField] private float topY = 2.5f;
    [SerializeField] private AudioClip m_birdSound;

    private bool isEnabled = true;
    private void FixedUpdate()
    {
        if (!isEnabled || GameManager.Instance.IsPaused) return;

        float direction = (moveDirection == Directions.Clockwise) ? 1 : -1;
        transform.parent.localEulerAngles += new Vector3(0, movementSpeed * direction, 0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (!isEnabled || GameManager.Instance.IsPaused || GameManager.Instance.InputManager.swiped || GameManager.Instance.PowerUpManager.hasGoldenAcorn)
            return;

        if (Managers.GameManager.Instance.PowerUpManager.hasShield)
            return;

        switch (onBirdTouchEvent)
        {
            case OnBirdTouchEvent.MovesToTopOfScreen:
                Debug.LogWarning("Not Yet Implemented dumbass");
                CarryToTop(collision.transform);
                break;
            case OnBirdTouchEvent.KillsPlayer:
                Debug.Log("Killing Player");
                collision.gameObject.GetComponent<PlayerDeath>().onDeadge?.Invoke();
                isEnabled = false;
                break;
        }
        Managers.GameManager.Instance.SoundManager.PlaySpatialOneShotSound(m_birdSound, transform.position);
    }
    /// <summary>
    /// Disables controls and starts sequence to move the player to the top of the screen
    /// </summary>
    private void CarryToTop(Transform _PlayerPos)
    {
        isEnabled = false;
        Managers.GameManager.Instance.PlayerControls.DisableInput();

        // Calculate angle from bird to player
        Vector3 playerPos = _PlayerPos.position;
        playerPos.y = 0;

        Vector3 birdPos = transform.position;
        birdPos.y = 0;

        float angle = Vector3.Angle(birdPos.normalized, playerPos.normalized);

        float dir = (transform.parent.localEulerAngles.y > 0) ? -1 : 1;

        transform.parent.localEulerAngles += new Vector3(0, angle * dir, 0);

        // Start moving away
        StartCoroutine(MoveAway(_PlayerPos));
    }
    /// <summary>
    /// Makes the bird move away to not clip through branches
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveAway(Transform player)
    {
        while (transform.localPosition.z > -2.5f)
        {
            if (Managers.GameManager.Instance.IsPaused)
            {
                yield return null;
                continue;
            }

            transform.localPosition += new Vector3(0, 0, -playerTakingSpeed * Time.deltaTime);
            player.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

            if (transform.localPosition.z < -2.5f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -4);
                player.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                break;
            }

            yield return null;
        }
        StartCoroutine(MoveUp(player));
    }
    /// <summary>
    /// Makes the bird move to the top of the screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveUp(Transform player)
    {
        while (transform.position.y < topY)
        {
            if (Managers.GameManager.Instance.IsPaused)
            {
                yield return null;
                continue;
            }

            transform.position += new Vector3(0, playerTakingSpeed * Time.deltaTime, 0);
            player.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

            if (transform.position.y > topY)
            {
                transform.position = new Vector3(transform.position.x, topY, transform.position.z);
                player.position = new Vector3(transform.position.x, topY, transform.position.z);
                break;
            }

            yield return null;
        }
        StartCoroutine(MoveTowards(player));
    }
    /// <summary>
    /// Returns the bird to the original z position so the player can land on branches again
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTowards(Transform player)
    {
        while (transform.position.z < -1)
        {
            if (Managers.GameManager.Instance.IsPaused)
            {
                yield return null;
                continue;
            }

            transform.position += new Vector3(0, 0, playerTakingSpeed * Time.deltaTime);
            player.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

            if (transform.position.z > -1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
                player.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                break;
            }

            yield return null;
        }
        isEnabled = true;
        Managers.GameManager.Instance.PlayerControls.EnableInput();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.AddForce(-1f * rb.GetAccumulatedForce());
        rb.angularVelocity = Vector3.zero;
        Destroy(transform.parent.gameObject);
    }
    public void SetDirection(Directions _Direction)
    {
        moveDirection = _Direction;

        transform.localEulerAngles = new Vector3(0, 90 * ((moveDirection == Directions.Clockwise) ? -1 : 1), 0); 
    }
}