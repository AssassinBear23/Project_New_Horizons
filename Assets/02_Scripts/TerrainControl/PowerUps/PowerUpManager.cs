using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace Managers
{
    public enum PowerUps { Shield, Lock, GoldenAcorn }
    public class PowerUpManager
    {
        UnityEvent DisableShield;
        UnityEvent DisableLock;
        UnityEvent DisableGoldenAcorn;

        [HideInInspector] public bool hasShield = false;
        [HideInInspector] public bool hasLock = false;
        [HideInInspector] public bool hasGoldenAcorn = false;
        public IEnumerator PowerDuration(float time, PowerUps powerUp)
        {
            switch (powerUp)
            {
                case PowerUps.Shield:
                    hasShield = true;
                    yield return new WaitForSeconds(time);
                    if (hasShield) DisablePower(PowerUps.Shield);
                    break;

                case PowerUps.Lock:
                    hasLock = true;
                    yield return new WaitForSeconds(time);
                    if (hasLock) DisablePower(PowerUps.Lock);
                    break;

                case PowerUps.GoldenAcorn:
                    hasGoldenAcorn = true;
                    yield return new WaitForSeconds(time);
                    if (hasGoldenAcorn) DisablePower(PowerUps.GoldenAcorn);
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