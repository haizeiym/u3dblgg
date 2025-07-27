using UnityEngine;
using UnityEditor;

public class SelectionTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/Test Selection")]
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
        
        EditorGUILayout.Space();
        
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
            Debug.Log("强制刷新UI完成");
            Repaint();
        }
    }
} 