using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Re-applies the lobby layout with anchors that match where each element actually sits,
// so nothing drifts when the screen size differs from the 1440x2560 design.
public static class LobbyUIBuilder
{
    private const string LobbyScenePath = "Assets/Scenes/01_Lobby.unity";

    private struct Rect2D
    {
        public string Name;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector2 OffsetMin;
        public Vector2 OffsetMax;
    }

    // Offsets are margins from the anchored edges, so they hold at any resolution.
    private static readonly Rect2D[] Layout =
    {
        // Bottom of the screen, stretched horizontally.
        Bottom("LogoutBtn", 163f, 50f, 216f),
        Bottom("playersOnlineLbl", 0f, 242f, 332f),
        Bottom("FindOpponentBtn", 200f, 1830f, 2060f),

        // Top of the screen, stretched horizontally.
        Top("TopPlayersContainer", 163f, 163f, -1011f, -2195f),
        Top("usernameLbl", 225f, 717f, -890f, -970f),
        Top("scoreLbl", 723f, 207f, -890f, -970f),
    };

    private static Rect2D Bottom(string name, float sideMargin, float bottom, float top)
    {
        return new Rect2D
        {
            Name = name,
            AnchorMin = new Vector2(0f, 0f),
            AnchorMax = new Vector2(1f, 0f),
            Pivot = new Vector2(0.5f, 0f),
            OffsetMin = new Vector2(sideMargin, bottom),
            OffsetMax = new Vector2(-sideMargin, top)
        };
    }

    private static Rect2D Top(string name, float left, float right, float top, float bottom)
    {
        return new Rect2D
        {
            Name = name,
            AnchorMin = new Vector2(0f, 1f),
            AnchorMax = new Vector2(1f, 1f),
            Pivot = new Vector2(0.5f, 1f),
            OffsetMin = new Vector2(left, bottom),
            OffsetMax = new Vector2(-right, top)
        };
    }

    [MenuItem("TTT/Anchor Lobby UI")]
    public static void Anchor()
    {
        var scene = EditorSceneManager.OpenScene(LobbyScenePath, OpenSceneMode.Single);

        var canvas = GameObject.Find("Canvas");
        var lobby = Object.FindFirstObjectByType<Lobby>();

        if (canvas == null || lobby == null)
        {
            Debug.LogError("[LobbyUIBuilder] Canvas or Lobby not found in the scene.");
            EditorApplication.Exit(1);
            return;
        }

        MoveLogoutButtonToCanvas(canvas.transform);

        foreach (var entry in Layout)
        {
            Apply(canvas.transform, entry);
        }

        CenterTopPlayers(canvas.transform);
        WireLogoutButton(lobby, canvas.transform.Find("LogoutBtn"));

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[LobbyUIBuilder] Lobby UI anchored.");
    }

    // It was living under Header (a top bar) while sitting at the bottom of the screen.
    private static void MoveLogoutButtonToCanvas(Transform canvas)
    {
        var logout = GameObject.Find("LogoutBtn");
        if (logout == null)
        {
            Debug.LogError("[LobbyUIBuilder] LogoutBtn not found in the scene.");
            EditorApplication.Exit(1);
            return;
        }

        if (logout.transform.parent == canvas)
        {
            return;
        }

        logout.transform.SetParent(canvas, false);
        logout.transform.SetAsLastSibling();
    }

    private static void Apply(Transform canvas, Rect2D entry)
    {
        var rect = canvas.Find(entry.Name) as RectTransform;
        if (rect == null)
        {
            Debug.LogWarning($"[LobbyUIBuilder] {entry.Name} not found under Canvas.");
            return;
        }

        rect.anchorMin = entry.AnchorMin;
        rect.anchorMax = entry.AnchorMax;
        rect.pivot = entry.Pivot;
        // Offsets last: they define the rect regardless of the anchors and pivot above.
        rect.offsetMin = entry.OffsetMin;
        rect.offsetMax = entry.OffsetMax;
    }

    // Fixed size label, so it only needs to be centered on the horizontal axis.
    private static void CenterTopPlayers(Transform canvas)
    {
        var rect = canvas.Find("TopPlayers") as RectTransform;
        if (rect == null)
        {
            Debug.LogWarning("[LobbyUIBuilder] TopPlayers not found under Canvas.");
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(660f, 157f);
        rect.anchoredPosition = new Vector2(0f, -779f);
    }

    private static void WireLogoutButton(Lobby lobby, Transform button)
    {
        if (button == null)
        {
            return;
        }

        var serialized = new SerializedObject(lobby);
        serialized.FindProperty("_logoutButton").objectReferenceValue = button;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
