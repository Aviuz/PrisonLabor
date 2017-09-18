:: ========= Build ==========

devenv "D:\Git Repository\Prison Labor Mod\Source\PrisonLabor.sln" /build Debug



:: ========= Copy ==========
 
rd "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor" /s /q
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor"

:: About
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\About"
copy "D:\Git Repository\Prison Labor Mod\About\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\About"

:: Assemblies
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Assemblies"
copy "D:\Git Repository\Prison Labor Mod\Assemblies\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Assemblies"

:: Defs 
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Defs"
copy "D:\Git Repository\Prison Labor Mod\Defs\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Defs"

:: Languages
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Languages"
copy "D:\Git Repository\Prison Labor Mod\Languages\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Languages"

:: Textures
mkdir "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Textures"
copy "D:\Git Repository\Prison Labor Mod\Textures\*.*" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\Textures"

:: changelog.txt
copy "D:\Git Repository\Prison Labor Mod\changelog.txt" "D:\Games\SteamLibrary\SteamApps\common\RimWorld\Mods\PrisonLabor\changelog.txt"



:: ========= Run ==========

start steam://rungameid/294100