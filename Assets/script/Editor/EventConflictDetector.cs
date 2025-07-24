using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 事件冲突检测工具
/// 用于检测LevelEditorUI中是否存在重复的事件绑定
/// </summary>
public class EventConflictDetector : EditorWindow
{
    [MenuItem("Tools/Level Editor/事件冲突检测")]
    public static void ShowWindow()
    {
        GetWindow<EventConflictDetector>("事件冲突检测");
    }

    private Vector2 scrollPosition;
    private string detectionLog = "";
    private Dictionary<string, List<string>> buttonEventMap = new Dictionary<string, List<string>>();

    void OnGUI()
    {
        EditorGUILayout.LabelField("事件冲突检测工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于检测LevelEditorUI中是否存在重复的事件绑定", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("检测所有按钮事件"))
        {
            DetectAllButtonEvents();
        }

        if (GUILayout.Button("检测特定按钮"))
        {
            DetectSpecificButtons();
        }

        if (GUILayout.Button("检查事件绑定来源"))
        {
            CheckEventBindingSources();
        }

        if (GUILayout.Button("清理检测日志"))
        {
            detectionLog = "";
            buttonEventMap.Clear();
        }

        EditorGUILayout.Space();

        // 显示检测日志
        if (!string.IsNullOrEmpty(detectionLog))
        {
            EditorGUILayout.LabelField("检测日志:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(detectionLog, GUILayout.Height(400));
            EditorGUILayout.EndScrollView();
        }
    }

    void DetectAllButtonEvents()
    {
        detectionLog = "=== 事件冲突检测开始 ===\n";
        buttonEventMap.Clear();

        // 查找所有Button组件
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        detectionLog += $"找到 {allButtons.Length} 个Button组件\n\n";

        foreach (Button button in allButtons)
        {
            string buttonName = button.name;
            int eventCount = button.onClick.GetPersistentEventCount();
            
            detectionLog += $"按钮: {buttonName}\n";
            detectionLog += $"事件数量: {eventCount}\n";
            
            List<string> events = new List<string>();
            
            for (int i = 0; i < eventCount; i++)
            {
                var target = button.onClick.GetPersistentTarget(i);
                var methodName = button.onClick.GetPersistentMethodName(i);
                string eventInfo = $"{target?.name} -> {methodName}";
                events.Add(eventInfo);
                detectionLog += $"  事件 {i}: {eventInfo}\n";
            }
            
            buttonEventMap[buttonName] = events;
            
            // 检查是否有重复事件
            if (eventCount > 1)
            {
                detectionLog += $"  ⚠️ 警告: {buttonName} 有 {eventCount} 个事件绑定，可能存在重复\n";
                
                // 检查是否有相同的方法绑定
                HashSet<string> uniqueMethods = new HashSet<string>();
                foreach (string eventInfo in events)
                {
                    if (!uniqueMethods.Add(eventInfo))
                    {
                        detectionLog += $"  ❌ 错误: {buttonName} 存在重复的事件绑定: {eventInfo}\n";
                    }
                }
            }
            else if (eventCount == 0)
            {
                detectionLog += $"  ⚠️ 警告: {buttonName} 没有绑定任何事件\n";
            }
            else
            {
                detectionLog += $"  ✅ 正常: {buttonName} 有 1 个事件绑定\n";
            }
            
            detectionLog += "\n";
        }

        detectionLog += "=== 事件冲突检测完成 ===\n";
        Repaint();
    }

    void DetectSpecificButtons()
    {
        detectionLog += "\n=== 检测特定按钮 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            detectionLog += "✗ 未找到LevelEditorUI\n";
            Repaint();
            return;
        }

        // 检测左侧面板按钮
        CheckButtonEvents("添加层级按钮", levelEditor.addLayerButton);
        CheckButtonEvents("删除层级按钮", levelEditor.deleteLayerButton);

        // 检测工具栏按钮
        CheckButtonEvents("添加形状按钮", levelEditor.addShapeButton);
        CheckButtonEvents("添加球按钮", levelEditor.addBallButton);
        CheckButtonEvents("背景按钮", levelEditor.backgroundButton);
        CheckButtonEvents("预览按钮", levelEditor.previewButton);

        // 检测右侧面板按钮
        CheckButtonEvents("导出按钮", levelEditor.exportButton);

        // 检测形状类型按钮
        if (levelEditor.shapeTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
            {
                CheckButtonEvents($"形状类型按钮[{i}]", levelEditor.shapeTypeButtons[i]);
            }
        }

        // 检测球类型按钮
        if (levelEditor.ballTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.ballTypeButtons.Length; i++)
            {
                CheckButtonEvents($"球类型按钮[{i}]", levelEditor.ballTypeButtons[i]);
            }
        }

