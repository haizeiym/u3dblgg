using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡编辑器数据管理器
/// 处理关卡数据相关的逻辑
/// </summary>
public class LevelEditorDataManager
{
    private LevelEditorUI editorUI;
    private LevelEditorUIManager uiManager;
    
    public LevelEditorDataManager(LevelEditorUI editor)
    {
        editorUI = editor;
        uiManager = new LevelEditorUIManager(editor);
        InitializeLevel();
    }
    
    void InitializeLevel()
    {
        Debug.Log("开始初始化关卡数据...");
        
        if (editorUI.currentLevel == null)
        {
            Debug.Log("创建新关卡");
            editorUI.currentLevel = new LevelData("新关卡");
        }
        
        // 确保至少有一个层级
        if (editorUI.currentLevel.layers.Count == 0)
        {
            Debug.Log("创建默认层级");
            editorUI.currentLayer = new LayerData("默认层级");
            editorUI.currentLevel.layers.Add(editorUI.currentLayer);
        }
        else if (editorUI.currentLayer == null)
        {
            // 如果层级列表不为空但当前层级为null，选择第一个层级
            Debug.Log($"选择第一个层级: {editorUI.currentLevel.layers[0].layerName}");
            editorUI.currentLayer = editorUI.currentLevel.layers[0];
        }
        
        Debug.Log($"初始化完成 - 关卡: {editorUI.currentLevel.levelName}, 层级数量: {editorUI.currentLevel.layers.Count}, 当前层级: {editorUI.currentLayer?.layerName}");
    }
    
    public void AddLayer()
    {
        LayerData newLayer = new LayerData($"层级{editorUI.currentLevel.layers.Count + 1}");
        editorUI.currentLevel.layers.Add(newLayer);
        editorUI.currentLayer = newLayer;
        editorUI.RefreshUI();
        ClearEditArea();
    }
    
    public void DeleteLayer()
    {
        if (editorUI.currentLevel.layers.Count > 1)
        {
            editorUI.currentLevel.layers.Remove(editorUI.currentLayer);
            editorUI.currentLayer = editorUI.currentLevel.layers[0];
            editorUI.RefreshUI();
        }
    }
    
