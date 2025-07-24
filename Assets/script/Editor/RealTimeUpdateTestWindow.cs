using UnityEngine;
using UnityEditor;

/// <summary>
/// 实时更新测试窗口
/// 用于测试配置变更时的实时UI更新功能
/// </summary>
public class RealTimeUpdateTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/实时更新测试")]
    public static void ShowWindow()
    {
        GetWindow<RealTimeUpdateTestWindow>("实时更新测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("实时更新测试", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个窗口用于测试配置变更时的实时UI更新功能", MessageType.Info);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("形状类型测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("添加测试形状类型"))
        {
            AddTestShapeType();
        }

        if (GUILayout.Button("删除最后一个形状类型"))
        {
            RemoveLastShapeType();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("球类型测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("添加测试球类型"))
        {
            AddTestBallType();
        }

        if (GUILayout.Button("删除最后一个球类型"))
        {
            RemoveLastBallType();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("背景配置测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("添加测试背景"))
        {
            AddTestBackground();
        }

        if (GUILayout.Button("切换当前背景"))
        {
            SwitchCurrentBackground();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("配置重新加载测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("重新加载配置"))
        {
            ReloadConfig();
        }

        if (GUILayout.Button("保存配置"))
        {
            SaveConfig();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("UI更新器测试:", EditorStyles.boldLabel);
        if (GUILayout.Button("检查UI更新器状态"))
        {
            CheckUIUpdaterStatus();
        }

        if (GUILayout.Button("手动触发形状类型更新"))
        {
            ManualTriggerShapeTypeUpdate();
        }

        if (GUILayout.Button("手动触发球类型更新"))
        {
            ManualTriggerBallTypeUpdate();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 点击按钮添加/删除配置项");
        EditorGUILayout.LabelField("2. 观察UI是否实时更新");
        EditorGUILayout.LabelField("3. 查看控制台输出确认更新过程");
        EditorGUILayout.LabelField("4. 检查形状和球类型按钮是否正确更新");
        EditorGUILayout.LabelField("5. 使用手动触发按钮测试事件系统");
    }

    void AddTestShapeType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string shapeName = $"实时测试形状{config.shapeTypes.Count + 1}";
            config.AddShapeType(shapeName);
            Debug.Log($"已添加测试形状类型: {shapeName}");
        }
    }

    void RemoveLastShapeType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null && config.shapeTypes.Count > 0)
        {
            string removedName = config.shapeTypes[config.shapeTypes.Count - 1].name;
            config.shapeTypes.RemoveAt(config.shapeTypes.Count - 1);
            config.SaveConfigToFile();
            config.TriggerShapeTypesChanged();
            Debug.Log($"已删除形状类型: {removedName}");
        }
    }

    void AddTestBallType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string ballName = $"实时测试球{config.ballTypes.Count + 1}";
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
            Color ballColor = colors[config.ballTypes.Count % colors.Length];
            config.AddBallType(ballName, ballColor);
            Debug.Log($"已添加测试球类型: {ballName}, 颜色: {ballColor}");
        }
    }

    void RemoveLastBallType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null && config.ballTypes.Count > 0)
        {
            string removedName = config.ballTypes[config.ballTypes.Count - 1].name;
            config.ballTypes.RemoveAt(config.ballTypes.Count - 1);
            config.SaveConfigToFile();
            config.TriggerBallTypesChanged();
            Debug.Log($"已删除球类型: {removedName}");
        }
    }

    void AddTestBackground()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string bgName = $"实时测试背景{config.backgroundConfigs.Count + 1}";
            Color[] colors = { Color.white, Color.gray, new Color(0.3f, 0.3f, 0.3f), new Color(0.8f, 0.8f, 0.8f) };
            Color bgColor = colors[config.backgroundConfigs.Count % colors.Length];
            config.AddBackgroundConfig(bgName, null, bgColor);
            Debug.Log($"已添加测试背景: {bgName}, 颜色: {bgColor}");
        }
    }

    void SwitchCurrentBackground()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null && config.backgroundConfigs.Count > 0)
        {
            int newIndex = (config.currentBackgroundIndex + 1) % config.backgroundConfigs.Count;
            config.SetCurrentBackground(newIndex);
            Debug.Log($"已切换到背景索引: {newIndex}");
        }
    }

    void ReloadConfig()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.LoadConfigFromFile();
            Debug.Log("配置已重新加载");
        }
    }

    void SaveConfig()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.SaveConfigToFile();
            Debug.Log("配置已保存");
        }
    }

    void CheckUIUpdaterStatus()
    {
        Debug.Log("=== UI更新器状态检查 ===");
        
        // 查找LevelEditorUI组件
        LevelEditorUI editorUI = Object.FindObjectOfType<LevelEditorUI>();
        if (editorUI != null)
        {
            Debug.Log("✓ 找到LevelEditorUI组件");
            
            // 检查UI更新器字段（通过反射）
            var field = typeof(LevelEditorUI).GetField("uiUpdater", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var uiUpdater = field.GetValue(editorUI);
                if (uiUpdater != null)
                {
                    Debug.Log("✓ UI更新器已初始化");
                    Debug.Log($"  UI更新器类型: {uiUpdater.GetType().Name}");
                }
                else
                {
                    Debug.LogWarning("✗ UI更新器未初始化");
                }
            }
        }
        else
        {
            Debug.LogWarning("✗ 未找到LevelEditorUI组件");
        }
        
        // 检查配置事件
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            Debug.Log($"✓ 配置实例存在");
            Debug.Log($"  形状类型数量: {config.shapeTypes.Count}");
            Debug.Log($"  球类型数量: {config.ballTypes.Count}");
            Debug.Log($"  背景配置数量: {config.backgroundConfigs.Count}");
        }
        else
        {
            Debug.LogWarning("✗ 配置实例不存在");
        }
    }

    void ManualTriggerShapeTypeUpdate()
    {
        Debug.Log("手动触发形状类型更新事件");
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.TriggerShapeTypesChanged();
        }
    }

    void ManualTriggerBallTypeUpdate()
    {
        Debug.Log("手动触发球类型更新事件");
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.TriggerBallTypesChanged();
        }
    }
} 