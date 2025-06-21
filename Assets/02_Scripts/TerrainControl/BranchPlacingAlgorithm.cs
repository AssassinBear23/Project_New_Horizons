using UnityEngine;
using System.Collections.Generic;
using Managers;

/// <summary>
/// Places branches on the empty tree segment randomly using a small algorithm
/// </summary>
public class BranchPlacingAlgorithm : MonoBehaviour
{
    [Header("Values")]
    [Tooltip("The Y Distance between branch layers")]
    [SerializeField] private float yInterval;

    [SerializeField] private int maxAmountOfBranches = 4;

    [Tooltip("The minimum angle between two branches on the same branch layer")]
    [SerializeField, Range(0,360)] private int sameYMinAngleDist;

    [Tooltip("The maximum angle between two branches on the same branch layer")]
    [SerializeField, Range(0, 360)] private int sameYMaxAngleDist;

    [Tooltip("The minimum angle between two branches on different branch layers")]
    [SerializeField, Range(0, 360)] private int differentYMinAngleDist;

    [Tooltip("The maximum angle between two branches on different branch layers")]
    [SerializeField, Range(0, 360)] private int differentYMaxAngleDist;

    [Tooltip("The Y offset of tree branches to make it feel more natural")]
    [SerializeField] private float yOffset;

    [Header("References")]
    [SerializeField] private Transform branchPrefab;

    [Header("DO NOT CHANGE")]
    [SerializeField] private bool hasBranches = false;

    // Internal
    [SerializeField] private Transform lastBranch;
    public List<Transform> lastBranches = new List<Transform>();
    void Start()
    {
        if (hasBranches) return;

        GetLastBranchFromPreviousTreeSegment();

        float yPos = lastBranch.position.y - yInterval;

        List<Transform> branchesOnThisLayer = new List<Transform>();

        int counter = 0;
        while (yPos > transform.localPosition.y - transform.localScale.y)
        {
            counter++;
            float amountOfBranches = Random.Range(1, maxAmountOfBranches + 1);
            Transform parent = new GameObject("BranchLayer"+counter).transform;
            parent.parent = transform;

            for (int i = 0; i <= amountOfBranches; i++)
            {
                float randomRotation = GetRandomRotation(i==0);

                if (i > 0)
                {
                    bool isTooClose = false;
                    foreach(Transform treeBranch in branchesOnThisLayer)
                    {
                        float angle = 0;
                        if (treeBranch.localEulerAngles.y < randomRotation)
                            angle = treeBranch.localEulerAngles.y + 360 - randomRotation;

                        else
                            angle = treeBranch.localEulerAngles.y - randomRotation;

                        if (angle < sameYMinAngleDist)
                        {
                            isTooClose = true;
                            break;
                        }
                    }

                    foreach(Transform treeBranch in lastBranches)
                    {
                        float angle = 0;
                        if (treeBranch.localEulerAngles.y < randomRotation)
                            angle = treeBranch.localEulerAngles.y + 360 - randomRotation;

                        else
                            angle = treeBranch.localEulerAngles.y - randomRotation;

                        if (angle < differentYMinAngleDist)
                        {
                            isTooClose = true;
                            break;
                        }
                    }

                    if (isTooClose)
                        break;
                }

                // Create Branch
                Transform branch = Instantiate(branchPrefab, lastBranch.position, lastBranch.rotation);

                // Apply random rotation and position
                float pos = yPos + Random.Range(-yOffset, yOffset);

                branch.localEulerAngles = new Vector3(branch.localEulerAngles.x, randomRotation, branch.localEulerAngles.z);
                branch.localPosition = new Vector3(transform.position.x, pos, transform.position.z);
                branch.parent = parent;

                // Add to necessary lists
                branchesOnThisLayer.Add(branch);
                lastBranch = branch;
            }

            lastBranches.Clear();
            
            foreach(Transform transform_ in branchesOnThisLayer)
            {
                lastBranches.Add(transform_);
            }

            branchesOnThisLayer.Clear();

            yPos -= yInterval;
        }
    }
    /// <summary>
    /// Returns a float that represents the rotation on the Y-axis in localEuler space, based on branches on the previous and current layer
    /// </summary>
    /// <param name="isFirstBranchOnLayer"></param>
    /// <returns></returns>
    private float GetRandomRotation(bool isFirstBranchOnLayer)
    {
        Vector3 startRotation = lastBranch.localEulerAngles;
        float randomRotation = 0;

        int leftOrRight = Random.Range(0, 2);

        // Make sure there is always a reachable branch from the previous layer to this one
        if (isFirstBranchOnLayer)
        {
            if (leftOrRight == 1)
                randomRotation = Random.Range((int)startRotation.y + differentYMinAngleDist,
                    (int)Mathf.Min(startRotation.y + 360 - differentYMinAngleDist, startRotation.y + differentYMaxAngleDist));
            else
                randomRotation = Random.Range((int)startRotation.y - differentYMinAngleDist,
                    (int)Mathf.Min(startRotation.y - 360 + differentYMinAngleDist, startRotation.y - differentYMaxAngleDist));
        }

        // Prevent branches from being too close
        else
        {
            if (leftOrRight == 1)
                randomRotation = Random.Range((int)startRotation.y + sameYMinAngleDist,
                    (int)Mathf.Min(startRotation.y + 360 - sameYMinAngleDist, startRotation.y + sameYMaxAngleDist));

            else
                randomRotation = Random.Range((int)startRotation.y - sameYMinAngleDist,
                    (int)Mathf.Min(startRotation.y - 360 + sameYMinAngleDist, startRotation.y - sameYMaxAngleDist));
        }
            
        // Prevent floating point errors by keeping the rotation between 0 and 360
        while (randomRotation > 360) randomRotation -= 360;
        while (randomRotation < 0) randomRotation += 360;

        return randomRotation;
    }
    /// <summary>
    /// Sets the lastBranch variable in this script to the last branch of the previously generated tree segment
    /// </summary>
    private void GetLastBranchFromPreviousTreeSegment()
    {
        foreach(Transform branch in GameManager.Instance.GetLastBranchList())
        {
            lastBranches.Add(branch);
        }
        lastBranch = lastBranches[lastBranches.Count - 1];
    }
}