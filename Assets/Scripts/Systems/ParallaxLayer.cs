using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float parallaxFactor = 0.5f;

    private Transform cam;
    private Vector3 startPosition;
    private float startCameraX;

    private void Start()
    {
        cam = Camera.main.transform;
        startPosition = transform.position;
        startCameraX = cam.position.x;
    }

    private void LateUpdate()
    {
        float travelled = cam.position.x - startCameraX;
        transform.position = new Vector3(
            startPosition.x + travelled * parallaxFactor,
            startPosition.y,
            startPosition.z);
    }
}