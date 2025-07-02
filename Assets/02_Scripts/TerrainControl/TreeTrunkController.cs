using Managers;
using Managers.Terrain;
using UnityEngine;

public class TreeTrunkController : MonoBehaviour
{
    private TreeManager m_Tm;
    private bool hasSpawned = false;

    private void Start()
    {
        m_Tm = GameManager.Instance.TreeManager;
        m_Tm.AddTreeSegment(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Removes and destroys tree segment when reaching the destroyer object
        if (other.CompareTag("Destroyer"))
        {
            m_Tm.RemoveTreeSegment(this);
            Destroy(gameObject);
        }

        // Spawns a new tree segment under this one when it touches the spawner object
        else if (other.CompareTag("Spawner") && !hasSpawned)
        {
            m_Tm.SpawnNewTreeSegment(this);
            hasSpawned = true;
        }
    }
}
