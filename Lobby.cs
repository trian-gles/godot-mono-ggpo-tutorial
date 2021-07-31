using Godot;
using System;

public class Lobby : Node2D
{
    private bool host;
    private bool syncTest;

    public void OnHostButtonDown()
    {
        syncTest = false;
        host = true;
        StartGame();
    }

    public void OnJoinButtonDown()
    {
        syncTest = false;
        host = false;
        StartGame();
    }

    public void OnSyncTestButtonDown()
    {
        syncTest = true;
        host = true;
        StartGame();
    }

    public void StartGame()
    {
        var mainInstance = (PackedScene) ResourceLoader.Load("res://Game.tscn");
        var mainScene = (Game) mainInstance.Instance();
        mainScene.Config(host, syncTest);
        AddChild(mainScene);
        GetNode<Button>("Host").Visible = false;
        GetNode<Button>("Join").Visible = false;
        GetNode<Button>("SyncTest").Visible = false;
    }
}
