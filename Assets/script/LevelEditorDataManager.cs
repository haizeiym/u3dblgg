using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 关卡编辑器数据管理器
/// 处理关卡数据相关的逻辑
/// </summary>
public class LevelEditorDataManager
{
    private LevelEditorUI editorUI;
    private LevelEditorUIManager uiManager;
    private string currentLevelFilePath; // 记录当前关卡的文件路径
    
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
        // 将所有现有层级置灰
        foreach (var layer in editorUI.currentLevel.layers)
        {
            layer.isActive = false;
        }
        
        // 创建新层级并插入到顶部
        LayerData newLayer = new LayerData($"层级{editorUI.currentLevel.layers.Count + 1}");
        editorUI.currentLevel.layers.Insert(0, newLayer); // 插入到列表开头
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
    
    public void CreateLevel()
    {
        Debug.Log("开始创建新关卡");
        
        // 从配置中获取并增加关卡索引
        int newIndex = LevelEditorConfig.Instance.IncrementLevelIndex();
        
        // 创建新关卡名称
        string newLevelName = $"LevelConfig_{newIndex}";
        LevelData newLevel = new LevelData(newLevelName);
        
        // 创建默认层级
        LayerData defaultLayer = new LayerData("默认层级");
        newLevel.layers.Add(defaultLayer);
        
        // 更新当前关卡和层级
        editorUI.currentLevel = newLevel;
        editorUI.currentLayer = defaultLayer;
        
        // 清除当前文件路径（新关卡未保存）
        currentLevelFilePath = "";
        
        // 更新UI显示
        UpdateLevelNameInUI(newLevelName);
        
        // 清空编辑区
        ClearEditArea();
        
        // 刷新UI
        editorUI.RefreshUI();
        
        Debug.Log($"新关卡已创建: {newLevelName} (索引: {newIndex})");
    }
    
