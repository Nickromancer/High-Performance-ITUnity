using UnityEditor.ShortcutManagement;
using UnityEngine;

public class DepthCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RenderTexture render;
    private Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        /*         cam.targetTexture = render;
         */
    }

    // Update is called once per frame
    void Update()
    {

    }
}
