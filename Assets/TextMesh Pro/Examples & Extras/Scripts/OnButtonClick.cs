using UnityEngine;
using UnityEngine.SceneManagement;

public class OnButtonClick : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("ShipYard Demo");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
    
}
