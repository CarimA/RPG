set /p Input=Enter filename of shader to compile:
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" "PhotoVs\content\Shaders\%Input%.fx" "PhotoVs\content\Shaders\%Input%.dx11" /Profile:DirectX_11
"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\2mgfx.exe" "PhotoVs\content\Shaders\%Input%.fx" "PhotoVs\content\Shaders\%Input%.ogl" /Profile:OpenGL
pause