    public void UpdateLevelName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            Debug.LogWarning("关卡名称不能为空");
            return;
        }
        
        if (editorUI.currentLevel != null)
        {
            string oldName = editorUI.currentLevel.levelName;
            editorUI.currentLevel.levelName = newName;
            Debug.Log($"关卡名称已更新: {oldName} -> {newName}");
        }
        else
        {
            Debug.LogWarning("当前没有关卡数据，无法更新名称");
        }
    }
    
    /// <summary>
    /// 更新UI中的关卡名称显示
    /// </summary>
    void UpdateLevelNameInUI(string levelName)
    {
        if (editorUI.levelNameInput != null)
        {
            editorUI.levelNameInput.text = levelName;
        }
    }
    
    public void AddShape()
    {
        Debug.Log($"开始添加形状，当前层级: {editorUI.currentLayer?.layerName}");
        
        // 检查层级状态
        if (editorUI.currentLayer == null)
        {
            Debug.LogWarning("当前没有可用的层级");
            return;
        }
        
        if (!editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法添加图形：当前层级未激活，请先激活该层级");
            return;
        }
        
        // 检查形状类型配置（不强制重新加载，避免循环）
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空");
            return;
        }
        
        string[] types = config.GetShapeTypeNames();
        if (types.Length == 0)
        {
            Debug.LogWarning("没有配置的形状类型！请先配置形状类型。");
            // 不显示对话框，避免UI卡死
            Debug.LogError("请打开 Tools/Level Editor/配置编辑器 添加形状类型");
            return;
        }
        
        // 检查形状类型索引
        if (editorUI.currentShapeTypeIndex < 0 || editorUI.currentShapeTypeIndex >= types.Length)
        {
            Debug.LogWarning($"形状类型索引超出范围: {editorUI.currentShapeTypeIndex}，重置为0");
            editorUI.currentShapeTypeIndex = 0;
        }
        
        string shapeType = types[editorUI.currentShapeTypeIndex];
        
        // 验证配置中的形状类型
        ShapeType shapeConfig = LevelEditorConfig.Instance.GetShapeConfig(shapeType);
        if (shapeConfig == null)
        {
            Debug.LogWarning($"配置中未找到形状类型: {shapeType}");
            return;
        }
        
        try
        {
            ShapeData newShape = new ShapeData(shapeType, Vector2.zero, 0f);
            editorUI.currentLayer.shapes.Add(newShape);
            
            Debug.Log($"形状已添加到层级 {editorUI.currentLayer.layerName}，当前形状数量: {editorUI.currentLayer.shapes.Count}");
            Debug.Log($"使用配置形状: {shapeType} (配置名称: {shapeConfig.name})");
            
            // 立即验证数据
            Debug.Log($"=== 添加形状后验证 ===");
            Debug.Log($"当前层级形状数量: {editorUI.currentLayer.shapes.Count}");
            Debug.Log($"最新添加的形状: {editorUI.currentLayer.shapes[editorUI.currentLayer.shapes.Count - 1].shapeType}");
            Debug.Log($"=== 验证结束 ===");
            
            // 检查uiManager
            if (uiManager == null)
            {
                Debug.LogError("uiManager为空，无法创建形状对象");
                return;
            }
            
            // 检查必要的组件
            if (editorUI.shapePrefab == null)
            {
                Debug.LogError("shapePrefab为空，无法创建形状对象");
                return;
            }
            
            if (editorUI.editAreaContent == null)
            {
                Debug.LogError("editAreaContent为空，无法创建形状对象");
                return;
            }
            
            GameObject newShapeObj = uiManager.CreateShapeObject(newShape);
            
            // 自动选中新创建的形状
            if (newShapeObj != null)
            {
                ShapeController controller = newShapeObj.GetComponent<ShapeController>();
                if (controller != null)
                {
                    editorUI.SelectShape(controller);
                    Debug.Log($"成功创建形状: {shapeType}，使用配置设置");
                }
            }
            else
            {
                Debug.LogError("创建形状对象失败");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"添加形状时发生错误: {e.Message}");
            Debug.LogError($"错误详情: {e.StackTrace}");
        }
    }
    
    public void AddBall()
    {
        Debug.Log($"开始添加球，当前层级: {editorUI.currentLayer?.layerName}");
        
        // 检查层级状态
        if (editorUI.currentLayer == null)
        {
            Debug.LogWarning("当前没有可用的层级");
            return;
        }
        
        if (!editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法添加球：当前层级未激活，请先激活该层级");
            return;
        }
        
        // 检查是否有选中的形状
        if (editorUI.selectedShape == null)
        {
            Debug.LogWarning("无法添加球：请先选中一个形状");
            return;
        }
        
        // 检查球类型配置
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("配置实例为空");
            return;
        }
        
        if (config.ballTypes.Count == 0)
        {
            ShowConfigWarning("球类型", "请先在配置编辑器中添加球类型");
            return;
        }
        
        // 获取当前选中的球类型
        int ballTypeIndex = editorUI.currentBallTypeIndex;
        if (ballTypeIndex >= config.ballTypes.Count)
        {
            ballTypeIndex = 0; // 默认使用第一个球类型
        }
        
        var ballType = config.ballTypes[ballTypeIndex];
        Debug.Log($"使用球类型: {ballType.name} (索引: {ballTypeIndex})");
        
        // 创建球数据
        BallData newBall = new BallData(ballType.name, Vector2.zero);
        
        // 添加到选中的形状中
        ShapeData selectedShapeData = editorUI.selectedShape.ShapeData;
        selectedShapeData.balls.Add(newBall);
        
        Debug.Log($"球已添加到形状 '{selectedShapeData.shapeType}' 中，球数量: {selectedShapeData.balls.Count}");
        
        // 刷新UI
        editorUI.RefreshUI();
    }
    
    public void DeleteShape()
    {
        Debug.Log("开始删除形状");
        
        if (editorUI.selectedShape == null)
        {
            Debug.LogWarning("没有选中的形状，无法删除");
            return;
        }
        
        // 检查层级状态
        if (editorUI.currentLayer == null)
        {
            Debug.LogWarning("当前没有可用的层级");
            return;
        }
        
        if (!editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法删除形状：当前层级未激活，请先激活该层级");
            return;
        }
        
        // 获取选中的形状数据
        ShapeData shapeToDelete = editorUI.selectedShape.ShapeData;
        
        // 从当前层级中删除形状（包括其关联的所有球）
        bool removed = editorUI.currentLayer.shapes.Remove(shapeToDelete);
        
        if (removed)
        {
            Debug.Log($"形状 '{shapeToDelete.shapeType}' 及其关联的 {shapeToDelete.balls.Count} 个球已删除");
            
            // 清除选中状态
            editorUI.selectedShape = null;
            editorUI.selectedBall = null;
            
            // 刷新UI
            editorUI.RefreshUI();
        }
        else
        {
            Debug.LogError("删除形状失败：未找到要删除的形状");
        }
    }
    
    public void DeleteBall()
    {
        Debug.Log("开始删除球");
        
        if (editorUI.selectedBall == null)
        {
            Debug.LogWarning("没有选中的球，无法删除");
            return;
        }
        
        // 检查层级状态
        if (editorUI.currentLayer == null)
        {
            Debug.LogWarning("当前没有可用的层级");
            return;
        }
        
        if (!editorUI.currentLayer.isActive)
        {
            Debug.LogWarning("无法删除球：当前层级未激活，请先激活该层级");
            return;
        }
        
        // 获取选中的球数据
        BallData ballToDelete = editorUI.selectedBall.BallData;
        
        // 查找包含该球的形状
        ShapeData parentShape = null;
        foreach (var shape in editorUI.currentLayer.shapes)
        {
            if (shape.balls.Contains(ballToDelete))
            {
                parentShape = shape;
                break;
            }
        }
        
        if (parentShape != null)
        {
            // 从形状中删除球
            bool removed = parentShape.balls.Remove(ballToDelete);
            
            if (removed)
            {
                Debug.Log($"球已从形状 '{parentShape.shapeType}' 中删除");
                
                // 清除选中状态
                editorUI.selectedBall = null;
                
                // 刷新UI
                editorUI.RefreshUI();
            }
            else
            {
                Debug.LogError("删除球失败：未找到要删除的球");
            }
        }
        else
        {
            Debug.LogError("删除球失败：未找到包含该球的形状");
        }
    }
    
    /// <summary>
    /// 显示配置警告
    /// </summary>
    void ShowConfigWarning(string configType, string message)
    {
        Debug.LogWarning($"[{configType}] {message}");
        
        // 在编辑器中显示对话框
        #if UNITY_EDITOR
        EditorUtility.DisplayDialog(
            $"{configType}配置缺失", 
            $"{message}\n\n点击确定打开配置编辑器", 
            "确定"
        );
        
        // 使用反射打开配置编辑器
        var configWindowType = System.Type.GetType("LevelEditorConfigWindow, Assembly-CSharp-Editor");
        if (configWindowType != null)
        {
            EditorWindow.GetWindow(configWindowType, false, "关卡编辑器配置");
        }
        else
        {
            Debug.LogError("无法找到 LevelEditorConfigWindow 类型");
        }
        #endif
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
        
        // 使用递增ID格式导出
        string json = LevelDataExporter.SaveToJsonWithIncrementalIds(exportData);
        Debug.Log("导出的JSON数据（递增ID格式）：\n" + json);
        
        // 确保SavedLevels目录存在
        string savedLevelsPath = Application.dataPath + "/SavedLevels";
        if (!System.IO.Directory.Exists(savedLevelsPath))
        {
            System.IO.Directory.CreateDirectory(savedLevelsPath);
        }
        
        string filePath;
        
        // 如果有当前关卡文件路径，则覆盖原文件；否则创建新文件
        if (!string.IsNullOrEmpty(currentLevelFilePath) && System.IO.File.Exists(currentLevelFilePath))
        {
            filePath = currentLevelFilePath;
            Debug.Log($"覆盖原文件: {filePath}");
        }
        else
        {
            // 生成文件名（使用时间戳避免重复）
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Level_{timestamp}.json";
            filePath = System.IO.Path.Combine(savedLevelsPath, fileName);
            currentLevelFilePath = filePath; // 记录新文件路径
            Debug.Log($"创建新文件: {filePath}");
        }
        
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
            // 尝试使用递增ID格式导入
            LevelData importedLevel = LevelDataExporter.LoadFromJsonWithIncrementalIds(jsonContent);
            if (importedLevel != null)
            {
                editorUI.currentLevel = importedLevel;
                currentLevelFilePath = latestFile; // 记录文件路径
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
                Debug.Log($"成功导入关卡（递增ID格式）: {System.IO.Path.GetFileName(latestFile)}");
            }
            else
            {
                // 如果递增ID格式导入失败，尝试使用原始格式
                Debug.Log("递增ID格式导入失败，尝试使用原始格式导入");
                importedLevel = LevelDataExporter.LoadFromJson(jsonContent);
                if (importedLevel != null)
                {
                    editorUI.currentLevel = importedLevel;
                    currentLevelFilePath = latestFile;
                    if (importedLevel.layers.Count > 0)
                    {
                        editorUI.currentLayer = importedLevel.layers[0];
                    }
                    else
                    {
                        editorUI.currentLayer = new LayerData("默认层级");
                        editorUI.currentLevel.layers.Add(editorUI.currentLayer);
                    }
                    
                    editorUI.RefreshUI();
                    Debug.Log($"成功导入关卡（原始格式）: {System.IO.Path.GetFileName(latestFile)}");
                }
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