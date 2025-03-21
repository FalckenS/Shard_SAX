# The Shard game engine extended by group SAX
The game engine **Shard** extended by group SAX for *TDA572 / DIT572 Game engine architecture* SP3 2024/2025. The starting point and initial commit is **Shard 1.3.0 - Horizons**.


## Build instructions
These build instructions are for windows machines. The build process should be similar on other operating systems as long as you have msbuild installed.
The target framework is .NET 8.0. To build, follow one of the following methods...

### Using Developer Command Prompt (Requires Visual Studio installation)
1. Make sure msbuild is installed. ``` dotnet msbuild --version ```
   *Note : msbuild is included in the Visual Studio installation, it is also included in the .net SDK.*
2. Open the **Developer Command Prompt for VS 2022** (On windows, just search for the application.)
3. Navigate to the project folder (folder with the Shard.csproj file in it) and run ```msbuild -v:normal -p:Configuration=RELEASE ```.

The build is generated under bin/RELEASE

### Using standard command line interface
1. Make sure msbuild is installed. ``` dotnet msbuild --version ```
   *Note : msbuild is included in the Visual Studio installation, it is also included in the .net SDK.*
2. Run ```dotnet msbuild -v:normal "PathToProject\Shard.csproj" -p:Configuration=RELEASE```  
**If this results in errors, do this instead:**  
Locate your MSBuild executable. (probably something like C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe)
And run ```"MSBuildPath\msbuild.exe" "ProjectPath\Shard.csproj" /p:Configuration=RELEASE /v:normal```

## Usage
Running the generated exe file runs the current setup. Change run settings in the Config.cfg file located in the project folder. Change environment variables in the envar.cfg file. To switch between games you have to manually change these files.

## Running Demo Games

### Flappy Bird
Change the Config.cfg to ...
```
display: DisplayOpenGL
sound: SoundSDL
game: GameFlappyBird
input: InputFramework
asset: AssetManager
```
Change the envar.cfg to...
```
assetpath: %BASE_DIR%\assets\
gravity_modifier: 0.1
gravity_dir: 0,-1
physics_debug: 0
```
Now run the generated exe file to start the game.
#### Instructions
Press space to flap.  
Do not hit the ground or ceiling.  
Avoid the pipes.  
Get as far as possible.  
*Note : Pressing R changes the player model.* 

### AimLab
Change the Config.cfg to ...
```
# Setup of internal systems
display: DisplayOpenGL3D
sound: SoundSDL
game: AimLab
input: InputFramework3D
asset: AssetManager
```
Change the envar.cfg file to ...
```
assetpath: %BASE_DIR%\assets\
gravity_modifier: 0.1
gravity_dir: 0,-1
physics_debug: 0
```
#### Instructions
You can move around with wasd, shoot with left mouse button.  
Aim at the blue square and try to shoot it and score points.  
Practice your aim by trying to hit the square faster and faster.  
*Note : Pressing E and Q lets you fly up and down.*
