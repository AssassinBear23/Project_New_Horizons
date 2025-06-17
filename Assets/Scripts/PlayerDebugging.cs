using UnityEngine;

public class PlayerDebugging : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("entered");
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("staying");
    }
}
