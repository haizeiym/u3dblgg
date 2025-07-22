# 拔罐小游戏关卡编辑器

一个基于Unity3D的关卡编辑器，用于创建和管理拔罐小游戏的关卡数据。

## 📋 功能特性

### 🎮 核心功能
- **关卡管理**：支持多个关卡，每个关卡包含多个层级
- **形状编辑**：支持圆形、矩形、三角形、菱形四种形状
- **球体放置**：在形状内放置不同颜色的球体
- **拖拽操作**：支持形状和球体的拖拽移动
- **旋转控制**：支持形状的旋转操作
- **属性编辑**：实时编辑位置、旋转、类型等属性
- **JSON导出**：将关卡数据导出为JSON格式

### 🎨 UI界面
- **左侧面板**：关卡和层级列表管理
- **中间编辑区**：可视化编辑区域
- **右侧属性面板**：属性编辑和导出功能
- **工具栏**：快速添加形状和球体

## 🚀 快速开始

### 1. 环境要求
- Unity 2021.3 或更高版本
- 支持UGUI系统

### 2. 一键配置
1. 打开Unity编辑器
2. 在菜单栏选择 `Tools/Level Editor/一键配置`
3. 系统会自动创建：
   - Canvas和EventSystem
   - 完整的关卡编辑器UI结构
   - 默认关卡数据

### 3. 开始编辑
配置完成后，你就可以开始编辑关卡了！

## 📖 详细使用说明

### 🎯 基本操作

#### 添加层级
1. 在左侧面板点击"添加层级"按钮
2. 新层级会自动添加到列表中
3. 点击层级名称可以切换当前编辑的层级

#### 删除层级
1. 选择要删除的层级
2. 点击"删除层级"按钮
3. 注意：至少保留一个层级

#### 添加形状
1. 在中间工具栏点击"添加形状"按钮
2. 形状会出现在编辑区域中心
3. 默认形状类型为圆形

#### 添加球体
1. 先选择一个形状（点击形状使其高亮）
2. 在工具栏点击"添加球"按钮
3. 球体会添加到选中的形状内

### 🎮 编辑操作

#### 拖拽移动
- **形状拖拽**：直接拖拽形状到新位置
- **球体拖拽**：拖拽球体在形状内移动
- **限制范围**：球体只能在所属形状内移动

#### 旋转操作
1. 选中要旋转的形状
2. 在右侧属性面板使用"旋转"滑块
3. 旋转角度范围：0-360度

#### 属性编辑
选中形状或球体后，在右侧面板可以编辑：
- **名称**：修改形状或球体的名称
- **X位置**：水平位置（-500到500）
- **Y位置**：垂直位置（-300到300）
- **旋转**：旋转角度（0到360度）
- **类型**：形状类型（圆形/矩形/三角形/菱形）

### 📊 数据管理

#### 导出JSON
1. 编辑完成后，点击右侧面板的"导出JSON"按钮
2. 关卡数据会自动复制到剪贴板
3. 在Console窗口可以看到完整的JSON数据

### 🎛️ 形状类型按钮列表

#### 形状类型按钮列表（使用Unity自带简单按钮）
- **全部列出**：所有形状类型（圆形、矩形、三角形、菱形）以按钮形式全部显示
- **直观选择**：点击按钮即可选择对应的形状类型
- **视觉反馈**：选中的按钮会高亮显示（蓝色），未选中的按钮为灰色
- **实时更新**：选择不同形状类型时，形状外观会实时更新
- **简单可靠**：使用Unity自带的Button组件，避免复杂的Dropdown模板问题

#### 按钮功能特性
- **一键切换**：点击任意按钮即可切换形状类型
- **状态同步**：选中形状时，对应的类型按钮会自动高亮
- **即时生效**：选择后立即应用到当前选中的形状
- **简单布局**：按钮垂直排列，间距合理

