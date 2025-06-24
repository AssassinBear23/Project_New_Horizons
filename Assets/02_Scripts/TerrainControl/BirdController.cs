using UnityEngine;
using System.Collections;
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
        transform.parent.localEulerAngles = new Vector3(
            transform.parent.localEulerAngles.x,
            transform.parent.localEulerAngles.y + movementSpeed * direction,
            transform.parent.localEulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && isEnabled)
        {
            switch(onBirdTouchEvent)
            {
                case OnBirdTouchEvent.MovesToTopOfScreen:
                    Debug.LogWarning("Not Yet Implemented dumbass");
                    StartCoroutine(CarryToTop());
                    break;
                case OnBirdTouchEvent.KillsPlayer:
                    Debug.Log("Killing Player");
                    collision.gameObject.GetComponent<PlayerDeath>().onDeadge?.Invoke();
                    isEnabled = false;
                    break;
            }
        }
    }
    private bool finishedMovingAway = false;
    private bool finishedMovingUp = false;
    private bool finishedMovingTowards = false;
    private IEnumerator CarryToTop()
    {
        finishedMovingAway = false;
        finishedMovingTowards = false;
        finishedMovingUp = false;

        Managers.GameManager.Instance.PlayerControls.DisableInput();
        yield return null;
        
        //yield return new WaitUntil(() => finishedMovingAway);

        Managers.GameManager.Instance.PlayerControls.EnableInput();
    }
}
