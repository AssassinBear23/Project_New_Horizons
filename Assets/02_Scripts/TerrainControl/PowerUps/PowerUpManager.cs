using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace Managers
{
    public enum PowerUps { Shield, Lock, GoldenAcorn }
    public class PowerUpManager : MonoBehaviour
    {
        public UnityEvent DisableShield;
        public UnityEvent DisableLock;
        public UnityEvent DisableGoldenAcorn;

        public UnityEvent EnableShield;
        public UnityEvent EnableLock;
        public UnityEvent EnableGoldenAcorn;

        [HideInInspector] public bool hasShield = false;
        [HideInInspector] public bool hasLock = false;
        [HideInInspector] public bool hasGoldenAcorn = false;
        private void Start()
        {
            GameManager.Instance.PowerUpManager = this;
        }
        public IEnumerator PowerDuration(float time, PowerUps powerUp)
        {
            switch (powerUp)
            {
                case PowerUps.Shield:
                    hasShield = true;
                    EnableShield?.Invoke();
                    yield return new WaitForSeconds(time);
                    if (hasShield) DisablePower(PowerUps.Shield);
                    break;

                case PowerUps.Lock:
                    hasLock = true;
                    EnableLock?.Invoke();
                    yield return new WaitForSeconds(time);
                    DisablePower(PowerUps.Lock);
                    break;

                case PowerUps.GoldenAcorn:
                    hasGoldenAcorn = true;
                    EnableGoldenAcorn?.Invoke();
                    yield return new WaitForSeconds(time);
                    DisablePower(PowerUps.GoldenAcorn);
                    break;
            }
        }
        public void DisablePower(PowerUps powerUp)
        {
            switch(powerUp)
            {
                case PowerUps.Shield:
                    hasShield = false;
                    DisableShield?.Invoke();
                    break;

                case PowerUps.Lock:
                    hasLock = false;
                    DisableLock.Invoke();
                    break;

                case PowerUps.GoldenAcorn:
                    hasGoldenAcorn = false;
                    DisableGoldenAcorn?.Invoke();
                    break;
            }
        }
    }
}