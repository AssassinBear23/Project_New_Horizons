using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Managers;
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float offScreenOffSet = 0.1f;
    [SerializeField] private float bounciness = 0.5f;
    public UnityEvent onDeadge;
    public Rigidbody rb;
    private void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (/*pos.x < 0 || pos.x > 1 ||*/ pos.y < -offScreenOffSet || pos.y > 1+offScreenOffSet) GoDie();
    }
    private void GoDie()
    {
        onDeadge?.Invoke();
    }
    private void OnCollisionEnter(Collision collision)
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