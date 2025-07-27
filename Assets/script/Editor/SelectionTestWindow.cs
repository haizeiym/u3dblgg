using UnityEngine;
using UnityEditor;

public class SelectionTestWindow : EditorWindow
{
    // [MenuItem("Tools/Level Editor/Test Selection")]
    public static void ShowWindow()
    {
        GetWindow<SelectionTestWindow>("选中状态测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("选中状态测试", EditorStyles.boldLabel);
        
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            EditorGUILayout.HelpBox("场景中未找到LevelEditorUI", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.Space();
        
        // 显示当前选中状态
        EditorGUILayout.LabelField("当前选中状态:", EditorStyles.boldLabel);
        
        if (levelEditorUI.selectedShape != null)
        {
            EditorGUILayout.LabelField($"选中形状: {levelEditorUI.selectedShape.ShapeData.shapeType}");
            EditorGUILayout.LabelField($"形状位置: {levelEditorUI.selectedShape.ShapeData.position}");
            EditorGUILayout.LabelField($"固定位置数量: {levelEditorUI.selectedShape.ShapeData.fixedPositions.Count}");
        }
        else
        {
            EditorGUILayout.LabelField("未选中形状", EditorStyles.helpBox);
        }
        
        if (levelEditorUI.selectedBall != null)
        {
            EditorGUILayout.LabelField($"选中球: {levelEditorUI.selectedBall.BallData.ballType}");
            EditorGUILayout.LabelField($"球位置: {levelEditorUI.selectedBall.BallData.position}");
        }
        else
        {
            EditorGUILayout.LabelField("未选中球", EditorStyles.helpBox);
        }
        
        EditorGUILayout.Space();
        
        // 测试按钮
        if (GUILayout.Button("测试添加固定位置"))
        {
            TestAddFixedPosition();
        }
        
        if (GUILayout.Button("测试清除固定位置"))
        {
            TestClearFixedPositions();
        }
        
        if (GUILayout.Button("强制刷新UI"))
        {
            TestForceRefreshUI();
        }

        if (GUILayout.Button("🧪 测试选中状态恢复"))
        {
            TestSelectionRestore();
        }
        
        if (GUILayout.Button("🔄 测试图形类型切换"))
        {
            TestShapeTypeSwitch();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // 显示所有形状的固定位置
        if (levelEditorUI.currentLevel != null)
        {
            EditorGUILayout.LabelField("所有形状的固定位置:", EditorStyles.boldLabel);
            foreach (var layer in levelEditorUI.currentLevel.layers)
            {
                foreach (var shape in layer.shapes)
                {
                    string info = $"  {shape.shapeType}: {shape.fixedPositions.Count}个固定位置";
                    EditorGUILayout.LabelField(info);
                    
                    for (int i = 0; i < shape.fixedPositions.Count; i++)
                    {
                        EditorGUILayout.LabelField($"    位置{i + 1}: {shape.fixedPositions[i]}");
                    }
                }
            }
        }
    }
    
    void TestAddFixedPosition()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            Vector2 position = levelEditorUI.selectedShape.ShapeData.position;
            levelEditorUI.AddFixedPosition(position);
            Debug.Log($"测试添加固定位置: {position}");
            Repaint();
        }
        else
        {
            Debug.LogWarning("请先选中一个形状");
        }
    }
    
    void TestClearFixedPositions()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            levelEditorUI.ClearFixedPositions();
            Debug.Log("测试清除固定位置");
            Repaint();
        }
        else
        {
            Debug.LogWarning("请先选中一个形状");
        }
    }
    
    void TestForceRefreshUI()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI != null)
        {
            levelEditorUI.RefreshUI();
            Debug.Log("已强制刷新UI");
        }
        else
        {
            Debug.LogError("未找到LevelEditorUI");
        }
    }
    
    void TestSelectionRestore()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            // 保存当前选中的形状信息
            ShapeData originalShapeData = levelEditorUI.selectedShape.ShapeData;
            Debug.Log($"测试前选中形状: {originalShapeData.shapeType} (位置: {originalShapeData.position})");
            
            // 强制刷新UI
            levelEditorUI.RefreshUI();
            
            // 检查刷新后是否仍然选中相同的形状
            if (levelEditorUI.selectedShape != null)
            {
                ShapeData currentShapeData = levelEditorUI.selectedShape.ShapeData;
                Debug.Log($"测试后选中形状: {currentShapeData.shapeType} (位置: {currentShapeData.position})");
                
                if (currentShapeData.shapeType == originalShapeData.shapeType &&
                    Vector2.Distance(currentShapeData.position, originalShapeData.position) < 0.1f)
                {
                    Debug.Log("✅ 选中状态恢复测试成功！");
                }
                else
                {
                    Debug.LogWarning("⚠️ 选中状态恢复测试失败！");
                }
            }
            else
            {
                Debug.LogError("❌ 刷新后没有选中任何形状！");
            }
        }
        else
        {
            Debug.LogWarning("请先选中一个形状进行测试");
        }
    }
    
    void TestShapeTypeSwitch()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            ShapeData shapeData = levelEditorUI.selectedShape.ShapeData;
            Debug.Log($"=== 图形类型切换测试 ===");
            Debug.Log($"当前形状类型: {shapeData.shapeType}");
            Debug.Log($"当前固定位置数量: {shapeData.fixedPositions.Count}");
            Debug.Log($"当前球数量: {shapeData.balls.Count}");
            
            // 获取所有可用的形状类型
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                string[] shapeTypes = config.GetShapeTypeNames();
                Debug.Log($"可用形状类型: {string.Join(", ", shapeTypes)}");
                
                // 找到下一个形状类型
                int currentIndex = System.Array.IndexOf(shapeTypes, shapeData.shapeType);
                int nextIndex = (currentIndex + 1) % shapeTypes.Length;
                string nextType = shapeTypes[nextIndex];
                
                Debug.Log($"切换到形状类型: {nextType}");
                
                // 执行切换
                levelEditorUI.UpdateShapeType(nextIndex);
                
                // 等待一帧让更新完成
                EditorApplication.delayCall += () =>
                {
                    Debug.Log($"=== 切换后状态 ===");
                    Debug.Log($"新形状类型: {shapeData.shapeType}");
                    Debug.Log($"新固定位置数量: {shapeData.fixedPositions.Count}");
                    Debug.Log($"球数量: {shapeData.balls.Count}");
                    
                    // 显示新形状类型的固定位置配置
                    var newFixedPosConfig = config.GetFixedPositionConfig(shapeData.shapeType);
                    if (newFixedPosConfig != null)
                    {
                        Debug.Log($"配置文件中的固定位置数量: {newFixedPosConfig.fixedPositions.Count}");
                        for (int i = 0; i < newFixedPosConfig.fixedPositions.Count; i++)
                        {
                            Debug.Log($"  配置位置{i + 1}: {newFixedPosConfig.fixedPositions[i]}");
                        }
                    }
                    else
                    {
                        Debug.Log("配置文件中没有该形状类型的固定位置配置");
                    }
                    
                    Debug.Log("=== 测试完成 ===");
                };
            }
        }
        else
        {
            Debug.LogWarning("请先选中一个形状进行测试");
        }
    }
} 