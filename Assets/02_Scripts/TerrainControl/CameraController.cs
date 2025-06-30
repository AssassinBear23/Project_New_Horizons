using UnityEngine;
public class CameraController : MonoBehaviour
{
    private Managers.GameManager m_GM;
    private Managers.Terrain.TreeManager m_TerrainManager;
    private bool isLockedToPlayer = false;
    private void Start()
    {
        m_GM = Managers.GameManager.Instance;
        m_TerrainManager = m_GM.TreeManager;
        m_TerrainManager.InitializeCameraController(this);
    }
    public void UpdatePosition(float movement)
    {
        if (isLockedToPlayer) LockToPlayer();

        else
        {
            Vector3 pos = transform.position;
            pos.y -= movement;
            transform.position = pos;
        }
    }
    private void LockToPlayer()
    {
        Vector3 pos = transform.position;
        pos.y = m_GM.GetPlayer().transform.position.y;
        transform.position = pos;
    }
    public void Lock()
    {
        isLockedToPlayer = true;
    }
    public void Unlock()
    {
        isLockedToPlayer = false;
    }
}
