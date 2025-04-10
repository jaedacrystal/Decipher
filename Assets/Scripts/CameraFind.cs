using UnityEngine;

public class CameraFind : MonoBehaviour
{
    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null && canvas.worldCamera != mainCamera)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null && canvas.worldCamera != mainCamera)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }
}
