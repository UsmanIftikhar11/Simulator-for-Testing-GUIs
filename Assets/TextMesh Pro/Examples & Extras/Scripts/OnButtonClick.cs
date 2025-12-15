using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OnButtonClick : MonoBehaviour
{
    public bool isPaused = false;

    private void Update()
    {
        // Use unscaledDeltaTime check to ensure Update runs even when Time.timeScale = 0
        string currentScene = SceneManager.GetActiveScene().name;

        // StartPage controls
        if (currentScene == "StartPage")
        {
            // X key or Gamepad South button (X on PlayStation, A on Xbox) - Exit
            if ((Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                QuitGame();
            }

            // A key or Gamepad East button (Circle on PlayStation, B on Xbox) - Start Game
            if ((Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                OnStartButton();
            }
        }

        // PausePage controls
        else if (currentScene == "PausePage")
        {
            // X key or Gamepad South button - Exit
            if ((Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                QuitGame();
            }

            // A key or Gamepad East button - Continue
            if ((Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                Debug.Log("A button pressed - Continue");
                onContinue();
            }

            // B key or Gamepad West button - Restart
            if ((Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame))
            {
                Debug.Log("B button pressed - Restart");
                onRestart();
            }
        }

        // ShipYard Demo controls are handled in RobotMovement script
    }

    private bool IsInGame()
    {
        // Check if we're in the main game scene
        return SceneManager.GetActiveScene().name == "ShipYard Demo";
    }

    public void OnPauseButton()
    {
        if (!IsInGame())
        {
            // If we're on the StartPage, this shouldn't be called
            Debug.LogWarning("Pause button called but not in game");
            return;
        }
        PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("PausePage", LoadSceneMode.Single);
        Debug.Log("Game paused");
    }

    public void onContinue()
    {
        Debug.Log("Continue method called");
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
        Debug.Log("Game continued");
    }

    public void onRestart()
    {
        Debug.Log("Restart method called");
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
        Debug.Log("Scene restarted");
    }

    public void OnStartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
    }

    public void OnExitButton()
    {
        QuitGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}