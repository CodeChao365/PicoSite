# PicoSite — 零配置静态站点生成器

> 写 Markdown，运行两条命令，得到一个网站。

PicoSite 是一个轻量级的静态站点生成器（SSG），基于 **.NET 10** 构建。  
无需安装 Node.js、无需配置文件，下载即用。

**PicoSite 的理想是：简单，没负担，好用。**

传统 SSG 多基于 Node.js 生态，功能强大但依赖臃肿、构建缓慢。  
PicoSite 走另一条路：**Markdown 写内容，Liquid 做主题，.NET 当引擎**。

- **Markdig** 解析 Markdown — 最快、最标准的 .NET Markdown 库  
- **Fluid** 渲染模板（Liquid 语法） — 高性能，AOT 兼容，被 Orchard Core 使用  
- **PicoServer** 托管预览 + 热重载 — 纯 .NET 内置，零外部依赖  
- **AOT 编译** — 可打包为 4-10 MB 的单文件，拷贝即用

---

## 快速开始

```bash
# 先进入演示项目
cd sample

# 开发预览（支持热重载）
dotnet run --project ../src -- serve

# 生成静态文件
dotnet run --project ../src -- build
```

然后打开 http://localhost:8080 就能看到网站了。

---

## 特性

- **零配置** — 自动检测 `content/` → `docs/` → `./`，放哪都能跑
- **热重载** — 修改 Markdown 后浏览器自动刷新，无需手动 F5
- **Markdown + YAML Front Matter** — 标题、日期、自定义字段
- **Liquid 模板引擎** — 主题系统，支持 `{% include %}`、`{% for %}` 等标签
- **双模式** — `serve` 动态预览 / `build` 生成可部署的 HTML
- **跨平台** — Windows / macOS / Linux
- **单文件发布** — 支持 AOT 编译为 4-10MB 的可执行文件

---

## 命令

| 命令 | 说明 |
|------|------|
| `serve` | 启动预览服务器，支持热重载 |
| `build` | 生成静态文件到 `_site/` |

### 选项

| 参数 | 适用命令 | 默认值 |
|------|----------|--------|
| `--port <端口>` | `serve` | 8080 |
| `--theme <主题名>` | `serve`, `build` | default |
| `--output <目录>` | `build` | `./_site` |

---

## 项目结构

```
├── PicoSite/             # C# 源代码
│   ├── Program.cs        # 入口
│   ├── Commands/         # CLI 命令
│   ├── Models/           # 数据模型
│   ├── Services/         # 核心服务
│   └── Themes/           # 主题模板
├── sample/               # 演示项目
│   ├── picosite.json     # 站点配置
│   └── content/          # Markdown 源文件
│       ├── index.md      # 首页
│       ├── about.md      # 关于页
│       └── blog/post-1.md
├── README.md
└── .gitignore
```

---

## 配置

在项目根目录创建 `picosite.json`（可选）：

```json
{
  "title": "我的站点",
  "description": "用 PicoSite 搭建的站点",
  "theme": "default",
  "port": 8080,
  "output": "./_site"
}
```

所有字段可选，不配置则使用默认值。

---

## 编写内容

每个 Markdown 文件可包含 YAML Front Matter：

```markdown
---
title: 文章标题
date: 2026-06-09
---

## 正文

支持 **Markdown** 语法。
```

| 字段 | 说明 |
|------|------|
| `title` | 页面标题 |
| `date` | 发布日期（可选） |

文件路径自动映射为 URL：

| Markdown 文件 | URL |
|--------------|-----|
| `content/index.md` | `/` |
| `content/about.md` | `/about` |
| `content/blog/post-1.md` | `/blog/post-1` |

---

## 多项目支持

每个目录就是一个独立项目。

```
my-project-a/         my-project-b/
├── content/          ├── content/
├── picosite.json     ├── picosite.json
└── _site/            └── _site/
```

分别在各自目录运行命令即可：

```bash
cd my-project-a
dotnet run --project ../src -- serve --port 8080

# 另一个终端
cd my-project-b
dotnet run --project ../src -- serve --port 8081
```

PicoSite 自动检测当前目录下的 `content/`、读取当前目录的 `picosite.json`。  
用 `--port` 分开端口就不会冲突。

---

## 主题系统

主题目录结构：

```
Themes/default/
├── index.html      # 主布局
├── header.html     # 头部片段
├── sidebar.html    # 侧边栏片段
└── assets/style.css # 样式
```

模板中可用的 Liquid 变量：

| 变量 | 类型 | 说明 |
|------|------|------|
| `{{ site.title }}` | 字符串 | 站点标题 |
| `{{ site.description }}` | 字符串 | 站点描述 |
| `{{ site.pages }}` | 数组 | 所有页面 |
| `{{ page.title }}` | 字符串 | 当前页面标题 |
| `{{ page.url }}` | 字符串 | 当前页面 URL |
| `{{ page.date }}` | 字符串 | 页面日期（从 Front Matter 读取） |
| `{{ page.excerpt }}` | 字符串 | 页面摘要 |
| `{{ content }}` | 字符串 | Markdown 渲染后的 HTML |
| `{{ theme.assets }}` | 字符串 | 主题资源路径 |

#### 可用 Liquid 标签

| 标签 | 示例 | 说明 |
|------|------|------|
| `{% include %}` | `{% include sidebar.html %}` | 包含子模板 |
| `{% for %}` | `{% for p in site.pages %}` | 循环 |
| `{% if %}` | `{% if page.url == '/' %}` | 条件判断 |
| `{% assign %}` | `{% assign t = page.title %}` | 赋值变量 |
| `{% capture %}` | `{% capture h %}...{% endcapture %}` | 捕获输出到变量 |

---

## 技术栈

| 组件 | 用途 |
|------|------|
| **.NET 10** | 运行时 |
| **Markdig** | Markdown → HTML |
| **Fluid.Core** | Liquid 模板引擎（AOT 兼容） |
| **PicoServer** | HTTP 服务器 + WebSocket 热重载 |
| **System.CommandLine** | CLI 框架 |

---

## 构建与发布

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build

# 发布为单文件
dotnet publish -c Release -r win-x64 --self-contained true

# 运行
dotnet run -- serve

# 测试
dotnet test
```

---

## 路线图

| 功能 | 说明 |
|------|------|
| 主题市场 | 社区贡献一键切换 |
| 插件系统 | 自定义处理管道 |
| 多语言 | 自动检测 `zh/` `en/` 目录生成双语站 |
| 侧边栏自动生成 | 根据目录结构生成导航 |
| API 文档 | 从 .NET XML 注释生成接口文档 |

---

## 贡献指南

欢迎贡献代码、主题或文档：

- **代码** — 提交 Pull Request 到 `main` 分支  
- **主题** — 放在 `Themes/` 目录，遵循主题开发规范  
- **文档** — 改进 `README.md` 或补充使用教程  

---

## 开源协议

MIT
