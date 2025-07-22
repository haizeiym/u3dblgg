using UnityEngine;
using UnityEditor;

public class LevelEditorConfigWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showShapeTypes = true;
    private bool showBallTypes = true;

    [MenuItem("Tools/Level Editor/配置编辑器")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorConfigWindow>("关卡编辑器配置");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("关卡编辑器配置", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        showShapeTypes = EditorGUILayout.Foldout(showShapeTypes, "形状类型配置");
        if (showShapeTypes) DrawShapeTypeConfig();

        EditorGUILayout.Space();

        showBallTypes = EditorGUILayout.Foldout(showBallTypes, "球类型配置");
        if (showBallTypes) DrawBallTypeConfig();

        EditorGUILayout.Space();

        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    void DrawShapeTypeConfig()
    {
        var config = LevelEditorConfig.Instance;
        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < config.shapeTypes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"形状 {i + 1}:", GUILayout.Width(60));
            config.shapeTypes[i].name = EditorGUILayout.TextField(config.shapeTypes[i].name, GUILayout.Width(100));
            EditorGUILayout.LabelField("图片:", GUILayout.Width(30));
            config.shapeTypes[i].sprite = (Sprite)EditorGUILayout.ObjectField(config.shapeTypes[i].sprite, typeof(Sprite), false, GUILayout.Width(100));
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                config.shapeTypes.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("添加形状类型"))
        {
            config.AddShapeType("新形状");
        }
        EditorGUILayout.EndVertical();
    }

    void DrawBallTypeConfig()
    {
        var config = LevelEditorConfig.Instance;
        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < config.ballTypes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"球 {i + 1}:", GUILayout.Width(40));
            config.ballTypes[i].name = EditorGUILayout.TextField(config.ballTypes[i].name, GUILayout.Width(100));
            EditorGUILayout.LabelField("颜色:", GUILayout.Width(30));
            config.ballTypes[i].color = EditorGUILayout.ColorField(config.ballTypes[i].color, GUILayout.Width(50));
            EditorGUILayout.LabelField("图片:", GUILayout.Width(30));
            config.ballTypes[i].sprite = (Sprite)EditorGUILayout.ObjectField(config.ballTypes[i].sprite, typeof(Sprite), false, GUILayout.Width(100));
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                config.ballTypes.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("添加球类型"))
        {
            config.AddBallType("新球", Color.white);
        }
        EditorGUILayout.EndVertical();
    }

    void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("保存配置"))
        {
            LevelEditorConfig.Instance.SaveConfigToFile();
            EditorUtility.DisplayDialog("保存成功", "配置已保存到 /config/level_editor_config.json", "确定");
        }

        if (GUILayout.Button("加载配置"))
        {
            LevelEditorConfig.Instance.LoadConfigFromFile();
            EditorUtility.DisplayDialog("加载成功", "配置已从 /config/level_editor_config.json 加载", "确定");
        }

        if (GUILayout.Button("重置为默认配置"))
        {
            if (EditorUtility.DisplayDialog("确认重置", "确定要重置为默认配置吗？", "确定", "取消"))
            {
                LevelEditorConfig.Instance.InitializeDefaultConfig();
            }
        }

        if (GUILayout.Button("刷新编辑器"))
        {
            LevelEditorMenu.SetupLevelEditor();
            EditorUtility.DisplayDialog("刷新提示", "配置已更新并应用。", "确定");
        }

        EditorGUILayout.EndHorizontal();
    }
}