using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TimeHollowLibrary : EditorWindow
{
    private const string EditorFolder = "Assets/Editor/";
    private const string EnemyFolder = "Assets/Prefabs/Enemys";

    [SerializeField] private VisualTreeAsset m_VisualTreeAsset;
    [SerializeField] private string m_Search = "";
    [SerializeField] private Transform m_ScenePlayer;

    private ScrollView m_EnemyList;
    private Label m_Count;

    [MenuItem("Window/UI Toolkit/TimeHollowLibrary")]
    public static void ShowExample()
    {
        TimeHollowLibrary wnd = GetWindow<TimeHollowLibrary>();
        wnd.titleContent = new GUIContent("TimeHollowLibrary");
        wnd.minSize = new Vector2(360, 400);
    }

    private void OnEnable() => EditorApplication.playModeStateChanged += OnPlayModeChanged;
    private void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    private void OnPlayModeChanged(PlayModeStateChange state) => RefreshEnemies();
    private void OnProjectChange() => RefreshEnemies();

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.Clear();
        minSize = new Vector2(360, 400);

        if (m_VisualTreeAsset == null)
            m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorFolder + "TimeHollowLibrary.uxml");

        if (m_VisualTreeAsset == null)
        {
            root.Add(new HelpBox("TimeHollowLibrary.uxml could not be found in Assets/Editor.", HelpBoxMessageType.Error));
            return;
        }

        m_VisualTreeAsset.CloneTree(root);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(EditorFolder + "TimeHollowLibrary.uss");
        if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            root.styleSheets.Add(styleSheet);

        m_EnemyList = root.Q<ScrollView>("enemies-list");
        m_Count = root.Q<Label>("enemy-count");
        var search = root.Q<ToolbarSearchField>("enemy-search");
        search.SetValueWithoutNotify(m_Search);
        search.RegisterValueChangedCallback(evt =>
        {
            m_Search = evt.newValue;
            RefreshEnemies();
        });
        root.Q<Button>("refresh-enemies").clicked += RefreshEnemies;

        if (m_ScenePlayer == null)
            m_ScenePlayer = GameObject.FindGameObjectsWithTag("Player")
                .FirstOrDefault(player => player.scene == SceneManager.GetActiveScene())?.transform;

        var playerField = root.Q<ObjectField>("scene-player");
        playerField.objectType = typeof(Transform);
        playerField.allowSceneObjects = true;
        playerField.SetValueWithoutNotify(m_ScenePlayer);
        playerField.RegisterValueChangedCallback(evt => m_ScenePlayer = evt.newValue as Transform);
        RefreshEnemies();
    }

    private void RefreshEnemies()
    {
        if (m_EnemyList == null || m_Count == null)
            return;

        m_EnemyList.Clear();
        if (!AssetDatabase.IsValidFolder(EnemyFolder))
        {
            m_Count.text = "No enemy folder";
            m_EnemyList.Add(new HelpBox("Create or restore " + EnemyFolder + " to populate this list.", HelpBoxMessageType.Info));
            return;
        }

        // Discover real assets on refresh; there are no temporary catalog entries to save.
        var prefabs = AssetDatabase.FindAssets("t:Prefab", new[] { EnemyFolder })
            .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(prefab => prefab != null)
            .OrderBy(prefab => prefab.name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var matches = prefabs.Where(prefab => string.IsNullOrWhiteSpace(m_Search)
            || prefab.name.IndexOf(m_Search.Trim(), StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

        m_Count.text = matches.Length == prefabs.Length
            ? $"{prefabs.Length} enemy prefabs"
            : $"{matches.Length} of {prefabs.Length} enemy prefabs";

        foreach (var prefab in matches)
            m_EnemyList.Add(CreateEnemyCard(prefab));

        if (matches.Length == 0)
            m_EnemyList.Add(new Label(prefabs.Length == 0
                ? "No prefabs found in " + EnemyFolder + "."
                : "No enemies match your search.") { name = "empty-state" });
    }

    private VisualElement CreateEnemyCard(GameObject prefab)
    {
        var card = new VisualElement();
        card.AddToClassList("enemy-card");
        var row = new VisualElement();
        row.AddToClassList("enemy-summary");
        card.Add(row);

        var preview = new Image { scaleMode = ScaleMode.ScaleToFit };
        preview.AddToClassList("enemy-preview");
        var renderer = prefab.GetComponentInChildren<SpriteRenderer>(true);
        if (renderer != null && renderer.sprite != null)
            preview.sprite = renderer.sprite;
        else
            preview.image = AssetPreview.GetMiniThumbnail(prefab);
        row.Add(preview);

        var details = new VisualElement();
        details.AddToClassList("enemy-details");
        row.Add(details);
        var title = new Label(ObjectNames.NicifyVariableName(prefab.name));
        title.AddToClassList("enemy-name");
        details.Add(title);
        var stats = new Label(ReadStats(prefab));
        stats.AddToClassList("enemy-stats");
        details.Add(stats);
        var path = new Label(AssetDatabase.GetAssetPath(prefab));
        path.AddToClassList("asset-path");
        details.Add(path);

        var actions = new VisualElement();
        actions.AddToClassList("enemy-actions");
        card.Add(actions);
        actions.Add(new Button(() =>
        {
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }) { text = "配置する", tooltip = "プロジェクトウィンドウでソースプレハブを選択します。" });
        actions.Add(new Button(() => AssetDatabase.OpenAsset(prefab))
            { text = "プレハブを開く", tooltip = "プレハブモードでソースアセットを開きます。" });
        var place = new Button(() => PlaceEnemy(prefab))
            { text = "シーン内の配置", tooltip = "Z=0のシーンビューセンターに配置し、地面に移動します。Un d oがサポートされました。" };
        place.AddToClassList("place-button");
        place.SetEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
        actions.Add(place);
        return card;
    }

    private static string ReadStats(GameObject prefab)
    {
        foreach (var component in prefab.GetComponents<MonoBehaviour>())
        {
            if (component == null)
                continue;
            using (var serialized = new SerializedObject(component))
            {
                var hp = serialized.FindProperty("maxHP");
                var attack = serialized.FindProperty("attackPower");
                if (hp != null && attack != null
                    && hp.propertyType == SerializedPropertyType.Integer
                    && attack.propertyType == SerializedPropertyType.Integer)
                    return $"HP {hp.intValue}   /   Attack {attack.intValue}";
            }
        }
        return "Enemy prefab";
    }

    private void PlaceEnemy(GameObject prefab)
    {
        var scene = SceneManager.GetActiveScene();
        if (prefab == null || EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        if (!scene.IsValid() || !scene.isLoaded || EditorSceneManager.IsPreviewScene(scene)
            || PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            ShowNotification(new GUIContent("Open a regular scene before placing an enemy."));
            return;
        }

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Place " + prefab.name);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        Undo.RegisterCreatedObjectUndo(instance, "Place " + prefab.name);
        var view = SceneView.lastActiveSceneView;
        var position = view != null ? view.pivot : Vector3.zero;
        instance.transform.position = new Vector3(position.x, position.y, 0);
        PrefabUtility.RecordPrefabInstancePropertyModifications(instance.transform);

        // Scene references belong to this placed instance, never to the source prefab.
        if (m_ScenePlayer != null && m_ScenePlayer.gameObject.scene == scene)
        {
            foreach (var component in instance.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component == null)
                    continue;
                using (var serialized = new SerializedObject(component))
                {
                    var player = serialized.FindProperty("player");
                    if (player == null || player.propertyType != SerializedPropertyType.ObjectReference
                        || !player.type.EndsWith("Transform>", StringComparison.Ordinal))
                        continue;
                    player.objectReferenceValue = m_ScenePlayer;
                    serialized.ApplyModifiedProperties();
                }
            }
        }

        Selection.activeGameObject = instance;
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);
        SceneView.RepaintAll();
        ShowNotification(new GUIContent("Placed " + prefab.name + ". Move it onto the ground; Ctrl+Z to undo."));
    }
}
