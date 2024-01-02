@echo off
chcp 65001
setlocal

rem 获取Batch文件所在的目录
set "batchDir=%~dp0"

set "toolFolderPath=%batchDir%\..\Tools"
set "inputFolderDir=%batchDir%..\Cfg"
set "csharpCfgOutFolderDir=%batchDir%\..\Framework\Assets\Scripts\Cfg

rem 构建C#可执行文件的相对路径
set "exePath=%batchDir%\..\Tools\CfgGenerator\bin\Debug\CfgGenerator.exe"

echo 执行C#控制台程序并传递参数
"%exePath%" "%toolFolderPath%" "%inputFolderDir%" "%csharpCfgOutFolderDir%"

pause