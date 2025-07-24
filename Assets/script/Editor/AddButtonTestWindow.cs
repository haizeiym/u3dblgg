using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 添加按钮测试工具
/// 用于测试添加形状和添加球按钮的功能是否正常
/// </summary>
public class AddButtonTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/添加按钮测试")]
    public static void ShowWindow()
    {
        GetWindow<AddButtonTestWindow>("添加按钮测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("添加按钮测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试添加形状和添加球按钮的功能是否正常", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("查找添加按钮"))
        {
            FindAddButtons();
        }

        if (GUILayout.Button("测试添加形状按钮"))
        {
            TestAddShapeButton();
        }

        if (GUILayout.Button("测试添加球按钮"))
        {
            TestAddBallButton();
        }

        if (GUILayout.Button("验证按钮事件绑定"))
        {
            ValidateButtonEvents();
        }

        if (GUILayout.Button("检查重复绑定"))
        {
            CheckDuplicateBindings();
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

    void FindAddButtons()
    {
        testLog = "=== 查找添加按钮 ===\n";
        
        // 查找所有按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        testLog += $"找到 {allButtons.Length} 个按钮\n";
        
        bool foundAddShapeButton = false;
        bool foundAddBallButton = false;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("添加形状") || button.name.Contains("AddShape"))
            {
                testLog += $"✓ 找到添加形状按钮: {button.name}\n";
                testLog += $"  父对象: {button.transform.parent?.name}\n";
                testLog += $"  可交互: {button.interactable}\n";
                testLog += $"  事件数量: {button.onClick.GetPersistentEventCount()}\n";
                foundAddShapeButton = true;
                
                // 检查事件监听器
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    testLog += $"    事件 {i}: {target?.name} -> {methodName}\n";
                }
            }
            else if (button.name.Contains("添加球") || button.name.Contains("AddBall"))
            {
                testLog += $"✓ 找到添加球按钮: {button.name}\n";
                testLog += $"  父对象: {button.transform.parent?.name}\n";
                testLog += $"  可交互: {button.interactable}\n";
                testLog += $"  事件数量: {button.onClick.GetPersistentEventCount()}\n";
                foundAddBallButton = true;
                
                // 检查事件监听器
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    testLog += $"    事件 {i}: {target?.name} -> {methodName}\n";
                }
            }
        }
        
        if (!foundAddShapeButton)
        {
            testLog += "✗ 未找到添加形状按钮\n";
        }
        
        if (!foundAddBallButton)
        {
            testLog += "✗ 未找到添加球按钮\n";
        }
        
        testLog += "=== 查找完成 ===\n";
        Repaint();
    }

    void TestAddShapeButton()
    {
        testLog += "\n=== 测试添加形状按钮 ===\n";
        
        // 查找添加形状按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        Button addShapeButton = null;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("添加形状") || button.name.Contains("AddShape"))
            {
                addShapeButton = button;
                break;
            }
        }
        
        if (addShapeButton != null)
        {
            testLog += $"找到添加形状按钮: {addShapeButton.name}\n";
            
            // 模拟点击
            try
            {
                addShapeButton.onClick.Invoke();
                testLog += "✓ 添加形状按钮点击事件触发成功\n";
                
                // 检查是否创建了新的形状对象
                var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
                if (levelEditor != null && levelEditor.currentLevel != null && levelEditor.currentLevel.layers.Count > 0)
                {
                    var currentLayer = levelEditor.currentLevel.layers[0];
                    testLog += $"当前层级形状数量: {currentLayer.shapes.Count}\n";
                }
            }
            catch (System.Exception e)
            {
                testLog += $"✗ 添加形状按钮点击事件触发失败: {e.Message}\n";
            }
        }
        else
        {
            testLog += "✗ 未找到添加形状按钮，无法测试点击\n";
        }
        
        testLog += "=== 添加形状测试完成 ===\n";
        Repaint();
    }

    void TestAddBallButton()
    {
        testLog += "\n=== 测试添加球按钮 ===\n";
        
        // 查找添加球按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        Button addBallButton = null;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("添加球") || button.name.Contains("AddBall"))
            {
                addBallButton = button;
                break;
            }
        }
        
        if (addBallButton != null)
        {
            testLog += $"找到添加球按钮: {addBallButton.name}\n";
            
            // 检查是否有选中的形状
            var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
            if (levelEditor != null && levelEditor.selectedShape != null)
            {
                testLog += $"当前选中形状: {levelEditor.selectedShape.name}\n";
                
                // 模拟点击
                try
                {
                    addBallButton.onClick.Invoke();
                    testLog += "✓ 添加球按钮点击事件触发成功\n";
                    
                    // 检查是否创建了新的球对象
                    if (levelEditor.selectedShape != null)
                    {
                        var shapeData = levelEditor.selectedShape.GetShapeData();
                        if (shapeData != null)
                        {
                            testLog += $"选中形状的球数量: {shapeData.balls.Count}\n";
                        }
                    }
                }
                catch (System.Exception e)
                {
                    testLog += $"✗ 添加球按钮点击事件触发失败: {e.Message}\n";
                }
            }
            else
            {
                testLog += "⚠ 没有选中形状，添加球功能可能无法正常工作\n";
            }
        }
        else
        {
            testLog += "✗ 未找到添加球按钮，无法测试点击\n";
        }
        
        testLog += "=== 添加球测试完成 ===\n";
        Repaint();
    }

    void ValidateButtonEvents()
    {
        testLog += "\n=== 验证按钮事件绑定 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null)
        {
            testLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";
            
            // 检查添加形状按钮
            if (levelEditor.addShapeButton != null)
            {
                testLog += $"✓ 添加形状按钮引用存在: {levelEditor.addShapeButton.name}\n";
                testLog += $"  事件数量: {levelEditor.addShapeButton.onClick.GetPersistentEventCount()}\n";
            }
            else
            {
                testLog += "✗ 添加形状按钮引用为空\n";
            }
            
            // 检查添加球按钮
            if (levelEditor.addBallButton != null)
            {
                testLog += $"✓ 添加球按钮引用存在: {levelEditor.addBallButton.name}\n";
                testLog += $"  事件数量: {levelEditor.addBallButton.onClick.GetPersistentEventCount()}\n";
            }
            else
            {
                testLog += "✗ 添加球按钮引用为空\n";
            }
        }
        else
        {
            testLog += "✗ 未找到LevelEditorUI\n";
        }
        
        testLog += "=== 验证完成 ===\n";
        Repaint();
    }

    void CheckDuplicateBindings()
    {
        testLog += "\n=== 检查重复绑定 ===\n";
        
        // 查找所有按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        int addShapeEventCount = 0;
        int addBallEventCount = 0;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains("添加形状") || button.name.Contains("AddShape"))
            {
                addShapeEventCount = button.onClick.GetPersistentEventCount();
                testLog += $"添加形状按钮事件数量: {addShapeEventCount}\n";
                
                if (addShapeEventCount > 1)
                {
                    testLog += "⚠ 警告：添加形状按钮可能有重复的事件绑定\n";
                }
            }
            else if (button.name.Contains("添加球") || button.name.Contains("AddBall"))
            {
                addBallEventCount = button.onClick.GetPersistentEventCount();
                testLog += $"添加球按钮事件数量: {addBallEventCount}\n";
                
                if (addBallEventCount > 1)
                {
                    testLog += "⚠ 警告：添加球按钮可能有重复的事件绑定\n";
                }
            }
        }
        
        if (addShapeEventCount == 1 && addBallEventCount == 1)
        {
            testLog += "✓ 按钮事件绑定正常，没有重复绑定\n";
        }
        else if (addShapeEventCount == 0 || addBallEventCount == 0)
        {
            testLog += "✗ 按钮事件绑定缺失\n";
        }
        
        testLog += "=== 重复绑定检查完成 ===\n";
        Repaint();
    }
} 