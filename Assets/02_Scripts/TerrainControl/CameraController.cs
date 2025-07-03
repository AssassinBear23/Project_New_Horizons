using UnityEngine;
using System.Collections;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float screenShakeDuration = 0.2f;
    [SerializeField] private float screenShakeIntensity = 0.1f;
    [SerializeField] private float screenShakeThreshold = 1;
    private Managers.GameManager m_GM;
    private Managers.Terrain.TreeManager m_TerrainManager;
    private bool isLockedToPlayer = false;
    private bool screenShaking = false;
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

        if (screenShaking)
        {
            Vector3 position = transform.position;
            position.x += Random.Range(-screenShakeIntensity, screenShakeIntensity);
            position.y += Random.Range(-screenShakeIntensity, screenShakeIntensity);
            transform.position = position;
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
        if (!Managers.GameManager.Instance.PowerUpManager.hasGoldenAcorn && !Managers.GameManager.Instance.PowerUpManager.hasLock) isLockedToPlayer = false;
    }
    public void ScreenShake(float playerSpeed)
    {
        StartCoroutine(DoScreenShake(playerSpeed));
    }
    private Vector3 startPos;
    private IEnumerator DoScreenShake(float playerSpeed)
    {
        Debug.Log((Mathf.Abs(playerSpeed) >= screenShakeThreshold) + " " + playerSpeed);
        //if (Mathf.Abs(playerSpeed) >= screenShakeThreshold)
        if (!screenShaking) startPos = transform.position;

        screenShaking = true;

        yield return new WaitForSeconds(screenShakeDuration);

        startPos.y = transform.position.y;
        transform.position = startPos;
        screenShaking = false;
    }
}