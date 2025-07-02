using UnityEngine;
using System.Collections;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float screenShakeDuration = 0.2f;
    [SerializeField] private float screenShakeIntensity = 0.1f;
    [SerializeField] private float screenShakeSpeedThreshold = 1;
    private Managers.GameManager m_GM;
    private Managers.Terrain.TreeManager m_TerrainManager;
    private bool isLockedToPlayer = false;
    private bool screenShaking = false;
    private float _PlayerSpeed;
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
            position.x += Random.Range(-screenShakeIntensity, screenShakeIntensity);// * _PlayerSpeed * 0.5f;
            position.y += Random.Range(-screenShakeIntensity, screenShakeIntensity);// * _PlayerSpeed * 0.5f;
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
        if (!screenShaking) StartCoroutine(DoScreenShake(playerSpeed));
    }
    private IEnumerator DoScreenShake(float playerSpeed)
    {
        if (Mathf.Abs(playerSpeed) >= screenShakeSpeedThreshold)
        {
            _PlayerSpeed = playerSpeed;

            screenShaking = true;

            float timePassed = 0;

            Vector3 startPos = transform.position;

            while (timePassed < screenShakeDuration)
            {
                yield return null;
                timePassed += Time.deltaTime;
            }

            startPos.y = transform.position.y;
            transform.position = startPos;
            screenShaking = false;
        }
    }
}