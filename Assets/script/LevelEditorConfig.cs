using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

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
            // 使用安全的加载方法，避免卡死
            try
            {
                backgroundSprite = LoadSpriteFromPath(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"BackgroundConfig.SetSpritePath异常: {path}, 错误: {e.Message}");
                backgroundSprite = null;
            }
        }
    }
    
    /// <summary>
    /// 从路径加载精灵
    /// </summary>
    private Sprite LoadSpriteFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        try
        {
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
        catch (System.Exception e)
        {
            Debug.LogError($"BackgroundConfig.LoadSpriteFromPath异常: {path}, 错误: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 获取资源路径
    /// </summary>
    private string GetResourcePath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return null;
        
        // 查找Resources文件夹
        int resourcesIndex = assetPath.IndexOf("/Resources/");
        if (resourcesIndex >= 0)
        {
            // 提取Resources后的路径，并移除扩展名
            string resourcePath = assetPath.Substring(resourcesIndex + 11);
            return Path.ChangeExtension(resourcePath, null);
        }
        
        return null;
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
}

/// <summary>
/// 固定位置配置
/// </summary>
[System.Serializable]
public class FixedPositionConfig
{
    public string shapeType;
    public List<Vector2> fixedPositions = new List<Vector2>();
    
    public FixedPositionConfig(string type)
    {
        shapeType = type;
        fixedPositions = new List<Vector2>();
    }
    
    public FixedPositionConfig(string type, List<Vector2> positions)
    {
        shapeType = type;
        fixedPositions = new List<Vector2>(positions);
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
                    // 新创建的实例先尝试从文件加载配置，避免重置levelIndex
                    try
                    {
                        _instance.LoadConfigFromFile();
                        Debug.Log("新创建的配置实例已从文件加载");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"从文件加载配置失败，使用默认配置: {e.Message}");
                        // 只有在加载失败时才初始化默认配置
                        _instance.InitializeDefaultConfig();
                    }
                }
            }
            return _instance;
        }
    }

    public List<ShapeType> shapeTypes = new List<ShapeType>();
    public List<BallType> ballTypes = new List<BallType>();
    public List<BackgroundConfig> backgroundConfigs = new List<BackgroundConfig>();
    public List<FixedPositionConfig> fixedPositionConfigs = new List<FixedPositionConfig>(); // 新增：固定位置配置
    public int currentBackgroundIndex = 0;
    public int levelIndex = 1; // 新增：关卡索引，初始值为1
    
    // 配置变更事件
    public event Action OnShapeTypesChanged;
    public event Action OnBallTypesChanged;
    public event Action OnBackgroundConfigsChanged;
    public event Action OnCurrentBackgroundChanged;
    public event Action OnConfigReloaded;

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
    private class SerializableShapeType
    {
        public string name;
        public string spritePath;
    }
    
    [System.Serializable]
    private class SerializableBallType
    {
        public string name;
        public Color color;
        public string spritePath;
    }
    
    [System.Serializable]
    private class SerializableFixedPositionConfig
    {
        public string shapeType;
        public List<SerializableVector2> fixedPositions;
        
        public SerializableFixedPositionConfig(FixedPositionConfig config)
        {
            shapeType = config.shapeType;
            fixedPositions = new List<SerializableVector2>();
            foreach (var pos in config.fixedPositions)
            {
                fixedPositions.Add(new SerializableVector2(pos));
            }
        }
        
        public FixedPositionConfig ToFixedPositionConfig()
        {
            var config = new FixedPositionConfig(shapeType);
            foreach (var pos in fixedPositions)
            {
                config.fixedPositions.Add(pos.ToVector2());
            }
            return config;
        }
    }
    
    [System.Serializable]
    private class SerializableVector2
    {
        public float x;
        public float y;
        
        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }
        
        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
    
    [System.Serializable]
    private class SerializableConfig
    {
        public List<SerializableShapeType> shapeTypes;
        public List<SerializableBallType> ballTypes;
        public List<SerializableBackgroundConfig> backgroundConfigs;
        public List<SerializableFixedPositionConfig> fixedPositionConfigs; // 新增：固定位置配置
        public int currentBackgroundIndex;
        public int levelIndex = 1; // 新增：关卡索引，初始值为1
    }

    public void SaveConfigToFile()
    {
        if (!Directory.Exists(ConfigDir))
            Directory.CreateDirectory(ConfigDir);

        // 转换为可序列化的形状配置
        var serializableShapes = new List<SerializableShapeType>();
        foreach (var shape in this.shapeTypes)
        {
            serializableShapes.Add(new SerializableShapeType
            {
                name = shape.name,
                spritePath = GetSpritePath(shape.sprite)
            });
        }

        // 转换为可序列化的球配置
        var serializableBalls = new List<SerializableBallType>();
        foreach (var ball in this.ballTypes)
        {
            serializableBalls.Add(new SerializableBallType
            {
                name = ball.name,
                color = ball.color,
                spritePath = GetSpritePath(ball.sprite)
            });
        }

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

        // 转换为可序列化的固定位置配置
        var serializableFixedPositions = new List<SerializableFixedPositionConfig>();
        foreach (var fp in this.fixedPositionConfigs)
        {
            serializableFixedPositions.Add(new SerializableFixedPositionConfig(fp));
        }

        // 创建完整的配置对象
        var config = new SerializableConfig
        {
            shapeTypes = serializableShapes,
            ballTypes = serializableBalls,
            backgroundConfigs = serializableBackgrounds,
            fixedPositionConfigs = serializableFixedPositions, // 新增：固定位置配置
            currentBackgroundIndex = this.currentBackgroundIndex,
            levelIndex = this.levelIndex // 新增：关卡索引
        };

        // 序列化为JSON并保存
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(ConfigPath, json);
        Debug.Log($"配置已保存到: {ConfigPath}");
    }
    
    /// <summary>
    /// 获取Sprite的路径
    /// </summary>
    private string GetSpritePath(Sprite sprite)
    {
        if (sprite == null) return null;
        
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(sprite);
        #else
        return null;
        #endif
    }

    public void LoadConfigFromFile()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                string json = File.ReadAllText(ConfigPath);
                var data = JsonUtility.FromJson<SerializableConfig>(json);
                if (data != null)
                {
                    // 转换形状配置
                    this.shapeTypes = new List<ShapeType>();
                    if (data.shapeTypes != null)
                    {
                        foreach (var serializableShape in data.shapeTypes)
                        {
                            var shape = new ShapeType
                            {
                                name = serializableShape.name
                            };
                            // 安全加载精灵，避免卡死
                            shape.sprite = SafeLoadSpriteFromPath(serializableShape.spritePath);
                            this.shapeTypes.Add(shape);
                        }
                    }
                    
                    // 转换球配置
                    this.ballTypes = new List<BallType>();
                    if (data.ballTypes != null)
                    {
                        foreach (var serializableBall in data.ballTypes)
                        {
                            var ball = new BallType
                            {
                                name = serializableBall.name,
                                color = serializableBall.color
                            };
                            // 安全加载精灵，避免卡死
                            ball.sprite = SafeLoadSpriteFromPath(serializableBall.spritePath);
                            this.ballTypes.Add(ball);
                        }
                    }
                    
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
                            // 安全加载背景精灵，避免卡死
                            if (!string.IsNullOrEmpty(serializableBg.spritePath))
                            {
                                bg.spritePath = serializableBg.spritePath;
                                bg.backgroundSprite = SafeLoadSpriteFromPath(serializableBg.spritePath);
                            }
                            this.backgroundConfigs.Add(bg);
                        }
                    }
                    
                    // 转换固定位置配置
                    this.fixedPositionConfigs = new List<FixedPositionConfig>();
                    if (data.fixedPositionConfigs != null)
                    {
                        foreach (var serializableFp in data.fixedPositionConfigs)
                        {
                            this.fixedPositionConfigs.Add(serializableFp.ToFixedPositionConfig());
                        }
                    }
                    
                    this.currentBackgroundIndex = data.currentBackgroundIndex;
                    this.levelIndex = data.levelIndex; // 新增：加载关卡索引
                    Debug.Log($"配置已从文件加载: {ConfigPath}, 固定位置配置数量: {this.fixedPositionConfigs.Count}");
                    
                    // 检查配置是否为空，如果为空则初始化默认配置（不保存，避免循环）
                    if (this.shapeTypes.Count == 0 && this.ballTypes.Count == 0)
                    {
                        Debug.Log("加载的配置为空，初始化默认配置（不保存）");
                        // 保存当前的levelIndex，避免被重置
                        int savedLevelIndex = this.levelIndex;
                        InitializeDefaultConfigWithoutSave();
                        // 确保levelIndex不被重置
                        this.levelIndex = savedLevelIndex;
                        Debug.Log($"保持levelIndex: {this.levelIndex}");
                    }
                    
                    // 触发配置重新加载事件
                    TriggerConfigReloaded();
                }
                else
                {
                    Debug.LogWarning("配置文件解析失败，初始化默认配置");
                    // 保存当前的levelIndex，避免被重置
                    int savedLevelIndex = this.levelIndex;
                    InitializeDefaultConfigWithoutSave();
                    // 确保levelIndex不被重置
                    this.levelIndex = savedLevelIndex;
                    Debug.Log($"保持levelIndex: {this.levelIndex}");
                }
            }
            else
            {
                Debug.LogWarning("配置文件不存在: " + ConfigPath + "，初始化默认配置");
                // 保存当前的levelIndex，避免被重置
                int savedLevelIndex = this.levelIndex;
                InitializeDefaultConfigWithoutSave();
                // 确保levelIndex不被重置
                this.levelIndex = savedLevelIndex;
                Debug.Log($"保持levelIndex: {this.levelIndex}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载配置文件时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
            // 发生异常时初始化默认配置
            // 保存当前的levelIndex，避免被重置
            int savedLevelIndex = this.levelIndex;
            InitializeDefaultConfigWithoutSave();
            // 确保levelIndex不被重置
            this.levelIndex = savedLevelIndex;
            Debug.Log($"保持levelIndex: {this.levelIndex}");
        }
    }
    
    /// <summary>
    /// 加载精灵
    /// </summary>
    private Sprite LoadSpriteFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        try
        {
            #if UNITY_EDITOR
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }
            #else
            // 运行时尝试从Resources加载
            string resourcePath = GetResourcePath(path);
            if (!string.IsNullOrEmpty(resourcePath))
            {
                return Resources.Load<Sprite>(resourcePath);
            }
            #endif
            
            Debug.LogWarning($"无法加载精灵: {path}");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoadSpriteFromPath异常: {path}, 错误: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 安全加载精灵，避免卡死
    /// </summary>
    private Sprite SafeLoadSpriteFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        try
        {
            return LoadSpriteFromPath(path);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载精灵时发生异常: {path}, 错误: {e.Message}");
            return null;
        }
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

    public void InitializeDefaultConfig()
    {
        // 保存当前的levelIndex，避免被重置
        int currentLevelIndex = this.levelIndex;
        
        shapeTypes = new List<ShapeType>
        {
            new ShapeType { name = "圆形", sprite = LoadDefaultShapeSprite("圆形", "蓝色圆") },
            new ShapeType { name = "矩形", sprite = LoadDefaultShapeSprite("矩形", "红色花瓣") },
            new ShapeType { name = "三角形", sprite = LoadDefaultShapeSprite("三角形", "蓝三角") },
            new ShapeType { name = "菱形", sprite = LoadDefaultShapeSprite("菱形", "蓝色菱形") }
        };
        ballTypes = new List<BallType>
        {
            new BallType { name = "红球", color = Color.red, sprite = LoadDefaultBallSprite("红球", "red") },
            new BallType { name = "蓝球", color = Color.blue, sprite = LoadDefaultBallSprite("蓝球", "blue") },
            new BallType { name = "绿球", color = Color.green, sprite = LoadDefaultBallSprite("绿球", "green") }
        };
        
        backgroundConfigs = new List<BackgroundConfig>
        {
            new BackgroundConfig { name = "默认背景", backgroundColor = Color.white, useSprite = false },
            new BackgroundConfig { name = "网格背景", backgroundColor = new Color(0.9f, 0.9f, 0.9f), useSprite = false },
            new BackgroundConfig { name = "深色背景", backgroundColor = new Color(0.2f, 0.2f, 0.2f), useSprite = false }
        };
        
        // 恢复levelIndex，避免被重置
        this.levelIndex = currentLevelIndex;
        
        // 保存默认配置到文件
        SaveConfigToFile();
        Debug.Log($"默认配置已初始化并保存到文件，保持levelIndex: {this.levelIndex}");
    }
    
    /// <summary>
    /// 初始化默认配置（不保存，避免循环调用）
    /// </summary>
    public void InitializeDefaultConfigWithoutSave()
    {
        // 保存当前的levelIndex，避免被重置
        int currentLevelIndex = this.levelIndex;
        
        shapeTypes = new List<ShapeType>
        {
            new ShapeType { name = "圆形", sprite = LoadDefaultShapeSprite("圆形", "蓝色圆") },
            new ShapeType { name = "矩形", sprite = LoadDefaultShapeSprite("矩形", "红色花瓣") },
            new ShapeType { name = "三角形", sprite = LoadDefaultShapeSprite("三角形", "蓝三角") },
            new ShapeType { name = "菱形", sprite = LoadDefaultShapeSprite("菱形", "蓝色菱形") }
        };
        ballTypes = new List<BallType>
        {
            new BallType { name = "红球", color = Color.red, sprite = LoadDefaultBallSprite("红球", "red") },
            new BallType { name = "蓝球", color = Color.blue, sprite = LoadDefaultBallSprite("蓝球", "blue") },
            new BallType { name = "绿球", color = Color.green, sprite = LoadDefaultBallSprite("绿球", "green") }
        };
        
        backgroundConfigs = new List<BackgroundConfig>
        {
            new BackgroundConfig { name = "默认背景", backgroundColor = Color.white, useSprite = false },
            new BackgroundConfig { name = "网格背景", backgroundColor = new Color(0.9f, 0.9f, 0.9f), useSprite = false },
            new BackgroundConfig { name = "深色背景", backgroundColor = new Color(0.2f, 0.2f, 0.2f), useSprite = false }
        };
        
        // 恢复levelIndex，避免被重置
        this.levelIndex = currentLevelIndex;
        
        Debug.Log($"默认配置已初始化（未保存到文件，避免循环调用），保持levelIndex: {this.levelIndex}");
    }
    
    /// <summary>
    /// 加载默认形状精灵
    /// </summary>
    private Sprite LoadDefaultShapeSprite(string shapeName, string fileName)
    {
        try
        {
            // 尝试从多个可能的路径加载
            string[] possiblePaths = {
                $"Assets/Textures/pieces/{fileName}.png",
                $"Assets/Textures/cicle/{fileName}.png",
                $"Assets/Textures/shapes/{fileName}.png",
                $"Assets/Sprites/{fileName}.png",
                $"Assets/Textures/pieces/{shapeName}.png",
                $"Assets/Textures/cicle/{shapeName}.png",
                $"Assets/Textures/shapes/{shapeName}.png",
                $"Assets/Sprites/{shapeName}.png"
            };
            
            foreach (string path in possiblePaths)
            {
                #if UNITY_EDITOR
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    Debug.Log($"找到形状精灵: {path}");
                    return sprite;
                }
                #endif
            }
            
            Debug.LogWarning($"未找到形状精灵: {shapeName} (尝试的文件名: {fileName})");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoadDefaultShapeSprite异常: {shapeName}, 错误: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 加载默认球精灵
    /// </summary>
    private Sprite LoadDefaultBallSprite(string ballName, string fileName)
    {
        try
        {
            // 尝试从多个可能的路径加载
            string[] possiblePaths = {
                $"Assets/Textures/ball/{fileName}.png",
                $"Assets/Textures/balls/{fileName}.png",
                $"Assets/Sprites/{fileName}.png",
                $"Assets/Textures/ball/{ballName}.png",
                $"Assets/Textures/balls/{ballName}.png",
                $"Assets/Sprites/{ballName}.png"
            };
            
            foreach (string path in possiblePaths)
            {
                #if UNITY_EDITOR
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    Debug.Log($"找到球精灵: {path}");
                    return sprite;
                }
                #endif
            }
            
            Debug.LogWarning($"未找到球精灵: {ballName} (尝试的文件名: {fileName})");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoadDefaultBallSprite异常: {ballName}, 错误: {e.Message}");
            return null;
        }
    }

    public void AddShapeType(string name)
    {
        // 尝试为新的形状类型找到合适的sprite
        Sprite sprite = FindSpriteForShape(name);
        shapeTypes.Add(new ShapeType { name = name, sprite = sprite });
        SaveConfigToFile();
        TriggerShapeTypesChanged();
    }
    
    /// <summary>
    /// 为形状类型查找合适的sprite
    /// </summary>
    private Sprite FindSpriteForShape(string shapeName)
    {
        // 尝试从现有的sprite中找到一个合适的
        string[] searchPaths = {
            "Assets/Textures/pieces",
            "Assets/Textures/cicle",
            "Assets/Textures/shapes",
            "Assets/Sprites"
        };
        
        foreach (string searchPath in searchPaths)
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new string[] { searchPath });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    Debug.Log($"为形状 '{shapeName}' 找到sprite: {path}");
                    return sprite;
                }
            }
            #endif
        }
        
        Debug.LogWarning($"未为形状 '{shapeName}' 找到合适的sprite");
        return null;
    }
    
    /// <summary>
    /// 触发形状类型变更事件
    /// </summary>
    public void TriggerShapeTypesChanged()
    {
        OnShapeTypesChanged?.Invoke();
    }

    public void AddBallType(string name, Color color)
    {
        // 尝试为新的球类型找到合适的sprite
        Sprite sprite = FindSpriteForBall(name);
        ballTypes.Add(new BallType { name = name, color = color, sprite = sprite });
        SaveConfigToFile();
        TriggerBallTypesChanged();
    }
    
    /// <summary>
    /// 为球类型查找合适的sprite
    /// </summary>
    private Sprite FindSpriteForBall(string ballName)
    {
        // 尝试从现有的sprite中找到一个合适的
        string[] searchPaths = {
            "Assets/Textures/ball",
            "Assets/Textures/balls",
            "Assets/Sprites"
        };
        
        foreach (string searchPath in searchPaths)
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new string[] { searchPath });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    Debug.Log($"为球 '{ballName}' 找到sprite: {path}");
                    return sprite;
                }
            }
            #endif
        }
        
        Debug.LogWarning($"未为球 '{ballName}' 找到合适的sprite");
        return null;
    }
    
    /// <summary>
    /// 触发球类型变更事件
    /// </summary>
    public void TriggerBallTypesChanged()
    {
        OnBallTypesChanged?.Invoke();
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
        SaveConfigToFile();
        TriggerBackgroundConfigsChanged();
    }
    
    /// <summary>
    /// 触发背景配置变更事件
    /// </summary>
    public void TriggerBackgroundConfigsChanged()
    {
        OnBackgroundConfigsChanged?.Invoke();
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
            SaveConfigToFile();
            TriggerCurrentBackgroundChanged();
        }
    }
    
    /// <summary>
    /// 触发当前背景变更事件
    /// </summary>
    public void TriggerCurrentBackgroundChanged()
    {
        OnCurrentBackgroundChanged?.Invoke();
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
                // 使用安全的加载方法，避免卡死
                bg.backgroundSprite = SafeLoadSpriteFromPath(bg.spritePath);
            }
        }
    }
    
    /// <summary>
    /// 触发配置重新加载事件
    /// </summary>
    public void TriggerConfigReloaded()
    {
        try
        {
            // 检查是否有订阅者，避免在对象已销毁时触发事件
            if (OnConfigReloaded != null)
            {
                OnConfigReloaded?.Invoke();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"触发配置重新加载事件时发生异常: {e.Message}");
            // 清除可能已失效的事件订阅者
            OnConfigReloaded = null;
        }
    }
    
    /// <summary>
    /// 获取当前关卡索引
    /// </summary>
    public int GetLevelIndex()
    {
        return levelIndex;
    }
    
    /// <summary>
    /// 增加关卡索引并保存配置
    /// </summary>
    public int IncrementLevelIndex()
    {
        levelIndex++;
        SaveConfigToFile();
        Debug.Log($"关卡索引已增加到: {levelIndex}");
        return levelIndex;
    }
    
    /// <summary>
    /// 设置关卡索引
    /// </summary>
    public void SetLevelIndex(int index)
    {
        this.levelIndex = index;
        SaveConfigToFile();
        Debug.Log($"关卡索引已设置为: {index}");
    }
    
    /// <summary>
    /// 获取指定形状类型的固定位置配置
    /// </summary>
    public FixedPositionConfig GetFixedPositionConfig(string shapeType)
    {
        foreach (var config in fixedPositionConfigs)
        {
            if (config.shapeType == shapeType)
            {
                return config;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 添加或更新固定位置配置
    /// </summary>
    public void SetFixedPositionConfig(string shapeType, List<Vector2> positions)
    {
        // 查找现有配置
        var existingConfig = GetFixedPositionConfig(shapeType);
        if (existingConfig != null)
        {
            // 更新现有配置
            existingConfig.fixedPositions.Clear();
            existingConfig.fixedPositions.AddRange(positions);
            Debug.Log($"已更新形状 '{shapeType}' 的固定位置配置，位置数量: {positions.Count}");
        }
        else
        {
            // 创建新配置
            var newConfig = new FixedPositionConfig(shapeType, positions);
            fixedPositionConfigs.Add(newConfig);
            Debug.Log($"已为形状 '{shapeType}' 创建固定位置配置，位置数量: {positions.Count}");
        }
        
        // 保存到文件
        SaveConfigToFile();
    }
    
    /// <summary>
    /// 添加单个固定位置
    /// </summary>
    public void AddFixedPosition(string shapeType, Vector2 position)
    {
        var config = GetFixedPositionConfig(shapeType);
        if (config == null)
        {
            config = new FixedPositionConfig(shapeType);
            fixedPositionConfigs.Add(config);
        }
        
        config.fixedPositions.Add(position);
        SaveConfigToFile();
        Debug.Log($"已为形状 '{shapeType}' 添加固定位置: {position}");
    }
    
    /// <summary>
    /// 清除指定形状类型的所有固定位置
    /// </summary>
    public void ClearFixedPositions(string shapeType)
    {
        var config = GetFixedPositionConfig(shapeType);
        if (config != null)
        {
            config.fixedPositions.Clear();
            SaveConfigToFile();
            Debug.Log($"已清除形状 '{shapeType}' 的所有固定位置");
        }
    }
    
    /// <summary>
    /// 获取所有固定位置配置
    /// </summary>
    public List<FixedPositionConfig> GetAllFixedPositionConfigs()
    {
        return new List<FixedPositionConfig>(fixedPositionConfigs);
    }
    
    /// <summary>
    /// 删除指定形状类型的固定位置配置
    /// </summary>
    public void RemoveFixedPositionConfig(string shapeType)
    {
        for (int i = fixedPositionConfigs.Count - 1; i >= 0; i--)
        {
            if (fixedPositionConfigs[i].shapeType == shapeType)
            {
                fixedPositionConfigs.RemoveAt(i);
                SaveConfigToFile();
                Debug.Log($"已删除形状 '{shapeType}' 的固定位置配置");
                return;
            }
        }
    }
}