using UnityEngine;
using UnityEngine.Events;
using System.Collections;
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
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Transform target = collision.transform;

        float frontDot = Vector3.Dot(normal, target.forward);
        float backDot = Vector3.Dot(normal, -target.forward);


        float threshold = 0.75f; // Adjust for tolerance due to floating point inaccuracies

        if (frontDot > threshold)
        {
            Debug.Log("Right");
            StartCoroutine(Controls.instance.PlayerBounce(1));
        }
        else if (backDot > threshold)
        {
            Debug.Log("Left");
            StartCoroutine(Controls.instance.PlayerBounce(-1));
        }
    }
}