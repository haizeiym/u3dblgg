using UnityEngine;
using UnityEditor;

public class LevelEditorConfigWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showShapeTypes = true;
    private bool showBallTypes = true;
    private bool showBackgroundConfigs = true;

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

        showBackgroundConfigs = EditorGUILayout.Foldout(showBackgroundConfigs, "背景配置");
        if (showBackgroundConfigs) DrawBackgroundConfig();

        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    void DrawShapeTypeConfig()
    {
        var config = LevelEditorConfig.Instance;
        EditorGUILayout.BeginVertical("box");
        
        // 使用反向循环避免删除时的索引问题
        for (int i = config.shapeTypes.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"形状 {i + 1}:", GUILayout.Width(60));
            config.shapeTypes[i].name = EditorGUILayout.TextField(config.shapeTypes[i].name, GUILayout.Width(100));
            EditorGUILayout.LabelField("图片:", GUILayout.Width(30));
            config.shapeTypes[i].sprite = (Sprite)EditorGUILayout.ObjectField(config.shapeTypes[i].sprite, typeof(Sprite), false, GUILayout.Width(100));
            
            bool shouldDelete = false;
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                shouldDelete = true;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 在GUI布局结束后执行删除操作
            if (shouldDelete)
            {
                config.shapeTypes.RemoveAt(i);
                // 触发配置变更事件
                config.TriggerShapeTypesChanged();
                // 强制重绘窗口
                Repaint();
                return; // 安全退出，避免继续处理已删除的项目
            }
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
        
        // 使用反向循环避免删除时的索引问题
        for (int i = config.ballTypes.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"球 {i + 1}:", GUILayout.Width(40));
            config.ballTypes[i].name = EditorGUILayout.TextField(config.ballTypes[i].name, GUILayout.Width(100));
            EditorGUILayout.LabelField("颜色:", GUILayout.Width(30));
            config.ballTypes[i].color = EditorGUILayout.ColorField(config.ballTypes[i].color, GUILayout.Width(50));
            EditorGUILayout.LabelField("图片:", GUILayout.Width(30));
            config.ballTypes[i].sprite = (Sprite)EditorGUILayout.ObjectField(config.ballTypes[i].sprite, typeof(Sprite), false, GUILayout.Width(100));
            
            bool shouldDelete = false;
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                shouldDelete = true;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 在GUI布局结束后执行删除操作
            if (shouldDelete)
            {
                config.ballTypes.RemoveAt(i);
                // 触发配置变更事件
                config.TriggerBallTypesChanged();
                // 强制重绘窗口
                Repaint();
                return; // 安全退出，避免继续处理已删除的项目
            }
        }
        
        if (GUILayout.Button("添加球类型"))
        {
            config.AddBallType("新球", Color.white);
        }
        EditorGUILayout.EndVertical();
    }

    void DrawBackgroundConfig()
    {
        var config = LevelEditorConfig.Instance;
        EditorGUILayout.BeginVertical("box");
        
        // 当前背景选择
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前背景:", GUILayout.Width(60));
        string[] backgroundNames = config.GetBackgroundConfigNames();
        int newIndex = EditorGUILayout.Popup(config.currentBackgroundIndex, backgroundNames);
        if (newIndex != config.currentBackgroundIndex)
        {
            config.SetCurrentBackground(newIndex);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 背景配置列表
        for (int i = config.backgroundConfigs.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"背景 {i + 1}: {config.backgroundConfigs[i].name}", EditorStyles.boldLabel);
            
            config.backgroundConfigs[i].name = EditorGUILayout.TextField("名称:", config.backgroundConfigs[i].name);
            config.backgroundConfigs[i].useSprite = EditorGUILayout.Toggle("使用图片:", config.backgroundConfigs[i].useSprite);
            
            if (config.backgroundConfigs[i].useSprite)
            {
                config.backgroundConfigs[i].backgroundSprite = (Sprite)EditorGUILayout.ObjectField("背景图片:", config.backgroundConfigs[i].backgroundSprite, typeof(Sprite), false);
                config.backgroundConfigs[i].spriteScale = EditorGUILayout.Vector2Field("图片缩放:", config.backgroundConfigs[i].spriteScale);
                config.backgroundConfigs[i].spriteOffset = EditorGUILayout.Vector2Field("图片偏移:", config.backgroundConfigs[i].spriteOffset);
            }
            else
            {
                config.backgroundConfigs[i].backgroundColor = EditorGUILayout.ColorField("背景颜色:", config.backgroundConfigs[i].backgroundColor);
            }
            
            bool shouldDelete = false;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("设为当前", GUILayout.Width(80)))
            {
                config.SetCurrentBackground(i);
            }
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                shouldDelete = true;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
            // 在GUI布局结束后执行删除操作
            if (shouldDelete)
            {
                config.backgroundConfigs.RemoveAt(i);
                // 触发配置变更事件
                config.TriggerBackgroundConfigsChanged();
                // 强制重绘窗口
                Repaint();
                return; // 安全退出，避免继续处理已删除的项目
            }
        }
        
        if (GUILayout.Button("添加背景配置"))
        {
            config.AddBackgroundConfig("新背景");
        }
        
        EditorGUILayout.EndVertical();
    }

    void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("保存配置"))
        {
            // 确保所有背景配置的精灵路径都已更新
            var config = LevelEditorConfig.Instance;
            foreach (var bg in config.backgroundConfigs)
            {
                if (bg.backgroundSprite != null)
                {
                    bg.SetSpritePath(bg.GetSpritePath());
                }
            }
            
            LevelEditorConfig.Instance.SaveConfigToFile();
            EditorUtility.DisplayDialog("保存成功", "配置已保存到 Assets/config/level_editor_config.json", "确定");
        }

        if (GUILayout.Button("加载配置"))
        {
            LevelEditorConfig.Instance.LoadConfigFromFile();
            EditorUtility.DisplayDialog("加载成功", "配置已从 Assets/config/level_editor_config.json 加载", "确定");
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