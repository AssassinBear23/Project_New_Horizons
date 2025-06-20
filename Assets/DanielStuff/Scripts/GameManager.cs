using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public List<TreeController> treeSegments = new List<TreeController>();
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void StartGamePlay()
    {
        Controls.instance.enabled = true;
        foreach(TreeController treeSegment in treeSegments)
        {
            treeSegment.enabled = true;
        }
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
        if (!treeSegments.Contains(treeSegment)) treeSegments.Add(treeSegment);
    }
    public void RemoveTreeSegment(TreeController treeSegment)
    {
        if (treeSegments.Contains(treeSegment)) treeSegments.Remove(treeSegment);
    }
}