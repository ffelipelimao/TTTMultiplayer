using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Rebuilds the lobby LoadingContainer as a real UI element. It was created as an empty
// GameObject (plain Transform), so its RectTransform children anchored against a 0x0 rect.
public static class LoadingUIBuilder
{
    private const string LobbyScenePath = "Assets/Scenes/01_Lobby.unity";
    private const string ContainerName = "LoadingContainer";
    private static readonly Vector2 ContainerSize = new Vector2(500f, 200f);
    private static readonly Vector2 ContainerPosition = new Vector2(0f, -276f);

    [MenuItem("TTT/Build Lobby Loading UI")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(LobbyScenePath, OpenSceneMode.Single);

        var old = GameObject.Find(ContainerName);

        if (old == null)
        {
            Debug.LogError("[LoadingUIBuilder] LoadingContainer not found in the scene.");
            EditorApplication.Exit(1);
            return;
        }

        var container = EnsureRectTransform(old);
        int uiLayer = LayerMask.NameToLayer("UI");
        SetLayerRecursively(container.gameObject, uiLayer);

        container.anchorMin = new Vector2(0.5f, 0.5f);
        container.anchorMax = new Vector2(0.5f, 0.5f);
        container.pivot = new Vector2(0.5f, 0.5f);
        container.sizeDelta = ContainerSize;
        container.anchoredPosition = ContainerPosition;

        Stretch(container.Find("Background") as RectTransform);
        Stretch(container.Find("Loading") as RectTransform);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[LoadingUIBuilder] Lobby loading UI rebuilt.");
    }

    // Like converting a Java object to another class: Transform can't become RectTransform
    // in place, so we create a new UI object, move the children over and drop the old one.
    private static RectTransform EnsureRectTransform(GameObject old)
    {
        if (old.transform is RectTransform existing)
        {
            return existing;
        }

        var parent = old.transform.parent;
        int siblingIndex = old.transform.GetSiblingIndex();

        var go = new GameObject(old.name, typeof(RectTransform));
        go.SetActive(old.activeSelf);
        var rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.SetSiblingIndex(siblingIndex);

        while (old.transform.childCount > 0)
        {
            old.transform.GetChild(0).SetParent(rect, false);
        }

        Object.DestroyImmediate(old);
        return rect;
    }

    private static void Stretch(RectTransform rect)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
