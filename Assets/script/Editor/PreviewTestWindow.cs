using UnityEngine;
using UnityEditor;

/// <summary>
/// 预览功能测试窗口
/// 用于测试配置预览的刷新和滚动功能
/// </summary>
public class PreviewTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/测试预览功能")]
    public static void ShowWindow()
    {
        GetWindow<PreviewTestWindow>("预览功能测试");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("预览功能测试", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个窗口用于测试配置预览的刷新和滚动功能", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("添加测试形状类型"))
        {
            AddTestShapeType();
        }

        if (GUILayout.Button("添加测试球类型"))
        {
            AddTestBallType();
        }

        if (GUILayout.Button("添加测试背景"))
        {
            AddTestBackground();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("保存配置"))
        {
            SaveConfig();
        }

        if (GUILayout.Button("打开预览窗口"))
        {
            OpenPreviewWindow();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 点击添加测试项目按钮");
        EditorGUILayout.LabelField("2. 保存配置");
        EditorGUILayout.LabelField("3. 打开预览窗口");
        EditorGUILayout.LabelField("4. 点击刷新按钮查看新添加的内容");
        EditorGUILayout.LabelField("5. 滚动查看所有内容");
    }

    void AddTestShapeType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string shapeName = $"测试形状{config.shapeTypes.Count + 1}";
            config.AddShapeType(shapeName);
            Debug.Log($"已添加测试形状类型: {shapeName}");
        }
    }

    void AddTestBallType()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string ballName = $"测试球{config.ballTypes.Count + 1}";
            Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };
            Color ballColor = colors[config.ballTypes.Count % colors.Length];
            config.AddBallType(ballName, ballColor);
            Debug.Log($"已添加测试球类型: {ballName}, 颜色: {ballColor}");
        }
    }

    void AddTestBackground()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            string bgName = $"测试背景{config.backgroundConfigs.Count + 1}";
            Color[] colors = { Color.gray, new Color(0.3f, 0.3f, 0.3f), new Color(0.8f, 0.8f, 0.8f), Color.white, Color.black };
            Color bgColor = colors[config.backgroundConfigs.Count % colors.Length];
            config.AddBackgroundConfig(bgName, null, bgColor);
            Debug.Log($"已添加测试背景: {bgName}, 颜色: {bgColor}");
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

    void OpenPreviewWindow()
    {
        var configWindowType = System.Type.GetType("LevelEditorPreviewWindow, Assembly-CSharp-Editor");
        if (configWindowType != null)
        {
            EditorWindow.GetWindow(configWindowType, false, "配置预览");
        }
        else
        {
            Debug.LogError("无法找到预览窗口类型");
        }
    }
} 