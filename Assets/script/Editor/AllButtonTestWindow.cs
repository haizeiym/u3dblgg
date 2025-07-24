using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 所有按钮综合测试工具
/// 用于测试一键配置后所有按钮的功能是否正常
/// </summary>
public class AllButtonTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/所有按钮测试")]
    public static void ShowWindow()
    {
        GetWindow<AllButtonTestWindow>("所有按钮测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("所有按钮综合测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试一键配置后所有按钮的功能是否正常", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("一键配置并测试"))
        {
            SetupAndTest();
        }

        if (GUILayout.Button("检查所有按钮"))
        {
            CheckAllButtons();
        }

        if (GUILayout.Button("测试左侧面板按钮"))
        {
            TestLeftPanelButtons();
        }

        if (GUILayout.Button("测试工具栏按钮"))
        {
            TestToolbarButtons();
        }

        if (GUILayout.Button("测试右侧面板按钮"))
        {
            TestRightPanelButtons();
        }

        if (GUILayout.Button("测试滑块控件"))
        {
            TestSliders();
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
            EditorGUILayout.TextArea(testLog, GUILayout.Height(400));
            EditorGUILayout.EndScrollView();
        }
    }

    void SetupAndTest()
    {
        testLog = "=== 一键配置并测试 ===\n";
        
        // 执行一键配置
        try
        {
            LevelEditorMenu.SetupLevelEditor();
            testLog += "✓ 一键配置执行成功\n";
            
            // 等待一帧让配置完成
            EditorApplication.delayCall += () => {
                CheckAllButtons();
            };
        }
        catch (System.Exception e)
        {
            testLog += $"✗ 一键配置执行失败: {e.Message}\n";
        }
        
        Repaint();
    }

    void CheckAllButtons()
    {
        testLog += "\n=== 检查所有按钮 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor == null)
        {
            testLog += "✗ 未找到LevelEditorUI\n";
            Repaint();
            return;
        }
        
        testLog += $"✓ 找到LevelEditorUI: {levelEditor.name}\n";
        
        // 检查左侧面板按钮
        CheckButton("添加层级按钮", levelEditor.addLayerButton);
        CheckButton("删除层级按钮", levelEditor.deleteLayerButton);
        
        // 检查工具栏按钮
        CheckButton("添加形状按钮", levelEditor.addShapeButton);
        CheckButton("添加球按钮", levelEditor.addBallButton);
        CheckButton("背景按钮", levelEditor.backgroundButton);
        CheckButton("预览按钮", levelEditor.previewButton);
        
        // 检查右侧面板按钮
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
        
        // 检查滑块控件
        CheckSlider("位置X滑块", levelEditor.positionXSlider);
        CheckSlider("位置Y滑块", levelEditor.positionYSlider);
        CheckSlider("旋转滑块", levelEditor.rotationSlider);
        
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
    
    void CheckSlider(string sliderName, Slider slider)
    {
        if (slider != null)
        {
            int eventCount = slider.onValueChanged.GetPersistentEventCount();
            testLog += $"✓ {sliderName}: 事件数量 = {eventCount}\n";
            
            if (eventCount == 0)
            {
                testLog += $"  ⚠ {sliderName} 没有绑定事件\n";
            }
        }
        else
        {
            testLog += $"✗ {sliderName}: 滑块引用为空\n";
        }
    }

    void TestLeftPanelButtons()
    {
        testLog += "\n=== 测试左侧面板按钮 ===\n";
        TestButton("添加层级", "添加层级", "AddLayer");
        TestButton("删除层级", "删除层级", "DeleteLayer");
        testLog += "=== 左侧面板按钮测试完成 ===\n";
        Repaint();
    }

    void TestToolbarButtons()
    {
        testLog += "\n=== 测试工具栏按钮 ===\n";
        TestButton("添加形状", "添加形状", "AddShape");
        TestButton("添加球", "添加球", "AddBall");
        TestButton("背景", "背景", "SwitchBackground");
        TestButton("预览", "预览", "ShowConfigPreview");
        testLog += "=== 工具栏按钮测试完成 ===\n";
        Repaint();
    }

    void TestRightPanelButtons()
    {
        testLog += "\n=== 测试右侧面板按钮 ===\n";
        TestButton("导出JSON", "导出JSON", "ExportLevel");
        
        // 测试形状类型按钮
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null && levelEditor.shapeTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.shapeTypeButtons.Length; i++)
            {
                var button = levelEditor.shapeTypeButtons[i];
                if (button != null)
                {
                    try
                    {
                        button.onClick.Invoke();
                        testLog += $"✓ 形状类型按钮[{i}] 点击事件触发成功\n";
                    }
                    catch (System.Exception e)
                    {
                        testLog += $"✗ 形状类型按钮[{i}] 点击事件触发失败: {e.Message}\n";
                    }
                }
            }
        }
        
        // 测试球类型按钮
        if (levelEditor != null && levelEditor.ballTypeButtons != null)
        {
            for (int i = 0; i < levelEditor.ballTypeButtons.Length; i++)
            {
                var button = levelEditor.ballTypeButtons[i];
                if (button != null)
                {
                    try
                    {
                        button.onClick.Invoke();
                        testLog += $"✓ 球类型按钮[{i}] 点击事件触发成功\n";
                    }
                    catch (System.Exception e)
                    {
                        testLog += $"✗ 球类型按钮[{i}] 点击事件触发失败: {e.Message}\n";
                    }
                }
            }
        }
        
        testLog += "=== 右侧面板按钮测试完成 ===\n";
        Repaint();
    }

    void TestSliders()
    {
        testLog += "\n=== 测试滑块控件 ===\n";
        
        var levelEditor = Object.FindObjectOfType<LevelEditorUI>();
        if (levelEditor != null)
        {
            // 测试位置X滑块
            if (levelEditor.positionXSlider != null)
            {
                try
                {
                    levelEditor.positionXSlider.value = 100f;
                    testLog += "✓ 位置X滑块值设置成功\n";
                }
                catch (System.Exception e)
                {
                    testLog += $"✗ 位置X滑块值设置失败: {e.Message}\n";
                }
            }
            
            // 测试位置Y滑块
            if (levelEditor.positionYSlider != null)
            {
                try
                {
                    levelEditor.positionYSlider.value = 50f;
                    testLog += "✓ 位置Y滑块值设置成功\n";
                }
                catch (System.Exception e)
                {
                    testLog += $"✗ 位置Y滑块值设置失败: {e.Message}\n";
                }
            }
            
            // 测试旋转滑块
            if (levelEditor.rotationSlider != null)
            {
                try
                {
                    levelEditor.rotationSlider.value = 45f;
                    testLog += "✓ 旋转滑块值设置成功\n";
                }
                catch (System.Exception e)
                {
                    testLog += $"✗ 旋转滑块值设置失败: {e.Message}\n";
                }
            }
        }
        
        testLog += "=== 滑块控件测试完成 ===\n";
        Repaint();
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
    }
} 