using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string levelName;
    public List<LayerData> layers = new List<LayerData>();

    public LevelData(string name)
    {
        levelName = name;
    }
}

[Serializable]
public class LayerData
{
    public string layerName;
    public List<ShapeData> shapes = new List<ShapeData>();

    public LayerData(string name)
    {
        layerName = name;
    }
}

[Serializable]
public class ShapeData
{
    public string shapeType;
    public Vector2 position;
    public float rotation;
    public List<BallData> balls = new List<BallData>();

    public ShapeData(string type, Vector2 pos, float rot)
    {
        shapeType = type;
        position = pos;
        rotation = rot;
    }
}

[Serializable]
public class BallData
{
    public string ballType;
    public Vector2 position;

    public BallData(string type, Vector2 pos)
    {
        ballType = type;
        position = pos;
    }
}

public static class LevelDataExporter
{
    public static string SaveToJson(LevelData data)
    {
        // 构造导出对象，确保字段完全一致
        var exportObj = new ExportLevelData(data);
        return JsonUtility.ToJson(exportObj, true);
    }

    public static LevelData LoadFromJson(string json)
    {
        return JsonUtility.FromJson<LevelData>(json);
    }
}

[Serializable]
public class ExportLevelData
{
    public string levelName;
    public List<ExportLayerData> layers;
    public ExportLevelData(LevelData src)
    {
        levelName = src.levelName;
        layers = new List<ExportLayerData>();
        foreach (var l in src.layers)
        {
            layers.Add(new ExportLayerData(l));
        }
    }
}
[Serializable]
public class ExportLayerData
{
    public string layerName;
    public List<ExportShapeData> shapes;
    public ExportLayerData(LayerData src)
    {
        layerName = src.layerName;
        shapes = new List<ExportShapeData>();
        foreach (var s in src.shapes)
        {
            shapes.Add(new ExportShapeData(s));
        }
    }
}
[Serializable]
public class ExportShapeData
{
    public string shapeType;
    public ExportVec2 position;
    public float rotation;
    public List<ExportBallData> balls;
    public ExportShapeData(ShapeData src)
    {
        shapeType = src.shapeType;
        position = new ExportVec2(src.position);
        rotation = src.rotation;
        balls = new List<ExportBallData>();
        foreach (var b in src.balls)
        {
            balls.Add(new ExportBallData(b));
        }
    }
}
[Serializable]
public class ExportBallData
{
    public string ballType;
    public ExportVec2 position;
    public ExportBallData(BallData src)
    {
        ballType = src.ballType;
        position = new ExportVec2(src.position);
    }
}
[Serializable]
public class ExportVec2
{
    public float x;
    public float y;
    public ExportVec2(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }
} 