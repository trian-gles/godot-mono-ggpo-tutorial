using Godot;
using System;


public class Player : KinematicBody2D
{
    const int WALKSPEED = 80;
    const int RIGHT = 1;
    const int LEFT = 2;
    const int UP = 3;
    const int DOWN = 4;

    public Vector2 velocity = new Vector2();

    public string playerID = "";

    public void SetIdName(string newID)
    {
        GetNode<Label>("Label1").Text = newID;
    }

    public void MovePlayer(int input)
    {
        switch (input)
        {
            case RIGHT:
                velocity.x = WALKSPEED;
                break;

            case LEFT:
                velocity.x = -WALKSPEED;
                break;

            case UP:
                velocity.y = -WALKSPEED;
                break;

            case DOWN:
                velocity.y = WALKSPEED;
                break;

            default:
                velocity.x = 0;
                velocity.y = 0;
                break;
        }

        MoveAndSlide(velocity);
        Position = new Vector2((float) Math.Round(Position.x), (float) Math.Round(Position.y));

    }
}
