This is a basic plugin loader setup for human fall flat.

It works by first merging the RealHooks.dll file with the Assembly-CSharp.dll file. This allows for seporation of the code used to load the main dll.
Its mainly for my sanity because one does not simply edit alot of MSil asm by hand.

The next bit does involve editing the MSIL of Game.Initialize function and is just an addition of a single line.

you just need to add
call	void Realhooks.loader::'init'()
below the line
stsfld	class Game Game::'instance'
and thats it.

The plugin loading code is held within human-fall-flat-hax and PluginContract.

the first dll holds the actual plugin loading code while the second one provides an interface to the plugins. 

in essence it allows me to load all the plugin dlls into a list and simply perform a foreach operation to initialize them.

In order to correctly write a plugin for this thing you must use dot net 3.5 because it is the most compatable with the mono engine used by unity.
