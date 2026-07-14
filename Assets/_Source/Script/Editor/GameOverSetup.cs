using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameOverSetup
{
    [MenuItem("Tools/Create Game Over UI")]
    public static void CreateGameOverUI()
    {
        // 1. Pastikan EventSystem ada
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 2. Buat Canvas
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Paling atas (di atas pause)
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 3. Controller
        GameObject controllerObj = new GameObject("GameOverController");
        controllerObj.transform.SetParent(canvasObj.transform, false);
        GameOverController controller = controllerObj.AddComponent<GameOverController>();

        // 4. Panel Game Over (Merah Transparan)
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0.5f, 0, 0, 0.8f); // Merah Gelap Transparan
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Hubungkan panel ke controller
        controller.gameOverPanel = panelObj;

        // 5. Text Judul "GAME OVER"
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "GAME OVER";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 150;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(1000, 250);

        // Fungsi Helper untuk membuat tombol
        void CreateMenuButton(string name, string text, Vector2 pos, Color color, UnityEngine.Events.UnityAction action)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(panelObj.transform, false);
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

        // 6. Buat tombol Retry & Main Menu
        CreateMenuButton("Btn_Retry", "RETRY", new Vector2(0, -50), new Color(0.2f, 0.6f, 0.2f), new UnityEngine.Events.UnityAction(controller.RetryGame));
        CreateMenuButton("Btn_MainMenu", "MAIN MENU", new Vector2(0, -180), new Color(0.2f, 0.4f, 0.8f), new UnityEngine.Events.UnityAction(controller.LoadMainMenu));

        // Sembunyikan panel secara default
        panelObj.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Game Over UI");
        Selection.activeGameObject = canvasObj;

        Debug.Log("Game Over UI berhasil dibuat di Scene!");
    }
}
