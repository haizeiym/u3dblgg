using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelManagerWindow : EditorWindow
{
    private Vector2 scrollPos;
    private string[] levelFiles;
    private string levelsDir;

    [MenuItem("Tools/Level Editor/关卡管理")]
    public static void ShowWindow()
    {
        GetWindow<LevelManagerWindow>("关卡管理");
    }

    void OnEnable()
    {
        levelsDir = Path.Combine(Application.dataPath, "SavedLevels");
        RefreshLevelFiles();
    }

    void RefreshLevelFiles()
    {
        if (Directory.Exists(levelsDir))
            levelFiles = Directory.GetFiles(levelsDir, "*.json");
        else
            levelFiles = new string[0];
    }

    void OnGUI()
    {
        if (GUILayout.Button("刷新关卡列表")) RefreshLevelFiles();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var file in levelFiles)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Path.GetFileName(file));
            if (GUILayout.Button("加载", GUILayout.Width(60)))
            {
                ImportLevelFromFile(file);
            }
            if (GUILayout.Button("重命名", GUILayout.Width(60)))
            {
                RenameLevelFile(file);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    void ImportLevelFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var level = LevelDataExporter.LoadFromJson(json);
        if (level != null)
        {
            var editorUI = GameObject.FindObjectOfType<LevelEditorUI>();
            if (editorUI != null)
            {
                editorUI.ClearAllUIAndSelection();
                editorUI.currentLevel = level;
                if (level.layers.Count > 0)
                    editorUI.currentLayer = level.layers[0];
                
                // 设置当前关卡的文件路径（用于覆盖保存）
                editorUI.SetCurrentLevelFilePath(filePath);
                
                editorUI.RefreshUI();
                Debug.Log("关卡已加载: " + Path.GetFileName(filePath));
            }
            else
            {
                Debug.LogError("未找到LevelEditorUI实例，无法加载关卡");
            }
        }
        else
        {
            Debug.LogError("关卡文件解析失败: " + filePath);
        }
    }

    void RenameLevelFile(string filePath)
    {
        string currentName = Path.GetFileNameWithoutExtension(filePath);
        string newName = EditorUtility.SaveFilePanel("重命名关卡", levelsDir, currentName, "json");
        
        if (!string.IsNullOrEmpty(newName))
        {
            string newPath = newName;
            if (!newPath.EndsWith(".json"))
                newPath += ".json";
            
            if (File.Exists(newPath))
            {
                EditorUtility.DisplayDialog("错误", "文件已存在，请选择其他名称", "确定");
                return;
            }
            
            try
            {
                File.Move(filePath, newPath);
                Debug.Log("关卡已重命名: " + Path.GetFileName(filePath) + " → " + Path.GetFileName(newPath));
                RefreshLevelFiles();
            }
            catch (System.Exception e)
            {
                Debug.LogError("重命名失败: " + e.Message);
                EditorUtility.DisplayDialog("错误", "重命名失败: " + e.Message, "确定");
            }
        }
    }
} 