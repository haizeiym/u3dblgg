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

    // 绘制辅助方法
    void DrawCircle(float centerX, float centerY, float radius, Color color)
    {
        int segments = 32;
        Vector3[] points = new Vector3[segments];
        
        for (int i = 0; i < segments; i++)
        {
            float angle = i * 2f * Mathf.PI / segments;
            points[i] = new Vector3(centerX + radius * Mathf.Cos(angle), centerY + radius * Mathf.Sin(angle), 0);
        }

        Handles.color = color;
        Handles.DrawPolyLine(points);
        Handles.DrawLine(points[segments - 1], points[0]);
    }

    void DrawRect(float centerX, float centerY, float width, float height, Color color)
    {
        float left = centerX - width * 0.5f;
        float top = centerY - height * 0.5f;
        
        Vector3[] points = new Vector3[]
        {
            new Vector3(left, top, 0),
            new Vector3(left + width, top, 0),
            new Vector3(left + width, top + height, 0),
            new Vector3(left, top + height, 0)
        };

        Handles.color = color;
        Handles.DrawPolyLine(points);
        Handles.DrawLine(points[3], points[0]);
    }

    void DrawTriangle(float centerX, float centerY, float size, Color color)
    {
        Vector3[] points = new Vector3[]
        {
            new Vector3(centerX, centerY + size * 0.5f, 0),
            new Vector3(centerX - size * 0.5f, centerY - size * 0.5f, 0),
            new Vector3(centerX + size * 0.5f, centerY - size * 0.5f, 0)
        };

        Handles.color = color;
        Handles.DrawPolyLine(points);
        Handles.DrawLine(points[2], points[0]);
    }

    void DrawDiamond(float centerX, float centerY, float size, Color color)
    {
        Vector3[] points = new Vector3[]
        {
            new Vector3(centerX, centerY + size * 0.5f, 0),
            new Vector3(centerX + size * 0.5f, centerY, 0),
            new Vector3(centerX, centerY - size * 0.5f, 0),
            new Vector3(centerX - size * 0.5f, centerY, 0)
        };

        Handles.color = color;
        Handles.DrawPolyLine(points);
        Handles.DrawLine(points[3], points[0]);
    }
} 