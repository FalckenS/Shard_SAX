/*
*
*   The Game class is the entry way to the system, and it's set in the config file.  The overarching class
*       that drives your game (think of it as your main program) should extend from this.
*   @author Michael Heron
*   @author Samuel Falck (added some stuff for 3d collision detection and did some refactoring)
*   @version 1.0 (SAX)
*   
*/

using System;

namespace Shard;

abstract class Game
{
    private AssetManagerBase assetManagerBase;
    
    public bool Is3d { get; private set; } = false;

    protected AssetManagerBase getAssetManager()
    {
        return assetManagerBase ??= Bootstrap.getAssetManager();
    }

    public abstract void initialize();
    public abstract void update();

    public virtual bool isRunning()
    {
        return true;
    }

    // By default, our games will run at the maximum speed possible, but note that we have millisecond timing precision.
    // Any frame rate that needs greater precision than that will start to go... weird.
    public virtual int getTargetFrameRate()
    {
        return int.MaxValue; 
    }
}