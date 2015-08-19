BoWind
=========
Another borderless windowed fullscreen application.

Downloads
=========
The [release page](https://github.com/Wsheerio/BoWind/releases) should have a build of the most recent version.

Run BoWind.exe to create config.txt and programs.txt or create the files yourself. With debug mode off simply run BoWind.exe after opening a game to have it resized.

Documentation
=========
To comment out a line start it with two forward slashes.

config.txt

This is where you'll put your config settings, all entries are "option setting". Currently supports debug, background, and backgroundcolor which are bool, bool, and hex triplet respectively.

    debug true
    background false
    backgroundcolor 0CD34E

programs.txt

This is where you'll put your program settings, all entries start with an aspect ration then process names and options separeated by ':'. Options are "option/setting". Currently supported options are b and c which are vertical shift (integer) and compatibility mode (bool) respectively. Compatibility mode grabs all related processes and resizes those instead of just the mainwindowhandle, this helps with some games but you'll probably never need to use it. Vertical shift moves the application up and increases the height in order to combat borderless games that won't draw to where the titlebar used to be.

    16:9:DarkSoulsII:RustClient:DOMO
    632:473:endless.b/25.c/true
    
Debug Menu

If debug is set to true the console will not be hidden and will stay open until you close it. Pressing enter will loop through window resizing again. After each window resize it will list the process ids and titles of the windows resized then the configuration used to resize them. This line is read "processname compatibility true/false aspect ratio x y - shift width height + shift". Using the program config provided above after resizing DarkSoulsII.exe and endless.exe you would see.

    (processid) DarkSoulsII
    DarkSoulsII compat false 1.77777777777778 0 0 - 0 1920 1080 + 0
    
    (processid) EndlessOnline
    (processid) Endless Online
    endless compat true 1.33615221987315 238 0 - 25 1443.044397463 1080 + 25
    
There are two commands you can use if debug is set to true, they are edit and reload. Edit opens up the given text file in your default text editor, reload reloads all config files.

    edit config
    edit programs
    reload
