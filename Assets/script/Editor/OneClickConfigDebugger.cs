using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 一键配置调试工具
/// 用于调试一键配置的执行流程和事件绑定问题
/// </summary>
public class OneClickConfigDebugger : EditorWindow
{
    [MenuItem("Tools/Level Editor/一键配置调试")]
    public static void ShowWindow()
    {
        GetWindow<OneClickConfigDebugger>("一键配置调试");
    }

    private Vector2 scrollPosition;
    private string debugLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("一键配置调试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于调试一键配置的执行流程和事件绑定问题", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("执行一键配置并调试"))
        {
            ExecuteAndDebug();
        }

        if (GUILayout.Button("检查配置加载状态"))
        {
            CheckConfigStatus();
        }

        if (GUILayout.Button("检查UI构建状态"))
        {
            CheckUIBuildStatus();
        }

        if (GUILayout.Button("检查事件绑定状态"))
        {
            CheckEventBindingStatus();
        }

        if (GUILayout.Button("清理调试日志"))
        {
            debugLog = "";
        }

        EditorGUILayout.Space();

        // 显示调试日志
        if (!string.IsNullOrEmpty(debugLog))
        {
            EditorGUILayout.LabelField("调试日志:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(debugLog, GUILayout.Height(400));
            EditorGUILayout.EndScrollView();
        }
    }

    void ExecuteAndDebug()
    {
        debugLog = "=== 开始执行一键配置并调试 ===\n";
        
        try
        {
            // 记录执行前的状态
            debugLog += "执行前状态检查:\n";
            CheckConfigStatus();
            CheckUIBuildStatus();
            
            // 执行一键配置
            debugLog += "\n开始执行一键配置...\n";
            LevelEditorMenu.SetupLevelEditor();
            
            // 延迟检查执行后的状态
            EditorApplication.delayCall += () => {
                debugLog += "\n执行后状态检查:\n";
                CheckConfigStatus();
                CheckUIBuildStatus();
                CheckEventBindingStatus();
                
                debugLog += "\n=== 一键配置调试完成 ===\n";
                Repaint();
            };
        }
        catch (System.Exception e)
        {
            debugLog += $"执行失败: {e.Message}\n";
            debugLog += $"错误详情: {e.StackTrace}\n";
        }
        
        Repaint();
    }

