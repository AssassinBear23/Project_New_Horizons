using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<TreeController> treeSegments = new List<TreeController>();
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public void StopGameplay()
    {
        Controls.instance.enabled = false;
        foreach(TreeController treeSegment in treeSegments)
        {
            treeSegment.enabled = false;
        }
    }
    public void AddTreeSegment(TreeController treeSegment)
    {
        treeSegments.Add(treeSegment);
    }
    public void RemoveTreeSegment(TreeController treeSegment)
    {
        treeSegments.Remove(treeSegment);
    }
}