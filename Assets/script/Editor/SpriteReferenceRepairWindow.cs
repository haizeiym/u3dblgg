using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Sprite引用修复工具
/// 用于修复配置中丢失的sprite引用
/// </summary>
public class SpriteReferenceRepairWindow : EditorWindow
{
    [MenuItem("Tools/Level Editor/修复Sprite引用")]
    public static void ShowWindow()
    {
        GetWindow<SpriteReferenceRepairWindow>("Sprite引用修复");
    }

    private Vector2 scrollPosition;
    private List<string> repairLog = new List<string>();

    void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite引用修复工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这个工具用于修复配置中丢失的sprite引用", MessageType.Info);
        EditorGUILayout.Space();

        if (GUILayout.Button("扫描丢失的Sprite引用"))
        {
            ScanMissingSprites();
        }

        if (GUILayout.Button("自动修复Sprite引用"))
        {
            AutoRepairSprites();
        }

        if (GUILayout.Button("重新生成默认配置"))
        {
            RegenerateDefaultConfig();
        }

        if (GUILayout.Button("清理修复日志"))
        {
            repairLog.Clear();
        }

        EditorGUILayout.Space();

        // 显示修复日志
        if (repairLog.Count > 0)
        {
            EditorGUILayout.LabelField("修复日志:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (string log in repairLog)
            {
                EditorGUILayout.LabelField(log);
            }
            
            EditorGUILayout.EndScrollView();
        }
    }

    void ScanMissingSprites()
    {
        repairLog.Clear();
        repairLog.Add("=== 开始扫描丢失的Sprite引用 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            repairLog.Add("错误: 无法获取配置实例");
            return;
        }

        int missingShapeSprites = 0;
        int missingBallSprites = 0;

        // 扫描形状类型
        repairLog.Add("扫描形状类型:");
        for (int i = 0; i < config.shapeTypes.Count; i++)
        {
            var shape = config.shapeTypes[i];
            if (shape.sprite == null)
            {
                repairLog.Add($"  ✗ [{i}] {shape.name} - 缺少Sprite");
                missingShapeSprites++;
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(shape.sprite);
                repairLog.Add($"  ✓ [{i}] {shape.name} - {path}");
            }
        }

        // 扫描球类型
        repairLog.Add("扫描球类型:");
        for (int i = 0; i < config.ballTypes.Count; i++)
        {
            var ball = config.ballTypes[i];
            if (ball.sprite == null)
            {
                repairLog.Add($"  ✗ [{i}] {ball.name} - 缺少Sprite");
                missingBallSprites++;
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(ball.sprite);
                repairLog.Add($"  ✓ [{i}] {ball.name} - {path}");
            }
        }

        repairLog.Add($"扫描完成 - 缺少形状Sprite: {missingShapeSprites}, 缺少球Sprite: {missingBallSprites}");
        Repaint();
    }

    void AutoRepairSprites()
    {
        repairLog.Clear();
        repairLog.Add("=== 开始自动修复Sprite引用 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            repairLog.Add("错误: 无法获取配置实例");
            return;
        }

        int repairedShapes = 0;
        int repairedBalls = 0;

        // 修复形状类型
        repairLog.Add("修复形状类型:");
        for (int i = 0; i < config.shapeTypes.Count; i++)
        {
            var shape = config.shapeTypes[i];
            if (shape.sprite == null)
            {
                Sprite newSprite = FindSpriteForShape(shape.name);
                if (newSprite != null)
                {
                    shape.sprite = newSprite;
                    string path = AssetDatabase.GetAssetPath(newSprite);
                    repairLog.Add($"  ✓ [{i}] {shape.name} - 已修复: {path}");
                    repairedShapes++;
                }
                else
                {
                    repairLog.Add($"  ✗ [{i}] {shape.name} - 无法找到合适的Sprite");
                }
            }
        }

        // 修复球类型
        repairLog.Add("修复球类型:");
        for (int i = 0; i < config.ballTypes.Count; i++)
        {
            var ball = config.ballTypes[i];
            if (ball.sprite == null)
            {
                Sprite newSprite = FindSpriteForBall(ball.name);
                if (newSprite != null)
                {
                    ball.sprite = newSprite;
                    string path = AssetDatabase.GetAssetPath(newSprite);
                    repairLog.Add($"  ✓ [{i}] {ball.name} - 已修复: {path}");
                    repairedBalls++;
                }
                else
                {
                    repairLog.Add($"  ✗ [{i}] {ball.name} - 无法找到合适的Sprite");
                }
            }
        }

        // 保存修复后的配置
        if (repairedShapes > 0 || repairedBalls > 0)
        {
            config.SaveConfigToFile();
            repairLog.Add($"修复完成 - 修复形状: {repairedShapes}, 修复球: {repairedBalls}");
            repairLog.Add("配置已保存到文件");
        }
        else
        {
            repairLog.Add("无需修复，所有Sprite引用都正常");
        }

        Repaint();
    }

    void RegenerateDefaultConfig()
    {
        repairLog.Clear();
        repairLog.Add("=== 重新生成默认配置 ===");
        
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            repairLog.Add("错误: 无法获取配置实例");
            return;
        }

        // 备份当前配置
        var backupShapeTypes = new List<ShapeType>(config.shapeTypes);
        var backupBallTypes = new List<BallType>(config.ballTypes);
        var backupBackgroundConfigs = new List<BackgroundConfig>(config.backgroundConfigs);
        int backupCurrentBackgroundIndex = config.currentBackgroundIndex;

        repairLog.Add("备份当前配置...");

        // 重新生成默认配置
        config.InitializeDefaultConfig();
        repairLog.Add("默认配置已重新生成");

        // 恢复用户自定义的配置项
        repairLog.Add("恢复用户自定义配置...");
        
        // 恢复形状类型（保留用户添加的）
        foreach (var backupShape in backupShapeTypes)
        {
            bool exists = false;
            foreach (var currentShape in config.shapeTypes)
            {
                if (currentShape.name == backupShape.name)
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                Sprite sprite = FindSpriteForShape(backupShape.name);
                config.shapeTypes.Add(new ShapeType { name = backupShape.name, sprite = sprite });
                repairLog.Add($"  恢复形状: {backupShape.name}");
            }
        }

        // 恢复球类型（保留用户添加的）
        foreach (var backupBall in backupBallTypes)
        {
            bool exists = false;
            foreach (var currentBall in config.ballTypes)
            {
                if (currentBall.name == backupBall.name)
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                Sprite sprite = FindSpriteForBall(backupBall.name);
                config.ballTypes.Add(new BallType { name = backupBall.name, color = backupBall.color, sprite = sprite });
                repairLog.Add($"  恢复球: {backupBall.name}");
            }
        }

        // 恢复背景配置（保留用户添加的）
        foreach (var backupBg in backupBackgroundConfigs)
        {
            bool exists = false;
            foreach (var currentBg in config.backgroundConfigs)
            {
                if (currentBg.name == backupBg.name)
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                config.backgroundConfigs.Add(backupBg);
                repairLog.Add($"  恢复背景: {backupBg.name}");
            }
        }

        // 恢复当前背景索引
        if (backupCurrentBackgroundIndex < config.backgroundConfigs.Count)
        {
            config.currentBackgroundIndex = backupCurrentBackgroundIndex;
        }

        // 保存配置
        config.SaveConfigToFile();
        repairLog.Add("配置重新生成完成并保存");

        Repaint();
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
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { searchPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    return sprite;
                }
            }
        }
        
        return null;
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
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { searchPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    return sprite;
                }
            }
        }
        
        return null;
    }
} 