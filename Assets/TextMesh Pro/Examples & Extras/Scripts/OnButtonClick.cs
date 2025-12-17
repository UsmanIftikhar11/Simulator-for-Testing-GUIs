using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class OnButtonClick : MonoBehaviour
{
   // public bool isPaused = false;
    public static bool IsPaused { get; private set; } = false; // ← Add this

    private static OnButtonClick Instance { get; set; }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // --- START PAGE ---
        if (currentScene == "StartPage")
        {
            // A / buttonSouth → go into the game
            if ((Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                OnStartButton();
            }

            // X / buttonWest → leave the game
            if ((Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
            {
                QuitGame();
            }
        }

        // --- PAUSE PAGE ---
        else if (SceneManager.GetSceneByName("PausePage").isLoaded)
        {
            // A / buttonSouth → continue
            if ((Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                onContinue();
            }

            // B / buttonEast → restart
            if ((Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                onRestart();
            }

            // X / buttonWest → leave game
            if ((Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
            {
                QuitGame();
            }
        }

        // --- ShipYard Demo (main game) ---
        // gameplay controls handled in RobotMovement.cs
    }


    private bool IsInGame()
    {
        // Check if we're in the main game scene
        return SceneManager.GetActiveScene().name == "ShipYard Demo";
    }

    public static float UnpauseTime { get; private set; } = 0f; 

    private System.Collections.IEnumerator CleanupEventSystems()
    {
        yield return null; // Wait one frame for scene to load

        var eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();

        // Keep only the first one, destroy the rest
        for (int i = 1; i < eventSystems.Length; i++)
        {
            Destroy(eventSystems[i].gameObject);
        }

        Debug.Log($"Cleaned up EventSystems.  Remaining: {FindObjectsOfType<UnityEngine.EventSystems.EventSystem>().Length}");
    }

    public void OnPauseButton()
    {
        if (SceneManager.GetActiveScene().name != "ShipYard Demo") return;

        IsPaused = true;
        Time.timeScale = 0f;

        // Disable robot input
        RobotMovement robot = FindFirstObjectByType<RobotMovement>();
        if (robot != null)
        {
            robot.SetInputEnabled(false);
        }

        SceneManager.LoadScene("PausePage", LoadSceneMode.Additive);
        StartCoroutine(CleanupEventSystems());
        Debug.Log("Game paused and moved to PausePage");
    }

    public void onContinue()
    {
        Debug.Log("Continue method called");
        StartCoroutine(ContinueAfterFrame());
    }

    private System.Collections.IEnumerator ContinueAfterFrame()
    {
        SceneManager.UnloadSceneAsync("PausePage");
        yield return null;
        yield return null; // Wait 2 frames

        IsPaused = false;
        Time.timeScale = 1f;

        // Re-enable robot input after a small delay
        yield return new WaitForSecondsRealtime(0.15f);

        RobotMovement robot = FindFirstObjectByType<RobotMovement>();
        if (robot != null)
        {
            robot.SetInputEnabled(true);
        }

        Debug.Log("Game continued and input re-enabled");
    }

    public void onRestart()
    {
        Debug.Log("Restart method called");
        IsPaused = false; // ← Set to false
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
        Debug.Log("Scene restarted");
    }

    public void OnStartButton()
    {
        // Only allow starting from StartPage
        if (SceneManager.GetActiveScene().name != "StartPage") return;

        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
        Debug.Log("Moved from StartPage to ShipYard Demo");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}