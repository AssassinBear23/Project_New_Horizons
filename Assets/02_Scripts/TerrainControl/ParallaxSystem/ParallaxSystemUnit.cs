using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParallaxSystemUnit : MonoBehaviour
{
    [Header("Variables and References")]
    [Tooltip("List of Transforms that are part of this background level.")]
    [SerializeField] private List<Transform> backgroundTransforms = new();
    [Tooltip("Speed of the parallax effect.\nHigher values mean faster movement of the background relative to the tree.")]
    [SerializeField] private float parallaxSpeed = 0.5f;
    [SerializeField] private Transform parentTerrain;
    [SerializeField] private Transform backgroundPrefab;

    [Header("Spawn and Destroy Positions")]
    [Tooltip("Position that a background needs to be at to be considered out of scope and destroyed\nVisualized with gizmo's with red box.")]
    [SerializeField] private Vector3 outOfScopePosition = new(0, 20, 0);
    [Tooltip("Position that a background needs to be at to be considered fully in-scope and spawn the next prefab\nVisualized with gizmo's with green box.")]
    [SerializeField] private Vector3 inScopePosition = new(0, -20, 0);

    private float prefabSizeY = 0f;

    private void Awake()
    {
        if (backgroundTransforms.Count == 0)
        {
            //Debug.Log("No background transforms assigned. Adding all children to list.", this);
            foreach (Transform child in transform)
            {
                AddBackground(child);
            }
        }

        prefabSizeY = GetPrefabSize().y;
    }

    /// <summary>
    /// Updates the position of the backgrounds elements based on the specified speed.
    /// The position is updated in a vertical manner, simulating a parallax effect.
    /// <para>
    /// Will apply the parallax multiplier to the speed,
    /// changing the speed that they move at to get a parallax effect in the vertical axis as well.
    /// </para>
    /// </summary>
    /// <param name="speed">The speed that the player/tree is moving at.</param>
    public void UpdateTransformPositions(float speed)
    {
        foreach (Transform backgroundTransform in backgroundTransforms)
        {
            Vector3 newPosition = backgroundTransform.position;
            newPosition.y += speed * parallaxSpeed * Time.deltaTime;
            backgroundTransform.position = newPosition;
        }
    }

    /// <summary>
    /// Gets the size of the assigned prefab's bounds, representing the size in global space.
    /// </summary>
    /// <returns>A Vector3 representing the size of the prefab</returns>
    private Vector3 GetPrefabSize()
    {
        if (backgroundPrefab != null)
        {
            if (backgroundPrefab.TryGetComponent<Renderer>(out var renderer))
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
    /// </summary>
    /// <remarks>
    /// This method is called automatically by the Unity engine during the physics update cycle.
    /// </remarks>
    private void FixedUpdate()
    {
        for (int i = 0; i < backgroundTransforms.Count; i++)
            UpdateRotation(i);
    }

    /// <summary>
    /// Checks the visibility of background elements and spawns or removes them as necessary.
    /// </summary>
    /// <remarks>
    /// This method is called automatically by the Unity engine every frame.
    /// </remarks>
    private void Update()
    {
        for (int i = 0; i < backgroundTransforms.Count; i++)
            CheckVisibility(i);
    }

    /// <summary>
    /// Method that compares the current position of the transform at the given index in the <see cref="backgroundTransforms">backgroundTransforms</see>
    /// list to see if it is still within the designated scope passed through the inspector using <see cref="outOfScopePosition">outOfScopePosition</see> and <see cref="inScopePosition">inScopePosition</see>.
    /// </summary>
    /// <param name="transformIndexPosition">The index of the transform in the <see cref="backgroundTransforms">backgroundTransforms</see> list.</param>
    private void CheckVisibility(int transformIndexPosition)
    {
        Transform backgroundTransform = backgroundTransforms[transformIndexPosition];
        if (transformIndexPosition == 0 && backgroundTransform.position.y > outOfScopePosition.y)
            RemoveBackground(backgroundTransform);
        else if (transformIndexPosition == backgroundTransforms.Count - 1 && backgroundTransform.position.y < inScopePosition.y)
            InstantiateNewBackground(GetSpawnPosition());
    }

    /// <summary>
    /// Method that returns the position where a new background prefab should be spawned.
    /// </summary>
    /// <returns>The calculated position as a <see cref="Vector3"/></returns>
    private Vector3 GetSpawnPosition()
    {
        return backgroundTransforms[^1].position - new Vector3(0, prefabSizeY, 0);
    }

    /// <summary>
    /// Updates the rotation of a background transform around the parent terrain based on the <see cref="parallaxSpeed">parallaxSpeed</see> variable.
    /// </summary>
    /// <param name="transformIndexPosition">The index of the transform in the <see cref="backgroundTransforms">backgroundTransforms</see> list.</param>
    private void UpdateRotation(int transformIndexPosition)
    {
        Transform backgroundTransform = backgroundTransforms[transformIndexPosition];
        backgroundTransform.RotateAround(
            parentTerrain.position,
            Vector3.up,
            parallaxSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Add the specified background transform to the list of backgrounds if it is not already present.
    /// </summary>
    /// <param name="toAddBackground">The transform to add to the list.</param>
    private void AddBackground(Transform toAddBackground)
    {
        if (toAddBackground != null && !backgroundTransforms.Contains(toAddBackground))
        {
            backgroundTransforms.Add(toAddBackground);
        }
    }

    /// <summary>
    /// Instantiates a new background prefab at the specified spawn position and adds it to the list of backgrounds.
    /// </summary>
    /// <param name="spawnPosition">The position to instantiate the transform at.</param>
    public void InstantiateNewBackground(Vector3 spawnPosition)
    {
        Transform instantiatedObject = Instantiate(backgroundPrefab, spawnPosition, Quaternion.identity, transform);
        AddBackground(instantiatedObject);
    }

    /// <summary>
    /// Remove the specified background transform from the list and destroy its GameObject.
    /// </summary>
    /// <param name="toRemoveBackground">The transform of the background element to remove.</param>
    public void RemoveBackground(Transform toRemoveBackground)
    {
        if (toRemoveBackground != null && backgroundTransforms.Contains(toRemoveBackground))
        {
            backgroundTransforms.Remove(toRemoveBackground);
            Destroy(toRemoveBackground.gameObject);
        }
    }

    /// <summary>
    /// Checks if the necessary components are assigned and logs warnings if not.
    /// </summary>
    /// <remarks>
    /// Exits early if the script is on a prefab asset.<br/>
    /// This method is called automatically by Unity when values are changed in the Inspector.
    /// </remarks>
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;
#endif
        if (backgroundPrefab == null)
        {
            Debug.LogWarning("Background prefab is not assigned in ParallaxSystemUnit.", this);
        }
        if (parentTerrain == null)
        {
            Debug.LogWarning("Parent terrain is not assigned in ParallaxSystemUnit.", this);
        }
    }

    /// <summary>
    /// Visualizes the out-of-scope and in-scope positions in the scene view using Gizmos if the object is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + outOfScopePosition, new Vector3(10, .1f, 10));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + inScopePosition, new Vector3(10, .1f, 10));
    }
}
