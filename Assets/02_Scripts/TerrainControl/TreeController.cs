using Managers;
using UnityEngine;

public enum IncreaseTypes { Linearly, Exponentially }
/// <summary>
/// Moves the tree segment upwards, destroys it once it's out of screen and spawns a new one
/// </summary>
public class TreeController : MonoBehaviour
{
    [Header("Stats")]
    /// <summary>
    /// The speed at which the terrain moves upwards
    /// </summary>
    public float movementSpeed = 5;

    /// <summary>
    /// The way in which the speed is increased over time
    /// </summary>
    [SerializeField] private IncreaseTypes speedIncreaseType = IncreaseTypes.Linearly;

    [SerializeField] private float speedIncrease = 0.001f;
    [SerializeField, Min(1)] private float speedMultiplier = 1.01f;
    
    [Header("References")]
    /// <summary>
    /// A reference to the parento bject in the scene to keep the hierarchy structured
    /// </summary>
    public GameObject _Parent;

    /// <summary>
    /// A reference to the tree segment prefab that  needs to be spawned
    /// </summary>
    [SerializeField] private PrefabReference prefab;

    private bool hasSpawned = false;

    private void Start()
    {
        GameManager.Instance.AddTreeSegment(this);
        if (GameManager.Instance.IsPaused) enabled = false; // Disable the script until gameplay starts
    }
    private void FixedUpdate()
    {
        UpdatePosition();
        IncreaseSpeed(); // Increases movement speed over time
    }
    private void UpdatePosition()
    {
        transform.position += new Vector3(0, movementSpeed, 0);
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
        // Removes and destroys tree segment when reaching the destroyer object
        if (other.tag == "Destroyer")
        {
            GameManager.Instance.RemoveTreeSegment(this);
            Destroy(gameObject);
        }

        // Spawns a new tree segment under this one when it touches the spawner object
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