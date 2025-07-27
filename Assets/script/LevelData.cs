using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string levelName;
    public List<LayerData> layers = new List<LayerData>();

    public LevelData(string name)
    {
        levelName = name;
    }
}

[Serializable]
public class LayerData
{
    public string layerName;
    public List<ShapeData> shapes = new List<ShapeData>();
    public bool isVisible = true; // 层级是否可见
    public bool isActive = true;  // 层级是否激活（非置灰状态）

    public LayerData(string name)
    {
        layerName = name;
        isVisible = true;
        isActive = true;
    }
}

[Serializable]
public class ShapeData
{
    public string shapeType;
    public Vector2 position;
    public float rotation;
    public List<BallData> balls = new List<BallData>();
    public List<Vector2> fixedPositions = new List<Vector2>(); // 新增：固定位置列表

    public ShapeData(string type, Vector2 pos, float rot)
    {
        shapeType = type;
        position = pos;
        rotation = rot;
        fixedPositions = new List<Vector2>();
    }
    
    /// <summary>
    /// 检查是否有固定位置配置
    /// </summary>
    public bool HasFixedPositions()
    {
        return fixedPositions != null && fixedPositions.Count > 0;
    }
    
    /// <summary>
    /// 获取最近的固定位置
    /// </summary>
    public Vector2 GetNearestFixedPosition(Vector2 targetPosition)
    {
        if (!HasFixedPositions())
            return targetPosition;
            
        Vector2 nearest = fixedPositions[0];
        float minDistance = Vector2.Distance(targetPosition, nearest);
        
        for (int i = 1; i < fixedPositions.Count; i++)
        {
            float distance = Vector2.Distance(targetPosition, fixedPositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = fixedPositions[i];
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// 添加固定位置
    /// </summary>
    public void AddFixedPosition(Vector2 position)
    {
        if (fixedPositions == null)
            fixedPositions = new List<Vector2>();
            
        fixedPositions.Add(position);
    }
    
    /// <summary>
    /// 清除所有固定位置
    /// </summary>
    public void ClearFixedPositions()
    {
        if (fixedPositions != null)
            fixedPositions.Clear();
    }
    
    /// <summary>
    /// 从配置文件加载固定位置
    /// </summary>
    public void LoadFixedPositionsFromConfig()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            var fixedPosConfig = config.GetFixedPositionConfig(shapeType);
            if (fixedPosConfig != null)
            {
                fixedPositions.Clear();
                fixedPositions.AddRange(fixedPosConfig.fixedPositions);
                Debug.Log($"已从配置文件加载形状 '{shapeType}' 的固定位置，数量: {fixedPositions.Count}");
            }
            else
            {
                Debug.Log($"配置文件中未找到形状 '{shapeType}' 的固定位置配置");
            }
        }
    }
    
    /// <summary>
    /// 保存固定位置到配置文件
    /// </summary>
    public void SaveFixedPositionsToConfig()
    {
        var config = LevelEditorConfig.Instance;
        if (config != null)
        {
            config.SetFixedPositionConfig(shapeType, new List<Vector2>(fixedPositions));
            Debug.Log($"已保存形状 '{shapeType}' 的固定位置到配置文件，数量: {fixedPositions.Count}");
        }
    }
}

[Serializable]
public class BallData
{
    public string ballType;
    public Vector2 position;

    public BallData(string type, Vector2 pos)
    {
        ballType = type;
        position = pos;
    }
}

public static class LevelDataExporter
{
    public static string SaveToJson(LevelData data)
    {
        // 构造导出对象，确保字段完全一致
        var exportObj = new ExportLevelData(data);
        return JsonUtility.ToJson(exportObj, true);
    }

    public static LevelData LoadFromJson(string json)
    {
        return JsonUtility.FromJson<LevelData>(json);
    }
    
    /// <summary>
    /// 导出时转换为递增ID格式
    /// </summary>
    public static string SaveToJsonWithIncrementalIds(LevelData data)
    {
        var exportObj = new ExportLevelDataWithIds(data);
        return JsonUtility.ToJson(exportObj, true);
    }
    
