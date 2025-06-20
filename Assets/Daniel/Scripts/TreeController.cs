using Managers;
using UnityEngine;

public enum IncreaseTypes { Linearly, Exponentially }
public class TreeController : MonoBehaviour
{
    [Header("Stats")]
    public float movementSpeed = 5;
    [SerializeField] private IncreaseTypes speedIncreaseType = IncreaseTypes.Linearly;

    [SerializeField] private float speedIncrease = 0.001f;
    [SerializeField, Min(1)] private float speedMultiplier = 1.01f;
    
    [Header("References")]
    public GameObject parent;
    [SerializeField] private TreeController prefab;

    private void Start()
    {
        GameManager.Instance.AddTreeSegment(this);
        enabled = false; // Disable the script until gameplay starts
    }

    private void FixedUpdate()
    {
        UpdatePosition();
        IncreaseSpeed();
    }
    private void UpdatePosition()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + movementSpeed,
            transform.position.z);
    }
    private void IncreaseSpeed()
    {
        switch (speedIncreaseType)
        {
            case IncreaseTypes.Linearly:
                movementSpeed += speedIncrease;
                break;
            case IncreaseTypes.Exponentially:
                movementSpeed *= speedMultiplier;
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Destroyer")
        {
            GameManager.Instance.RemoveTreeSegment(this);
            Destroy(gameObject);
        }

        else if (other.tag == "Spawner")
        {
            Vector3 pos = transform.position;
            pos.y -= 10f;
            TreeController spawned = Instantiate(prefab, pos, Quaternion.identity, parent.transform);
            spawned.movementSpeed = movementSpeed;
            spawned.parent = parent;
        }
    }
}