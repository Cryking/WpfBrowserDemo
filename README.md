# WpfBrowserDemo

一个基于 WPF 的多浏览器引擎演示项目，支持集成 WebView2 和 CefSharp 两种浏览器引擎。

## 项目简介

WpfBrowserDemo 是一个演示如何在 WPF 应用程序中使用多种浏览器引擎的示例项目。该项目集成了 Microsoft WebView2 和 CefSharp 两种浏览器控件，允许用户在不同引擎之间切换并进行网页浏览。

## 功能特性

- **多浏览器引擎支持**
  - **WebView2**: 基于 Microsoft Edge Chromium 的现代浏览器控件
  - **CefSharp**: 基于 Chromium 的开源浏览器框架

- **基本浏览器功能**
  - 网页导航
  - 页面刷新
  - 停止加载
  - 地址栏输入和更新

- **CefSharp 特性**
  - 忽略证书错误
  - 媒体流支持（屏幕共享、摄像头、麦克风）
  - 打印预览支持
  - 自定义缓存路径
  - 日志记录

## 技术栈

- **框架**: .NET 6.0
- **UI 框架**: WPF (Windows Presentation Foundation)
- **主要依赖**:
  - `CefSharp.Wpf.HwndHost` v109.1.110
  - `Microsoft.Web.WebView2` v1.0.2277.86

## 系统要求

- Windows 7 或更高版本
- .NET 6.0 Runtime 或 SDK
- Microsoft Edge WebView2 Runtime（使用 WebView2 时需要）

## 项目结构

```
WpfBrowserDemo/
├── App.xaml                 # 应用程序入口
├── App.xaml.cs             # CefSharp 初始化配置
├── MainWindow.xaml         # 主窗口 UI 定义
├── MainWindow.xaml.cs      # 主窗口逻辑代码
└── WpfBrowserDemo.csproj   # 项目配置文件
```

## 构建和运行

### 前置条件

1. 安装 [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. 安装 [Visual Studio 2022](https://visualstudio.microsoft.com/) 或更高版本
3. （可选）安装 Microsoft Edge WebView2 Runtime

### 构建步骤

1. 克隆或下载项目代码
2. 使用 Visual Studio 打开 `WpfBrowserDemo.sln` 解决方案
3. 还原 NuGet 包（自动或手动执行 `dotnet restore`）
4. 选择目标平台（AnyCPU）
5. 按 F5 或点击"启动"按钮运行项目

### 命令行构建

```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build --configuration Release

# 运行项目
dotnet run
```

## 使用说明

1. **选择浏览器引擎**: 在顶部工具栏选择 "WebView2" 或 "CefSharp" 单选按钮
2. **导航**: 在地址栏输入 URL，点击"导航"按钮
3. **刷新**: 点击"刷新"按钮重新加载当前页面
4. **停止**: 点击"停止"按钮中断页面加载

## 配置说明

### CefSharp 配置

CefSharp 的配置位于 `App.xaml.cs` 的 `InitCefSharp` 方法中：

- **日志路径**: `log/cef.log`
- **缓存路径**: `%LocalAppData%/CefSharp/Cache`
- **语言设置**: zh-CN
- **证书验证**: 已忽略（适合测试环境）
- **媒体流**: 已启用（包括屏幕共享）

## 常见问题

### WebView2 初始化失败

确保已安装 Microsoft Edge WebView2 Runtime，可以从以下地址下载：
https://developer.microsoft.com/microsoft-edge/webview2/

### CefSharp 加载失败

确保 CefSharp 的依赖文件（.dll、.pak 等）在正确的位置，并且具有执行权限。

## 许可证

本项目仅用于演示和学习目的。

## 相关链接

- [CefSharp 官方文档](https://cefsharp.github.io/)
- [Microsoft WebView2 文档](https://docs.microsoft.com/en-us/microsoft-edge/webview2/)
- [.NET 6.0 文档](https://docs.microsoft.com/en-us/dotnet/core/)
