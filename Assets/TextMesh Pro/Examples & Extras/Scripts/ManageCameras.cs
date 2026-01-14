using UnityEngine;
using UnityEngine.InputSystem;

public class ManageCameras : MonoBehaviour
{
    public Camera thirdPerson;    // Always fullscreen on display 0
    public Camera firstPerson;   // Toggle on display 1
    public Camera firstPersonBack;    // Toggle on display 1

    private int display1Index = 0; // 0 = firstPerson, 1 = firstPersonBack

    private float fullDepth = 0f;
    private float overlayDepth = 1f;

    void Start()
    {
        if (thirdPerson == null || firstPerson == null || firstPersonBack == null)
        {
            Debug.LogError("Cameras not assigned!");
            return;
        }

        thirdPerson.gameObject.SetActive(true);
        firstPerson.gameObject.SetActive(false);
        firstPersonBack.gameObject.SetActive(false);

        SetupDisplays();
        UpdateCameras();
    }

    void Update()
    {
        // Press C to toggle cameras on display 1 (only in multi-display mode)
        if (Display.displays.Length > 1 && Keyboard.current.cKey.wasPressedThisFrame)
        {
            display1Index = 1 - display1Index; // toggle 0 ↔ 1
            UpdateCameras();
        }
    }

    void SetupDisplays()
    {
        // Activate second display if available
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        // Main camera always fullscreen on display 0
        thirdPerson.targetDisplay = 0;
        thirdPerson.rect = new Rect(0, 0, 1, 1);
        thirdPerson.depth = fullDepth;
        thirdPerson.enabled = true;
    }

    void UpdateCameras()
    {
        if (Display.displays.Length > 1)
        {
            // Multi-display: thirdPerson fixed, toggle front/back on display 1
            thirdPerson.targetDisplay = 0;
            thirdPerson.rect = new Rect(0, 0, 1, 1);
            thirdPerson.depth = fullDepth;
            thirdPerson.enabled = true;
            if (display1Index == 0)
            {
                firstPerson.targetDisplay = 1;
                firstPerson.rect = new Rect(0, 0, 1, 1);
                firstPerson.depth = fullDepth;
                firstPerson.enabled = true;
                firstPersonBack.enabled = false;
            }
            else
            {
                firstPersonBack.targetDisplay = 1;
                firstPersonBack.rect = new Rect(0, 0, 1, 1);
                firstPersonBack.depth = fullDepth;
                firstPersonBack.enabled = true;
                firstPerson.enabled = false;
            }
        }
        else
        {
            // Single display: main fullscreen, BOTH front and back as overlays side-by-side
            thirdPerson.targetDisplay = 0;
            thirdPerson.rect = new Rect(0, 0, 1, 1);
            thirdPerson.depth = fullDepth;
            thirdPerson.enabled = true;
            float overlayWidth = 0.25f;
            float overlayHeight = 0.25f;
            // Front overlay: top-right, left side
            firstPerson.targetDisplay = 0;
            firstPerson.rect = new Rect(0.5f, 0.75f, overlayWidth, overlayHeight);
            firstPerson.depth = overlayDepth;
            firstPerson.enabled = true;
            // Back overlay: top-right, right side
            firstPersonBack.targetDisplay = 0;
            firstPersonBack.rect = new Rect(0.75f, 0.75f, overlayWidth, overlayHeight);
            firstPersonBack.depth = overlayDepth;
            firstPersonBack.enabled = true;
        }
    }

}
