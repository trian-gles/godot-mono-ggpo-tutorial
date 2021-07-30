using Godot;
using System;

public class GameConfig : Node2D
{
    private bool host;
    public override void _Ready()
    {
        
    }

    public void OnHostButtonDown()
    {
        host = true;
        StartGame();
    }

    public void OnJoinButtonDown()
    {
        host = false;
        StartGame();
    }

    public void StartGame()
    {
        var mainInstance = (PackedScene) ResourceLoader.Load("res://Main.tscn");
        var mainScene = (Main) mainInstance.Instance();
        mainScene.Setup(host);
        AddChild(mainScene);
        GetNode<Button>("Host").Visible = false;
        GetNode<Button>("Join").Visible = false;
    }
}
