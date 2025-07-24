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
                    // 新创建的实例需要初始化默认配置
                    _instance.InitializeDefaultConfig();
                }
            }
            return _instance;
        }
    }

    public List<ShapeType> shapeTypes = new List<ShapeType>();
    public List<BallType> ballTypes = new List<BallType>();
    public List<BackgroundConfig> backgroundConfigs = new List<BackgroundConfig>();
    public int currentBackgroundIndex = 0;
    
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
    private class SerializableConfig
    {
        public List<SerializableShapeType> shapeTypes;
        public List<SerializableBallType> ballTypes;
        public List<SerializableBackgroundConfig> backgroundConfigs;
        public int currentBackgroundIndex;
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

        var data = new SerializableConfig
        {
            shapeTypes = serializableShapes,
            ballTypes = serializableBalls,
            backgroundConfigs = serializableBackgrounds,
            currentBackgroundIndex = this.currentBackgroundIndex
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(ConfigPath, json);
        Debug.Log("配置已保存到: " + ConfigPath);
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
                            bg.SetSpritePath(serializableBg.spritePath);
                            this.backgroundConfigs.Add(bg);
                        }
                    }
                    
                    this.currentBackgroundIndex = data.currentBackgroundIndex;
                    Debug.Log("配置已从文件加载: " + ConfigPath);
                    
                    // 检查配置是否为空，如果为空则初始化默认配置（不保存，避免循环）
                    if (this.shapeTypes.Count == 0 && this.ballTypes.Count == 0)
                    {
                        Debug.Log("加载的配置为空，初始化默认配置（不保存）");
                        InitializeDefaultConfigWithoutSave();
                    }
                    
                    // 触发配置重新加载事件
                    TriggerConfigReloaded();
                }
                else
                {
                    Debug.LogWarning("配置文件解析失败，初始化默认配置");
                    InitializeDefaultConfigWithoutSave();
                }
            }
            else
            {
                Debug.LogWarning("配置文件不存在: " + ConfigPath + "，初始化默认配置");
                InitializeDefaultConfigWithoutSave();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载配置文件时发生异常: {e.Message}");
            Debug.LogError($"异常堆栈: {e.StackTrace}");
            // 发生异常时初始化默认配置
            InitializeDefaultConfigWithoutSave();
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
        
        // 保存默认配置到文件
        SaveConfigToFile();
        Debug.Log("默认配置已初始化并保存到文件");
    }
    
    /// <summary>
    /// 初始化默认配置（不保存，避免循环调用）
    /// </summary>
    public void InitializeDefaultConfigWithoutSave()
    {
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
        
        Debug.Log("默认配置已初始化（未保存到文件，避免循环调用）");
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
                bg.SetSpritePath(bg.spritePath);
            }
        }
    }
    
    /// <summary>
    /// 触发配置重新加载事件
    /// </summary>
    public void TriggerConfigReloaded()
    {
        OnConfigReloaded?.Invoke();
    }
}