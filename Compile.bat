:: ========= Build ==========

devenv "D:\Git Repository\Prison Labor Mod\Source\PrisonLabor.sln" /build Debug



:: ========= Copy ==========
 
rd "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor" /s /q
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor"

:: About
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\About"
xcopy "D:\Git Repository\Prison Labor Mod\About\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\About" /e

:: Assemblies
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Assemblies"
xcopy "D:\Git Repository\Prison Labor Mod\Assemblies\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Assemblies" /e

:: Defs 
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Defs"
xcopy "D:\Git Repository\Prison Labor Mod\Defs\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Defs" /e

:: Languages
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Languages"
xcopy "D:\Git Repository\Prison Labor Mod\Languages\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Languages" /e

:: Textures
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Textures"
xcopy "D:\Git Repository\Prison Labor Mod\Textures\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Textures" /e

:: changelog.txt
copy "D:\Git Repository\Prison Labor Mod\changelog.txt" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\changelog.txt"