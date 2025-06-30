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

    [SerializeField, Range(0f,1f)] private float chanceToSpawnBird;

    [Header("Power Ups")]
    [SerializeField, Range(0f, 1f)] private float chanceForPowerUpOnBranch;
    [SerializeField, Min(0)] private int maxPowerUpsOnSegment;

    [Header("References")]
    [SerializeField] private Transform lastBranch;
    public List<Transform> lastBranches = new();
    [SerializeField] private List<Transform> branchPrefabs = new();
    [SerializeField] private Transform birdPrefab;
    [SerializeField] private List<Transform> powerUpPrefabs = new();

    [Header("DO NOT CHANGE")]
    [SerializeField] private bool hasBranches;
    public bool lastWasBird = false;

    private int powerUpsOnSegment;
    void Start()
    {
        if (hasBranches) return;

        GetLastBranchFromPreviousTreeSegment();

        CreateLayers();
    }
    /// <summary>
    /// Creates all the layers of branches/birds on this tree segment
    /// </summary>
    private void CreateLayers()
    {
        float yPos = lastBranch.position.y - yInterval;

        List<Transform> branchesOnThisLayer = new List<Transform>();
        int counter = 0;
        while (yPos > transform.localPosition.y - transform.localScale.y)
        {
            Transform parent = new GameObject("BranchLayer" + counter).transform;
            parent.parent = transform;
            counter++;

            // 2 birds can't spawn under each other, chance to spawn a bird
            if (!lastWasBird && Random.Range(0f, 1f) <= chanceToSpawnBird)
            {
                Transform bird = CreateBird(yPos, parent);

                // Add to necessary lists
                branchesOnThisLayer.Add(bird);
                lastBranch = bird;
                lastWasBird = true;
            }

            // If chance to spawn bird was not hit, make branch layer instead
            else
            {
                foreach (Transform t in CreateBranches(branchesOnThisLayer, yPos, parent))
                {
                    branchesOnThisLayer.Add(t);
                }

                lastBranches.Clear();

                foreach (Transform transform_ in branchesOnThisLayer)
                {
                    lastBranches.Add(transform_);
                }

                lastWasBird = false;
            }

            

            branchesOnThisLayer.Clear();

            yPos -= yInterval;
        }
    }
    /// <summary>
    /// Instantiates a random amount of branches
    /// </summary>
    /// <param name="branchesOnThisLayer"></param>
    /// <returns>a list of transforms of all branches created</returns>
    private List<Transform> CreateBranches(List<Transform> _BranchesOnThisLayer, float _YPos, Transform _Parent)
    {
        List<Transform> list = new List<Transform>();

        float amountOfBranches = Random.Range(1, maxAmountOfBranches + 1);
        
        for (int i = 0; i <= amountOfBranches; i++)
        {
            float randomRotation = GetRandomRotation(i == 0);

            if (i > 0)
            {
                bool isTooClose = false;
                foreach (Transform treeBranch in _BranchesOnThisLayer)
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

                foreach (Transform treeBranch in lastBranches)
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
            Transform toSpawn = branchPrefabs[Random.Range(0, branchPrefabs.Count)];
            Transform branch = Instantiate(toSpawn, lastBranch.position, lastBranch.rotation);

            // Apply random rotation and position
            float pos = _YPos + Random.Range(-yOffset, yOffset);

            branch.localEulerAngles = new Vector3(branch.localEulerAngles.x, randomRotation, branch.localEulerAngles.z);
            branch.localPosition = new Vector3(transform.position.x, pos, transform.position.z);
            branch.parent = _Parent;

            SpawnPowerUps(branch);

            // Add to necessary lists
            list.Add(branch);
            lastBranch = branch;
        }

        return list;
    }
    private void SpawnPowerUps(Transform branch)
    {
        if (Random.Range(0f,1f) <= chanceForPowerUpOnBranch && powerUpsOnSegment < maxPowerUpsOnSegment)
        {
            powerUpsOnSegment++;
            Transform toSpawnPowerup = powerUpPrefabs[^1];//Random.Range(0, powerUpPrefabs.Count)];
            Transform spawned = Instantiate(toSpawnPowerup, branch);
            Vector3 pos = spawned.localPosition;
            pos.x = 0;
            pos.y = 0.5f;
            pos.z = 0;
            spawned.localPosition = pos;
            spawned.localEulerAngles = new Vector3(0, 90, 0);
        }
    }
    /// <summary>
    /// Instantiates a bird
    /// </summary>
    /// <returns>the transform of the bird that is instantiated</returns>
    private Transform CreateBird(float _YPos, Transform _Parent)
    {
        Transform bird = Instantiate(birdPrefab, lastBranch.position, lastBranch.rotation);

        // Apply random rotation and position
        float pos = _YPos + Random.Range(-yOffset, yOffset);

        bird.localEulerAngles = new Vector3(bird.localEulerAngles.x, 0, bird.localEulerAngles.z);
        bird.localPosition = new Vector3(transform.position.x, pos, transform.position.z);
        bird.parent = _Parent;

        lastBranch = bird;

        lastBranches.Clear();
        lastBranches.Add(bird);

        return bird;
    }
    /// <summary>
    /// Returns a float that represents the rotation on the Y-axis in localEuler space, based on branches on the previous and current layer
    /// </summary>
    /// <param name="isFirstBranchOnLayer"></param>
    /// <returns></returns>
    private float GetRandomRotation(bool isFirstBranchOnLayer)
    {
        Vector3 startRotation = lastBranch.localEulerAngles;
        float randomRotation;

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
        var branches = GameManager.Instance.TreeManager.GetLastBranchList();
        lastBranches = branches.Item1;
        lastBranch = lastBranches[^1];
        lastWasBird = branches.Item2;
    }
}