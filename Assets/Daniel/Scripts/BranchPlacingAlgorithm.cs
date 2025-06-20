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
        List<Transform> branchesOnPreviousLayer = new List<Transform>();

        int counter = 0;
        while (yPos > transform.localPosition.y - transform.localScale.y)
        {
            counter++;
            float amountOfBranches = random.Next(1, Mathf.FloorToInt(360 / sameYMinAngleDist));
            Debug.Log(amountOfBranches);
            Transform parent = new GameObject("BranchLayer"+counter).transform;
            parent.parent = transform;

            for (int i = 0; i <= amountOfBranches; i++)
            {
                float randomRotation = GetRandomRotation(i);

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

                    foreach(Transform treeBranch in branchesOnPreviousLayer)
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
                    {
                        Debug.Log("Rotation illegal at layer " + parent.name);
                        break;
                    }
                }

                // Create Branch
                Transform branch = Instantiate(branchPrefab, lastBranch.position, lastBranch.rotation);

                // Apply random rotation and position
                branch.localEulerAngles = new Vector3(branch.localEulerAngles.x, randomRotation, branch.localEulerAngles.z);
                branch.localPosition = new Vector3(transform.position.x, yPos, transform.position.z);
                branch.parent = parent;

                // Add to necessary lists
                branchesOnThisLayer.Add(branch);
                lastBranch = branch;
            }

            branchesOnPreviousLayer.Clear();
            
            foreach(Transform transform_ in branchesOnThisLayer)
            {
                branchesOnPreviousLayer.Add(transform_);
            }

            branchesOnThisLayer.Clear();

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