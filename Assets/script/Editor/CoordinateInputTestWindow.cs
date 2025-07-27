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
        var window = GetWindow<CoordinateInputTestWindow>("固定位置编辑器");
        window.minSize = new Vector2(400, 600);
        window.maxSize = new Vector2(500, 800);
    }
    
    /// <summary>
    /// 公共方法，供其他脚本调用打开窗口
    /// </summary>
    public static void OpenWindow()
    {
        ShowWindow();
    }
    
    void OnEnable()
    {
        // 窗口打开时自动获取当前选中形状的位置
        InitializeWithSelectedShape();
    }
    
    void OnGUI()
    {
        GUILayout.Label("🎯 固定位置编辑器", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("为选中的形状添加精确的固定位置", EditorStyles.miniLabel);
        
        EditorGUILayout.Space();
        
        // 查找场景中的LevelEditorUI
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI == null)
        {
            EditorGUILayout.HelpBox("❌ 场景中未找到LevelEditorUI", MessageType.Warning);
            return;
        }
        
        // 显示当前选中形状信息
        if (levelEditorUI.selectedShape != null)
        {
            ShapeData shapeData = levelEditorUI.selectedShape.ShapeData;
            EditorGUILayout.LabelField("✅ 当前选中形状:", shapeData.shapeType);
            EditorGUILayout.LabelField("📍 形状位置:", shapeData.position.ToString());
            EditorGUILayout.LabelField("📌 固定位置数量:", shapeData.fixedPositions.Count.ToString());
            
            // 显示当前固定位置列表
            if (shapeData.HasFixedPositions())
            {
                EditorGUILayout.LabelField("📋 当前固定位置:", EditorStyles.boldLabel);
                for (int i = 0; i < shapeData.fixedPositions.Count; i++)
                {
                    EditorGUILayout.LabelField($"  位置{i + 1}: {shapeData.fixedPositions[i]}");
                }
            }
            
            // 显示配置文件中的固定位置信息
            var config = LevelEditorConfig.Instance;
            if (config != null)
            {
                var configFixedPos = config.GetFixedPositionConfig(shapeData.shapeType);
                if (configFixedPos != null && configFixedPos.fixedPositions.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("⚙️ 配置文件中的固定位置:", EditorStyles.boldLabel);
                    for (int i = 0; i < configFixedPos.fixedPositions.Count; i++)
                    {
                        EditorGUILayout.LabelField($"  配置位置{i + 1}: {configFixedPos.fixedPositions[i]}");
                    }
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ 请先选中一个形状", MessageType.Info);
            
            // 如果没有选中形状，显示所有可用形状
            if (levelEditorUI.currentLevel != null)
            {
                EditorGUILayout.LabelField("📝 可用形状:", EditorStyles.boldLabel);
                foreach (var layer in levelEditorUI.currentLevel.layers)
                {
                    foreach (var shape in layer.shapes)
                    {
                        EditorGUILayout.LabelField($"  {shape.shapeType} (位置: {shape.position})");
                    }
                }
            }
            return;
        }
        
        EditorGUILayout.Space();
        
        // 坐标输入区域
        EditorGUILayout.LabelField("🎮 坐标输入:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("X坐标:", GUILayout.Width(50));
        inputX = EditorGUILayout.FloatField(inputX, GUILayout.Width(100));
        EditorGUILayout.LabelField("Y坐标:", GUILayout.Width(50));
        inputY = EditorGUILayout.FloatField(inputY, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 主要操作按钮
        EditorGUILayout.LabelField("🚀 主要操作:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("➕ 添加固定位置", GUILayout.Height(30)))
        {
            TestAddFixedPosition();
        }
        
        if (GUILayout.Button("🖱️ 获取鼠标位置", GUILayout.Height(30)))
        {
            GetMousePosition();
        }
        
        if (GUILayout.Button("📍 使用形状位置", GUILayout.Height(30)))
        {
            UseShapePosition();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 快速操作按钮
        EditorGUILayout.LabelField("⚡ 快速操作:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🗑️ 清除所有固定位置"))
        {
            ClearAllFixedPositions();
        }
        
        if (GUILayout.Button("👁️ 显示固定位置"))
        {
            ShowFixedPositions();
        }
        
        if (GUILayout.Button("📥 从配置文件加载"))
        {
            LoadFromConfig();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 显示结果
        EditorGUILayout.LabelField("📊 操作结果:", EditorStyles.boldLabel);
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
        
        // 使用说明
        EditorGUILayout.LabelField("📖 使用说明:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "1️⃣ 手动输入坐标值，然后点击'添加固定位置'\n" +
            "2️⃣ 点击'获取鼠标位置'自动获取当前鼠标在编辑区的位置\n" +
            "3️⃣ 点击'使用形状位置'使用当前选中形状的位置\n" +
            "4️⃣ 使用'清除所有固定位置'可以清除当前形状的所有固定位置", 
            MessageType.Info);
    }
    
    /// <summary>
    /// 初始化窗口时获取当前选中形状的信息
    /// </summary>
    void InitializeWithSelectedShape()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            Vector2 shapePos = levelEditorUI.selectedShape.ShapeData.position;
            inputX = shapePos.x;
            inputY = shapePos.y;
            
            lastResult = $"窗口已初始化，使用形状位置: ({inputX:F2}, {inputY:F2})";
            Debug.Log(lastResult);
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
    
    void ClearAllFixedPositions()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            levelEditorUI.ClearFixedPositions();
            lastResult = "已清除所有固定位置";
            Debug.Log(lastResult);
            Repaint();
        }
        else
        {
            lastResult = "错误：请先选中一个形状";
        }
    }
    
    void ShowFixedPositions()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            levelEditorUI.ShowFixedPositions();
            lastResult = "已在控制台显示固定位置信息";
            Debug.Log(lastResult);
        }
        else
        {
            lastResult = "错误：请先选中一个形状";
        }
    }
    
    void LoadFromConfig()
    {
        LevelEditorUI levelEditorUI = FindObjectOfType<LevelEditorUI>();
        if (levelEditorUI?.selectedShape != null)
        {
            levelEditorUI.selectedShape.ShapeData.LoadFixedPositionsFromConfig();
            lastResult = "已从配置文件加载固定位置";
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