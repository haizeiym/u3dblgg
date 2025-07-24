using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 图片引用修复工具
/// 用于查找和修复配置中丢失的图片引用
/// </summary>
public class SpriteReferenceFixer : EditorWindow
{
    [MenuItem("Tools/Level Editor/修复图片引用")]
    public static void ShowWindow()
    {
        GetWindow<SpriteReferenceFixer>("图片引用修复");
    }

    private Vector2 scrollPosition;
    private List<string> missingSprites = new List<string>();
    private List<string> foundSprites = new List<string>();

    void OnGUI()
    {
        GUILayout.Label("图片引用修复工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("扫描丢失的图片引用"))
        {
            ScanMissingSprites();
        }

        if (GUILayout.Button("自动修复图片引用"))
        {
            AutoFixSpriteReferences();
        }

        if (GUILayout.Button("重新生成配置文件"))
        {
            RegenerateConfigFile();
        }

        EditorGUILayout.Space();

        // 显示扫描结果
        if (missingSprites.Count > 0 || foundSprites.Count > 0)
        {
            GUILayout.Label("扫描结果:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (missingSprites.Count > 0)
            {
                GUILayout.Label($"丢失的图片引用 ({missingSprites.Count}):", EditorStyles.boldLabel);
                foreach (var missing in missingSprites)
                {
                    EditorGUILayout.LabelField($"✗ {missing}");
                }
                EditorGUILayout.Space();
            }

            if (foundSprites.Count > 0)
            {
                GUILayout.Label($"找到的图片 ({foundSprites.Count}):", EditorStyles.boldLabel);
                foreach (var found in foundSprites)
                {
                    EditorGUILayout.LabelField($"✓ {found}");
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    void ScanMissingSprites()
    {
        missingSprites.Clear();
        foundSprites.Clear();

        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("无法获取配置实例");
            return;
        }

        Debug.Log("开始扫描图片引用...");

        // 扫描形状类型
        foreach (var shape in config.shapeTypes)
        {
            if (shape.sprite == null)
            {
                missingSprites.Add($"形状: {shape.name} - 缺少精灵");
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(shape.sprite);
                foundSprites.Add($"形状: {shape.name} - {path}");
            }
        }

        // 扫描球类型
        foreach (var ball in config.ballTypes)
        {
            if (ball.sprite == null)
            {
                missingSprites.Add($"球: {ball.name} - 缺少精灵");
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(ball.sprite);
                foundSprites.Add($"球: {ball.name} - {path}");
            }
        }

        // 扫描背景配置
        foreach (var bg in config.backgroundConfigs)
        {
            if (bg.useSprite && bg.backgroundSprite == null)
            {
                missingSprites.Add($"背景: {bg.name} - 缺少精灵");
            }
            else if (bg.backgroundSprite != null)
            {
                string path = AssetDatabase.GetAssetPath(bg.backgroundSprite);
                foundSprites.Add($"背景: {bg.name} - {path}");
            }
        }

        Debug.Log($"扫描完成 - 丢失: {missingSprites.Count}, 找到: {foundSprites.Count}");
        Repaint();
    }

    void AutoFixSpriteReferences()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("无法获取配置实例");
            return;
        }

        Debug.Log("开始自动修复图片引用...");

        // 修复形状类型
        foreach (var shape in config.shapeTypes)
        {
            if (shape.sprite == null)
            {
                shape.sprite = FindSpriteByName(shape.name);
                if (shape.sprite != null)
                {
                    Debug.Log($"修复形状精灵: {shape.name} -> {AssetDatabase.GetAssetPath(shape.sprite)}");
                }
            }
        }

        // 修复球类型
        foreach (var ball in config.ballTypes)
        {
            if (ball.sprite == null)
            {
                ball.sprite = FindSpriteByName(ball.name);
                if (ball.sprite != null)
                {
                    Debug.Log($"修复球精灵: {ball.name} -> {AssetDatabase.GetAssetPath(ball.sprite)}");
                }
            }
        }

        // 修复背景配置
        foreach (var bg in config.backgroundConfigs)
        {
            if (bg.useSprite && bg.backgroundSprite == null)
            {
                bg.backgroundSprite = FindSpriteByName(bg.name);
                if (bg.backgroundSprite != null)
                {
                    Debug.Log($"修复背景精灵: {bg.name} -> {AssetDatabase.GetAssetPath(bg.backgroundSprite)}");
                }
            }
        }

        // 保存修复后的配置
        config.SaveConfigToFile();
        Debug.Log("图片引用修复完成并保存");
        
        // 重新扫描
        ScanMissingSprites();
    }

    void RegenerateConfigFile()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null)
        {
            Debug.LogError("无法获取配置实例");
            return;
        }

        Debug.Log("重新生成配置文件...");

        // 重新初始化默认配置
        config.InitializeDefaultConfig();
        
        // 保存配置
        config.SaveConfigToFile();
        
        Debug.Log("配置文件重新生成完成");
        
        // 重新扫描
        ScanMissingSprites();
    }

    /// <summary>
    /// 根据名称查找精灵
    /// </summary>
    private Sprite FindSpriteByName(string name)
    {
        // 搜索所有精灵资源
        string[] guids = AssetDatabase.FindAssets("t:Sprite");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            // 尝试多种匹配方式
            if (fileName.ToLower().Contains(name.ToLower()) ||
                fileName.ToLower().Contains(GetSpriteNameMapping(name).ToLower()))
            {
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
    /// 获取精灵名称映射
    /// </summary>
    private string GetSpriteNameMapping(string originalName)
    {
        var mappings = new Dictionary<string, string>
        {
            { "圆形", "circle" },
            { "矩形", "rectangle" },
            { "三角形", "triangle" },
            { "菱形", "diamond" },
            { "红球", "red" },
            { "蓝球", "blue" },
            { "绿球", "green" },
            { "默认背景", "background" },
            { "网格背景", "grid" },
            { "深色背景", "dark" }
        };

        return mappings.ContainsKey(originalName) ? mappings[originalName] : originalName;
    }
} 