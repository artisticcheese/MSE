@REM Copyright (C) Microsoft Corporation. All rights reserved.

@if not "%_echo%"=="1" (echo off)
setlocal ENABLEDELAYEDEXPANSION

@REM Usage: build.cmd [Rebuild|Clean] [Debug|Release|FxCop-Debug|FxCop-Release]

@REM Add .NET 2.0 Framework directory to the path to find msbuild.exe.
@REM set PATH=%windir%\Microsoft.NET\Framework\v2.0.50727;%PATH%

set TARGET=%~1
set BUILD_TYPE=%~2

call msbuild.exe "/t:%TARGET%" "/p:Configuration=%BUILD_TYPE%" "%~dp0..\StackExplorer\mse.sln"
if not "!ERRORLEVEL!"=="0" (exit /b !ERRORLEVEL!)

if /i "%BUILD_TYPE%"=="debug" (
  call msbuild.exe "/t:%TARGET%" "/p:Configuration=%BUILD_TYPE%" "%~dp0..\Sample\Sample.sln"
)

if /i "%BUILD_TYPE%"=="release" (
  call msbuild.exe "/t:%TARGET%" "/p:Configuration=%BUILD_TYPE%" "%~dp0..\Sample\Sample.sln"
)

exit /b !ERRORLEVEL!
