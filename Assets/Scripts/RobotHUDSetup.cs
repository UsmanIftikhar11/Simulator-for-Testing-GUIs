using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to create the Robot HUD structure in Unity Editor.
/// Attach to an empty GameObject and click context menu "Setup HUD" to generate the UI hierarchy.
/// After setup, assign your sprites and configure the RobotHUD component.
/// </summary>
public class RobotHUDSetup : MonoBehaviour
{
    [ContextMenu("Setup HUD")]
    public void SetupHUD()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("RobotHUDCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add RobotHUD component
        RobotHUD hud = canvasObj.AddComponent<RobotHUD>();
        
        // === TOP LEFT - Motor Data Panel ===
        GameObject motorPanel = CreatePanel(canvasObj.transform, "MotorDataPanel", 
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(50, -50), new Vector2(397, 360));
        
        // Motor data sprite background (assign data sprite.png here)
        Image motorBg = motorPanel.AddComponent<Image>();
        motorBg.color = new Color(1, 1, 1, 0.9f);
        
        // Create motor text labels
        float yOffset = -60f;
        float spacing = 50f;
        
        // RPM Row
        hud.rpmLeftText = CreateText(motorPanel.transform, "RPM_L", new Vector2(280, yOffset), "300");
        hud.rpmRightText = CreateText(motorPanel.transform, "RPM_R", new Vector2(340, yOffset), "300");
        yOffset -= spacing;
        
        // TEMP Row
        hud.tempLeftText = CreateText(motorPanel.transform, "TEMP_L", new Vector2(280, yOffset), "40.0");
        hud.tempRightText = CreateText(motorPanel.transform, "TEMP_R", new Vector2(340, yOffset), "40.0");
        yOffset -= spacing;
        
        // VOLT Row
        hud.voltLeftText = CreateText(motorPanel.transform, "VOLT_L", new Vector2(280, yOffset), "23");
        hud.voltRightText = CreateText(motorPanel.transform, "VOLT_R", new Vector2(340, yOffset), "23");
        yOffset -= spacing;
        
        // CURR Row
        hud.currLeftText = CreateText(motorPanel.transform, "CURR_L", new Vector2(280, yOffset), "23");
        hud.currRightText = CreateText(motorPanel.transform, "CURR_R", new Vector2(340, yOffset), "23");
        
        // === LEFT SIDE - Tool Display ===
        GameObject toolPanel = CreatePanel(canvasObj.transform, "ToolPanel",
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(50, 0), new Vector2(200, 150));
        
        // BAR label and value
        CreateText(toolPanel.transform, "BAR_Label", new Vector2(30, 50), "BAR").alignment = TextAlignmentOptions.Left;
        hud.barValueText = CreateText(toolPanel.transform, "BAR_Value", new Vector2(120, 50), "120.0");
        
        // Pressure graphic (for cleaning) - assign pressure.png
        GameObject pressureObj = new GameObject("PressureGraphic");
        pressureObj.transform.SetParent(toolPanel.transform, false);
        hud.pressureGraphic = pressureObj.AddComponent<Image>();
        RectTransform pressureRect = pressureObj.GetComponent<RectTransform>();
        pressureRect.anchoredPosition = new Vector2(100, -20);
        pressureRect.sizeDelta = new Vector2(150, 100);
        pressureObj.SetActive(false);
        
        // Torch graphic (for cutting) - assign torch.png
        GameObject torchObj = new GameObject("TorchGraphic");
        torchObj.transform.SetParent(toolPanel.transform, false);
        hud.torchGraphic = torchObj.AddComponent<Image>();
        RectTransform torchRect = torchObj.GetComponent<RectTransform>();
        torchRect.anchoredPosition = new Vector2(100, -20);
        torchRect.sizeDelta = new Vector2(150, 100);
        torchObj.SetActive(false);
        
        // === BOTTOM LEFT - Status Panel ===
        GameObject statusPanel = CreatePanel(canvasObj.transform, "StatusPanel",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(50, 150), new Vector2(200, 120));
        
        CreateText(statusPanel.transform, "STATUS_Label", new Vector2(50, 80), "STATUS").alignment = TextAlignmentOptions.Left;
        hud.statusText = CreateText(statusPanel.transform, "STATUS_Value", new Vector2(150, 80), "normal");
        hud.statusText.color = Color.green;
        
        // Sensor status graphics - assign sensorstatus1.png and sensorstatus2.png
        GameObject status1Obj = new GameObject("SensorStatus1");
        status1Obj.transform.SetParent(statusPanel.transform, false);
        hud.sensorStatus1 = status1Obj.AddComponent<Image>();
        RectTransform status1Rect = status1Obj.GetComponent<RectTransform>();
        status1Rect.anchoredPosition = new Vector2(100, 20);
        status1Rect.sizeDelta = new Vector2(150, 60);
        
        GameObject status2Obj = new GameObject("SensorStatus2");
        status2Obj.transform.SetParent(statusPanel.transform, false);
        hud.sensorStatus2 = status2Obj.AddComponent<Image>();
        RectTransform status2Rect = status2Obj.GetComponent<RectTransform>();
        status2Rect.anchoredPosition = new Vector2(100, 20);
        status2Rect.sizeDelta = new Vector2(150, 60);
        
        // === BOTTOM CENTER - Orientation Panel ===
        GameObject orientPanel = CreatePanel(canvasObj.transform, "OrientationPanel",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 100), new Vector2(600, 350));
        
