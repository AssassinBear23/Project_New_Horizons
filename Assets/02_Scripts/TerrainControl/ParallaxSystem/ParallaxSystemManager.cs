using UnityEngine;

/// <summary>
/// Manages the spawning and destruction of parallax background elements in the scene.
/// Implements a singleton pattern for global access.
/// </summary>
public class ParallaxSystemManager : MonoBehaviour
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="ParallaxSystemManager"/>.
    /// </summary>
    public static ParallaxSystemManager Instance { get; private set; }

    /// <summary>
    /// Unity Awake callback. Initializes the singleton instance or destroys duplicate instances.
    /// </summary>
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Spawns a new background element for the parallax system.
    /// </summary>
    /// <param name="unit">The <see cref="ParallaxSystemUnit"/> to use as a reference for spawning.</param>
    /// <param name="parentTerrain">The parent <see cref="Transform"/> under which the new background will be placed.</param>
    /// <param name="prefabSizeY">The vertical size of the prefab to be spawned.</param>
    public void SpawnNewBackground(ParallaxSystemUnit unit, Transform parentTerrain, float prefabSizeY)
    {
        Vector3 spawnPosition = GetSpawnPosition(unit.transform, prefabSizeY);

        Transform instantiated = Instantiate(unit.BackgroundPrefab, spawnPosition, unit.transform.rotation, parentTerrain);
        

        if (instantiated.TryGetComponent<ParallaxSystemUnit>(out var parallaxUnit))
        {
            parallaxUnit.Initialize(parentTerrain);
        }
    }

    /// <summary>
    /// Calculates the position where a new background prefab should be spawned, based on the given transform and prefab size.
    /// </summary>
    /// <param name="transform">The <see cref="Transform"/> to use as a reference point.</param>
    /// <param name="prefabSizeY">The vertical size of the prefab.</param>
    /// <returns>The calculated spawn position as a <see cref="Vector3"/>.</returns>
    private Vector3 GetSpawnPosition(Transform transform, float prefabSizeY)
    {
        return transform.position - new Vector3(0, prefabSizeY, 0);
    }

    /// <summary>
    /// Destroys the specified background GameObject.
    /// </summary>
    /// <param name="gameObject">The <see cref="GameObject"/> to destroy.</param>
    public void DestroyBackground(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
