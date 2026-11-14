@echo off
REM Build the HumanUI .gha, stage the release outputs into dist\, and pack a .yak.
REM Requires Visual Studio 2022+ (MSBuild 17) and Rhino 8 (for yak.exe).

setlocal
set REPO=%~dp0
REM Locate MSBuild from any installed VS 2022+ edition (Community/Professional/Enterprise).
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -version [17.0^,^) -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set MSBUILD="%%i"
if not defined MSBUILD (echo MSBuild not found - install Visual Studio 2022 or later. & exit /b 1)
set YAK="C:\Program Files\Rhino 8\System\Yak.exe"

%MSBUILD% "%REPO%HumanUI.sln" -restore -t:Rebuild -p:Configuration=Release -v:minimal || exit /b 1

del /Q "%REPO%dist\*.yak" 2>nul
copy /Y "%REPO%HumanUI\HumanUI\bin\Release\*.gha" "%REPO%dist\" >nul
REM Copy supplementary DLLs only — skip HumanUI.dll (it's the same bits as
REM HumanUI.gha and yak only needs the .gha; bundling both would just bloat
REM the package).
copy /Y "%REPO%HumanUI\HumanUI\bin\Release\*.dll" "%REPO%dist\" >nul
del /Q "%REPO%dist\HumanUI.dll" 2>nul
copy /Y "%REPO%HumanUI\HumanUI\bin\Release\Styles.Default.xaml" "%REPO%dist\" >nul

pushd "%REPO%dist"
%YAK% build || (popd & exit /b 1)
popd
