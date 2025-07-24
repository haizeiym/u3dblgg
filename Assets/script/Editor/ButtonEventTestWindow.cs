using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 按钮事件测试工具
/// 用于测试所有按钮的事件绑定是否正常
/// </summary>
public class ButtonEventTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/按钮事件测试")]
    public static void ShowWindow()
    {
        GetWindow<ButtonEventTestWindow>("按钮事件测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("按钮事件测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试所有按钮的事件绑定是否正常", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("检查所有按钮事件"))
        {
            CheckAllButtonEvents();
        }

        if (GUILayout.Button("测试添加形状按钮"))
        {
            TestAddShapeButton();
        }

        if (GUILayout.Button("测试添加球按钮"))
        {
            TestAddBallButton();
        }

        if (GUILayout.Button("测试添加层级按钮"))
        {
            TestAddLayerButton();
        }

        if (GUILayout.Button("测试删除层级按钮"))
        {
            TestDeleteLayerButton();
        }

        if (GUILayout.Button("测试导入按钮"))
        {
            TestImportButton();
        }

        if (GUILayout.Button("测试导出按钮"))
        {
            TestExportButton();
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

    void CheckAllButtonEvents()
    {
        testLog = "=== 检查所有按钮事件 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            testLog += "✗ 未找到LevelEditorUI\n";
            Repaint();
            return;
        }
        
        testLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";
        
        // 检查所有按钮
        CheckButton("添加形状按钮", levelEditor.addShapeButton);
        CheckButton("添加球按钮", levelEditor.addBallButton);
        CheckButton("添加层级按钮", levelEditor.addLayerButton);
        CheckButton("删除层级按钮", levelEditor.deleteLayerButton);
        CheckButton("导出按钮", levelEditor.exportButton);
        
        // 检查形状类型按钮
        if (levelEditor.shapeTypeButtons != null)
        {
            testLog += $"形状类型按钮数量: {levelEditor.shapeTypeButtons.Length}\n";
            for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
            {
                CheckButton($"形状类型按钮[{i}]", levelEditor.shapeTypeButtons[i]);
            }
        }
        else
        {
            testLog += "✗ 形状类型按钮数组为空\n";
        }
        
        // 检查球类型按钮
        if (levelEditor.ballTypeButtons != null)
        {
            testLog += $"球类型按钮数量: {levelEditor.ballTypeButtons.Length}\n";
            for (int i = 0; i < levelEditor.ballTypeButtons.Length; i++)
            {
                CheckButton($"球类型按钮[{i}]", levelEditor.ballTypeButtons[i]);
            }
        }
        else
        {
            testLog += "✗ 球类型按钮数组为空\n";
        }
        
        testLog += "=== 检查完成 ===\n";
        Repaint();
    }
    
    void CheckButton(string buttonName, Button button)
    {
        if (button != null)
        {
            int eventCount = button.onClick.GetPersistentEventCount();
            testLog += $"✓ {buttonName}: 事件数量 = {eventCount}\n";
            
            if (eventCount == 0)
            {
                testLog += $"  ⚠ {buttonName} 没有绑定事件\n";
            }
            else if (eventCount > 1)
            {
                testLog += $"  ⚠ {buttonName} 可能有重复事件绑定\n";
            }
            
            // 显示事件详情
            for (int i = 0; i < eventCount; i++)
            {
                var target = button.onClick.GetPersistentTarget(i);
                var methodName = button.onClick.GetPersistentMethodName(i);
                testLog += $"    事件 {i}: {target?.name} -> {methodName}\n";
            }
        }
        else
        {
            testLog += $"✗ {buttonName}: 按钮引用为空\n";
        }
    }

    void TestAddShapeButton()
    {
        testLog += "\n=== 测试添加形状按钮 ===\n";
        TestButton("添加形状", "添加形状", "AddShape");
    }

    void TestAddBallButton()
    {
        testLog += "\n=== 测试添加球按钮 ===\n";
        TestButton("添加球", "添加球", "AddBall");
    }

    void TestAddLayerButton()
    {
        testLog += "\n=== 测试添加层级按钮 ===\n";
        TestButton("添加层级", "添加层级", "AddLayer");
    }

    void TestDeleteLayerButton()
    {
        testLog += "\n=== 测试删除层级按钮 ===\n";
        TestButton("删除层级", "删除层级", "DeleteLayer");
    }

    void TestImportButton()
    {
        testLog += "\n=== 测试导入按钮 ===\n";
        TestButton("导入关卡", "导入关卡", "ImportLevel");
    }

    void TestExportButton()
    {
        testLog += "\n=== 测试导出按钮 ===\n";
        TestButton("导出JSON", "导出JSON", "ExportLevel");
    }

    void TestButton(string buttonName, string buttonText, string expectedMethod)
    {
        // 查找按钮
        Button[] allButtons = Object.FindObjectsOfType<Button>();
        Button targetButton = null;
        
        foreach (Button button in allButtons)
        {
            if (button.name.Contains(buttonText) || button.name.Contains(buttonText.Replace(" ", "")))
            {
                targetButton = button;
                break;
            }
        }
        
        if (targetButton != null)
        {
            testLog += $"找到{buttonName}按钮: {targetButton.name}\n";
            
            // 检查事件数量
            int eventCount = targetButton.onClick.GetPersistentEventCount();
            testLog += $"事件数量: {eventCount}\n";
            
            if (eventCount > 0)
            {
                // 检查是否有正确的方法绑定
                bool hasCorrectMethod = false;
                for (int i = 0; i < eventCount; i++)
                {
                    var methodName = targetButton.onClick.GetPersistentMethodName(i);
                    if (methodName.Contains(expectedMethod))
                    {
                        hasCorrectMethod = true;
                        testLog += $"✓ 找到正确的方法绑定: {methodName}\n";
                    }
                }
                
                if (!hasCorrectMethod)
                {
                    testLog += $"✗ 未找到正确的方法绑定，期望: {expectedMethod}\n";
                }
                
                // 尝试点击
                try
                {
                    targetButton.onClick.Invoke();
                    testLog += "✓ 按钮点击事件触发成功\n";
                }
                catch (System.Exception e)
                {
                    testLog += $"✗ 按钮点击事件触发失败: {e.Message}\n";
                }
            }
            else
            {
                testLog += "✗ 按钮没有绑定任何事件\n";
            }
        }
        else
        {
            testLog += $"✗ 未找到{buttonName}按钮\n";
        }
        
        testLog += $"=== {buttonName}测试完成 ===\n";
        Repaint();
    }
} 