    /// <summary>
    /// 从递增ID格式导入
    /// </summary>
    public static LevelData LoadFromJsonWithIncrementalIds(string json)
    {
        var exportData = JsonUtility.FromJson<ExportLevelDataWithIds>(json);
        if (exportData == null) return null;
        
        return exportData.ToLevelData();
    }
}

[Serializable]
public class ExportLevelData
{
    public string levelName;
    [SerializeField]
    public List<ExportLayerData> layers;
    public ExportLevelData(LevelData src)
    {
        levelName = src.levelName;
        layers = new List<ExportLayerData>();
        foreach (var l in src.layers)
        {
            layers.Add(new ExportLayerData(l));
        }
    }
}
[Serializable]
public class ExportLayerData
{
    public string layerName;
    [SerializeField]
    public List<ExportShapeData> shapes;
    public ExportLayerData(LayerData src)
    {
        layerName = src.layerName;
        shapes = new List<ExportShapeData>();
        foreach (var s in src.shapes)
        {
            shapes.Add(new ExportShapeData(s));
        }
    }
}
[Serializable]
public class ExportShapeData
{
    public string shapeType;
    public ExportVec2 position;
    public float rotation;
    [SerializeField]
    public List<ExportBallData> balls;
    [SerializeField]
    public List<ExportVec2> fixedPositions; // 新增：固定位置列表
    
    public ExportShapeData(ShapeData src)
    {
        shapeType = src.shapeType;
        position = new ExportVec2(src.position);
        rotation = src.rotation;
        balls = new List<ExportBallData>();
        foreach (var b in src.balls)
        {
            balls.Add(new ExportBallData(b));
        }
        
        // 序列化固定位置
        fixedPositions = new List<ExportVec2>();
        if (src.fixedPositions != null)
        {
            foreach (var pos in src.fixedPositions)
            {
                fixedPositions.Add(new ExportVec2(pos));
            }
        }
    }
}
[Serializable]
public class ExportBallData
{
    public string ballType;
    public ExportVec2 position;
    public ExportBallData(BallData src)
    {
        ballType = src.ballType;
        position = new ExportVec2(src.position);
    }
}
[Serializable]
public class ExportVec2
{
    public float x;
    public float y;
    public ExportVec2(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
}

/// <summary>
/// 支持递增ID的导出数据结构
/// </summary>
[Serializable]
public class ExportLevelDataWithIds
{
    public string levelName;
    public List<ExportLayerDataWithIds> layers;
    public List<string> layerNames; // 层级名称映射表
    public List<string> shapeTypes; // 形状类型映射表
    public List<string> ballTypes;  // 球类型映射表
    
    public ExportLevelDataWithIds(LevelData src)
    {
        levelName = src.levelName;
        layers = new List<ExportLayerDataWithIds>();
        layerNames = new List<string>();
        shapeTypes = new List<string>();
        ballTypes = new List<string>();
        
        // 收集所有唯一的名称
        var uniqueLayerNames = new HashSet<string>();
        var uniqueShapeTypes = new HashSet<string>();
        var uniqueBallTypes = new HashSet<string>();
        
        foreach (var layer in src.layers)
        {
            uniqueLayerNames.Add(layer.layerName);
            foreach (var shape in layer.shapes)
            {
                uniqueShapeTypes.Add(shape.shapeType);
                foreach (var ball in shape.balls)
                {
                    uniqueBallTypes.Add(ball.ballType);
                }
            }
        }
        
        // 转换为有序列表
        layerNames.AddRange(uniqueLayerNames);
        shapeTypes.AddRange(uniqueShapeTypes);
        ballTypes.AddRange(uniqueBallTypes);
        
        // 创建层级数据
        foreach (var layer in src.layers)
        {
            layers.Add(new ExportLayerDataWithIds(layer, this));
        }
    }
    
    /// <summary>
    /// 转换为LevelData
    /// </summary>
    public LevelData ToLevelData()
    {
        var levelData = new LevelData(levelName);
        
        foreach (var layer in layers)
        {
            levelData.layers.Add(layer.ToLayerData(this));
        }
        
        return levelData;
    }
}

[Serializable]
public class ExportLayerDataWithIds
{
    public int layerNameId; // 层级名称ID（从1开始）
    public List<ExportShapeDataWithIds> shapes;
    
