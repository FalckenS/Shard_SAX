/*
*   @author Samuel Falck
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using GameFlappyBird;

namespace Shard;

internal class GameFlappyBird : Game
{
    private const int GapBetweenWallsX = 300;
    private const int GapBetweenWallsY = 300;
    private const int ShortestAllowedWall = 0;
    
    public bool GameOver { get; set; }
    private Bird _bird;
    private List<List<Wall>> _walls; // Every pair of walls is a list
    private Random _random;
    private int _score;
        
    public override void initialize()
    {
        GameOver = false;
        _bird = new Bird { Game = this };
        _walls = new List<List<Wall>>();
        _random = new Random();
        _score = 0;
        CheckWallSettings();
    }

    // Checks if the wall settings are allowed
    private static void CheckWallSettings()
    {
        if (GapBetweenWallsX < 0 ||
            GapBetweenWallsY < 0 ||
            GapBetweenWallsY + ShortestAllowedWall*2 > Bootstrap.getDisplay().getHeight() ||
            ShortestAllowedWall < 0)
        {
            throw new Exception("Invalid wall settings!");
        }
    }

    public override void update()
    {
        DestroyWallsOutsideWindow();
        CreateNewWalls();
        CheckIfBirdPassedWall();
        
        Bootstrap.getDisplay().showText("Score: " + _score, 0, 300, 2, Color.White);
    }
    
    private void DestroyWallsOutsideWindow()
    {
        if (_walls.Count == 0) return;
        
        List<Wall> oldestWallPair = _walls[0];
        if (oldestWallPair[0].MyBody.MinAndMaxX[1] < -Bootstrap.getDisplay().getWidth() / 2f)
        {
            oldestWallPair[0].ToBeDestroyed = true;
            oldestWallPair[1].ToBeDestroyed = true;
            _walls.RemoveAt(0);
        }
    }
    
    private void CreateNewWalls()
    {
        if (!ShouldNewWallsBeCreated()) return;
        
        int topYPos = Bootstrap.getDisplay().getHeight() / 2;
        int botYPos = -topYPos;
        
        // Generate min Y position of the gap
        int gapMinYPos = _random.Next(
            botYPos + ShortestAllowedWall, 
            topYPos - ShortestAllowedWall - GapBetweenWallsY);
        
        Wall lowerWall = new Wall();
        lowerWall.Transform2D.Y = botYPos;
        // Calculate the height of lower wall (distance between botYPos to gapMinYPos)
        lowerWall.Transform2D.Height = gapMinYPos - botYPos; // botYPos will always be negative, so Height will be positive
        
        Wall upperWall = new Wall();
        upperWall.Transform2D.Y = botYPos + lowerWall.Transform2D.Height + GapBetweenWallsY;
        // Calculate the height of upper wall by doing (display height) - (lower wall height and gap)
        upperWall.Transform2D.Height = Bootstrap.getDisplay().getHeight() - (lowerWall.Transform2D.Height + GapBetweenWallsY);

        _walls.Add([lowerWall, upperWall]);
    }

    private void CheckIfBirdPassedWall()
    {
        if (GameOver) return;
        foreach (List<Wall> wallPair in _walls)
        {
            if (wallPair[0].MyBody.MinAndMaxX[1] < _bird.MyBody.MinAndMaxX[0] && !wallPair[0].BirdHasPassed)
            {
                _score++;
                wallPair[0].BirdHasPassed = true;
                wallPair[1].BirdHasPassed = true;
            }
        }
    }

    private bool ShouldNewWallsBeCreated()
    {
        if (_walls.Count == 0) return true;
        
        // The initial collider is always at 0, 0. If this is the case, don't generate any more walls
        if (_walls[^1][0].MyBody.MinAndMaxX[1] == 0) return false;
        
        // (Left window border x pos) - (latest created wall left x pos) > _gapBetweenWallsX
        return Bootstrap.getDisplay().getWidth()/2f - _walls[^1][0].MyBody.MinAndMaxX[1] > GapBetweenWallsX;
    }
}