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
    [SerializeField] public GameObject _Parent;
    [SerializeField] public PrefabReference prefab;

    private bool hasSpawned = false;
    private void Start()
    {
        GameManager.instance.AddTreeSegment(this);
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
            GameManager.instance.RemoveTreeSegment(this);
            Destroy(gameObject);
        }

        else if (other.tag == "Spawner" && !hasSpawned)
        {
            Vector3 pos = transform.position;
            pos.y -= 10f;
            TreeController spawned = Instantiate(prefab.prefab, pos, Quaternion.identity, _Parent.transform);
            spawned.movementSpeed = movementSpeed;
            spawned._Parent = _Parent;
            hasSpawned = true;
        }
    }
}