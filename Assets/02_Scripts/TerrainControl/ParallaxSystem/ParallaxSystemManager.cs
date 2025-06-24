using System.Collections.Generic;
using UnityEngine;

public class ParallaxSystemManager : MonoBehaviour
{
    public static ParallaxSystemManager Instance { get; private set; }
    [SerializeField] private List<ParallaxSystemUnit> parallaxUnits;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="verticalSpeed"></param>
    public void UpdateBackgroundPositions(float verticalSpeed)
    {
        foreach (ParallaxSystemUnit unit in parallaxUnits)
        {
            unit.UpdateTransformPositions(verticalSpeed);
        }
    }
}
