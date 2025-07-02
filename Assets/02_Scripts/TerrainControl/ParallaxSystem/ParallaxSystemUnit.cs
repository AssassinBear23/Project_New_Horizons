using System;
using UnityEngine;

/// <summary>
/// Handles the behavior of a single parallax background unit, including movement, rotation, and spawning logic.
/// </summary>
public class ParallaxSystemUnit : MonoBehaviour
{
    [Header("Variables and References")]
    [Tooltip("Speed of the parallax effect.\nHigher values mean faster movement of the background relative to the tree.")]
    [SerializeField] private float parallaxSpeed = 0.5f;
    [SerializeField] private Transform m_parentTerrain;

    /// <summary>
    /// Gets the background prefab transform associated with this unit.
    /// </summary>
    [field: SerializeField]
    public Transform BackgroundPrefab { get; private set; }

    //public 
    /// <summary>
    /// Indicates whether a new background has already been spawned by this unit.
    /// </summary>
    private bool hasSpawned = false;

    /// <summary>
    /// Gets the vertical size (Y axis) of the background prefab.
    /// </summary>
    public float PrefabSizeY { get; private set; } = 0f;

    /// <summary>
    /// Unity Awake callback. Initializes the prefab size.
    /// </summary>
    private void Awake()
    {
        PrefabSizeY = GetPrefabSize().y;
    }

    /// <summary>
    /// Updates the position of the background element based on the specified speed.
    /// The position is updated in a vertical manner, simulating a parallax effect.
    /// Applies the parallax multiplier to the speed, changing the speed that the background moves at.
    /// </summary>
    /// <param name="speed">The speed that the player/tree is moving at.</param>
    public void UpdateTransformPositions(float speed)
    {
        Vector3 newPosition = transform.position;
        newPosition.y += speed * parallaxSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    /// <summary>
    /// Gets the size of the assigned prefab's bounds, representing the size in global space.
    /// </summary>
    /// <returns>A <see cref="Vector3"/> representing the size of the prefab.</returns>
    private Vector3 GetPrefabSize()
    {
        if (BackgroundPrefab != null)
        {
            if (BackgroundPrefab.TryGetComponent<Renderer>(out var renderer))
            {
                return renderer.bounds.size;
            }
            else
            {
                Debug.LogWarning("Background prefab does not have a Renderer component.", this);
                return Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("Background prefab is not assigned.", this);
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Updates the state of background elements at fixed intervals.
    /// Called automatically by the Unity engine during the physics update cycle.
    /// </summary>
    private void FixedUpdate()
    {
        UpdateRotation();
    }

    /// <summary>
    /// Handles trigger events for spawning or destroying background units.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BackgroundDestroyer"))
        {
            Debug.Log($"Destroying background: {gameObject.name}", this);
            ParallaxSystemManager.Instance.DestroyBackground(gameObject);
        }
        else if (other.CompareTag("BackgroundSpawner") && !hasSpawned)
        {
            Debug.Log($"Spawning new background for: {gameObject.name}", this);
            ParallaxSystemManager.Instance.SpawnNewBackground(this, m_parentTerrain, PrefabSizeY);
            hasSpawned = true;
        }
    }

    /// <summary>
    /// Updates the rotation of the background transform around the parent terrain based on the <see cref="parallaxSpeed"/> variable.
    /// </summary>
    private void UpdateRotation()
    {
        transform.RotateAround(
            m_parentTerrain.position,
            Vector3.up,
            parallaxSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Initializes the parallax system unit with the specified parent terrain transform.
    /// Resets the spawn state to allow spawning of new background elements.
    /// </summary>
    /// <param name="parentTerrain">The transform of the parent terrain to associate with this unit.</param>
    public void Initialize(Transform parentTerrain)
    {
        m_parentTerrain = parentTerrain;
        hasSpawned = false; // Reset the spawn state when initialized
    }
}
