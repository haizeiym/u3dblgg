using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 添加按钮调试工具
/// 专门用于调试添加形状和添加球按钮的功能
/// </summary>
public class AddButtonDebugger : EditorWindow
{
    // [MenuItem("Tools/Level Editor/添加按钮调试")]
    // public static void ShowWindow()
    // {
    //     GetWindow<AddButtonDebugger>("添加按钮调试");
    // }

    private Vector2 scrollPosition;
    private string debugLog = "";
    private bool autoTest = false;

    void OnGUI()
    {
        EditorGUILayout.LabelField("添加按钮调试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具专门用于调试添加形状和添加球按钮的功能", MessageType.Info);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("测试添加形状按钮"))
        {
            TestAddShapeButton();
        }
        if (GUILayout.Button("测试添加球按钮"))
        {
            TestAddBallButton();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("检查按钮状态"))
        {
            CheckButtonStatus();
        }
        if (GUILayout.Button("检查配置状态"))
        {
            CheckConfigStatus();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("检查层级状态"))
        {
            CheckLayerStatus();
        }
        if (GUILayout.Button("检查UI管理器"))
        {
            CheckUIManager();
        }
        EditorGUILayout.EndHorizontal();

        autoTest = EditorGUILayout.Toggle("自动测试模式", autoTest);
        if (autoTest)
        {
            EditorGUILayout.HelpBox("自动测试模式：每次点击按钮后自动检查状态", MessageType.Info);
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

    void TestAddShapeButton()
    {
        debugLog += "\n=== 测试添加形状按钮 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ 未找到LevelEditorUI\n";
            return;
        }

        debugLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";

        // 检查按钮
        if (levelEditor.addShapeButton == null)
        {
            debugLog += "✗ 添加形状按钮为空\n";
            return;
        }

        debugLog += $"✓ 添加形状按钮存在: {levelEditor.addShapeButton.name}\n";
        debugLog += $"  事件数量: {levelEditor.addShapeButton.onClick.GetPersistentEventCount()}\n";

        // 记录测试前状态
        debugLog += "\n--- 测试前状态 ---\n";
        CheckLayerStatus();
        CheckConfigStatus();

        // 模拟点击
        debugLog += "\n--- 模拟点击添加形状按钮 ---\n";
        try
        {
            levelEditor.addShapeButton.onClick.Invoke();
            debugLog += "✓ 按钮点击成功\n";
        }
        catch (System.Exception e)
        {
            debugLog += $"✗ 按钮点击失败: {e.Message}\n";
            debugLog += $"  错误详情: {e.StackTrace}\n";
        }

        // 延迟检查测试后状态
        EditorApplication.delayCall += () => {
            debugLog += "\n--- 测试后状态 ---\n";
            CheckLayerStatus();
            CheckConfigStatus();
            debugLog += "=== 添加形状按钮测试完成 ===\n";
            Repaint();
        };

        Repaint();
    }

    void TestAddBallButton()
    {
        debugLog += "\n=== 测试添加球按钮 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ 未找到LevelEditorUI\n";
            return;
        }

        debugLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";

        // 检查按钮
        if (levelEditor.addBallButton == null)
        {
            debugLog += "✗ 添加球按钮为空\n";
            return;
        }

        debugLog += $"✓ 添加球按钮存在: {levelEditor.addBallButton.name}\n";
        debugLog += $"  事件数量: {levelEditor.addBallButton.onClick.GetPersistentEventCount()}\n";

        // 记录测试前状态
        debugLog += "\n--- 测试前状态 ---\n";
        CheckLayerStatus();
        CheckConfigStatus();

        // 模拟点击
        debugLog += "\n--- 模拟点击添加球按钮 ---\n";
        try
        {
            levelEditor.addBallButton.onClick.Invoke();
            debugLog += "✓ 按钮点击成功\n";
        }
        catch (System.Exception e)
        {
            debugLog += $"✗ 按钮点击失败: {e.Message}\n";
            debugLog += $"  错误详情: {e.StackTrace}\n";
        }

        // 延迟检查测试后状态
        EditorApplication.delayCall += () => {
            debugLog += "\n--- 测试后状态 ---\n";
            CheckLayerStatus();
            CheckConfigStatus();
            debugLog += "=== 添加球按钮测试完成 ===\n";
            Repaint();
        };

