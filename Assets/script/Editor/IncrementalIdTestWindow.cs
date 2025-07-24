using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 递增ID格式测试工具
/// 用于测试递增ID格式的导出和导入功能
/// </summary>
public class IncrementalIdTestWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/递增ID格式测试")]
    public static void ShowWindow()
    {
        GetWindow<IncrementalIdTestWindow>("递增ID格式测试");
    }

    private Vector2 scrollPosition;
    private string testLog = "";
    private string sampleJson = "";

    void OnGUI()
    {
        EditorGUILayout.LabelField("递增ID格式测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于测试递增ID格式的导出和导入功能", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("创建测试数据"))
        {
            CreateTestData();
        }

        if (GUILayout.Button("测试递增ID导出"))
        {
            TestIncrementalIdExport();
        }

        if (GUILayout.Button("测试递增ID导入"))
        {
            TestIncrementalIdImport();
        }

        if (GUILayout.Button("比较两种格式"))
        {
            CompareFormats();
        }

        if (GUILayout.Button("清理测试日志"))
        {
            testLog = "";
            sampleJson = "";
        }

        EditorGUILayout.Space();

        // 显示测试日志
        if (!string.IsNullOrEmpty(testLog))
        {
            EditorGUILayout.LabelField("测试日志:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(testLog, GUILayout.Height(200));
            EditorGUILayout.EndScrollView();
        }

        // 显示示例JSON
        if (!string.IsNullOrEmpty(sampleJson))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("示例JSON（递增ID格式）:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.TextArea(sampleJson, GUILayout.Height(300));
            EditorGUILayout.EndScrollView();
        }
    }

    void CreateTestData()
    {
        testLog = "=== 创建测试数据 ===\n";
        
        // 创建测试关卡数据
        var testLevel = new LevelData("测试关卡");
        
        // 创建多个层级
        var layer1 = new LayerData("主层级");
        var layer2 = new LayerData("背景层级");
        var layer3 = new LayerData("装饰层级");
        
        // 添加形状到层级1
        var shape1 = new ShapeData("圆形", new Vector2(100, 50), 0f);
        var shape2 = new ShapeData("矩形", new Vector2(200, 100), 45f);
        var shape3 = new ShapeData("三角形", new Vector2(300, 150), 90f);
        
        // 添加球到形状
        shape1.balls.Add(new BallData("红球", new Vector2(10, 5)));
        shape1.balls.Add(new BallData("蓝球", new Vector2(-5, 10)));
        shape2.balls.Add(new BallData("绿球", new Vector2(15, -5)));
        shape3.balls.Add(new BallData("红球", new Vector2(0, 0)));
        shape3.balls.Add(new BallData("蓝球", new Vector2(20, 20)));
        
        layer1.shapes.Add(shape1);
        layer1.shapes.Add(shape2);
        layer1.shapes.Add(shape3);
        
        // 添加形状到层级2
        var shape4 = new ShapeData("菱形", new Vector2(400, 200), 30f);
        shape4.balls.Add(new BallData("绿球", new Vector2(5, 5)));
        layer2.shapes.Add(shape4);
        
        // 添加形状到层级3
        var shape5 = new ShapeData("圆形", new Vector2(500, 300), 60f);
        var shape6 = new ShapeData("矩形", new Vector2(600, 400), 120f);
        shape5.balls.Add(new BallData("红球", new Vector2(-10, -10)));
        shape6.balls.Add(new BallData("蓝球", new Vector2(25, 15)));
        shape6.balls.Add(new BallData("绿球", new Vector2(-15, 25)));
        
        layer3.shapes.Add(shape5);
        layer3.shapes.Add(shape6);
        
        // 添加到关卡
        testLevel.layers.Add(layer1);
        testLevel.layers.Add(layer2);
        testLevel.layers.Add(layer3);
        
        testLog += $"创建测试关卡: {testLevel.levelName}\n";
        testLog += $"层级数量: {testLevel.layers.Count}\n";
        testLog += $"形状总数: {GetTotalShapes(testLevel)}\n";
        testLog += $"球总数: {GetTotalBalls(testLevel)}\n";
        
        // 保存测试数据到临时文件
        string tempPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level.json");
        string json = LevelDataExporter.SaveToJson(testLevel);
        File.WriteAllText(tempPath, json);
        
        testLog += $"测试数据已保存到: {tempPath}\n";
        testLog += "=== 测试数据创建完成 ===\n";
        
        Repaint();
    }

    void TestIncrementalIdExport()
    {
        testLog += "\n=== 测试递增ID导出 ===\n";
        
        // 加载测试数据
        string tempPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level.json");
        if (!File.Exists(tempPath))
        {
            testLog += "错误: 测试数据文件不存在，请先创建测试数据\n";
            Repaint();
            return;
        }
        
        string json = File.ReadAllText(tempPath);
        var testLevel = LevelDataExporter.LoadFromJson(json);
        
        if (testLevel == null)
        {
            testLog += "错误: 无法加载测试数据\n";
            Repaint();
            return;
        }
        
        // 使用递增ID格式导出
        string incrementalJson = LevelDataExporter.SaveToJsonWithIncrementalIds(testLevel);
        
        testLog += "递增ID格式导出成功\n";
        testLog += $"原始JSON长度: {json.Length}\n";
        testLog += $"递增ID JSON长度: {incrementalJson.Length}\n";
        
        // 保存递增ID格式的JSON
        string incrementalPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level_incremental.json");
        File.WriteAllText(incrementalPath, incrementalJson);
        testLog += $"递增ID格式JSON已保存到: {incrementalPath}\n";
        
        // 显示示例JSON
        sampleJson = incrementalJson;
        
        testLog += "=== 递增ID导出测试完成 ===\n";
        Repaint();
    }

    void TestIncrementalIdImport()
    {
        testLog += "\n=== 测试递增ID导入 ===\n";
        
        // 加载递增ID格式的JSON
        string incrementalPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level_incremental.json");
        if (!File.Exists(incrementalPath))
        {
            testLog += "错误: 递增ID格式文件不存在，请先进行导出测试\n";
            Repaint();
            return;
        }
        
        string incrementalJson = File.ReadAllText(incrementalPath);
        
        // 尝试导入
        var importedLevel = LevelDataExporter.LoadFromJsonWithIncrementalIds(incrementalJson);
        
        if (importedLevel != null)
        {
            testLog += "递增ID格式导入成功\n";
            testLog += $"导入关卡名称: {importedLevel.levelName}\n";
            testLog += $"导入层级数量: {importedLevel.layers.Count}\n";
            testLog += $"导入形状总数: {GetTotalShapes(importedLevel)}\n";
            testLog += $"导入球总数: {GetTotalBalls(importedLevel)}\n";
            
            // 验证数据完整性
            bool isValid = ValidateImportedData(importedLevel);
            testLog += $"数据完整性验证: {(isValid ? "通过" : "失败")}\n";
        }
        else
        {
            testLog += "错误: 递增ID格式导入失败\n";
        }
        
        testLog += "=== 递增ID导入测试完成 ===\n";
        Repaint();
    }

    void CompareFormats()
    {
        testLog += "\n=== 比较两种格式 ===\n";
        
        // 加载原始格式
        string originalPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level.json");
        string incrementalPath = Path.Combine(Application.dataPath, "SavedLevels", "test_level_incremental.json");
        
        if (!File.Exists(originalPath) || !File.Exists(incrementalPath))
        {
            testLog += "错误: 需要先创建测试数据并进行递增ID导出测试\n";
            Repaint();
            return;
        }
        
        string originalJson = File.ReadAllText(originalPath);
        string incrementalJson = File.ReadAllText(incrementalPath);
        
        // 解析两种格式
        var originalLevel = LevelDataExporter.LoadFromJson(originalJson);
        var incrementalLevel = LevelDataExporter.LoadFromJsonWithIncrementalIds(incrementalJson);
        
        if (originalLevel != null && incrementalLevel != null)
        {
            testLog += "格式比较结果:\n";
            testLog += $"原始格式文件大小: {originalJson.Length} 字符\n";
            testLog += $"递增ID格式文件大小: {incrementalJson.Length} 字符\n";
            testLog += $"大小差异: {incrementalJson.Length - originalJson.Length} 字符\n";
            
            // 比较数据内容
            bool contentMatch = CompareLevelData(originalLevel, incrementalLevel);
            testLog += $"数据内容匹配: {(contentMatch ? "是" : "否")}\n";
            
            // 显示层级名称映射
            testLog += "\n层级名称映射:\n";
            for (int i = 0; i < originalLevel.layers.Count; i++)
            {
                string originalName = originalLevel.layers[i].layerName;
                string incrementalName = incrementalLevel.layers[i].layerName;
                testLog += $"  层级{i + 1}: {originalName} -> {incrementalName}\n";
            }
            
            // 显示形状类型映射
            var originalShapeTypes = GetUniqueShapeTypes(originalLevel);
            var incrementalShapeTypes = GetUniqueShapeTypes(incrementalLevel);
            testLog += "\n形状类型映射:\n";
            for (int i = 0; i < originalShapeTypes.Count; i++)
            {
                testLog += $"  形状{i + 1}: {originalShapeTypes[i]} -> {incrementalShapeTypes[i]}\n";
            }
            
            // 显示球类型映射
            var originalBallTypes = GetUniqueBallTypes(originalLevel);
            var incrementalBallTypes = GetUniqueBallTypes(incrementalLevel);
            testLog += "\n球类型映射:\n";
            for (int i = 0; i < originalBallTypes.Count; i++)
            {
                testLog += $"  球{i + 1}: {originalBallTypes[i]} -> {incrementalBallTypes[i]}\n";
            }
        }
        else
        {
            testLog += "错误: 无法解析一种或两种格式的数据\n";
        }
        
        testLog += "=== 格式比较完成 ===\n";
        Repaint();
    }

    int GetTotalShapes(LevelData level)
    {
        int total = 0;
        foreach (var layer in level.layers)
        {
            total += layer.shapes.Count;
        }
        return total;
    }

    int GetTotalBalls(LevelData level)
    {
        int total = 0;
        foreach (var layer in level.layers)
        {
            foreach (var shape in layer.shapes)
            {
                total += shape.balls.Count;
            }
        }
        return total;
    }

    bool ValidateImportedData(LevelData level)
    {
        if (level == null || level.layers.Count == 0)
            return false;
        
        foreach (var layer in level.layers)
        {
            if (string.IsNullOrEmpty(layer.layerName))
                return false;
            
            foreach (var shape in layer.shapes)
            {
                if (string.IsNullOrEmpty(shape.shapeType))
                    return false;
                
                foreach (var ball in shape.balls)
                {
                    if (string.IsNullOrEmpty(ball.ballType))
                        return false;
                }
            }
        }
        
        return true;
    }

    bool CompareLevelData(LevelData level1, LevelData level2)
    {
        if (level1.levelName != level2.levelName)
            return false;
        
        if (level1.layers.Count != level2.layers.Count)
            return false;
        
        for (int i = 0; i < level1.layers.Count; i++)
        {
            var layer1 = level1.layers[i];
            var layer2 = level2.layers[i];
            
            if (layer1.shapes.Count != layer2.shapes.Count)
                return false;
            
            for (int j = 0; j < layer1.shapes.Count; j++)
            {
                var shape1 = layer1.shapes[j];
                var shape2 = layer2.shapes[j];
                
                if (shape1.balls.Count != shape2.balls.Count)
                    return false;
                
                if (shape1.position != shape2.position || shape1.rotation != shape2.rotation)
                    return false;
                
                for (int k = 0; k < shape1.balls.Count; k++)
                {
                    var ball1 = shape1.balls[k];
                    var ball2 = shape2.balls[k];
                    
                    if (ball1.position != ball2.position)
                        return false;
                }
            }
        }
        
        return true;
    }

    System.Collections.Generic.List<string> GetUniqueShapeTypes(LevelData level)
    {
        var types = new System.Collections.Generic.HashSet<string>();
        foreach (var layer in level.layers)
        {
            foreach (var shape in layer.shapes)
            {
                types.Add(shape.shapeType);
            }
        }
        return new System.Collections.Generic.List<string>(types);
    }

    System.Collections.Generic.List<string> GetUniqueBallTypes(LevelData level)
    {
        var types = new System.Collections.Generic.HashSet<string>();
        foreach (var layer in level.layers)
        {
            foreach (var shape in layer.shapes)
            {
                foreach (var ball in shape.balls)
                {
                    types.Add(ball.ballType);
                }
            }
        }
        return new System.Collections.Generic.List<string>(types);
    }
} 