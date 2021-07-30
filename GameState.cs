using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{

    public List<Player> Players = new List<Player>();
    private const int PLAYERNUMBERS = 2;
    public override void _Ready()
    {
        
    }

    public void Update(int[] inputs)
    {
        int i = 0;
        foreach (Player p in Players)
        {
            p.MovePlayer(inputs[i]);
            i++;
        }
    }
    public StreamPeerBuffer SaveGameState()
    {
        var stream = new StreamPeerBuffer();
        stream.Clear();
        stream.Put32(0);
        foreach (Player p in Players)
        {
            stream.Put16((short)Math.Round(p.velocity.x));
            stream.Put16((short)Math.Round(p.velocity.y));
            stream.Put16((short)Math.Round(p.Position.x));
            stream.Put16((short)Math.Round(p.Position.y));
        }
        int checkSum = CalcFletcher32(stream);
        stream.Seek(0);
        stream.Put32(checkSum);

        return stream;
    }

    public void LoadGameState(StreamPeerBuffer buffer)
    {
        buffer.Seek(0);
        buffer.Get32();
        foreach (Player p in Players)
        {
            p.velocity.x = buffer.Get16();
            p.velocity.y = buffer.Get16();
            p.Position = new Vector2(buffer.Get16(), buffer.Get16());
        }
    }

    public int CalcFletcher32(StreamPeerBuffer stream)
    {
        int sum1 = 0;
        int sum2 = 0;
        var index = stream.DataArray;
        for (int i = 0; i < index.Length; i++)
        {
            sum1 = (sum1 + index[i] % 0xffff);
            sum2 = (sum1 + sum2) % 0xffff;
           
        }
        return ((sum2 << 16) | sum1);
    }

    
}
