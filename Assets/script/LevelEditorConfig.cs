using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡编辑器配置
/// 管理形状类型和球类型的自定义配置
/// </summary>
[Serializable]
public class ShapeTypeConfig
{
    public string name;
    public Sprite sprite;
    
    public ShapeTypeConfig(string typeName, Sprite typeSprite = null)
    {
        name = typeName;
        sprite = typeSprite;
    }
}

[Serializable]
public class BallTypeConfig
{
    public string name;
    public Color color;
    public Sprite sprite;
    
    public BallTypeConfig(string typeName, Color typeColor, Sprite typeSprite = null)
    {
        name = typeName;
        color = typeColor;
        sprite = typeSprite;
    }
}

/// <summary>
/// 关卡编辑器配置管理器
/// </summary>
public class LevelEditorConfig
{
    private static LevelEditorConfig instance;
    public static LevelEditorConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelEditorConfig();
                instance.InitializeDefaultConfig();
            }
            return instance;
        }
    }
    
    public List<ShapeTypeConfig> shapeTypes = new List<ShapeTypeConfig>();
    public List<BallTypeConfig> ballTypes = new List<BallTypeConfig>();
    
    /// <summary>
    /// 初始化默认配置
    /// </summary>
    public void InitializeDefaultConfig()
    {
        // 默认形状类型
        shapeTypes.Clear();
        shapeTypes.Add(new ShapeTypeConfig("圆形"));
        shapeTypes.Add(new ShapeTypeConfig("矩形"));
        shapeTypes.Add(new ShapeTypeConfig("三角形"));
        shapeTypes.Add(new ShapeTypeConfig("菱形"));
        
        // 默认球类型
        ballTypes.Clear();
        ballTypes.Add(new BallTypeConfig("红球", Color.red));
        ballTypes.Add(new BallTypeConfig("蓝球", Color.blue));
        ballTypes.Add(new BallTypeConfig("绿球", Color.green));
    }
    
    /// <summary>
    /// 获取形状类型名称数组
    /// </summary>
    public string[] GetShapeTypeNames()
    {
        string[] names = new string[shapeTypes.Count];
        for (int i = 0; i < shapeTypes.Count; i++)
        {
            names[i] = shapeTypes[i].name;
        }
        return names;
    }
    
    /// <summary>
    /// 获取球类型名称数组
    /// </summary>
    public string[] GetBallTypeNames()
    {
        string[] names = new string[ballTypes.Count];
        for (int i = 0; i < ballTypes.Count; i++)
        {
            names[i] = ballTypes[i].name;
        }
        return names;
    }
    
    /// <summary>
    /// 根据名称获取形状配置
    /// </summary>
    public ShapeTypeConfig GetShapeConfig(string name)
    {
        foreach (var config in shapeTypes)
        {
            if (config.name == name)
                return config;
        }
        return shapeTypes.Count > 0 ? shapeTypes[0] : null;
    }
    
    /// <summary>
    /// 根据名称获取球配置
    /// </summary>
    public BallTypeConfig GetBallConfig(string name)
    {
        foreach (var config in ballTypes)
        {
            if (config.name == name)
                return config;
        }
        return ballTypes.Count > 0 ? ballTypes[0] : null;
    }
    
    /// <summary>
    /// 添加形状类型
    /// </summary>
    public void AddShapeType(string name, Sprite sprite = null)
    {
        shapeTypes.Add(new ShapeTypeConfig(name, sprite));
    }
    
    /// <summary>
    /// 添加球类型
    /// </summary>
    public void AddBallType(string name, Color color, Sprite sprite = null)
    {
        ballTypes.Add(new BallTypeConfig(name, color, sprite));
    }
    
    /// <summary>
    /// 移除形状类型
    /// </summary>
    public void RemoveShapeType(string name)
    {
        shapeTypes.RemoveAll(config => config.name == name);
    }
    
    /// <summary>
    /// 移除球类型
    /// </summary>
    public void RemoveBallType(string name)
    {
        ballTypes.RemoveAll(config => config.name == name);
    }
} 