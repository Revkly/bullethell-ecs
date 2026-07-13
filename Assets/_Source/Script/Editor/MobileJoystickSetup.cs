using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileJoystickSetup
{
    [MenuItem("Tools/Create Mobile Joystick UI")]
    public static void CreateJoystick()
    {
        // 1. Pastikan ada EventSystem
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 2. Buat Canvas jika belum ada atau buat baru khusus untuk Joystick
        GameObject canvasObj = new GameObject("Mobile Joystick Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // 3. Buat Container (Background Joystick)
        GameObject bgObj = new GameObject("Joystick Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.5f); // Hitam transparan
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(0, 0);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = new Vector2(250, 250);
        bgRect.sizeDelta = new Vector2(250, 250);

        // Buat agar background menjadi lingkaran (mask atau border), kita pakai sprite default Unity: UISprite
        bgImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        // 4. Buat Knob (Pegangan Joystick)
        GameObject knobObj = new GameObject("Joystick Knob");
        knobObj.transform.SetParent(bgObj.transform, false);
        Image knobImg = knobObj.AddComponent<Image>();
        knobImg.color = new Color(1, 1, 1, 0.8f); // Putih
        
        RectTransform knobRect = knobObj.GetComponent<RectTransform>();
        knobRect.anchorMin = new Vector2(0.5f, 0.5f);
        knobRect.anchorMax = new Vector2(0.5f, 0.5f);
        knobRect.pivot = new Vector2(0.5f, 0.5f);
        knobRect.anchoredPosition = Vector2.zero;
        knobRect.sizeDelta = new Vector2(100, 100);
        
        knobImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        // 5. Tambahkan komponen MobileJoystick
        MobileJoystick joystick = bgObj.AddComponent<MobileJoystick>();
        joystick.joystickBackground = bgRect;
        joystick.joystickKnob = knobRect;
        joystick.movementRange = 75f;

        // Tampilkan di hierarchy dan select
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Mobile Joystick");
        Selection.activeGameObject = bgObj;

        Debug.Log("Mobile Joystick UI berhasil dibuat di scene!");
    }
}
