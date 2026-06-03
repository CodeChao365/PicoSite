# PicoSite 静态站点生成器

## 技术选型

> **.NET 打底，Markdig 解析，DotLiquid 模板，PicoServer 托管。**

## 目标

**简单、轻量、易用、好用**

> *初版，部分内容可能会调整优化*

### 基本理念

传统 SSG 多基于 Node.js 生态，功能强大但依赖臃肿、构建缓慢。Go、Rust 生态虽有轻量级工具，但大多沿用「配置驱动+固定主题结构」的模式，学习成本和扩展门槛依然不低。

**PicoSite 的理想是：简单，没负担，好用。**

于是引入模板建站的「模板标签」方式：
- 模板引擎选用 **Liquid**（简洁安全，被 Shopify、Jekyll 广泛使用）
- 内容编写用 **Markdown**（创作者熟悉，普通人也能快速上手）
- 技术基座是 **.NET**（已开源多年，成熟稳定的跨平台框架）

## 简介（给所有人）

- 零配置，开箱即用
- 写 Markdown，运行两个命令，得到一个网站
- 既可以生成静态文件，也可以直接动态托管
- 跨平台（Windows / macOS / Linux）

### 快速开始

```bash
# 1. 下载 picosite.exe
# 2. 在放 Markdown 的文件夹运行

picosite serve    # 看到网站了
picosite build    # 生成静态文件
```



## 用户文档（给普通用户）

### 安装

下载 `picosite.exe`，放到系统 PATH 或项目目录中。

### 命令

| 命令 | 说明 |
|------|------|
| `picosite serve` | 启动预览服务器（http://localhost:8080） |
| `picosite build` | 生成静态文件到 `./_site/` |

### 命令行参数

| 命令 | 参数 | 说明 | 默认值 |
|------|------|------|--------|
| `serve` | `--port` | 预览端口 | 8080 |
| `serve` | `--theme` | 指定主题 | default |
| `build` | `--output` | 输出目录 | ./_site |
| `build` | `--theme` | 指定主题 | default |

### 主题

#### 使用默认主题

开箱即用，无需配置。

#### 切换主题

```bash
picosite serve --theme dark
```

主题放在 `./themes/` 目录下。

### 配置文件（可选）

创建 `picosite.json` 覆盖默认值：

```json
{
  "title": "我的文档站",
  "theme": "dark",
  "port": 3000
}
```

所有字段可选，不配置就用默认值。

---

## 贡献者文档（给开发者）

### 技术栈

| 技术 | 用途 | 选型理由 |
|------|------|----------|
| **.NET 10** | 跨平台基础 | 跨平台、AOT、单文件发布、性能好 |
| **Markdig** | Markdown 转 HTML | 最快、最标准、扩展性强 |
| **DotLiquid** | Liquid 语法主题 | 门槛低，懂 HTML 的人 30 分钟学会、AI友好 |
| **PicoServer** | 实时预览 + 热重载 + 动态托管 | 简单完善零依赖，跨平台、AOT |
| **System.CommandLine** | `serve` / `build` 命令 | 微软官方，标准可靠 |
| **System.Text.Json** | 配置文件解析 | .NET 原生，无需第三方依赖 |

### 约定规则（自动检测）

| 检测项 | 规则 | 示例 |
|--------|------|------|
| **源目录** | 依次检测 `./content/` → `./docs/` → `./` | 放哪都能跑 |
| **多语言** | 检测到 `zh/` `en/` 目录自动开启 | `/zh/` → 中文，`/en/` → 英文 |
| **首页** | `index.md` 作为目录默认页 | `/guide/` → `guide/index.md` |
| **路由** | 文件路径 = URL 路径 | `docs/getting-started.md` → `/docs/getting-started` |

### 主题开发

#### 主题目录结构

```
themes/default/
├── index.html        # 主布局（入口）
├── sidebar.html      # 片段
├── header.html
└── assets/
└── config.json         # 主题配置（可选）
```

#### 可用变量（Liquid 模板）

| 变量 | 类型 | 说明 |
|------|------|------|
| `site.title` | 字符串 | 站点标题 |
| `site.description` | 字符串 | 站点描述 |
| `site.pages` | 数组 | 所有页面（用于生成列表、导航） |
| `page.title` | 字符串 | 当前页面标题 |
| `page.url` | 字符串 | 当前页面 URL |
| `page.date` | 日期 | 页面日期（从 Front Matter 读取） |
| `page.excerpt` | 字符串 | 页面摘要 |
| `content` | 字符串 | Markdown 渲染后的 HTML |
| `theme.assets` | 字符串 | 主题资源路径 |

#### 可用标签

| 标签 | 示例 | 说明 |
|------|------|------|
| `{% include %}` | `{% include sidebar.liquid %}` | 包含子模板 |
| `{% for %}` | `{% for page in site.pages %}` | 循环 |
| `{% if %}` | `{% if page.url == '/' %}` | 条件判断 |
| `{% assign %}` | `{% assign title = page.title %}` | 赋值变量 |
| `{% capture %}` | `{% capture html %}...{% endcapture %}` | 捕获输出 |

### 开发与构建

```bash
# 克隆仓库
git clone https://github.com/xxx/picosite

# 还原依赖
dotnet restore

# 构建
dotnet build

# 运行
dotnet run -- serve

# 测试
dotnet test
```

---

## 路线图（给所有人）

### v1.0 必备功能

| 功能 | 说明 |
|------|------|
| `serve` + `build` | 预览和生成 |
| 自动检测源目录 | 放哪都能跑 |
| 默认主题 | 开箱即好看 |

### 后续迭代功能

| 功能 | 说明 |
|------|------|
| 主题切换 | 换主题换风格 |
| 插件 | 功能扩展 |
| 多语言 | 有 zh/en 就自动双语 |
| 侧边栏自动生成 | 目录变导航 |
| 配置文件 | 想定制就写 `picosite.json` |

---

## 贡献指南

欢迎贡献代码、主题或文档。

- **代码**：请提交 Pull Request 到 `main` 分支
- **主题**：主题放在 `themes/` 目录，遵循主题开发规范
- **文档**：欢迎改进文档或翻译

---

## 开源协议 MIT
