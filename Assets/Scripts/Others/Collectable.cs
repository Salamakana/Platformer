using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void Update()
    {
        if (Time.timeScale.Equals(0))
            return;
        transform.Rotate(Vector3.right, Space.World);
    }
}
