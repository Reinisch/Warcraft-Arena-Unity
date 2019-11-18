using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player;
using ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub;
using Grpc.Core;
using MagicOnion.Client;
using UnityEngine;

public class InitClient : MonoBehaviour, IGamingHubReceiver
{
    public static InitClient Instance { get { return _instance; } }
    private Channel channel;
    private IGamingHub streamingClient;
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private static string currentPlayerName = "";

    private bool isJoin;
    private bool isSelfDisConnected;

    public GameObject myModel;
    private static InitClient _instance;

    void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            
            this.InitializeClient();
        }
        catch (RpcException ex)
        {
            Debug.Log("Cannot connect to a server.");
        }
        finally
        {
            Debug.Log("Initialized.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string GetCurrentPlayerName()
    {
        return currentPlayerName;
    }

    private async void InitializeClient()
    {
        currentPlayerName = "Elle_" + Guid.NewGuid();

        // Initialize the Hub
        this.channel = new Channel("localhost", 12345, ChannelCredentials.Insecure);
        GameObject player = await ConnectAsync(channel, "test", currentPlayerName);
        this.RegisterDisconnectEvent(streamingClient);
    }

    private async Task<GameObject> ConnectAsync(Channel grpcChannel, string roomName, string playerName)
    {
        this.streamingClient = StreamingHubClient.Connect<IGamingHub, IGamingHubReceiver>(grpcChannel, this);

        var roomPlayers = await this.streamingClient.JoinAsync(roomName, Client.GlobalVariables.CurrentAccountID);
        foreach (var player in roomPlayers)
        {
            (this as IGamingHubReceiver).OnJoin(player);
        }

        return players[Client.GlobalVariables.CurrentAccountID.ToString()];
    }

    private async void RegisterDisconnectEvent(IGamingHub streamingClient)
    {
        try
        {
            // you can wait disconnected event
            await streamingClient.WaitForDisconnect();
        }
        finally
        {
            // try-to-reconnect? logging event? close? etc...
            Debug.Log("disconnected server.");

            if (this.isSelfDisConnected)
            {
                // there is no particular meaning
                await Task.Delay(2000);

                // reconnect
                this.ReconnectServer();
            }
        }
    }

    private void ReconnectServer()
    {
        this.streamingClient = StreamingHubClient.Connect<IGamingHub, IGamingHubReceiver>(this.channel, this);
        this.RegisterDisconnectEvent(streamingClient);
        Debug.Log("Reconnected server.");

        this.isSelfDisConnected = false;
    }

    // methods send to server.

    public Task LeaveAsync()
    {
        return streamingClient.LeaveAsync();
    }

    public Task MoveAsync(Vector3 position, Quaternion rotation)
    {
        return streamingClient.MoveAsync(position, rotation);
    }

    public Task SendAnimationAsync(CharAnimState state)
    {
        return streamingClient.SendAnimStateAsync((int)state);
    }

    public Task SavePlayer()
    {
        return streamingClient.SavePlayerAsync();
    }

    // dispose client-connection before channel.ShutDownAsync is important!
    public Task DisposeAsync()
    {
        return streamingClient.DisposeAsync();
    }

    protected async void OnDestroy()
    {
        //It seems it doesn't let the client close.
        //await LeaveAsync();
        await this.streamingClient.DisposeAsync();
    }

    // You can watch connection state, use this for retry etc.
    public Task WaitForDisconnect()
    {
        return streamingClient.WaitForDisconnect();
    }

    void IGamingHubReceiver.OnJoin(Player player)
    {
        GameObject p = null;
        Debug.Log("Join Player:" + player.Name);
        if (!players.ContainsKey(player.Name))
        {
            p = Instantiate(myModel, player.Position, player.Rotation);
            p.name = player.Name;
            players.Add(player.Name, p);
        }
        else
        {
            Debug.Log("OnJoin: player already joined!!");
        }
        
    }

    void IGamingHubReceiver.OnLeave(Player player)
    {
        Debug.Log("Leave Player:" + player.Name);

        if (players.TryGetValue(player.Name, out var otherPerson))
        {
            GameObject.Destroy(otherPerson);
            players.Remove(player.Name);
        }
    }

    void IGamingHubReceiver.OnMove(Player player)
    {
        Debug.Log($"{player.Name} si sta muovendo!!");

        if (players.TryGetValue(player.Name, out var otherPerson))
        {
            if (otherPerson != null && otherPerson.name != currentPlayerName)
            {
                otherPerson.transform.position = Vector3.MoveTowards(otherPerson.transform.position, player.Position, 1.0f * Time.deltaTime);
                otherPerson.transform.Rotate(Vector3.up, Quaternion.Angle(otherPerson.transform.rotation, player.Rotation), Space.World);
                //otherPerson.transform.rotation = Quaternion.RotateTowards(otherPerson.transform.rotation, player.Rotation, 1.0f * Time.deltaTime);
            }
        }
    }

    void IGamingHubReceiver.OnAnimStateChange(string playerName, int state)
    {
        Debug.Log($"{playerName} CAMBIA ANIM STATE!!");

        if (players.TryGetValue(playerName, out var otherPerson))
        {
            if (otherPerson != null && otherPerson.name != currentPlayerName)
            {
                var animator = otherPerson.GetComponent<Animator>();
                animator.SetInteger("CharAnimState", state);
            }
        }
    }

    void IGamingHubReceiver.OnPlayerInfoSaved()
    {
        //ToDO: close the Pause menu
        Debug.Log("Successfully saved!");
    }
}
