BoWind
=========
Another borderless windowed fullscreen application.

Downloads
=========
The [release page](https://github.com/Wsheerio/BoWind/releases) should have a build of the most recent version.

Run BoWind.exe to create config.txt and programs.txt or create the files yourself. With debug mode off simply run BoWind.exe after opening a game to have it resized.

Documentation
=========
config.txt

This is where you'll put your config settings, all entries are "option setting". Currently supports debug, background, and backgroundcolor which are bool, bool, and hex triplet respectively. Background and backgroundcolor are not implemented yet.

    debug true
    background false
    backgroundcolor 0CD34E

programs.txt

This is where you'll put your program settings, all entries are "aspect ratio width:aspect ratio height:option/setting". Currently supported options are c, n, and s which are compatibility mode (bool), process name, and shift (x,y,w,h) respectively. Compatibility mode grabs all related processes and resizes those instead of just the mainwindowhandle, this helps with some games but you'll probably never need to use it. Shift moves the application x pixels left and y pixels up and increases the width by w and height by h.

    16:9:n DarkSoulsII.s 2,2,4,4:n RustClient:n DOMO
    632:473:c true.n endless.b 25
    
Debug Menu

If debug is set to true the console will not be hidden and will stay open until you close it. Pressing enter will loop through window resizing again. After each window resize it will list the process ids and titles of the windows resized then the configuration used to resize them. This line is read "processname compatibility true/false aspect ratio x y - shift width height + shift". Using the program config provided above after resizing DarkSoulsII.exe and endless.exe you would see.

    (processid) DarkSoulsII
    DarkSoulsII compat false 1.77777777777778 0 - 2 0 - 2 1920 + 4 1080 + 4
    
    (processid) EndlessOnline
    (processid) Endless Online
    endless compat true 1.33615221987315 238 - 0 0 - 25 1443.044397463 + 0 1080 + 25
    
There are two commands you can use if debug is set to true, they are edit and reload. Edit opens up the given text file in your default text editor, reload reloads all config files.

    edit config
    edit programs
    reload
