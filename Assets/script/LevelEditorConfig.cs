using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ShapeType
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class BallType
{
    public string name;
    public Color color;
    public Sprite sprite;
}

[System.Serializable]
public class BackgroundConfig
{
    public string name;
    public Sprite backgroundSprite;
    public Color backgroundColor = Color.white;
    public bool useSprite = true;
    public Vector2 spriteScale = Vector2.one;
    public Vector2 spriteOffset = Vector2.zero;
    
    // 用于JSON序列化的精灵路径
    [System.NonSerialized]
    public string spritePath;
    
    /// <summary>
    /// 获取精灵路径
    /// </summary>
    public string GetSpritePath()
    {
        if (backgroundSprite != null)
        {
            return GetAssetPath(backgroundSprite);
        }
        return spritePath;
    }
    
    /// <summary>
    /// 设置精灵路径
    /// </summary>
    public void SetSpritePath(string path)
    {
        spritePath = path;
        if (!string.IsNullOrEmpty(path))
        {
            backgroundSprite = LoadSpriteFromPath(path);
        }
    }
    
    /// <summary>
    /// 从路径加载精灵
    /// </summary>
    private Sprite LoadSpriteFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        // 尝试从Resources加载
        string resourcePath = GetResourcePath(path);
        if (!string.IsNullOrEmpty(resourcePath))
        {
            return Resources.Load<Sprite>(resourcePath);
        }
        
        // 尝试从AssetDatabase加载（仅在编辑器中）
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
        #else
        return null;
        #endif
    }
    
    /// <summary>
    /// 获取资源路径
    /// </summary>
    private string GetAssetPath(Sprite sprite)
    {
        if (sprite == null) return null;
        
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(sprite);
        #else
        return null;
        #endif
    }
    
    /// <summary>
    /// 获取Resources路径
    /// </summary>
    private string GetResourcePath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return null;
        
        // 检查是否在Resources文件夹中
        int resourcesIndex = assetPath.IndexOf("/Resources/");
        if (resourcesIndex >= 0)
        {
            string relativePath = assetPath.Substring(resourcesIndex + 11); // "/Resources/".Length
            return Path.ChangeExtension(relativePath, null); // 移除扩展名
        }
        
        return null;
    }
}

