using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 导入按钮测试工具
/// 用于测试导入按钮的功能是否正常
/// </summary>
public class ImportButtonTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/导入按钮测试")]
    public static void ShowWindow()
    {
        GetWindow<ImportButtonTestWindow>("导入按钮测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("导入按钮测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试导入按钮的功能是否正常", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("查找导入按钮"))
        {
            FindImportButton();
        }

        if (GUILayout.Button("测试按钮点击"))
        {
            TestButtonClick();
        }

        if (GUILayout.Button("创建测试关卡文件"))
        {
            CreateTestLevelFile();
        }

        if (GUILayout.Button("验证LevelEditorUI"))
        {
            ValidateLevelEditorUI();
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

    void FindImportButton()
    {
        testLog = "=== 查找导入按钮 ===\n";
        
        // 查找所有按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        testLog += $"找到 {allButtons.Length} 个按钮\n";
        
        bool foundImportButton = false;
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("导入关卡") || button.name.Contains("Import"))
            {
                testLog += $"✓ 找到导入按钮: {button.name}\n";
                testLog += $"  父对象: {button.transform.parent?.name}\n";
                testLog += $"  可交互: {button.interactable}\n";
                testLog += $"  事件数量: {button.onClick.GetPersistentEventCount()}\n";
                foundImportButton = true;
                
                // 检查事件监听器
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    testLog += $"    事件 {i}: {target?.name} -> {methodName}\n";
                }
            }
        }
        
        if (!foundImportButton)
        {
            testLog += "✗ 未找到导入按钮\n";
            
            // 列出所有按钮名称
            testLog += "所有按钮名称:\n";
            foreach (Button button in allButtons)
            {
                testLog += $"  - {button.name}\n";
            }
        }
        
        testLog += "=== 查找完成 ===\n";
        Repaint();
    }

    void TestButtonClick()
    {
        testLog += "\n=== 测试按钮点击 ===\n";
        
        // 查找导入按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        Button importButton = null;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("导入关卡") || button.name.Contains("Import"))
            {
                importButton = button;
                break;
            }
        }
        
        if (importButton != null)
        {
            testLog += $"找到导入按钮: {importButton.name}\n";
            
            // 模拟点击
            try
            {
                importButton.onClick.Invoke();
                testLog += "✓ 按钮点击事件触发成功\n";
            }
            catch (System.Exception e)
            {
                testLog += $"✗ 按钮点击事件触发失败: {e.Message}\n";
            }
        }
        else
        {
            testLog += "✗ 未找到导入按钮，无法测试点击\n";
        }
        
        testLog += "=== 点击测试完成 ===\n";
        Repaint();
    }

    void CreateTestLevelFile()
    {
        testLog += "\n=== 创建测试关卡文件 ===\n";
        
        // 创建测试关卡数据
        var testLevel = new LevelData("测试关卡");
        var layer = new LayerData("测试层级");
        var shape = new ShapeData("圆形", new Vector2(100, 100), 0f);
        var ball = new BallData("红球", new Vector2(10, 10));
        shape.balls.Add(ball);
        layer.shapes.Add(shape);
        testLevel.layers.Add(layer);
        
        // 保存到文件
        string savedLevelsPath = Application.dataPath + "/SavedLevels";
        if (!System.IO.Directory.Exists(savedLevelsPath))
        {
            System.IO.Directory.CreateDirectory(savedLevelsPath);
        }
        
        string json = LevelDataExporter.SaveToJson(testLevel);
        string filePath = System.IO.Path.Combine(savedLevelsPath, "test_level.json");
        System.IO.File.WriteAllText(filePath, json);
        
        testLog += $"✓ 测试关卡文件已创建: {filePath}\n";
        testLog += $"文件大小: {json.Length} 字符\n";
        
        testLog += "=== 测试文件创建完成 ===\n";
        Repaint();
    }

    void ValidateLevelEditorUI()
    {
        testLog += "\n=== 验证LevelEditorUI ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null)
        {
            testLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";
            testLog += $"  当前关卡: {levelEditor.currentLevel?.levelName ?? "无"}\n";
            testLog += $"  当前层级: {levelEditor.currentLayer?.layerName ?? "无"}\n";
            testLog += $"  层级数量: {levelEditor.currentLevel?.layers.Count ?? 0}\n";
            
            // 检查数据管理器
            var dataManagerField = typeof(LevelEditorUI).GetField("dataManager", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (dataManagerField != null)
            {
                var dataManager = dataManagerField.GetValue(levelEditor);
                testLog += $"  数据管理器: {(dataManager != null ? "存在" : "不存在")}\n";
            }
            
            // 检查UI管理器
            var uiManagerField = typeof(LevelEditorUI).GetField("uiManager", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (uiManagerField != null)
            {
                var uiManager = uiManagerField.GetValue(levelEditor);
                testLog += $"  UI管理器: {(uiManager != null ? "存在" : "不存在")}\n";
            }
        }
        else
        {
            testLog += "✗ 未找到LevelEditorUI\n";
        }
        
        testLog += "=== 验证完成 ===\n";
        Repaint();
    }
} 