    public ExportLayerDataWithIds(LayerData src, ExportLevelDataWithIds parent)
    {
        // 找到层级名称的ID（从1开始）
        layerNameId = parent.layerNames.IndexOf(src.layerName) + 1;
        if (layerNameId == 0) layerNameId = 1; // 如果没找到，默认为1
        
        shapes = new List<ExportShapeDataWithIds>();
        foreach (var shape in src.shapes)
        {
            shapes.Add(new ExportShapeDataWithIds(shape, parent));
        }
    }
    
    public LayerData ToLayerData(ExportLevelDataWithIds parent)
    {
        // 从ID获取层级名称
        string layerName = layerNameId > 0 && layerNameId <= parent.layerNames.Count 
            ? parent.layerNames[layerNameId - 1] 
            : "层级" + layerNameId;
            
        var layerData = new LayerData(layerName);
        
        foreach (var shape in shapes)
        {
            layerData.shapes.Add(shape.ToShapeData(parent));
        }
        
        return layerData;
    }
}

[Serializable]
public class ExportShapeDataWithIds
{
    public int shapeTypeId; // 形状类型ID（从1开始）
    public ExportVec2 position;
    public float rotation;
    public List<ExportBallDataWithIds> balls;
    public List<ExportVec2> fixedPositions; // 新增：固定位置列表
    
    public ExportShapeDataWithIds(ShapeData src, ExportLevelDataWithIds parent)
    {
        // 找到形状类型的ID（从1开始）
        shapeTypeId = parent.shapeTypes.IndexOf(src.shapeType) + 1;
        if (shapeTypeId == 0) shapeTypeId = 1; // 如果没找到，默认为1
        
        position = new ExportVec2(src.position);
        rotation = src.rotation;
        balls = new List<ExportBallDataWithIds>();
        
        foreach (var ball in src.balls)
        {
            balls.Add(new ExportBallDataWithIds(ball, parent));
        }
        
        // 序列化固定位置
        fixedPositions = new List<ExportVec2>();
        if (src.fixedPositions != null)
        {
            foreach (var pos in src.fixedPositions)
            {
                fixedPositions.Add(new ExportVec2(pos));
            }
        }
    }
    
    public ShapeData ToShapeData(ExportLevelDataWithIds parent)
    {
        // 从ID获取形状类型名称
        string shapeType = shapeTypeId > 0 && shapeTypeId <= parent.shapeTypes.Count 
            ? parent.shapeTypes[shapeTypeId - 1] 
            : "形状" + shapeTypeId;
            
        var shapeData = new ShapeData(shapeType, new Vector2(position.x, position.y), rotation);
        
        foreach (var ball in balls)
        {
            shapeData.balls.Add(ball.ToBallData(parent));
        }
        
        // 反序列化固定位置
        if (fixedPositions != null)
        {
            foreach (var pos in fixedPositions)
            {
                shapeData.AddFixedPosition(new Vector2(pos.x, pos.y));
            }
        }
        
        return shapeData;
    }
}

[Serializable]
public class ExportBallDataWithIds
{
    public int ballTypeId; // 球类型ID（从1开始）
    public ExportVec2 position;
    
    public ExportBallDataWithIds(BallData src, ExportLevelDataWithIds parent)
    {
        // 找到球类型的ID（从1开始）
        ballTypeId = parent.ballTypes.IndexOf(src.ballType) + 1;
        if (ballTypeId == 0) ballTypeId = 1; // 如果没找到，默认为1
        
        position = new ExportVec2(src.position);
    }
    
    public BallData ToBallData(ExportLevelDataWithIds parent)
    {
        // 从ID获取球类型名称
        string ballType = ballTypeId > 0 && ballTypeId <= parent.ballTypes.Count 
            ? parent.ballTypes[ballTypeId - 1] 
            : "球" + ballTypeId;
            
        return new BallData(ballType, new Vector2(position.x, position.y));
    }
} 