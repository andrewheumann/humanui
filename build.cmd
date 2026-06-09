@echo off
REM Build the HumanUI .gha, stage the release outputs into dist\, and pack a .yak.
REM Requires Visual Studio 2022 (MSBuild 17) and Rhino 8 (for yak.exe).

setlocal
set REPO=%~dp0
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"
set YAK="C:\Program Files\Rhino 8\System\Yak.exe"

%MSBUILD% "%REPO%HumanUI.sln" -t:Rebuild -p:Configuration=Release -v:minimal || exit /b 1

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
