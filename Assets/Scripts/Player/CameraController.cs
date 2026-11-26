using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    [Header("Horizontal")]
    public float horizontalSmoothTime = 0.15f;

    [Header("Verticaal (beperkt zodat daklijn in beeld blijft)")]
    public float minY = 0f;
    public float maxY = 5f;

    private float velocityX;

    void LateUpdate()
    {
        if (target == null) return;

        // Horizontaal volgen met demping
        float newX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocityX, horizontalSmoothTime);

        // Verticaal geclampt tussen min/max zodat daklijn niet uit beeld schuift
        float clampedY = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(newX, clampedY, transform.position.z);
    }
}
