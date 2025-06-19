using UnityEngine;
using System.Collections.Generic;
using RangeAttribute = UnityEngine.RangeAttribute;
public class BranchPlacingAlgorithm : MonoBehaviour
{
    [Header("Values")]
    [Tooltip("The Y Distance between branch layers")]
    [SerializeField] private float yInterval;

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
    System.Random random;
    private List<Transform> branches = new List<Transform>();
    public Transform lastBranch;
    private void Awake()
    {
        random = new System.Random(System.Environment.TickCount);
    }
    void Start()
    {
        if (hasBranches) return;

        GetLastBranchFromPreviousTreeSegment();

        float yPos = transform.position.y + transform.localScale.y;

        List<Transform> branchesOnThisLayer = new List<Transform>();

        while (yPos > transform.localPosition.y - transform.localScale.y)
        {
            float amountOfBranches = random.Next(1, Mathf.FloorToInt(360 / sameYMinAngleDist));

            for (int i = 0; i <= amountOfBranches; i++)
            {
                float randomRotation = GetRandomRotation(i);

                if (i > 0)
                {
                    float angle = 0;
                    if (branchesOnThisLayer[0].localEulerAngles.y < randomRotation)
                        angle = branchesOnThisLayer[0].localEulerAngles.y + 360 - randomRotation;

                    else
                        angle = branchesOnThisLayer[0].localEulerAngles.y - randomRotation;

                    if (angle < sameYMinAngleDist) break;
                }

                // Create Branch
                Transform branch = Instantiate(branchPrefab, lastBranch.position, lastBranch.rotation);

                // Apply random rotation and position
                branch.localEulerAngles = new Vector3(branch.localEulerAngles.x, randomRotation, branch.localEulerAngles.z);
                branch.position = new Vector3(transform.position.x, yPos, transform.position.z);
                branch.parent = transform;

                // Add to necessary lists
                branchesOnThisLayer.Add(branch);
                branches.Add(branch);
                lastBranch = branch;
            }

            yPos -= yInterval;
        }
    }
    private float GetRandomRotation(int i)
    {
        Vector3 startRotation = lastBranch.localEulerAngles;
        float randomRotation = 0;

        if (i == 0)
            randomRotation = random.Next((int)startRotation.y + differentYMinAngleDist,
                (int)Mathf.Min(startRotation.y + 360 - differentYMinAngleDist, startRotation.y + differentYMaxAngleDist));

        else
            randomRotation = random.Next((int)startRotation.y + sameYMinAngleDist,
                (int)Mathf.Min(startRotation.y + 360 - sameYMinAngleDist, startRotation.y + sameYMaxAngleDist));

        while (randomRotation > 360) randomRotation -= 360;

        return randomRotation;
    }
    private void GetLastBranchFromPreviousTreeSegment()
    {
        BranchPlacingAlgorithm last = GameManager.instance.treeSegments[GameManager.instance.treeSegments.Count - 2].GetComponent<BranchPlacingAlgorithm>();
        lastBranch = last.lastBranch;
    }
}