using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum IncreaseTypes { Linearly, Exponentially }
/// <summary>
/// Moves the tree segment upwards, destroys it once it's out of screen and spawns a new one
/// </summary>
public class TreeManager : MonoBehaviour
{
    [Header("Stats")]
    /// <summary>
    /// The speed at which the terrain moves upwards
    /// </summary>
    public float MovementSpeed { get; private set; } = 5;
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// The way in which the speed is increased over time
    /// </summary>
    [SerializeField] private IncreaseTypes speedIncreaseType = IncreaseTypes.Linearly;

    [SerializeField] private float speedIncrease = 0.001f;
    [SerializeField, Min(1)] private float speedMultiplier = 1.01f;

    [Header("References")]
    [SerializeField] private List<TreeTrunkController> m_treeSegments = new();
    /// <summary>
    /// A reference to the parent object in the scene to keep the hierarchy structured
    /// </summary>
    public GameObject _Parent;

    /// <summary>
    /// A reference to the tree segment prefab that  needs to be spawned
    /// </summary>
    [SerializeField] private PrefabReference prefab;

    private void Start()
    {
        if (GameManager.Instance.IsPaused) enabled = false; // Disable the script until gameplay starts
    }

    public void SetupTreeManager()
    {
        if (GameManager.Instance != null && GameManager.Instance.TreeManager == null)
            GameManager.Instance.TreeManager = this;
        else
            Debug.LogError("GameManager instance is null or TreeManager is already set.");
    }

    private void FixedUpdate()
    {
        if(!IsEnabled) return;
        
        foreach (TreeTrunkController treeSegment in m_treeSegments)
        {
            treeSegment.UpdatePosition(MovementSpeed);
        }
        IncreaseSpeed(); // Increases movement speed over time
    }

    private void IncreaseSpeed()
    {
        switch (speedIncreaseType)
        {
            case IncreaseTypes.Linearly:
                MovementSpeed += speedIncrease;
                break;
            case IncreaseTypes.Exponentially:
                MovementSpeed *= speedMultiplier;
                break;
        }
    }


    /// <summary>
    /// Adds a tree segment to the managed list.
    /// </summary>
    /// <param name="treeSegment">The tree segment to add.</param>
    public void AddTreeSegment(TreeTrunkController treeSegment)
    {
        if (!m_treeSegments.Contains(treeSegment)) m_treeSegments.Add(treeSegment);
    }

    /// <summary>
    /// Removes a tree segment from the managed list.
    /// </summary>
    /// <param name="treeSegment">The tree segment to remove.</param>
    public void RemoveTreeSegment(TreeTrunkController treeSegment)
    {
        if (m_treeSegments.Contains(treeSegment)) m_treeSegments.Remove(treeSegment);
    }

    /// <summary>
    /// Returns a list of all the branches on the last layer of the last placed & finished tree segment
    /// </summary>
    /// <returns></returns>
    public (List<Transform>, bool) GetLastBranchList()
    {
        BranchPlacingAlgorithm reference = m_treeSegments[m_treeSegments.Count - 2].GetComponent<BranchPlacingAlgorithm>();
        List<Transform> transforms = reference.lastBranches;
        bool isBird = reference.lastWasBird;
        return (transforms, isBird);
    }

    public void SpawnNewTreeSegment(TreeTrunkController treeTrunkController)
    {
        Vector3 pos = transform.position;
        pos.y -= 10f;
        Instantiate(prefab.prefab, pos, Quaternion.identity, _Parent.transform);
    }
}