using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitApplication()
    {
        Debug.Log("Quitting application...");

        #if UNITY_EDITOR
            // Stop play mode in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Quit in a real build
            Application.Quit();
        #endif
    }
}