        detectionLog += "=== 特定按钮检测完成 ===\n";
        Repaint();
    }

    void CheckButtonEvents(string buttonName, Button button)
    {
        if (button == null)
        {
            detectionLog += $"✗ {buttonName}: 按钮引用为空\n";
            return;
        }

        int eventCount = button.onClick.GetPersistentEventCount();
        detectionLog += $"{buttonName}: {eventCount} 个事件\n";

        if (eventCount == 0)
        {
            detectionLog += $"  ⚠️ {buttonName} 没有绑定事件\n";
        }
        else if (eventCount > 1)
        {
            detectionLog += $"  ⚠️ {buttonName} 有 {eventCount} 个事件，可能存在重复绑定\n";
            
            // 检查是否有重复的方法调用
            HashSet<string> methods = new HashSet<string>();
            for (int i = 0; i < eventCount; i++)
            {
                var methodName = button.onClick.GetPersistentMethodName(i);
                if (!methods.Add(methodName))
                {
                    detectionLog += $"  ❌ {buttonName} 存在重复的方法调用: {methodName}\n";
                }
            }
        }
        else
        {
            var methodName = button.onClick.GetPersistentMethodName(0);
            detectionLog += $"  ✅ {buttonName} 绑定方法: {methodName}\n";
        }
    }

    void CheckEventBindingSources()
    {
        detectionLog += "\n=== 检查事件绑定来源 ===\n";
        
        // 检查LevelEditorUI中的SetupEventListeners方法
        var levelEditorType = typeof(LevelEditorUI);
        var setupEventListenersMethod = levelEditorType.GetMethod("SetupEventListeners", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (setupEventListenersMethod != null)
        {
            var obsoleteAttribute = setupEventListenersMethod.GetCustomAttributes(typeof(System.ObsoleteAttribute), false);
            if (obsoleteAttribute.Length > 0)
            {
                detectionLog += "✅ LevelEditorUI.SetupEventListeners 已标记为废弃\n";
            }
            else
            {
                detectionLog += "⚠️ LevelEditorUI.SetupEventListeners 未标记为废弃\n";
            }
        }
        else
        {
            detectionLog += "✅ LevelEditorUI.SetupEventListeners 方法不存在\n";
        }

        // 检查LevelEditorUIBuilder中的事件绑定
        detectionLog += "\nLevelEditorUIBuilder 事件绑定位置:\n";
        detectionLog += "  - CreateLeftPanelButtons(): 添加层级、删除层级按钮\n";
        detectionLog += "  - CreateToolbar(): 添加形状、添加球、背景、预览按钮\n";
        detectionLog += "  - CreateShapeTypeButtons(): 形状类型按钮\n";
        detectionLog += "  - CreateBallTypeButtons(): 球类型按钮\n";
        detectionLog += "  - CreatePropertyControls(): 导入、导出按钮\n";
        detectionLog += "  - SetupEventListeners(): 滑块控件\n";

        // 检查是否有重复的事件绑定调用
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null)
        {
            // 检查Awake方法是否调用了SetupEventListeners
            var awakeMethod = levelEditorType.GetMethod("Awake", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (awakeMethod != null)
            {
                detectionLog += "\n✅ LevelEditorUI.Awake 方法存在\n";
                // 这里可以进一步检查Awake方法的内容，但比较复杂
            }
        }

        detectionLog += "=== 事件绑定来源检查完成 ===\n";
        Repaint();
    }

    /// <summary>
    /// 检查是否有重复的事件绑定
    /// </summary>
    public static bool HasDuplicateEvents(Button button)
    {
        if (button == null) return false;
        
        int eventCount = button.onClick.GetPersistentEventCount();
        if (eventCount <= 1) return false;
        
        HashSet<string> methods = new HashSet<string>();
        for (int i = 0; i < eventCount; i++)
        {
            var methodName = button.onClick.GetPersistentMethodName(i);
            if (!methods.Add(methodName))
            {
                return true; // 发现重复
            }
        }
        
        return false;
    }

    /// <summary>
    /// 获取按钮的事件绑定信息
    /// </summary>
    public static string GetButtonEventInfo(Button button)
    {
        if (button == null) return "按钮为空";
        
        int eventCount = button.onClick.GetPersistentEventCount();
        string info = $"事件数量: {eventCount}\n";
        
        for (int i = 0; i < eventCount; i++)
        {
            var target = button.onClick.GetPersistentTarget(i);
            var methodName = button.onClick.GetPersistentMethodName(i);
            info += $"  事件 {i}: {target?.name} -> {methodName}\n";
        }
        
        return info;
    }
} 