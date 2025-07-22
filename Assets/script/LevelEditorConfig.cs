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

    private static string ConfigDir => Path.Combine(Application.dataPath, "config");
    private static string ConfigPath => Path.Combine(ConfigDir, "level_editor_config.json");

    [System.Serializable]
    private class SerializableConfig
    {
        public List<ShapeType> shapeTypes;
        public List<BallType> ballTypes;
    }

    public void SaveConfigToFile()
    {
        if (!Directory.Exists(ConfigDir))
            Directory.CreateDirectory(ConfigDir);

        var data = new SerializableConfig
        {
            shapeTypes = this.shapeTypes,
            ballTypes = this.ballTypes
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
}