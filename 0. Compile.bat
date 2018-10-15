:: ========= Variables =========
SET target_directory=D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor



:: ========= Build ==========

devenv "Source\PrisonLabor.sln" /build Debug



:: ========= Copy ==========
 
rd "%target_directory%" /s /q
mkdir "%target_directory%"

:: About
mkdir "%target_directory%\About"
xcopy "About\*.*" "%target_directory%\About" /e

:: Assemblies
mkdir "%target_directory%\Assemblies"
xcopy "Assemblies\*.*" "%target_directory%\Assemblies" /e

:: Defs 
mkdir "%target_directory%\Defs"
xcopy "Defs\*.*" "%target_directory%\Defs" /e

:: Languages
mkdir "%target_directory%\Languages"
xcopy "Languages\*.*" "%target_directory%\Languages" /e

:: Textures
mkdir "%target_directory%\Textures"
xcopy "Textures\*.*" "%target_directory%\Textures" /e

:: changelog.txt
copy "changelog.txt" "%target_directory%\changelog.txt"