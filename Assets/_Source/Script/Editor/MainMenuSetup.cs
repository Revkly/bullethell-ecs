using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

public class MainMenuSetup
{
    [MenuItem("Tools/Create Main Menu Scene")]
    public static void CreateScene()
    {
        // 1. Buat scene baru
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // 2. Setup Camera
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f); // Warna background gelap

        // 3. Setup EventSystem
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // 4. Setup Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // 5. Setup Judul (Title)
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(canvasObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "BULLET HELL ECS";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 120;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 250);
        titleRect.sizeDelta = new Vector2(1200, 200);

        // 6. Buat Controller
        GameObject controllerObj = new GameObject("MainMenuController");
        MainMenuController controller = controllerObj.AddComponent<MainMenuController>();

        // 7. Setup Tombol PLAY (Manual tanpa DefaultControls)
        GameObject playBtnObj = new GameObject("PlayButton");
        playBtnObj.transform.SetParent(canvasObj.transform, false);
        Image playImg = playBtnObj.AddComponent<Image>();
        playImg.color = new Color(0.2f, 0.6f, 0.2f); // Warna hijau
        
        RectTransform playRect = playBtnObj.GetComponent<RectTransform>();
        playRect.anchoredPosition = new Vector2(0, -50);
        playRect.sizeDelta = new Vector2(400, 100);
        
        Button playBtn = playBtnObj.AddComponent<Button>();
        UnityEngine.Events.UnityAction playAction = new UnityEngine.Events.UnityAction(controller.PlayGame);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, playAction);

        // Text Play
        GameObject playTextObj = new GameObject("Text");
        playTextObj.transform.SetParent(playBtnObj.transform, false);
        Text playText = playTextObj.AddComponent<Text>();
        playText.text = "PLAY";
        playText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        playText.fontSize = 50;
        playText.alignment = TextAnchor.MiddleCenter;
        playText.color = Color.white;
        RectTransform playTextRect = playTextObj.GetComponent<RectTransform>();
        playTextRect.anchorMin = Vector2.zero;
        playTextRect.anchorMax = Vector2.one;
        playTextRect.sizeDelta = Vector2.zero;

        // 8. Setup Tombol EXIT (Manual)
        GameObject exitBtnObj = new GameObject("ExitButton");
        exitBtnObj.transform.SetParent(canvasObj.transform, false);
        Image exitImg = exitBtnObj.AddComponent<Image>();
        exitImg.color = new Color(0.8f, 0.2f, 0.2f); // Warna merah
        
        RectTransform exitRect = exitBtnObj.GetComponent<RectTransform>();
        exitRect.anchoredPosition = new Vector2(0, -200);
        exitRect.sizeDelta = new Vector2(400, 100);
        
        Button exitBtn = exitBtnObj.AddComponent<Button>();
        UnityEngine.Events.UnityAction quitAction = new UnityEngine.Events.UnityAction(controller.QuitGame);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(exitBtn.onClick, quitAction);

        // Text Exit
        GameObject exitTextObj = new GameObject("Text");
        exitTextObj.transform.SetParent(exitBtnObj.transform, false);
        Text exitText = exitTextObj.AddComponent<Text>();
        exitText.text = "EXIT";
        exitText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        exitText.fontSize = 50;
        exitText.alignment = TextAnchor.MiddleCenter;
        exitText.color = Color.white;
        RectTransform exitTextRect = exitTextObj.GetComponent<RectTransform>();
        exitTextRect.anchorMin = Vector2.zero;
        exitTextRect.anchorMax = Vector2.one;
        exitTextRect.sizeDelta = Vector2.zero;

        // 9. Simpan Scene
        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);

        // 10. Tambahkan ke Build Settings (MainMenu di urutan pertama)
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        
        bool foundSample = false;
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (s.path != scenePath) scenes.Add(s);
            if (s.path == "Assets/Scenes/SampleScene.unity") foundSample = true;
        }
        
        // Pastikan SampleScene juga ada di Build Settings
        if (!foundSample) 
            scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true));

        EditorBuildSettings.scenes = scenes.ToArray();

        Debug.Log("Main Menu Scene berhasil dibuat dan ditambahkan ke Build Settings!");
    }
}