    public void AddShape()
    {
        // 强制加载最新配置
        LevelEditorConfig.Instance.LoadConfigFromFile();
        
        Debug.Log($"开始添加形状，当前层级: {editorUI.currentLayer?.layerName}");
        
        if (editorUI.currentLayer != null)
        {
            string[] types = LevelEditorConfig.Instance.GetShapeTypeNames();
            if (editorUI.currentShapeTypeIndex >= 0 && editorUI.currentShapeTypeIndex < types.Length)
            {
                string shapeType = types[editorUI.currentShapeTypeIndex];
                ShapeData newShape = new ShapeData(shapeType, Vector2.zero, 0f);
                editorUI.currentLayer.shapes.Add(newShape);
                
                Debug.Log($"形状已添加到层级 {editorUI.currentLayer.layerName}，当前形状数量: {editorUI.currentLayer.shapes.Count}");
                
                // 立即验证数据
                Debug.Log($"=== 添加形状后验证 ===");
                Debug.Log($"当前层级形状数量: {editorUI.currentLayer.shapes.Count}");
                Debug.Log($"最新添加的形状: {editorUI.currentLayer.shapes[editorUI.currentLayer.shapes.Count - 1].shapeType}");
                Debug.Log($"=== 验证结束 ===");
                
                GameObject newShapeObj = uiManager.CreateShapeObject(newShape);
                
                // 自动选中新创建的形状
                if (newShapeObj != null)
                {
                    ShapeController controller = newShapeObj.GetComponent<ShapeController>();
                    if (controller != null)
                    {
                        editorUI.SelectShape(controller);
                        Debug.Log($"成功创建形状: {shapeType}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"形状类型索引超出范围: {editorUI.currentShapeTypeIndex}");
            }
        }
        else
        {
            Debug.LogWarning("当前没有可用的层级");
        }
    }
    
    public void AddBall()
    {
        // 强制加载最新配置
        LevelEditorConfig.Instance.LoadConfigFromFile();
        
        if (editorUI.selectedShape != null)
        {
            ShapeData shapeData = editorUI.selectedShape.GetShapeData();
            if (shapeData != null)
            {
                BallData newBall = new BallData("红球", Vector2.zero);
                shapeData.balls.Add(newBall);
                
                // 将球创建在选中的形状内部
                GameObject ballObj = uiManager.CreateBallObject(newBall);
                if (ballObj != null)
                {
                    // 将球设置为形状的子对象
                    ballObj.transform.SetParent(editorUI.selectedShape.transform, false);
                    
                    Debug.Log($"成功添加球到形状: {shapeData.shapeType}");
                }
                else
                {
                    Debug.LogError("创建球对象失败");
                }
            }
        }
        else
        {
            Debug.LogWarning("请先选择一个形状，然后点击添加球");
        }
    }
    
    public void ExportLevel()
    {
        Debug.Log($"开始导出关卡，当前层级数量: {editorUI.currentLevel.layers.Count}");
        
        // 验证数据完整性
        ValidateDataBeforeExport();
        
        // 创建导出用的LevelData副本，包含所有层级
        LevelData exportData = new LevelData(editorUI.currentLevel.levelName);
        
        foreach (var layer in editorUI.currentLevel.layers)
        {
            Debug.Log($"检查层级: {layer.layerName}, 形状数量: {layer.shapes.Count}");
            
            // 复制层级数据（包括默认层级）
            LayerData newLayer = new LayerData(layer.layerName);
            foreach (var shape in layer.shapes)
            {
                Debug.Log($"复制形状: {shape.shapeType}, 位置: {shape.position}");
                ShapeData newShape = new ShapeData(shape.shapeType, shape.position, shape.rotation);
                foreach (var ball in shape.balls)
                {
                    newShape.balls.Add(new BallData(ball.ballType, ball.position));
                }
                newLayer.shapes.Add(newShape);
            }
            exportData.layers.Add(newLayer);
            Debug.Log($"添加层级到导出数据: {layer.layerName}, 导出形状数量: {newLayer.shapes.Count}");
        }
        
        string json = LevelDataExporter.SaveToJson(exportData);
        Debug.Log("导出的JSON数据：\n" + json);
        
        // 确保SavedLevels目录存在
        string savedLevelsPath = Application.dataPath + "/SavedLevels";
        if (!System.IO.Directory.Exists(savedLevelsPath))
        {
            System.IO.Directory.CreateDirectory(savedLevelsPath);
        }
        
        // 生成文件名（使用时间戳避免重复）
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Level_{timestamp}.json";
        string filePath = System.IO.Path.Combine(savedLevelsPath, fileName);
        
        // 保存到文件
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log($"关卡数据已保存到: {filePath}");
        
        // 复制到剪贴板
        GUIUtility.systemCopyBuffer = json;
        Debug.Log("关卡数据已复制到剪贴板");
    }
    
    public void ImportLevel()
    {
        // 确保SavedLevels目录存在
        string savedLevelsPath = Application.dataPath + "/SavedLevels";
        if (!System.IO.Directory.Exists(savedLevelsPath))
        {
            Debug.LogWarning("SavedLevels目录不存在，无法导入关卡");
            return;
        }
        
        // 获取所有JSON文件
        string[] jsonFiles = System.IO.Directory.GetFiles(savedLevelsPath, "*.json");
        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning("SavedLevels目录中没有找到JSON文件");
            return;
        }
        
        // 选择最新的文件（按修改时间排序）
        System.Array.Sort(jsonFiles, (a, b) => 
            System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a)));
        
        string latestFile = jsonFiles[0];
        string jsonContent = System.IO.File.ReadAllText(latestFile);
        
        try
        {
            LevelData importedLevel = LevelDataExporter.LoadFromJson(jsonContent);
            if (importedLevel != null)
            {
                editorUI.currentLevel = importedLevel;
                if (importedLevel.layers.Count > 0)
                {
                    editorUI.currentLayer = importedLevel.layers[0];
                }
                else
                {
                    // 如果没有层级，创建默认层级
                    editorUI.currentLayer = new LayerData("默认层级");
                    editorUI.currentLevel.layers.Add(editorUI.currentLayer);
                }
                
                editorUI.RefreshUI();
                Debug.Log($"成功导入关卡: {System.IO.Path.GetFileName(latestFile)}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"导入关卡失败: {e.Message}");
        }
    }
    
    void ClearEditArea()
    {
        // 清除编辑区域的所有对象
        if (editorUI.editAreaContent)
        {
            foreach (Transform child in editorUI.editAreaContent)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
    
    void ValidateDataBeforeExport()
    {
        Debug.Log("=== 数据验证开始 ===");
        Debug.Log($"关卡名称: {editorUI.currentLevel?.levelName}");
        Debug.Log($"当前层级: {editorUI.currentLayer?.layerName}");
        Debug.Log($"层级总数: {editorUI.currentLevel?.layers.Count}");
        
        if (editorUI.currentLevel != null)
        {
            for (int i = 0; i < editorUI.currentLevel.layers.Count; i++)
            {
                var layer = editorUI.currentLevel.layers[i];
                Debug.Log($"层级[{i}]: {layer.layerName}, 形状数量: {layer.shapes.Count}");
                
                for (int j = 0; j < layer.shapes.Count; j++)
                {
                    var shape = layer.shapes[j];
                    Debug.Log($"  形状[{j}]: {shape.shapeType}, 位置: {shape.position}, 球数量: {shape.balls.Count}");
                }
            }
        }
        Debug.Log("=== 数据验证结束 ===");
    }
} 