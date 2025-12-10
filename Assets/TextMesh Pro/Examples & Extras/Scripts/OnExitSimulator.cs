using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
