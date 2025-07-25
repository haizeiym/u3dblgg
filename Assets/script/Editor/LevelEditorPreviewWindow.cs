using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 关卡编辑器预览窗口
/// 展示配置中的所有形状和球类型
/// </summary>
public class LevelEditorPreviewWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showShapes = true;
    private bool showBalls = true;
    private bool showBackgrounds = true;
    private float previewSize = 100f;
    private Vector2 gridSize = new Vector2(4, 4);

    [MenuItem("Tools/Level Editor/预览配置")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorPreviewWindow>("配置预览");
    }

    void OnGUI()
    {
        try
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("配置预览", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 预览设置
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("预览设置", EditorStyles.boldLabel);
            previewSize = EditorGUILayout.Slider("预览大小", previewSize, 50f, 200f);
            gridSize.x = EditorGUILayout.IntSlider("网格列数", (int)gridSize.x, 2, 8);
            gridSize.y = EditorGUILayout.IntSlider("网格行数", (int)gridSize.y, 2, 8);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // 形状预览
            showShapes = EditorGUILayout.Foldout(showShapes, $"形状类型预览 ({LevelEditorConfig.Instance?.shapeTypes.Count ?? 0})");
            if (showShapes) DrawShapePreview();

            EditorGUILayout.Space();

            // 球预览
            showBalls = EditorGUILayout.Foldout(showBalls, $"球类型预览 ({LevelEditorConfig.Instance?.ballTypes.Count ?? 0})");
            if (showBalls) DrawBallPreview();

            EditorGUILayout.Space();

            // 背景预览
            showBackgrounds = EditorGUILayout.Foldout(showBackgrounds, $"背景预览 ({LevelEditorConfig.Instance?.backgroundConfigs.Count ?? 0})");
            if (showBackgrounds) DrawBackgroundPreview();

            EditorGUILayout.EndScrollView();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LevelEditorPreviewWindow OnGUI Error: {e.Message}");
            
            // 确保所有布局组都被正确关闭
            try
            {
                EditorGUILayout.EndScrollView();
            }
            catch { }
            try
            {
                EditorGUILayout.EndVertical();
            }
            catch { }
            try
            {
                EditorGUILayout.EndHorizontal();
            }
            catch { }
            
            EditorGUILayout.HelpBox($"界面渲染错误: {e.Message}", MessageType.Error);
        }
    }

    void DrawShapePreview()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null) return;

        EditorGUILayout.BeginVertical("box");
        
        int columns = (int)gridSize.x;
        int currentColumn = 0;

        for (int i = 0; i < config.shapeTypes.Count; i++)
        {
            if (currentColumn == 0)
            {
                EditorGUILayout.BeginHorizontal();
            }

            DrawShapeItem(config.shapeTypes[i], i);

            currentColumn++;
            if (currentColumn >= columns)
            {
                EditorGUILayout.EndHorizontal();
                currentColumn = 0;
            }
        }

        // 补齐最后一行
        while (currentColumn > 0 && currentColumn < columns)
        {
            GUILayout.Space(previewSize);
            currentColumn++;
        }
        if (currentColumn > 0)
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    void DrawShapeItem(ShapeType shapeType, int index)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(previewSize), GUILayout.Height(previewSize + 40));

        // 预览区域
        Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
        EditorGUI.DrawRect(previewRect, new Color(0.9f, 0.9f, 0.9f));

        // 绘制形状
        if (shapeType.sprite != null)
        {
            // 使用精灵绘制
            float spriteAspect = shapeType.sprite.rect.width / shapeType.sprite.rect.height;
            float drawSize = Mathf.Min(previewSize * 0.8f, previewSize * 0.8f / spriteAspect);
            float x = previewRect.x + (previewSize - drawSize) * 0.5f;
            float y = previewRect.y + (previewSize - drawSize) * 0.5f;
            
            Rect spriteRect = new Rect(x, y, drawSize, drawSize);
            GUI.DrawTextureWithTexCoords(spriteRect, shapeType.sprite.texture, 
                new Rect(shapeType.sprite.rect.x / shapeType.sprite.texture.width,
                        shapeType.sprite.rect.y / shapeType.sprite.texture.height,
                        shapeType.sprite.rect.width / shapeType.sprite.texture.width,
                        shapeType.sprite.rect.height / shapeType.sprite.texture.height));
        }
        else
        {
            // 绘制默认形状
            float centerX = previewRect.x + previewSize * 0.5f;
            float centerY = previewRect.y + previewSize * 0.5f;
            float size = previewSize * 0.4f;

            switch (index % 4)
            {
                case 0: // 圆形
                    DrawCircle(centerX, centerY, size, Color.blue);
                    break;
                case 1: // 矩形
                    DrawRect(centerX, centerY, size, size, Color.red);
                    break;
                case 2: // 三角形
                    DrawTriangle(centerX, centerY, size, Color.green);
                    break;
                case 3: // 菱形
                    DrawDiamond(centerX, centerY, size, Color.yellow);
                    break;
            }
        }

        // 名称标签
        EditorGUILayout.LabelField(shapeType.name, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    void DrawBallPreview()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null) return;

        EditorGUILayout.BeginVertical("box");
        
        int columns = (int)gridSize.x;
        int currentColumn = 0;

        for (int i = 0; i < config.ballTypes.Count; i++)
        {
            if (currentColumn == 0)
            {
                EditorGUILayout.BeginHorizontal();
            }

            DrawBallItem(config.ballTypes[i], i);

            currentColumn++;
            if (currentColumn >= columns)
            {
                EditorGUILayout.EndHorizontal();
                currentColumn = 0;
            }
        }

        // 补齐最后一行
        while (currentColumn > 0 && currentColumn < columns)
        {
            GUILayout.Space(previewSize);
            currentColumn++;
        }
        if (currentColumn > 0)
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    void DrawBallItem(BallType ballType, int index)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(previewSize), GUILayout.Height(previewSize + 40));

        // 预览区域
        Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
        EditorGUI.DrawRect(previewRect, new Color(0.9f, 0.9f, 0.9f));

        // 绘制球
        if (ballType.sprite != null)
        {
            // 使用精灵绘制
            float centerX = previewRect.x + previewSize * 0.5f;
            float centerY = previewRect.y + previewSize * 0.5f;
            float size = previewSize * 0.6f;
            
            float spriteAspect = ballType.sprite.rect.width / ballType.sprite.rect.height;
            float drawSize = Mathf.Min(size, size / spriteAspect);
            float x = centerX - drawSize * 0.5f;
            float y = centerY - drawSize * 0.5f;
            
            Rect spriteRect = new Rect(x, y, drawSize, drawSize);
            GUI.DrawTextureWithTexCoords(spriteRect, ballType.sprite.texture, 
                new Rect(ballType.sprite.rect.x / ballType.sprite.texture.width,
                        ballType.sprite.rect.y / ballType.sprite.texture.height,
                        ballType.sprite.rect.width / ballType.sprite.texture.width,
                        ballType.sprite.rect.height / ballType.sprite.texture.height));
        }
        else
        {
            // 绘制默认球
            float centerX = previewRect.x + previewSize * 0.5f;
            float centerY = previewRect.y + previewSize * 0.5f;
            float size = previewSize * 0.4f;
            
            DrawCircle(centerX, centerY, size, ballType.color);
        }

        // 名称标签
        EditorGUILayout.LabelField(ballType.name, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    void DrawBackgroundPreview()
    {
        var config = LevelEditorConfig.Instance;
        if (config == null) return;

        EditorGUILayout.BeginVertical("box");
        
        int columns = (int)gridSize.x;
        int currentColumn = 0;

        for (int i = 0; i < config.backgroundConfigs.Count; i++)
        {
            if (currentColumn == 0)
            {
                EditorGUILayout.BeginHorizontal();
            }

            DrawBackgroundItem(config.backgroundConfigs[i], i);

            currentColumn++;
            if (currentColumn >= columns)
            {
                EditorGUILayout.EndHorizontal();
                currentColumn = 0;
            }
        }

        // 补齐最后一行
        while (currentColumn > 0 && currentColumn < columns)
        {
            GUILayout.Space(previewSize);
            currentColumn++;
        }
        if (currentColumn > 0)
        {
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    void DrawBackgroundItem(BackgroundConfig backgroundConfig, int index)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(previewSize), GUILayout.Height(previewSize + 40));

        // 预览区域
        Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
        
        // 绘制背景
        if (backgroundConfig.useSprite && backgroundConfig.backgroundSprite != null)
        {
            // 使用精灵绘制
            GUI.DrawTextureWithTexCoords(previewRect, backgroundConfig.backgroundSprite.texture, 
                new Rect(backgroundConfig.backgroundSprite.rect.x / backgroundConfig.backgroundSprite.texture.width,
                        backgroundConfig.backgroundSprite.rect.y / backgroundConfig.backgroundSprite.texture.height,
                        backgroundConfig.backgroundSprite.rect.width / backgroundConfig.backgroundSprite.texture.width,
                        backgroundConfig.backgroundSprite.rect.height / backgroundConfig.backgroundSprite.texture.height));
        }
        else
        {
            // 绘制颜色背景
            EditorGUI.DrawRect(previewRect, backgroundConfig.backgroundColor);
        }

        // 添加边框
        EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, previewRect.width, 1), Color.black);
        EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, 1, previewRect.height), Color.black);
        EditorGUI.DrawRect(new Rect(previewRect.x + previewRect.width - 1, previewRect.y, 1, previewRect.height), Color.black);
        EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y + previewRect.height - 1, previewRect.width, 1), Color.black);

        // 名称标签
        EditorGUILayout.LabelField(backgroundConfig.name, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    // 绘制辅助方法 - 使用GUI而不是Handles
    void DrawCircle(float centerX, float centerY, float radius, Color color)
    {
        // 在EditorWindow中使用简单的圆形纹理绘制
        float size = radius * 2;
        Rect rect = new Rect(centerX - radius, centerY - radius, size, size);
        
        // 创建圆形纹理
        Texture2D circleTexture = CreateCircleTexture((int)size, color);
        GUI.DrawTexture(rect, circleTexture);
        
        // 清理纹理
        if (Application.isEditor)
        {
            DestroyImmediate(circleTexture);
        }
        else
        {
            Destroy(circleTexture);
        }
    }

    void DrawRect(float centerX, float centerY, float width, float height, Color color)
    {
        // 在EditorWindow中使用矩形绘制
        Rect rect = new Rect(centerX - width * 0.5f, centerY - height * 0.5f, width, height);
        EditorGUI.DrawRect(rect, color);
    }

    void DrawTriangle(float centerX, float centerY, float size, Color color)
    {
        // 在EditorWindow中使用简单的三角形纹理绘制
        Rect rect = new Rect(centerX - size * 0.5f, centerY - size * 0.5f, size, size);
        
        // 创建三角形纹理
        Texture2D triangleTexture = CreateTriangleTexture((int)size, color);
        GUI.DrawTexture(rect, triangleTexture);
        
        // 清理纹理
        if (Application.isEditor)
        {
            DestroyImmediate(triangleTexture);
        }
        else
        {
            Destroy(triangleTexture);
        }
    }

    void DrawDiamond(float centerX, float centerY, float size, Color color)
    {
        // 在EditorWindow中使用简单的菱形纹理绘制
        Rect rect = new Rect(centerX - size * 0.5f, centerY - size * 0.5f, size, size);
        
        // 创建菱形纹理
        Texture2D diamondTexture = CreateDiamondTexture((int)size, color);
        GUI.DrawTexture(rect, diamondTexture);
        
        // 清理纹理
        if (Application.isEditor)
        {
            DestroyImmediate(diamondTexture);
        }
        else
        {
            Destroy(diamondTexture);
        }
    }
    
    // 创建圆形纹理
    Texture2D CreateCircleTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.5f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = distance <= radius ? color : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    // 创建三角形纹理
    Texture2D CreateTriangleTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float halfSize = size * 0.5f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                if (IsPointInTriangle(point, center, halfSize))
                {
                    pixels[y * size + x] = color;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    // 创建菱形纹理
    Texture2D CreateDiamondTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float halfSize = size * 0.5f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                if (IsPointInDiamond(point, center, halfSize))
                {
                    pixels[y * size + x] = color;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    // 判断点是否在三角形内
    bool IsPointInTriangle(Vector2 point, Vector2 center, float size)
    {
        Vector2 p1 = center + new Vector2(0, size * 0.5f);
        Vector2 p2 = center + new Vector2(-size * 0.5f, -size * 0.5f);
        Vector2 p3 = center + new Vector2(size * 0.5f, -size * 0.5f);
        
        return IsPointInTriangle(point, p1, p2, p3);
    }
    
    bool IsPointInTriangle(Vector2 point, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = Sign(point, p1, p2);
        float d2 = Sign(point, p2, p3);
        float d3 = Sign(point, p3, p1);
        
        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        
        return !(hasNeg && hasPos);
    }
    
    float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
    
    // 判断点是否在菱形内
    bool IsPointInDiamond(Vector2 point, Vector2 center, float size)
    {
        Vector2 diff = point - center;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) <= size;
    }
} 