        Repaint();
    }

    void CheckButtonStatus()
    {
        debugLog += "\n--- 按钮状态检查 ---\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ 未找到LevelEditorUI\n";
            return;
        }

        // 检查添加形状按钮
        if (levelEditor.addShapeButton != null)
        {
            debugLog += $"✓ 添加形状按钮: {levelEditor.addShapeButton.name}\n";
            debugLog += $"  事件数量: {levelEditor.addShapeButton.onClick.GetPersistentEventCount()}\n";
            debugLog += $"  按钮激活状态: {levelEditor.addShapeButton.gameObject.activeInHierarchy}\n";
            debugLog += $"  按钮交互状态: {levelEditor.addShapeButton.interactable}\n";
        }
        else
        {
            debugLog += "✗ 添加形状按钮为空\n";
        }

        // 检查添加球按钮
        if (levelEditor.addBallButton != null)
        {
            debugLog += $"✓ 添加球按钮: {levelEditor.addBallButton.name}\n";
            debugLog += $"  事件数量: {levelEditor.addBallButton.onClick.GetPersistentEventCount()}\n";
            debugLog += $"  按钮激活状态: {levelEditor.addBallButton.gameObject.activeInHierarchy}\n";
            debugLog += $"  按钮交互状态: {levelEditor.addBallButton.interactable}\n";
        }
        else
        {
            debugLog += "✗ 添加球按钮为空\n";
        }
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
                
                if (config.shapeTypes != null && config.shapeTypes.Count > 0)
                {
                    debugLog += "形状类型列表:\n";
                    for (int i = 0; i < config.shapeTypes.Count; i++)
                    {
                        debugLog += $"  [{i}] {config.shapeTypes[i].name}\n";
                    }
                }
                
                if (config.ballTypes != null && config.ballTypes.Count > 0)
                {
                    debugLog += "球类型列表:\n";
                    for (int i = 0; i < config.ballTypes.Count; i++)
                    {
                        debugLog += $"  [{i}] {config.ballTypes[i].name}\n";
                    }
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

    void CheckLayerStatus()
    {
        debugLog += "\n--- 层级状态检查 ---\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ 未找到LevelEditorUI\n";
            return;
        }

        if (levelEditor.currentLevel != null)
        {
            debugLog += $"✓ 当前关卡: {levelEditor.currentLevel.levelName}\n";
            debugLog += $"层级数量: {levelEditor.currentLevel.layers.Count}\n";
            
            if (levelEditor.currentLevel.layers.Count > 0)
            {
                for (int i = 0; i < levelEditor.currentLevel.layers.Count; i++)
                {
                    var layer = levelEditor.currentLevel.layers[i];
                    debugLog += $"  层级[{i}]: {layer.layerName} (激活: {layer.isActive})\n";
                    debugLog += $"    形状数量: {layer.shapes.Count}\n";
                    debugLog += $"    球数量: {GetTotalBallsInLayer(layer)}\n";
                }
            }
        }
        else
        {
            debugLog += "✗ 当前关卡为空\n";
        }

        if (levelEditor.currentLayer != null)
        {
            debugLog += $"✓ 当前层级: {levelEditor.currentLayer.layerName}\n";
            debugLog += $"激活状态: {levelEditor.currentLayer.isActive}\n";
            debugLog += $"形状数量: {levelEditor.currentLayer.shapes.Count}\n";
            debugLog += $"球数量: {GetTotalBallsInLayer(levelEditor.currentLayer)}\n";
        }
        else
        {
            debugLog += "✗ 当前层级为空\n";
        }

        debugLog += $"当前形状类型索引: {levelEditor.currentShapeTypeIndex}\n";
        debugLog += $"当前球类型索引: {levelEditor.currentBallTypeIndex}\n";
        debugLog += $"选中形状: {(levelEditor.selectedShape != null ? levelEditor.selectedShape.name : "无")}\n";
        debugLog += $"选中球: {(levelEditor.selectedBall != null ? levelEditor.selectedBall.name : "无")}\n";
    }

    void CheckUIManager()
    {
        debugLog += "\n--- UI管理器检查 ---\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            debugLog += "✗ 未找到LevelEditorUI\n";
            return;
        }

        // 使用反射检查私有字段
        var uiManagerField = typeof(LevelEditorUI).GetField("uiManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (uiManagerField != null)
        {
            var uiManager = uiManagerField.GetValue(levelEditor);
            if (uiManager != null)
            {
                debugLog += $"✓ UI管理器存在: {uiManager.GetType().Name}\n";
            }
            else
            {
                debugLog += "✗ UI管理器为空\n";
            }
        }

        var dataManagerField = typeof(LevelEditorUI).GetField("dataManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (dataManagerField != null)
        {
            var dataManager = dataManagerField.GetValue(levelEditor);
            if (dataManager != null)
            {
                debugLog += $"✓ 数据管理器存在: {dataManager.GetType().Name}\n";
            }
            else
            {
                debugLog += "✗ 数据管理器为空\n";
            }
        }
    }

    int GetTotalBallsInLayer(LayerData layer)
    {
        int total = 0;
        foreach (var shape in layer.shapes)
        {
            total += shape.balls.Count;
        }
        return total;
    }
} 