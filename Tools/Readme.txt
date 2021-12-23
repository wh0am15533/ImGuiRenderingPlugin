This tool is used on newer versions of Unity to preload our plugin or it won't get initialized,
with the required pointer to UnityInterfaces. If the API check fail's when loading your plugin
the run this tool. It has no side-effects if you remove your plugin or it doesn't exist.

Usage:   pluginspatcher "fullpath_to_game_data_folder"
Example: pluginspatcher "C:\Games\Bloons TD 6\BloonsTD6_Data"