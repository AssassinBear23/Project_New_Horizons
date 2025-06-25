using Managers;
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

    public void UpdatePosition(float movementSpeed)
    {
        //Debug.Log($"Updating position of {gameObject.name} with speed {movementSpeed}", this);
        transform.position += new Vector3(0, movementSpeed, 0);
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
