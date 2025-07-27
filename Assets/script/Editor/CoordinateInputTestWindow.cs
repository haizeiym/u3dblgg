using UnityEngine;
using UnityEditor;

public class CoordinateInputTestWindow : EditorWindow
{
    // 坐标输入字段
    private float inputX = 0f;
    private float inputY = 0f;
    
    // 测试结果
    private Vector2 lastAddedPosition = Vector2.zero;
    private string lastResult = "";
    
    [MenuItem("Tools/Level Editor/Test Coordinate Input")]
    public static void ShowWindow()
    {
        GetWindow<CoordinateInputTestWindow>("坐标输入测试");
    }
    
    void OnGUI()
    {
        GUILayout.Label("坐标输入功能测试", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        // 查找场景中的LevelEditorUI
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            EditorGUILayout.HelpBox("场景中未找到LevelEditorUI", MessageType.Warning);
            return;
        }
        
        // 显示当前选中形状信息
        if (levelEditorUI.selectedShape != null)
        {
            ShapeData shapeData = levelEditorUI.selectedShape.ShapeData;
            EditorGUILayout.LabelField("当前选中形状:", shapeData.shapeType);
            EditorGUILayout.LabelField("形状位置:", shapeData.position.ToString());
            EditorGUILayout.LabelField("固定位置数量:", shapeData.fixedPositions.Count.ToString());
        }
        else
        {
            EditorGUILayout.HelpBox("请先选中一个形状", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        // 坐标输入区域
        EditorGUILayout.LabelField("坐标输入:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("X坐标:", GUILayout.Width(50));
        inputX = EditorGUILayout.FloatField(inputX, GUILayout.Width(100));
        EditorGUILayout.LabelField("Y坐标:", GUILayout.Width(50));
        inputY = EditorGUILayout.FloatField(inputY, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 操作按钮
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("添加固定位置"))
        {
            TestAddFixedPosition();
        }
        
        if (GUILayout.Button("获取鼠标位置"))
        {
            GetMousePosition();
        }
        
        if (GUILayout.Button("使用形状位置"))
        {
            UseShapePosition();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 显示结果
        EditorGUILayout.LabelField("测试结果:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"当前输入: ({inputX:F2}, {inputY:F2})", EditorStyles.helpBox);
        
        if (!string.IsNullOrEmpty(lastResult))
        {
            EditorGUILayout.LabelField($"最后操作: {lastResult}", EditorStyles.helpBox);
        }
        
        if (lastAddedPosition != Vector2.zero)
        {
            EditorGUILayout.LabelField($"最后添加位置: {lastAddedPosition}", EditorStyles.helpBox);
        }
        
        EditorGUILayout.Space();
        
        // 显示所有固定位置
        if (levelEditorUI.selectedShape != null)
        {
            ShapeData shapeData = levelEditorUI.selectedShape.ShapeData;
            if (shapeData.HasFixedPositions())
            {
                EditorGUILayout.LabelField("当前固定位置列表:", EditorStyles.boldLabel);
                for (int i = 0; i < shapeData.fixedPositions.Count; i++)
                {
                    EditorGUILayout.LabelField($"位置{i + 1}: {shapeData.fixedPositions[i]}");
                }
            }
        }
    }
    
    void TestAddFixedPosition()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape == null)
        {
            lastResult = "错误：请先选中一个形状";
            return;
        }
        
        Vector2 position = new Vector2(inputX, inputY);
        levelEditorUI.AddFixedPosition(position);
        
        lastAddedPosition = position;
        lastResult = $"成功添加固定位置: {position}";
        
        Debug.Log(lastResult);
        Repaint();
    }
    
    void GetMousePosition()
    {
        Vector2 mousePos = GetMousePositionInEditArea();
        inputX = mousePos.x;
        inputY = mousePos.y;
        
        lastResult = $"获取鼠标位置: ({inputX:F2}, {inputY:F2})";
        Debug.Log(lastResult);
        Repaint();
    }
    
    void UseShapePosition()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            Vector2 shapePos = levelEditorUI.selectedShape.ShapeData.position;
            inputX = shapePos.x;
            inputY = shapePos.y;
            
            lastResult = $"使用形状位置: ({inputX:F2}, {inputY:F2})";
            Debug.Log(lastResult);
            Repaint();
        }
        else
        {
            lastResult = "错误：请先选中一个形状";
        }
    }
    
    /// <summary>
    /// 获取鼠标在编辑区的位置
    /// </summary>
    private Vector2 GetMousePositionInEditArea()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.editAreaBackground == null)
        {
            return Vector2.zero;
        }
            
        Vector3 mouseScreenPos = Input.mousePosition;
        
        // 尝试转换为编辑区的本地坐标
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            levelEditorUI.editAreaBackground.rectTransform, 
            mouseScreenPos, 
            null, 
            out localPoint))
        {
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
            return localFromWorld;
        }
        
        return Vector2.zero;
    }
} 