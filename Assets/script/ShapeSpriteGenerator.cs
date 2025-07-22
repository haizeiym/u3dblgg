using UnityEngine;

/// <summary>
/// 形状精灵生成器
/// 负责生成各种形状的精灵
/// </summary>
public class ShapeSpriteGenerator
{
    public static Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(point, center);
                
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateRectangleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (x >= 2 && x < size - 2 && y >= 2 && y < size - 2)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateTriangleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        Vector2[] triangle = new Vector2[]
        {
            new Vector2(size / 2f, size - 4f),
            new Vector2(4f, 4f),
            new Vector2(size - 4f, 4f)
        };
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y);
                if (IsPointInTriangle(point, triangle))
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateDiamondSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        Vector2[] diamond = new Vector2[]
        {
            new Vector2(size / 2f, size - 4f),
            new Vector2(size - 4f, size / 2f),
            new Vector2(size / 2f, 4f),
            new Vector2(4f, size / 2f)
        };
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 point = new Vector2(x, y);
                if (IsPointInPolygon(point, diamond))
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    static bool IsPointInTriangle(Vector2 point, Vector2[] triangle)
    {
        if (triangle.Length != 3) return false;
        
        Vector2 v0 = triangle[2] - triangle[0];
        Vector2 v1 = triangle[1] - triangle[0];
        Vector2 v2 = point - triangle[0];
        
        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);
        
        float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
        
        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }
    
    static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        if (polygon.Length < 3) return false;
        
        int intersections = 0;
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[(i + 1) % polygon.Length];
            
            if (((p1.y > point.y) != (p2.y > point.y)) &&
                (point.x < (p2.x - p1.x) * (point.y - p1.y) / (p2.y - p1.y) + p1.x))
            {
                intersections++;
            }
        }
        return (intersections % 2) == 1;
    }
} 