using UnityEngine;
public enum Directions { Clockwise, Counterclockwise }
public enum OnBirdTouchEvent { MovesToTopOfScreen, KillsPlayer}
public class BirdController : MonoBehaviour
{
    public Directions moveDirection = Directions.Clockwise;
    [SerializeField] private OnBirdTouchEvent onBirdTouchEvent = OnBirdTouchEvent.KillsPlayer;
    [SerializeField] private float movementSpeed = 10;

    private bool isEnabled = true;
    private void FixedUpdate()
    {
        if (!isEnabled) return;

        float direction = (moveDirection == Directions.Clockwise) ? 1 : -1;
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + movementSpeed * direction,
            transform.localEulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && isEnabled)
        {
            Debug.Log("Player hit bird");

            switch(onBirdTouchEvent)
            {
                case OnBirdTouchEvent.MovesToTopOfScreen:
                    Debug.LogWarning("Not Yet Implemented dumbass");
                    break;
                case OnBirdTouchEvent.KillsPlayer:
                    Debug.Log("Killing Player");
                    collision.gameObject.GetComponent<PlayerDeath>().onDeadge?.Invoke();
                    isEnabled = false;
                    break;
            }
        }
    }
}
