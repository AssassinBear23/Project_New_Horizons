using UnityEngine;
public class PlayerDeath : MonoBehaviour
{
    private void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (/*pos.x < 0 || pos.x > 1 ||*/ pos.y < -0.05f || pos.y > 1.05f) GoDie();
    }
    private void GoDie()
    {
        Debug.Log("Deadge");
    }
}
