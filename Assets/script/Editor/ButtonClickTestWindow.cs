using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 按钮点击测试工具
/// 用于测试形状类型和球类型按钮的点击事件
/// </summary>
public class ButtonClickTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/按钮点击测试")]
    public static void ShowWindow()
    {
        GetWindow<ButtonClickTestWindow>("按钮点击测试");
    }

    void OnGUI()
    {
        GUILayout.Label("按钮点击测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("测试形状类型按钮点击"))
        {
            TestShapeTypeButtonClick();
        }

        if (GUILayout.Button("测试球类型按钮点击"))
        {
            TestBallTypeButtonClick();
        }

        if (GUILayout.Button("检查按钮状态"))
        {
            CheckButtonStatus();
        }

        if (GUILayout.Button("模拟按钮点击事件"))
        {
            SimulateButtonClickEvents();
        }

        EditorGUILayout.Space();

        GUILayout.Label("测试说明:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 确保已运行一键配置");
        EditorGUILayout.LabelField("2. 点击测试按钮验证事件触发");
        EditorGUILayout.LabelField("3. 查看控制台输出确认功能正常");
        EditorGUILayout.LabelField("4. 检查按钮状态是否正确更新");
    }

    void TestShapeTypeButtonClick()
    {
        Debug.Log("=== 测试形状类型按钮点击 ===");
        
        // 查找LevelEditorUI实例
        var levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            Debug.LogError("未找到LevelEditorUI实例，请先运行一键配置");
            return;
        }

        // 检查形状类型按钮数组
        if (levelEditorUI.shapeTypeButtons == null || levelEditorUI.shapeTypeButtons.Length == 0)
        {
            Debug.LogError("形状类型按钮数组为空");
            return;
        }

        Debug.Log($"找到 {levelEditorUI.shapeTypeButtons.Length} 个形状类型按钮");

        // 测试每个按钮的点击事件
        for (int i = 0; i < levelEditorUI.shapeTypeButtons.Length; i++)
        {
            var button = levelEditorUI.shapeTypeButtons[i];
            if (button != null)
            {
                Debug.Log($"测试按钮 {i}: {button.name}");
                
                // 检查按钮是否有点击事件
                var onClick = button.onClick;
                if (onClick != null && onClick.GetPersistentEventCount() > 0)
                {
                    Debug.Log($"按钮 {i} 有 {onClick.GetPersistentEventCount()} 个点击事件");
                    
                    // 模拟点击
                    button.onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning($"按钮 {i} 没有点击事件");
                }
            }
            else
            {
                Debug.LogWarning($"按钮 {i} 为空");
            }
        }
    }

    void TestBallTypeButtonClick()
    {
        Debug.Log("=== 测试球类型按钮点击 ===");
        
        // 查找LevelEditorUI实例
        var levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            Debug.LogError("未找到LevelEditorUI实例，请先运行一键配置");
            return;
        }

        // 检查球类型按钮数组
        if (levelEditorUI.ballTypeButtons == null || levelEditorUI.ballTypeButtons.Length == 0)
        {
            Debug.LogError("球类型按钮数组为空");
            return;
        }

        Debug.Log($"找到 {levelEditorUI.ballTypeButtons.Length} 个球类型按钮");

        // 测试每个按钮的点击事件
        for (int i = 0; i < levelEditorUI.ballTypeButtons.Length; i++)
        {
            var button = levelEditorUI.ballTypeButtons[i];
            if (button != null)
            {
                Debug.Log($"测试按钮 {i}: {button.name}");
                
                // 检查按钮是否有点击事件
                var onClick = button.onClick;
                if (onClick != null && onClick.GetPersistentEventCount() > 0)
                {
                    Debug.Log($"按钮 {i} 有 {onClick.GetPersistentEventCount()} 个点击事件");
                    
                    // 模拟点击
                    button.onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning($"按钮 {i} 没有点击事件");
                }
            }
            else
            {
                Debug.LogWarning($"按钮 {i} 为空");
            }
        }
    }

    void CheckButtonStatus()
    {
        Debug.Log("=== 检查按钮状态 ===");
        
        var levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            Debug.LogError("未找到LevelEditorUI实例");
            return;
        }

        // 检查形状类型按钮
        Debug.Log("形状类型按钮状态:");
        if (levelEditorUI.shapeTypeButtons != null)
        {
            for (int i = 0; i < levelEditorUI.shapeTypeButtons.Length; i++)
            {
                var button = levelEditorUI.shapeTypeButtons[i];
                if (button != null)
                {
                    var image = button.GetComponent<Image>();
                    var color = image != null ? image.color : Color.white;
                    Debug.Log($"  按钮 {i}: {button.name}, 颜色: {color}, 事件数量: {button.onClick.GetPersistentEventCount()}");
                }
            }
        }

        // 检查球类型按钮
        Debug.Log("球类型按钮状态:");
        if (levelEditorUI.ballTypeButtons != null)
        {
            for (int i = 0; i < levelEditorUI.ballTypeButtons.Length; i++)
            {
                var button = levelEditorUI.ballTypeButtons[i];
                if (button != null)
                {
                    var image = button.GetComponent<Image>();
                    var color = image != null ? image.color : Color.white;
                    Debug.Log($"  按钮 {i}: {button.name}, 颜色: {color}, 事件数量: {button.onClick.GetPersistentEventCount()}");
                }
            }
        }

        // 检查当前索引
        Debug.Log($"当前形状类型索引: {levelEditorUI.currentShapeTypeIndex}");
        Debug.Log($"当前球类型索引: {levelEditorUI.currentBallTypeIndex}");
    }

    void SimulateButtonClickEvents()
    {
        Debug.Log("=== 模拟按钮点击事件 ===");
        
        var levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            Debug.LogError("未找到LevelEditorUI实例");
            return;
        }

        // 模拟形状类型按钮点击
        Debug.Log("模拟形状类型按钮点击...");
        for (int i = 0; i < 3; i++) // 测试前3个按钮
        {
            if (levelEditorUI.shapeTypeButtons != null && i < levelEditorUI.shapeTypeButtons.Length)
            {
                var button = levelEditorUI.shapeTypeButtons[i];
                if (button != null)
                {
                    Debug.Log($"模拟点击形状类型按钮 {i}");
                    button.onClick.Invoke();
                }
            }
        }

        // 模拟球类型按钮点击
        Debug.Log("模拟球类型按钮点击...");
        for (int i = 0; i < 3; i++) // 测试前3个按钮
        {
            if (levelEditorUI.ballTypeButtons != null && i < levelEditorUI.ballTypeButtons.Length)
            {
                var button = levelEditorUI.ballTypeButtons[i];
                if (button != null)
                {
                    Debug.Log($"模拟点击球类型按钮 {i}");
                    button.onClick.Invoke();
                }
            }
        }

        Debug.Log("按钮点击事件模拟完成");
    }
} 