using UnityEngine;
using UnityEditor;

public class FixedPositionTestWindow : EditorWindow
{
    // 添加字段保存用户输入的坐标值
    private float inputX = 0f;
    private float inputY = 0f;
    
    [MenuItem("Tools/Level Editor/Test Fixed Positions")]
    public static void ShowWindow()
    {
        GetWindow<FixedPositionTestWindow>("固定位置测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("固定位置功能测试", EditorStyles.boldLabel);
        
        // 查找场景中的LevelEditorUI
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            EditorGUILayout.HelpBox("场景中未找到LevelEditorUI", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.Space();
        
        // 显示当前选中形状的信息
        if (levelEditorUI.selectedShape != null)
        {
            ShapeData shapeData = levelEditorUI.selectedShape.ShapeData;
            EditorGUILayout.LabelField("当前选中形状:", shapeData.shapeType);
            EditorGUILayout.LabelField("形状位置:", shapeData.position.ToString());
            EditorGUILayout.LabelField("球数量:", shapeData.balls.Count.ToString());
            EditorGUILayout.LabelField("固定位置数量:", shapeData.fixedPositions.Count.ToString());
            
            EditorGUILayout.Space();
            
            // 显示固定位置列表
            if (shapeData.HasFixedPositions())
            {
                EditorGUILayout.LabelField("固定位置列表:", EditorStyles.boldLabel);
                for (int i = 0; i < shapeData.fixedPositions.Count; i++)
                {
                    EditorGUILayout.LabelField($"位置{i + 1}:", shapeData.fixedPositions[i].ToString());
                }
            }
            else
            {
                EditorGUILayout.LabelField("没有配置固定位置", EditorStyles.helpBox);
            }
            
            EditorGUILayout.Space();
            
            // 测试按钮
            if (GUILayout.Button("添加固定位置（使用形状当前位置）"))
            {
                TestAddFixedPosition();
            }
            
            if (GUILayout.Button("清除所有固定位置"))
            {
                TestClearFixedPositions();
            }
            
            if (GUILayout.Button("测试添加球"))
            {
                TestAddBall();
            }
            
            EditorGUILayout.Space();
            
            // 手动添加固定位置
            EditorGUILayout.LabelField("手动添加固定位置:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            // 使用保存的字段值，并更新用户输入
            inputX = EditorGUILayout.FloatField("X:", inputX);
            inputY = EditorGUILayout.FloatField("Y:", inputY);
            
            if (GUILayout.Button("添加"))
            {
                TestAddFixedPositionAt(new Vector2(inputX, inputY));
            }
            EditorGUILayout.EndHorizontal();
            
            // 添加一个按钮来使用形状当前位置作为默认值
            if (GUILayout.Button("使用形状当前位置作为默认值"))
            {
                inputX = shapeData.position.x;
                inputY = shapeData.position.y;
                Debug.Log($"已设置默认坐标: X={inputX}, Y={inputY}");
            }
            
            // 添加一个按钮来获取鼠标位置
            if (GUILayout.Button("获取当前鼠标位置"))
            {
                Vector2 mousePos = GetMousePositionInEditArea();
                inputX = mousePos.x;
                inputY = mousePos.y;
                Debug.Log($"已获取鼠标位置: X={inputX}, Y={inputY}");
            }
            
            // 显示当前输入的坐标
            EditorGUILayout.LabelField($"当前输入坐标: ({inputX:F2}, {inputY:F2})", EditorStyles.helpBox);
        }
        else
        {
            EditorGUILayout.HelpBox("请先选中一个形状", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        // 显示所有形状的固定位置信息
        EditorGUILayout.LabelField("所有形状的固定位置信息:", EditorStyles.boldLabel);
        if (levelEditorUI.currentLevel != null)
        {
            foreach (var layer in levelEditorUI.currentLevel.layers)
            {
                EditorGUILayout.LabelField($"层级: {layer.layerName}", EditorStyles.boldLabel);
                foreach (var shape in layer.shapes)
                {
                    string info = $"  {shape.shapeType}: {shape.balls.Count}个球, {shape.fixedPositions.Count}个固定位置";
                    EditorGUILayout.LabelField(info);
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
            levelEditorUI.selectedShape.ShapeData.AddFixedPosition(position);
            Debug.Log($"已添加固定位置: {position}");
            Repaint();
        }
    }
    
    void TestAddFixedPositionAt(Vector2 position)
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            // 使用新的重载方法
            levelEditorUI.AddFixedPosition(position);
            Debug.Log($"已添加固定位置: {position}");
            Repaint();
        }
    }
    
    void TestClearFixedPositions()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            levelEditorUI.selectedShape.ShapeData.ClearFixedPositions();
            Debug.Log("已清除所有固定位置");
            Repaint();
        }
    }
    
    void TestAddBall()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI != null)
        {
            levelEditorUI.AddBall();
            Debug.Log("已尝试添加球");
            Repaint();
        }
    }
    
    /// <summary>
    /// 获取鼠标在编辑区的位置（复制自LevelEditorUI）
    /// </summary>
    private Vector2 GetMousePositionInEditArea()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.editAreaBackground == null)
        {
            Debug.LogWarning("编辑区背景为空，无法获取鼠标位置");
            return Vector2.zero;
        }
            
        // 获取鼠标屏幕位置
        Vector3 mouseScreenPos = Input.mousePosition;
        
        // 尝试转换为编辑区的本地坐标
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            levelEditorUI.editAreaBackground.rectTransform, 
            mouseScreenPos, 
            null, 
            out localPoint))
        {
            Debug.Log($"成功获取鼠标位置: 屏幕坐标={mouseScreenPos}, 本地坐标={localPoint}");
            return localPoint;
        }
        
        // 如果转换失败，尝试使用世界坐标转换
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            levelEditorUI.editAreaBackground.rectTransform,
            mouseScreenPos,
            null,
            out worldPoint))
        {
            Vector2 localFromWorld = levelEditorUI.editAreaBackground.rectTransform.InverseTransformPoint(worldPoint);
            Debug.Log($"通过世界坐标获取鼠标位置: 屏幕坐标={mouseScreenPos}, 世界坐标={worldPoint}, 本地坐标={localFromWorld}");
            return localFromWorld;
        }
        
        Debug.LogWarning($"无法将鼠标位置转换为编辑区坐标: 屏幕坐标={mouseScreenPos}");
        return Vector2.zero;
    }
} 