#### 使用方式
```csharp
// 使用UIComponentBuilder创建简单按钮
GameObject button = UIComponentBuilder.CreateButton(parent, "按钮文本", position);

// 形状类型按钮在LevelEditorUIBuilder中自动创建
// 包含4个按钮：圆形、矩形、三角形、菱形
// 点击按钮即可切换形状类型
```

#### 扩展功能（保留Dropdown支持）
如果需要Dropdown功能，仍可使用：
```csharp
// 创建球类型Dropdown
GameObject ballDropdown = DropdownBuilder.CreateBallTypeDropdown(parent, position);

// 创建自定义Dropdown
string[] options = { "选项1", "选项2", "选项3" };
GameObject dropdown = DropdownBuilder.CreateSimpleDropdown(parent, "标签", position, options);
```

#### JSON数据格式
```json
{
  "levelName": "关卡名称",
  "layers": [
    {
      "layerName": "层级名称",
      "shapes": [
        {
          "shapeType": "形状类型",
          "position": {"x": 100, "y": 50},
          "rotation": 45.0,
          "balls": [
            {
              "ballType": "球体类型",
              "position": {"x": 10, "y": 5}
            }
          ]
        }
      ]
    }
  ]
}
```

## 🛠️ 技术架构

### 📁 文件结构
```
Assets/script/
├── LevelData.cs                    # 数据结构定义
├── LevelEditorUI.cs                # 主控制器
├── LevelEditorUIManager.cs         # UI管理器
├── LevelEditorDataManager.cs       # 数据管理器
├── ShapeController.cs              # 形状控制器
├── BallController.cs               # 球体控制器
└── Editor/
    ├── LevelEditorMenu.cs          # 菜单项
    └── LevelEditorUIBuilder.cs     # UI构建器
```

### 🔧 核心组件

#### LevelData.cs (69行)
- 定义关卡数据结构
- 包含LevelData、LayerData、ShapeData、BallData类
- 提供JSON序列化功能

#### LevelEditorUI.cs (152行)
- 主控制器，管理所有UI组件
- 处理用户交互和事件
- 协调UI管理器和数据管理器

#### ShapeController.cs (317行)
- 形状的可视化表示
- 处理拖拽、旋转、点击事件
- 生成不同形状的Sprite

#### BallController.cs (179行)
- 球体的可视化表示
- 处理拖拽和点击事件
- 限制在形状内移动

## 🎨 自定义扩展

### 添加新的形状类型
1. 在`ShapeController.cs`的`UpdateShapeAppearance()`方法中添加新的case
2. 创建对应的Sprite生成方法
3. 在UI下拉框中添加新选项

### 添加新的球体类型
1. 在`BallController.cs`中添加新的颜色或样式
2. 修改`UpdateBallAppearance()`方法
3. 更新相关的UI显示

### 修改UI布局
1. 编辑`LevelEditorUIBuilder.cs`中的布局代码
2. 调整面板大小和位置
3. 添加新的UI控件

## 🐛 常见问题

### Q: 一键配置后没有显示UI？
A: 检查Console窗口是否有错误信息，确保Canvas和EventSystem正确创建。

### Q: 拖拽操作不响应？
A: 确保EventSystem存在，检查GraphicRaycaster组件是否正确配置。

### Q: 导出JSON失败？
A: 检查Console窗口的错误信息，确保关卡数据正确初始化。

### Q: 球体无法拖拽到形状外？
A: 这是正常行为，球体被限制在所属形状内移动。

## 📝 更新日志

### v1.0.0
- 初始版本发布
- 支持基本的关卡编辑功能
- 实现拖拽和旋转操作
- 支持JSON数据导出

## 🤝 贡献指南

欢迎提交Issue和Pull Request来改进这个项目！

## 📄 许可证

本项目采用MIT许可证，详见LICENSE文件。

---

**注意**：这是一个Unity编辑器工具，需要在Unity编辑器中运行。确保你的Unity版本支持UGUI系统。