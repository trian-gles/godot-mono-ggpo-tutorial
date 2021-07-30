using Godot;
using System;

public class Main : Node2D
{
    private int localPort;
    private int localHand;

    private int otherPort;
    private int otherHand;

    private const int MAXPLAYERS = 2;
    private const string IP = "127.0.0.1";
    private const int PLAYERNUMBERS = 2;

    [Export]
    private bool syncTest = false;

    private int localPlayerHandle;
    // result
    private Player playerInstance;
    private GameState gameState;
    public void Setup(bool host)
    {
        if (host)
        {
            localPort = 7000;
            localHand = 1;
            otherPort = 7001;
            otherHand = 2;
        }
        else
        {
            localPort = 7001;
            localHand = 2;
            otherPort = 7000;
            otherHand = 1;
        }
        if (syncTest)
        {
            GGPO.StartSynctest("Test", PLAYERNUMBERS, 7);
        }
        else
        {
            GGPO.StartSession("Match", PLAYERNUMBERS, localHand);
        }
        ConnectEvents();
        gameState = new GameState();
        Godot.Collections.Dictionary localHandle = GGPO.AddPlayer(GGPO.PlayertypeLocal, localHand, IP, localPort);
        localPlayerHandle = (int) localHandle["playerHandle"];
        GGPO.SetFrameDelay(localPlayerHandle, 2);
        GGPO.CreateInstance(gameState, nameof(gameState.SaveGameState));
        GGPO.AddPlayer(GGPO.PlayertypeRemote, otherHand, IP, otherPort);
        RegisterPlayer(localPlayerHandle);

    }

    public override void _PhysicsProcess(float delta)
    {
        GGPOIdle();
        int input = ReadInput();
        int result = 999;
        if (localPlayerHandle != GGPO.InvalidHandle)
        {
            result = GGPO.AddLocalInput(localPlayerHandle, input);
        }
        if (result == GGPO.ErrorcodeSuccess)
        {
            Godot.Collections.Dictionary resultDict = GGPO.SynchronizeInput(MAXPLAYERS);
            if ((int)resultDict["result"] == GGPO.ErrorcodeSuccess)
            {
                GD.Print(resultDict["inputs"]);
                Advance_Frame((int[])resultDict["inputs"]);
            }
        }
    }
    public void Advance_Frame(int[] inputs)
    {
        gameState.Update(inputs);
        GGPO.AdvanceFrame();
    }
    private void GGPOIdle()
    {
    int start = 0;

    int next = 0;

        int now = 0;


        GGPO.Idle(Math.Max(0, next - now - 1));


    if (now >= next)
        {
            next = now + (1000 / 60);
        }
		
    }

    private int ReadInput()
    {
        
        if (Input.IsActionPressed("ui_right"))
        {
            return 1;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            return 2;
        }
        else if (Input.IsActionPressed("ui_up"))
        {
            return 3;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            return 4;
        }
        else
        {
            return 0;
        }
        
    }

    public void OnSaveGameState()
    {

    }

    public void OnLoadGameState(StreamPeerBuffer buffer)
    {
        gameState.LoadGameState(buffer);
    }

    public void OnEventConnectedToPeer(int handle)
    {
        RegisterPlayer(handle);
    }

    public void OnAdvanceFrame(int[] inputs)
    {
        GD.Print($"Advance frame callback with inputs {inputs}");
        gameState.Update(inputs);
        GGPO.AdvanceFrame();
    }

    public void OnEventDisconnectedFromPeer()
    {
        GGPO.CloseSession();
    }
    public void RegisterPlayer(int handle)
    {
        var playerResource = (PackedScene) ResourceLoader.Load("res://Player.tscn");
        playerInstance = (Player) playerResource.Instance();
        playerInstance.Name = handle.ToString();
        playerInstance.SetIdName(handle.ToString());
        AddChild(playerInstance);
        gameState.Players.Add(playerInstance);
    }

    private void ConnectEvents()
    {
        GGPO.Singleton.Connect("advance_frame", this, nameof(OnAdvanceFrame));
        GGPO.Singleton.Connect("load_game_state", this, nameof(OnLoadGameState));
        GGPO.Singleton.Connect("event_disconnected_from_peer", this, nameof(OnEventDisconnectedFromPeer));
        GGPO.Singleton.Connect("save_game_state", this, nameof(OnSaveGameState));
        GGPO.Singleton.Connect("event_connected_to_peer", this, nameof(OnEventConnectedToPeer));
    }

}
