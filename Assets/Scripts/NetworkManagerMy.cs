using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ClassTypesDictionary : SerializableDictionary<ClassTypes, GameObject> {

}

public class NetworkManagerMy : NetworkManager {
    [SerializeField]
    private ChosenClass chosenClass;

    [SerializeField]
    private ClassTypesDictionary players;

    [SerializeField]
    private UnityEvent OnPlayerDisconnect;

    private Dictionary<int, NetworkConnection> _connections;

    public override void OnStartServer() {
        base.OnStartServer();

        _connections = new Dictionary<int, NetworkConnection>();
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        _connections.Add(conn.connectionId, conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);

        _connections.Remove(conn.connectionId);
    }

    //[Command]
    private void SpawnPlayer(int chosenPlayerId, int connId) {
        var chosenPlayer = players[(ClassTypes)chosenPlayerId];
        var conn = _connections[connId];

        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(chosenPlayer, startPos.position, startPos.rotation)
            : Instantiate(chosenPlayer);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{chosenPlayer.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        playerPrefab = players[chosenClass.Class];

        SpawnPlayer((int)chosenClass.Class, conn.connectionId);
    }
}