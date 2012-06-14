@REM Copyright (C) Microsoft Corporation. All rights reserved.

@if not "%_echo%"=="1" (echo off)
setlocal ENABLEDELAYEDEXPANSION

@REM Usage: runtests.cmd [Release|Debug]

@REM Add .NET 2.0 Framework directory to the path to find msbuild.exe.
set PATH=%windir%\Microsoft.NET\Framework\v2.0.50727;%PATH%

@REM Call msbuild.exe to invoke the runtests target, which runs the tests.
call msbuild.exe /t:RunTests /p:Configuration=%~1 "%~dp0runtests.targets"
exit /b !ERRORLEVEL!
