using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Creates Assets/Scenes/02_Game.unity as a placeholder and adds it to Build Settings,
// so SceneManager.LoadScene("02_Game") works. Safe to run again: it rebuilds the scene.
public static class GameSceneBuilder
{
    private const string GameScenePath = "Assets/Scenes/02_Game.unity";

    [MenuItem("TTT/Create Game Scene (placeholder)")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        var camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.12f, 0.12f, 0.16f);
        cameraGo.transform.position = new Vector3(0f, 0f, -10f);

        var canvasGo = new GameObject("Canvas", typeof(RectTransform));
        canvasGo.layer = LayerMask.NameToLayer("UI");
        canvasGo.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();

        var labelGo = new GameObject("PlaceholderLabel", typeof(RectTransform));
        labelGo.layer = canvasGo.layer;
        var rect = (RectTransform)labelGo.transform;
        rect.SetParent(canvasGo.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.text = "02_Game (placeholder)";
        label.fontSize = 72;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;

        EditorSceneManager.SaveScene(scene, GameScenePath);
        AddToBuildSettings(GameScenePath);

        Debug.Log("[GameSceneBuilder] 02_Game created and added to Build Settings.");
    }

    private static void AddToBuildSettings(string path)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (scenes.Exists(s => s.path == path))
        {
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
