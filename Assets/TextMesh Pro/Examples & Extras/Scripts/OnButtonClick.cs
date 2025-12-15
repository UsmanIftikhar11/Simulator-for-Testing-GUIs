using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OnButtonClick : MonoBehaviour
{
    public bool isPaused = false;

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
        else if (currentScene == "PausePage")
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

    public void OnPauseButton()
    {
        if (SceneManager.GetActiveScene().name != "ShipYard Demo") return;

        Time.timeScale = 0f;
        SceneManager.LoadScene("PausePage", LoadSceneMode.Single);
        Debug.Log("Game paused and moved to PausePage");
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
        // Only allow starting from StartPage
        if (SceneManager.GetActiveScene().name != "StartPage") return;

        Time.timeScale = 1f;
        SceneManager.LoadScene("ShipYard Demo", LoadSceneMode.Single);
        Debug.Log("Moved from StartPage to ShipYard Demo");
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