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

    private int localPlayerHandle;
    // result
    private Player playerInstance;
    private GameState gameState;
    public void Config(bool host, bool syncTest)
    {
        
        if (host) {
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

        int errorcode;


        if (syncTest)
        {
            errorcode = GGPO.StartSynctest("Test", PLAYERNUMBERS, 7);
            GD.Print($"Starting synctest, errorcode {errorcode}");
        }
        else
        {
            errorcode = GGPO.StartSession("ark", PLAYERNUMBERS, localPort);
            GD.Print($"Starting regular session, errorcode {errorcode}");
        }
        ConnectEvents();
        gameState = new GameState();
        Godot.Collections.Dictionary localHandle = GGPO.AddPlayer(GGPO.PlayertypeLocal, localHand, IP, localPort);
        localPlayerHandle = (int) localHandle["playerHandle"];
        GD.Print($"Local add result: {localHandle["result"]}");

        int frameDelayError = GGPO.SetFrameDelay(localPlayerHandle, 2);
        GD.Print($"Frame delay error code: {frameDelayError}");
        GGPO.CreateInstance(gameState, nameof(gameState.SaveGameState));
        Godot.Collections.Dictionary remoteHandle = GGPO.AddPlayer(GGPO.PlayertypeRemote, otherHand, IP, otherPort);
        GD.Print($"Remote add result:{remoteHandle["result"]}");
        RegisterPlayer(localPlayerHandle);

    }

    public override void _PhysicsProcess(float delta)
    {
        GGPOIdle();
        GD.Print(GGPO.GetNetworkStats(2));
        int input = ReadInput();
        int result = 999;
        if (localPlayerHandle != GGPO.InvalidHandle)
        {
            // GD.Print($"Adding local input {input}"); this works
            result = GGPO.AddLocalInput(localPlayerHandle, input);
        }
        if (result == GGPO.ErrorcodeSuccess)
        {
            Godot.Collections.Dictionary resultDict = GGPO.SynchronizeInput(MAXPLAYERS);
            if ((int)resultDict["result"] == GGPO.ErrorcodeSuccess)
            {
                GD.Print(resultDict["inputs"]);
                
                Advance_Frame((Godot.Collections.Array)resultDict["inputs"]);
            }
            
        }
        else if (result == GGPO.ErrorcodeNotSynchronized)
        {
            GetNode<Label>("Sync").Text = "Not synchronized";
        }
    }
    public void Advance_Frame(Godot.Collections.Array inputs)
    {
        gameState.Update(inputs);
        GGPO.AdvanceFrame();
    }

    private void ConnectEvents()
    {
        GGPO.Singleton.Connect("advance_frame", this, nameof(OnAdvanceFrame));
        GGPO.Singleton.Connect("load_game_state", this, nameof(OnLoadGameState));
        GGPO.Singleton.Connect("event_disconnected_from_peer", this, nameof(OnEventDisconnectedFromPeer));
        GGPO.Singleton.Connect("save_game_state", this, nameof(OnSaveGameState));
        GGPO.Singleton.Connect("event_connected_to_peer", this, nameof(OnEventConnectedToPeer));
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
        GD.Print("Connected to peer");
        RegisterPlayer(handle);
    }

    public void OnAdvanceFrame(Godot.Collections.Array inputs)
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

    

}
