using UnityEngine;

[RequireComponent(typeof(Camera))]
public class KeepOrthographicScale : MonoBehaviour
{
    [SerializeField, Min(1f)] private float referenceHeightPixels = 1080f;
    [SerializeField, Min(0.1f)] private float referenceOrthographicSize = 5f;

    private Camera sceneCamera;
    private int previousHeight = -1;

    private void Awake()
    {
        sceneCamera = GetComponent<Camera>();
        sceneCamera.orthographic = true;
    }

    private void LateUpdate()
    {
        if (Screen.height <= 0 || previousHeight == Screen.height)
            return;

        previousHeight = Screen.height;
        // Pixel density remains constant: pixels/unit = Screen.height / (2 * orthographicSize).
        sceneCamera.orthographicSize = referenceOrthographicSize *
            Screen.height / referenceHeightPixels;
    }
}
