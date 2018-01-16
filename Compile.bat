:: ========= Build ==========

devenv "Source\PrisonLabor.sln" /build Debug



:: ========= Copy ==========
 
rd "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor" /s /q
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor"

:: About
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\About"
xcopy "About\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\About" /e

:: Assemblies
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Assemblies"
xcopy "Assemblies\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Assemblies" /e

:: Defs 
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Defs"
xcopy "Defs\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Defs" /e

:: Languages
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Languages"
xcopy "Languages\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Languages" /e

:: Textures
mkdir "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Textures"
xcopy "Textures\*.*" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\Textures" /e

:: changelog.txt
copy "changelog.txt" "D:\Program Files\Steam\steamapps\common\RimWorld\Mods\PrisonLabor\changelog.txt"