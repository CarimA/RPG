set /p Input=Enter filename of shader to compile:
mgfxc "PhotoVs\content\Shaders\%Input%.fx" "PhotoVs\content\Shaders\%Input%.dx11" /Profile:DirectX_11
mgfxc "PhotoVs\content\Shaders\%Input%.fx" "PhotoVs\content\Shaders\%Input%.ogl" /Profile:OpenGL
pause