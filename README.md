# UnityShaderCompiler

## 简介

编译Unity引擎的Shader文件，能够将其转换为各个图形API的原生Shader，并提供全面的Shader性能分析功能，支持导出详细的性能分析报告。

## 功能

### 编译Unity Shader

![image-20250402220454088](E:\UnityShaderCompiler\imgs\image-20250402220454088.png)

编译后的Shader源码，以gles3.0为例

![image-20250402220932520](E:\UnityShaderCompiler\imgs\image-20250402220932520.png)

### 生成Shader性能报告

![image-20250402220959951](E:\UnityShaderCompiler\imgs\image-20250402220959951.png)

### 导出Shader性能报告

![10591c2e4b679a0b3b5e34ced66fc1eb](E:\UnityShaderCompiler\imgs\10591c2e4b679a0b3b5e34ced66fc1eb.png)

## 如何使用

将**Assets->UnityShaderCompiler**目录复制到目标项目，用Unity打开项目后。在菜单项点击**Tools->Unity Shader Compiler**打开界面

### 编译配置

打开工具窗口选择**配置**选项卡

![image-20250402223004368](E:\UnityShaderCompiler\imgs\image-20250402223004368.png)

#### 变体文件收集

收集需要参与编译的Shader变体文件，可以手动添加。也可以通过扫描路径添加。完成配置后，点击**保存配置**

#### 编译

在设置完**编译平台**和**编译目标**以及要编译的Shader变体文件后，点击**开始编译**

### 文件

打开工具窗口选择**文件**选项卡，该界面显示所有编译后的Shader文件。

![image-20250402224644751](E:\UnityShaderCompiler\imgs\image-20250402224644751.png)

#### 生成报告

设置好**使用工具**和**目标架构**后，点击**生成报告**按钮。生成报告保存在设置好的Report文件夹中

#### 导出csv报告

点击**导出csv报告**按钮，选择保存路径。该功能目前只支持AOC报告

### 设置

打开工具窗口选择**设置**选项卡

![image-20250402221528876](E:\UnityShaderCompiler\imgs\image-20250402221528876.png)

#### 离线编译器设置

用于生成Shader性能报告，需要自行下载。界面右下角提供了**下载链接**

#### 解析器设置

用于解析导入Shader性能报告，默认在该项目目录**shader_report_parser.exe**

#### 输出路径设置

用于保存Shader以及Shader性能报告的路径（目前需要放在Assets目录下，以支持通过界面打开文件）