        // Roll graphic (left) - assign rollgraphic.png
        GameObject rollObj = new GameObject("RollGraphic");
        rollObj.transform.SetParent(orientPanel.transform, false);
        rollObj.AddComponent<Image>();
        hud.rollGraphic = rollObj.GetComponent<RectTransform>();
        hud.rollGraphic.anchoredPosition = new Vector2(-200, 100);
        hud.rollGraphic.sizeDelta = new Vector2(146, 146);
        
        hud.rollText = CreateText(orientPanel.transform, "Roll_Text", new Vector2(-200, 0), "0°");
        
        // Yaw graphic (center) - assign yawgraphic.png
        GameObject yawObj = new GameObject("YawGraphic");
        yawObj.transform.SetParent(orientPanel.transform, false);
        yawObj.AddComponent<Image>();
        hud.yawGraphic = yawObj.GetComponent<RectTransform>();
        hud.yawGraphic.anchoredPosition = new Vector2(0, 100);
        hud.yawGraphic.sizeDelta = new Vector2(300, 300);
        
        // Yaw bubble (on top of yaw) - assign yawbubble.png
        GameObject yawBubbleObj = new GameObject("YawBubble");
        yawBubbleObj.transform.SetParent(orientPanel.transform, false);
        yawBubbleObj.AddComponent<Image>();
        hud.yawBubble = yawBubbleObj.GetComponent<RectTransform>();
        hud.yawBubble.anchoredPosition = new Vector2(0, 200);
        hud.yawBubble.sizeDelta = new Vector2(90, 90);
        
        hud.yawText = CreateText(orientPanel.transform, "Yaw_Text", new Vector2(0, 0), "0°");
        
        // Pitch graphic (right) - assign pitchgraphic.png
        GameObject pitchObj = new GameObject("PitchGraphic");
        pitchObj.transform.SetParent(orientPanel.transform, false);
        pitchObj.AddComponent<Image>();
        hud.pitchGraphic = pitchObj.GetComponent<RectTransform>();
        hud.pitchGraphic.anchoredPosition = new Vector2(200, 100);
        hud.pitchGraphic.sizeDelta = new Vector2(146, 146);
        
        hud.pitchText = CreateText(orientPanel.transform, "Pitch_Text", new Vector2(200, 0), "0°");
        
        // === TOP RIGHT - Back Camera ===
        GameObject cameraPanel = CreatePanel(canvasObj.transform, "BackCameraPanel",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(-50, -50), new Vector2(432, 243));
        
        // Frame - assign back camera frame.png
        Image frameImg = cameraPanel.AddComponent<Image>();
        frameImg.color = Color.white;
        
        // Camera display
        GameObject camDisplay = new GameObject("BackCameraDisplay");
        camDisplay.transform.SetParent(cameraPanel.transform, false);
        hud.backCameraDisplay = camDisplay.AddComponent<RawImage>();
        RectTransform camRect = camDisplay.GetComponent<RectTransform>();
        camRect.anchorMin = Vector2.zero;
        camRect.anchorMax = Vector2.one;
        camRect.offsetMin = new Vector2(10, 10);
        camRect.offsetMax = new Vector2(-10, -10);
        
        // === BOTTOM BAR ===
        GameObject bottomBar = new GameObject("BottomBar");
        bottomBar.transform.SetParent(canvasObj.transform, false);
        Image barImg = bottomBar.AddComponent<Image>();
        barImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform barRect = bottomBar.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 0);
        barRect.anchorMax = new Vector2(1, 0);
        barRect.pivot = new Vector2(0.5f, 0);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta = new Vector2(0, 60);
        // Set to stretch horizontally - assign bottombar.png and set Image type to Sliced or Simple
        
        // === BOTTOM RIGHT - Camera Icon ===
        GameObject cameraIcon = new GameObject("CameraIcon");
        cameraIcon.transform.SetParent(canvasObj.transform, false);
        cameraIcon.AddComponent<Image>();
        RectTransform iconRect = cameraIcon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(1, 0);
        iconRect.anchorMax = new Vector2(1, 0);
        iconRect.pivot = new Vector2(1, 0);
        iconRect.anchoredPosition = new Vector2(-50, 80);
        iconRect.sizeDelta = new Vector2(60, 60);
        
        Debug.Log("HUD Setup Complete! Now assign your sprites and the Robot reference.");
    }
    
    GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return panel;
    }
    
    TextMeshProUGUI CreateText(Transform parent, string name, Vector2 position, string defaultText)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(80, 40);
        return tmp;
    }
}
