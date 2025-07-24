using UnityEngine;
using UnityEditor;

/// <summary>
/// SceneView错误测试工具
/// 用于验证SceneView相关的错误是否已修复
/// </summary>
public class SceneViewErrorTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/SceneView错误测试")]
    public static void ShowWindow()
    {
        GetWindow<SceneViewErrorTestWindow>("SceneView错误测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("SceneView错误测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试SceneView相关的错误是否已修复", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("测试预览窗口"))
        {
            TestPreviewWindow();
        }

        if (GUILayout.Button("测试配置编辑器"))
        {
            TestConfigEditor();
        }

        if (GUILayout.Button("测试所有编辑器窗口"))
        {
            TestAllEditorWindows();
        }

        if (GUILayout.Button("清理测试日志"))
        {
            testLog = "";
        }

        EditorGUILayout.Space();

        // 显示测试日志
        if (!string.IsNullOrEmpty(testLog))
        {
            EditorGUILayout.LabelField("测试日志:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(testLog, GUILayout.Height(300));
            EditorGUILayout.EndScrollView();
        }
    }

    void TestPreviewWindow()
    {
        testLog = "=== 测试预览窗口 ===\n";
        
        try
        {
            // 打开预览窗口
            var previewWindow = EditorWindow.GetWindow<LevelEditorPreviewWindow>();
            if (previewWindow != null)
            {
                testLog += "✓ 预览窗口打开成功\n";
                
                // 强制重绘
                previewWindow.Repaint();
                testLog += "✓ 预览窗口重绘成功\n";
                
                // 关闭窗口
                previewWindow.Close();
                testLog += "✓ 预览窗口关闭成功\n";
            }
            else
            {
                testLog += "✗ 预览窗口打开失败\n";
            }
        }
        catch (System.Exception e)
        {
            testLog += $"✗ 预览窗口测试失败: {e.Message}\n";
            testLog += $"错误详情: {e.StackTrace}\n";
        }
        
        testLog += "=== 预览窗口测试完成 ===\n";
        Repaint();
    }

    void TestConfigEditor()
    {
        testLog += "\n=== 测试配置编辑器 ===\n";
        
        try
        {
            // 打开配置编辑器
            var configWindow = EditorWindow.GetWindow<LevelEditorConfigWindow>();
            if (configWindow != null)
            {
                testLog += "✓ 配置编辑器打开成功\n";
                
                // 强制重绘
                configWindow.Repaint();
                testLog += "✓ 配置编辑器重绘成功\n";
                
                // 关闭窗口
                configWindow.Close();
                testLog += "✓ 配置编辑器关闭成功\n";
            }
            else
            {
                testLog += "✗ 配置编辑器打开失败\n";
            }
        }
        catch (System.Exception e)
        {
            testLog += $"✗ 配置编辑器测试失败: {e.Message}\n";
            testLog += $"错误详情: {e.StackTrace}\n";
        }
        
        testLog += "=== 配置编辑器测试完成 ===\n";
        Repaint();
    }

    void TestAllEditorWindows()
    {
        testLog += "\n=== 测试所有编辑器窗口 ===\n";
        
        // 测试所有相关的编辑器窗口
        var windowTypes = new System.Type[]
        {
            typeof(LevelEditorPreviewWindow),
            typeof(LevelEditorConfigWindow),
            typeof(LevelManagerWindow),
            typeof(SpriteReferenceRepairWindow),
            typeof(IncrementalIdTestWindow),
            typeof(RealTimeUpdateTestWindow),
            typeof(ButtonClickTestWindow),
            typeof(GUILayoutTestWindow)
        };
        
        foreach (var windowType in windowTypes)
        {
            try
            {
                var window = EditorWindow.GetWindow(windowType);
                if (window != null)
                {
                    testLog += $"✓ {windowType.Name} 打开成功\n";
                    
                    // 强制重绘
                    window.Repaint();
                    testLog += $"✓ {windowType.Name} 重绘成功\n";
                    
                    // 关闭窗口
                    window.Close();
                    testLog += $"✓ {windowType.Name} 关闭成功\n";
                }
                else
                {
                    testLog += $"✗ {windowType.Name} 打开失败\n";
                }
            }
            catch (System.Exception e)
            {
                testLog += $"✗ {windowType.Name} 测试失败: {e.Message}\n";
            }
        }
        
        testLog += "=== 所有编辑器窗口测试完成 ===\n";
        Repaint();
    }
} 