# ServerTool
Warframe Auto Restart Tool - POC

This tool will enumerate all window handles and read the Warframe window handle to retrieve the title (e.g. Retail Windows x86 x player(s)....)
This will be used to verify that the server is not restarted if active players are on the server. If it's 0 players then server is restarted on 
the pre-set intervall hours. If there are players online, then the interval is skipped.