[CreateAssetMenu(fileName = "LevelEditorConfig", menuName = "LevelEditor/Config", order = 1)]
public class LevelEditorConfig : ScriptableObject
{
    private static LevelEditorConfig _instance;
    public static LevelEditorConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<LevelEditorConfig>("LevelEditorConfig");
                if (_instance == null)
                {
                    _instance = CreateInstance<LevelEditorConfig>();
                }
            }
            return _instance;
        }
    }

    public List<ShapeType> shapeTypes = new List<ShapeType>();
    public List<BallType> ballTypes = new List<BallType>();
    public List<BackgroundConfig> backgroundConfigs = new List<BackgroundConfig>();
    public int currentBackgroundIndex = 0;

    private static string ConfigDir => Path.Combine(Application.dataPath, "config");
    private static string ConfigPath => Path.Combine(ConfigDir, "level_editor_config.json");

    [System.Serializable]
    private class SerializableBackgroundConfig
    {
        public string name;
        public string spritePath;
        public Color backgroundColor;
        public bool useSprite;
        public Vector2 spriteScale;
        public Vector2 spriteOffset;
    }
    
    [System.Serializable]
    private class SerializableConfig
    {
        public List<ShapeType> shapeTypes;
        public List<BallType> ballTypes;
        public List<SerializableBackgroundConfig> backgroundConfigs;
        public int currentBackgroundIndex;
    }

    public void SaveConfigToFile()
    {
        if (!Directory.Exists(ConfigDir))
            Directory.CreateDirectory(ConfigDir);

        // 转换为可序列化的背景配置
        var serializableBackgrounds = new List<SerializableBackgroundConfig>();
        foreach (var bg in this.backgroundConfigs)
        {
            serializableBackgrounds.Add(new SerializableBackgroundConfig
            {
                name = bg.name,
                spritePath = bg.GetSpritePath(),
                backgroundColor = bg.backgroundColor,
                useSprite = bg.useSprite,
                spriteScale = bg.spriteScale,
                spriteOffset = bg.spriteOffset
            });
        }

        var data = new SerializableConfig
        {
            shapeTypes = this.shapeTypes,
            ballTypes = this.ballTypes,
            backgroundConfigs = serializableBackgrounds,
            currentBackgroundIndex = this.currentBackgroundIndex
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(ConfigPath, json);
        Debug.Log("配置已保存到: " + ConfigPath);
    }

    public void LoadConfigFromFile()
    {
        if (File.Exists(ConfigPath))
        {
            string json = File.ReadAllText(ConfigPath);
            var data = JsonUtility.FromJson<SerializableConfig>(json);
            if (data != null)
            {
                this.shapeTypes = data.shapeTypes ?? new List<ShapeType>();
                this.ballTypes = data.ballTypes ?? new List<BallType>();
                
                // 转换背景配置
                this.backgroundConfigs = new List<BackgroundConfig>();
                if (data.backgroundConfigs != null)
                {
                    foreach (var serializableBg in data.backgroundConfigs)
                    {
                        var bg = new BackgroundConfig
                        {
                            name = serializableBg.name,
                            backgroundColor = serializableBg.backgroundColor,
                            useSprite = serializableBg.useSprite,
                            spriteScale = serializableBg.spriteScale,
                            spriteOffset = serializableBg.spriteOffset
                        };
                        bg.SetSpritePath(serializableBg.spritePath);
                        this.backgroundConfigs.Add(bg);
                    }
                }
                
                this.currentBackgroundIndex = data.currentBackgroundIndex;
                Debug.Log("配置已从文件加载: " + ConfigPath);
            }
        }
        else
        {
            Debug.LogWarning("配置文件不存在: " + ConfigPath);
        }
    }

    public void InitializeDefaultConfig()
    {
        shapeTypes = new List<ShapeType>
        {
            new ShapeType { name = "圆形" },
            new ShapeType { name = "矩形" },
            new ShapeType { name = "三角形" },
            new ShapeType { name = "菱形" }
        };
        ballTypes = new List<BallType>
        {
            new BallType { name = "红球", color = Color.red },
            new BallType { name = "蓝球", color = Color.blue },
            new BallType { name = "绿球", color = Color.green }
        };
        
        backgroundConfigs = new List<BackgroundConfig>
        {
            new BackgroundConfig { name = "默认背景", backgroundColor = Color.white, useSprite = false },
            new BackgroundConfig { name = "网格背景", backgroundColor = new Color(0.9f, 0.9f, 0.9f), useSprite = false },
            new BackgroundConfig { name = "深色背景", backgroundColor = new Color(0.2f, 0.2f, 0.2f), useSprite = false }
        };
    }

    public void AddShapeType(string name)
    {
        shapeTypes.Add(new ShapeType { name = name });
    }

    public void AddBallType(string name, Color color)
    {
        ballTypes.Add(new BallType { name = name, color = color });
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
    public ShapeType GetShapeConfig(string name)
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
    public BallType GetBallConfig(string name)
    {
        foreach (var config in ballTypes)
        {
            if (config.name == name)
                return config;
        }
        return ballTypes.Count > 0 ? ballTypes[0] : null;
    }
    
    /// <summary>
    /// 添加背景配置
    /// </summary>
    public void AddBackgroundConfig(string name, Sprite sprite = null, Color? backgroundColor = null)
    {
        var config = new BackgroundConfig
        {
            name = name,
            backgroundSprite = sprite,
            backgroundColor = backgroundColor ?? Color.white,
            useSprite = sprite != null
        };
        backgroundConfigs.Add(config);
    }
    
    /// <summary>
    /// 获取背景配置名称数组
    /// </summary>
    public string[] GetBackgroundConfigNames()
    {
        string[] names = new string[backgroundConfigs.Count];
        for (int i = 0; i < backgroundConfigs.Count; i++)
        {
            names[i] = backgroundConfigs[i].name;
        }
        return names;
    }
    
    /// <summary>
    /// 根据名称获取背景配置
    /// </summary>
    public BackgroundConfig GetBackgroundConfig(string name)
    {
        return backgroundConfigs.Find(bg => bg.name == name);
    }
    
    /// <summary>
    /// 获取当前背景配置
    /// </summary>
    public BackgroundConfig GetCurrentBackground()
    {
        if (backgroundConfigs.Count == 0) return null;
        if (currentBackgroundIndex >= backgroundConfigs.Count)
            currentBackgroundIndex = 0;
        return backgroundConfigs[currentBackgroundIndex];
    }
    
    /// <summary>
    /// 设置当前背景索引
    /// </summary>
    public void SetCurrentBackground(int index)
    {
        if (index >= 0 && index < backgroundConfigs.Count)
        {
            currentBackgroundIndex = index;
        }
    }
    
    /// <summary>
    /// 刷新所有背景配置的精灵引用
    /// </summary>
    public void RefreshBackgroundSprites()
    {
        foreach (var bg in backgroundConfigs)
        {
            if (!string.IsNullOrEmpty(bg.spritePath))
            {
                bg.SetSpritePath(bg.spritePath);
            }
        }
    }
}