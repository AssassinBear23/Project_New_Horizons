using System.Collections.Generic;
using UnityEngine;

public class ParallaxSystemManager : MonoBehaviour
{
    [SerializeField] private List<ParallaxSystemUnit> parallaxUnits;

    public void UpdateSpeed(float newSpeed)
    {
        foreach (ParallaxSystemUnit unit in parallaxUnits)
        {
            unit.UpdateTransformPositions(newSpeed);
        }
    }
}
