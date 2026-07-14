using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuSetup
{
    [MenuItem("Tools/Create Pause Menu UI")]
    public static void CreatePauseMenu()
    {
        // 1. Pastikan EventSystem ada
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 2. Buat Canvas
        GameObject canvasObj = new GameObject("PauseMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50; // Pause menu harus tampil paling atas
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 3. Controller
        GameObject controllerObj = new GameObject("PauseMenuController");
        controllerObj.transform.SetParent(canvasObj.transform, false);
        PauseMenuController controller = controllerObj.AddComponent<PauseMenuController>();

        // 4. Tombol Pause di pojok kanan atas (HUD)
        GameObject btnPauseHUD = new GameObject("Btn_PauseHUD");
        btnPauseHUD.transform.SetParent(canvasObj.transform, false);
        Image hudImg = btnPauseHUD.AddComponent<Image>();
        hudImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform hudRect = btnPauseHUD.GetComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(1, 1);
        hudRect.anchorMax = new Vector2(1, 1);
        hudRect.pivot = new Vector2(1, 1);
        hudRect.anchoredPosition = new Vector2(-50, -50);
        hudRect.sizeDelta = new Vector2(120, 120);

        Button hudBtn = btnPauseHUD.AddComponent<Button>();
        UnityEngine.Events.UnityAction pauseAction = new UnityEngine.Events.UnityAction(controller.PauseGame);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(hudBtn.onClick, pauseAction);

        // Icon Pause "||"
        GameObject hudTextObj = new GameObject("Text");
        hudTextObj.transform.SetParent(btnPauseHUD.transform, false);
        Text hudText = hudTextObj.AddComponent<Text>();
        hudText.text = "||";
        hudText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        hudText.fontSize = 60;
        hudText.alignment = TextAnchor.MiddleCenter;
        hudText.color = Color.white;
        RectTransform hudTextRect = hudTextObj.GetComponent<RectTransform>();
        hudTextRect.anchorMin = Vector2.zero;
        hudTextRect.anchorMax = Vector2.one;
        hudTextRect.sizeDelta = Vector2.zero;

        // 5. Panel Pause (Overlay Gelap)
        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);
        Image panelImg = pausePanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.85f); // Hitam transparan
        
        RectTransform panelRect = pausePanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Hubungkan panel ke controller
        controller.pausePanel = pausePanel;

        // 6. Text Judul "PAUSED"
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(pausePanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "PAUSED";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 120;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 300);
        titleRect.sizeDelta = new Vector2(800, 200);

        // Fungsi Helper untuk membuat tombol
        void CreateMenuButton(string name, string text, Vector2 pos, Color color, UnityEngine.Events.UnityAction action)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(pausePanel.transform, false);
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = color;
            
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(400, 100);
            
            Button btn = btnObj.AddComponent<Button>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, action);

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 45;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            RectTransform txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;
        }

        // 7. Buat ke-3 tombol
        CreateMenuButton("Btn_Resume", "RESUME", new Vector2(0, 50), new Color(0.2f, 0.6f, 0.2f), new UnityEngine.Events.UnityAction(controller.ResumeGame));
        CreateMenuButton("Btn_MainMenu", "MAIN MENU", new Vector2(0, -80), new Color(0.2f, 0.4f, 0.8f), new UnityEngine.Events.UnityAction(controller.LoadMainMenu));
        CreateMenuButton("Btn_Exit", "EXIT GAME", new Vector2(0, -210), new Color(0.8f, 0.2f, 0.2f), new UnityEngine.Events.UnityAction(controller.QuitGame));

        // Sembunyikan panel secara default
        pausePanel.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Pause Menu");
        Selection.activeGameObject = canvasObj;

        Debug.Log("Pause Menu berhasil dibuat di Scene!");
    }
}
