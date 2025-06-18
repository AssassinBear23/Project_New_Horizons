using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RangeAttribute = UnityEngine.RangeAttribute;
public class BranchPlacingAlgorithm : MonoBehaviour
{
    [Header("Values")]
    [Tooltip("The Y Distance between branch layers")]
    [SerializeField] private float yInterval;

    [Tooltip("The minimum angle between two branches on the same branch layer")]
    [SerializeField] private float sameYMinAngleDist;

    [Tooltip("The maximum angle between two branches on the same branch layer")]
    [SerializeField] private float sameYMaxAngleDist;

    [Tooltip("The minimum angle between two branches on different branch layers")]
    [SerializeField] private float differentYMinAngleDist;

    [Tooltip("The maximum angle between two branches on different branch layers")]
    [SerializeField] private float differentYMaxAngleDist;

    [Tooltip("The Y offset of tree branches to make it feel more natural")]
    [SerializeField] private float yOffset;

    [Header("DO NOT CHANGE")]
    [SerializeField] private bool hasBranches = false;

    // Internal
    System.Random random;
    private List<Transform> branches = new List<Transform>();
    private Transform lastBranch;
    private void Awake()
    {
        random = new System.Random(System.Environment.TickCount);
    }
    void Start()
    {
        if (hasBranches) return;

        float amountOfBranches = random.Next(1, Mathf.FloorToInt(360 / sameYMinAngleDist) + 1);

        for (int i = 0; i <= amountOfBranches; i++)
        {
        }
    }
    private void GetLastBranchFromPreviousTreeSegment()
    {

    }
}
