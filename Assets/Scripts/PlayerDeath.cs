using UnityEngine;
using UnityEngine.Events;
public class PlayerDeath : MonoBehaviour
{
    public UnityEvent onDeadge;
    public Rigidbody rb;

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (/*pos.x < 0 || pos.x > 1 ||*/ pos.y < -0.05f || pos.y > 1.05f) GoDie();

        //Debug.Log("Inertia:" + rb.linearVelocity);
    }
    private void GoDie()
    {
        Debug.Log("Deadge");
        onDeadge?.Invoke();
    }
}
