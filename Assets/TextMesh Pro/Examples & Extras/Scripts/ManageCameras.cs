using UnityEngine;
using UnityEngine.InputSystem;

public class ManageCameras : MonoBehaviour
{
    public Camera thirdPerson;    // Always fullscreen on display 1 (when available)
    public Camera firstPerson;   // Toggle on display 0
    public Camera firstPersonBack;    // Toggle on display 0

    private int display0Index = 0; // 0 = firstPerson main, 1 = firstPersonBack main

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
        firstPerson.gameObject.SetActive(true);
        firstPersonBack.gameObject.SetActive(true);

        SetupDisplays();
        UpdateCameras();
    }

    void Update()
    {
        // Press C to toggle cameras on display 0 (only in multi-display mode)
        if (Display.displays.Length > 1 && Keyboard.current.cKey.wasPressedThisFrame)
        {
            display0Index = 1 - display0Index; // toggle 0 ↔ 1
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
    }

    void UpdateCameras()
    {
        if (Display.displays.Length > 1)
        {
            // Multi-display: thirdPerson on display 1, toggle front/back on display 0

            // Display 1:  thirdPerson fullscreen
            thirdPerson.targetDisplay = 1;
            thirdPerson.rect = new Rect(0, 0, 1, 1);
            thirdPerson.depth = fullDepth;
            thirdPerson.enabled = true;

            // Display 0: toggle between firstPerson and firstPersonBack
            float overlayWidth = 0.25f;
            float overlayHeight = 0.25f;

            if (display0Index == 0)
            {
                // firstPerson fullscreen, firstPersonBack overlay top-right
                firstPerson.targetDisplay = 0;
                firstPerson.rect = new Rect(0, 0, 1, 1);
                firstPerson.depth = fullDepth;
                firstPerson.enabled = true;

                firstPersonBack.targetDisplay = 0;
                firstPersonBack.rect = new Rect(0.75f, 0.75f, overlayWidth, overlayHeight);
                firstPersonBack.depth = overlayDepth;
                firstPersonBack.enabled = true;
            }
            else
            {
                // firstPersonBack fullscreen, firstPerson overlay top-right
                firstPersonBack.targetDisplay = 0;
                firstPersonBack.rect = new Rect(0, 0, 1, 1);
                firstPersonBack.depth = fullDepth;
                firstPersonBack.enabled = true;

                firstPerson.targetDisplay = 0;
                firstPerson.rect = new Rect(0.75f, 0.75f, overlayWidth, overlayHeight);
                firstPerson.depth = overlayDepth;
                firstPerson.enabled = true;
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