    void CheckConfigStatus()
    {
        debugLog += "\n--- 配置状态检查 ---\n";
        
        try
        {
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                debugLog += $"✓ LevelEditorConfig.Instance 存在\n";
                debugLog += $"形状类型数量: {config.shapeTypes?.Count ?? 0}\n";
                debugLog += $"球类型数量: {config.ballTypes?.Count ?? 0}\n";
                debugLog += $"背景配置数量: {config.backgroundConfigs?.Count ?? 0}\n";
                
                // 检查配置是否为空
                if ((config.shapeTypes?.Count ?? 0) == 0 && (config.ballTypes?.Count ?? 0) == 0)
                {
                    debugLog += "⚠️ 配置为空，需要初始化默认配置\n";
                }
                else
                {
                    debugLog += "✅ 配置已加载\n";
                }
            }
            else
            {
                debugLog += "✗ LevelEditorConfig.Instance 为空\n";
            }
        }
        catch (System.Exception e)
        {
            debugLog += $"✗ 配置检查失败: {e.Message}\n";
        }
    }

    void CheckUIBuildStatus()
    {
        debugLog += "\n--- UI构建状态检查 ---\n";
        
        // 检查Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            debugLog += $"✓ Canvas存在: {canvas.name}\n";
        }
        else
        {
            debugLog += "✗ Canvas不存在\n";
        }
        
        // 检查EventSystem
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            debugLog += $"✓ EventSystem存在: {eventSystem.name}\n";
        }
        else
        {
            debugLog += "✗ EventSystem不存在\n";
        }
        
        // 检查LevelEditorUI
        LevelEditorUI levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null)
        {
            debugLog += $"✓ LevelEditorUI存在: {levelEditor.name}\n";
            
            // 检查UI组件
            CheckUIComponent("leftPanel", levelEditor.leftPanel);
            CheckUIComponent("centerPanel", levelEditor.centerPanel);
            CheckUIComponent("rightPanel", levelEditor.rightPanel);
            CheckUIComponent("addLayerButton", levelEditor.addLayerButton);
            CheckUIComponent("deleteLayerButton", levelEditor.deleteLayerButton);
            CheckUIComponent("addShapeButton", levelEditor.addShapeButton);
            CheckUIComponent("addBallButton", levelEditor.addBallButton);
            CheckUIComponent("backgroundButton", levelEditor.backgroundButton);
            CheckUIComponent("previewButton", levelEditor.previewButton);
            CheckUIComponent("exportButton", levelEditor.exportButton);
            
            // 检查按钮数组
            if (levelEditor.shapeTypeButtons != null)
            {
                debugLog += $"形状类型按钮数组: {levelEditor.shapeTypeButtons.Length} 个\n";
                for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
                {
                    CheckUIComponent($"shapeTypeButtons[{i}]", levelEditor.shapeTypeButtons[i]);
                }
            }
            else
            {
                debugLog += "✗ shapeTypeButtons数组为空\n";
            }
            
            if (levelEditor.ballTypeButtons != null)
            {
                debugLog += $"球类型按钮数组: {levelEditor.ballTypeButtons.Length} 个\n";
                for (int i = 0; i < levelEditor.ballTypeButtons.Length; i++)
                {
                    CheckUIComponent($"ballTypeButtons[{i}]", levelEditor.ballTypeButtons[i]);
                }
            }
            else
            {
                debugLog += "✗ ballTypeButtons数组为空\n";
            }
        }
        else
        {
            debugLog += "✗ LevelEditorUI不存在\n";
        }
    }

    void CheckUIComponent(string componentName, Object component)
    {
        if (component != null)
        {
            debugLog += $"  ✓ {componentName}: 存在\n";
        }
        else
        {
            debugLog += $"  ✗ {componentName}: 为空\n";
        }
    }

    void CheckEventBindingStatus()
    {
        debugLog += "\n--- 事件绑定状态检查 ---\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ LevelEditorUI不存在，无法检查事件绑定\n";
            return;
        }
        
        // 检查主要按钮的事件绑定
        CheckButtonEvents("添加层级按钮", levelEditor.addLayerButton);
        CheckButtonEvents("删除层级按钮", levelEditor.deleteLayerButton);
        CheckButtonEvents("添加形状按钮", levelEditor.addShapeButton);
        CheckButtonEvents("添加球按钮", levelEditor.addBallButton);
        CheckButtonEvents("背景按钮", levelEditor.backgroundButton);
        CheckButtonEvents("预览按钮", levelEditor.previewButton);
        CheckButtonEvents("导出按钮", levelEditor.exportButton);
        
        // 检查形状类型按钮
        if (levelEditor.shapeTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
            {
                CheckButtonEvents($"形状类型按钮[{i}]", levelEditor.shapeTypeButtons[i]);
            }
        }
        
        // 检查球类型按钮
        if (levelEditor.ballTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.ballTypeButtons.Length; i++)
            {
                CheckButtonEvents($"球类型按钮[{i}]", levelEditor.ballTypeButtons[i]);
            }
        }
    }

    void CheckButtonEvents(string buttonName, Button button)
    {
        if (button == null)
        {
            debugLog += $"✗ {buttonName}: 按钮为空\n";
            return;
        }
        
        int eventCount = button.onClick.GetPersistentEventCount();
        debugLog += $"{buttonName}: {eventCount} 个事件\n";
        
        if (eventCount == 0)
        {
            debugLog += $"  ⚠️ {buttonName} 没有绑定事件\n";
        }
        else if (eventCount > 1)
        {
            debugLog += $"  ⚠️ {buttonName} 有 {eventCount} 个事件，可能存在重复绑定\n";
        }
        else
        {
            var methodName = button.onClick.GetPersistentMethodName(0);
            debugLog += $"  ✅ {buttonName} 绑定方法: {methodName}\n";
        }
    }
} 