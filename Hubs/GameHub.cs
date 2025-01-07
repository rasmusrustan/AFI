using BattleShits.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class GameHub : Hub
{
    // En lista som kopplar lobbyId till en lista av spelare
    private static ConcurrentDictionary<string, List<string>> _lobbies = new ConcurrentDictionary<string, List<string>>();
    private static DatabaseMethods database = new DatabaseMethods();

    public string GetPlayerNameByIndex(string lobbyId, int index)
    {
        if (_lobbies.TryGetValue(lobbyId, out var players))
        {
            if (index >= 0 && index < players.Count)
            {
                return players[index]; // Returnera namnet vid det angivna indexet
            }

            throw new ArgumentOutOfRangeException($"Index {index} är utanför intervallet för spelarna i lobbyn.");
        }

        throw new InvalidOperationException($"Lobbyn med ID {lobbyId} existerar inte.");
    }

    public async Task JoinLobby(string lobbyId)
    {
        var username = Context.User.Identity.Name; // Hämta användarnamn från autentisering

        // Lägg till spelaren i lobbyn
        _lobbies.AddOrUpdate(lobbyId,
            new List<string> { username },
            (key, existingPlayers) =>
            {
                if (!existingPlayers.Contains(username))
                    existingPlayers.Add(username);
                return existingPlayers;
            });

        // Lägg till spelaren i SignalR-gruppen för lobbyn
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);

        // Uppdatera alla spelare i lobbyn med den nya listan
        await Clients.Group(lobbyId).SendAsync("UpdatePlayerList", _lobbies[lobbyId]);
    }

    public async Task StartGame(string lobbyId)
    {
        /*if (!_lobbies.TryGetValue(lobbyId, out var players) || players.Count != 2)
        {
            throw new InvalidOperationException("Det måste finnas exakt två spelare i lobbyn för att starta spelet.");
        }

        //int gameId = database.CreateGame(GetPlayerNameByIndex(lobbyId, 0), GetPlayerNameByIndex(lobbyId, 1));

        // Skapa URL:er för spelarna
        string hostRedirectUrl = $"/Home";
        string playerRedirectUrl = $"/Users/Test";

        // Skicka URL till rätt spelare (Värden - index 0, Spelare - index 1)
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            string redirectUrl = i == 0 ? hostRedirectUrl : playerRedirectUrl;

            // Skicka till rätt spelare via deras användarnamn
            await Clients.User(player).SendAsync("GameStarted", redirectUrl);
        }*/

        await Clients.Group(lobbyId).SendAsync("GameStarted");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Ta bort spelaren från alla lobbys de är med i
        foreach (var lobby in _lobbies)
        {
            if (lobby.Value.Remove(Context.User.Identity.Name))
            {
                // Uppdatera gruppen om en spelare lämnar
                await Clients.Group(lobby.Key).SendAsync("UpdatePlayerList", lobby.Value);
                break;
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
