using BattleShits.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System;
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

        int gameId = database.CreateGame(GetPlayerNameByIndex(lobbyId, 0), GetPlayerNameByIndex(lobbyId, 1));
        string message = "";
        await Clients.Group(lobbyId).SendAsync("GameStarted", gameId, message);
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
    public async Task StartTimer(int gameId)
    {
        await Clients.Group(gameId.ToString()).SendAsync("StartTimer", gameId);
    }

    public async Task Shoot(int gameId, int x, int y)
    {
        // Inför skottet i databasen (som redan görs)
        await Clients.Group(gameId.ToString()).SendAsync("ReceiveShot", gameId, x, y);

        // Kontrollera om skottantalet har ändrats
        int previousShotCount = database.GetCurrentRowCount();
        int currentShotCount = database.GetPreviousRowCount();

        // Om skottantalet har förändrats, skicka en uppdatering till alla spelare
        if (currentShotCount != previousShotCount)
        {
            await Clients.Group(gameId.ToString()).SendAsync("ShotCountChanged", gameId, currentShotCount); // Uppdatera skottantalet till alla spelare
        }

        // Starta timer (eller hantera någon annan logik efter skott)
        await StartTimer(gameId);
    }



    public async Task EndTurn(int gameId, int nextPlayerId)
    {
        // Meddela alla att det är nästa spelares tur
        await Clients.Group(gameId.ToString()).SendAsync("NextTurn", gameId, nextPlayerId);
    }
    
 
    private int previousRowCount = 0;
    private int noChangeCount = 0;  
    
    public async Task CheckShotCountChange(int gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId,gameId.ToString());

        if (!database.timerGameOver(gameId))
        {
            int currentRowCount = database.GetCurrentRowCount();

            if (currentRowCount != previousRowCount)
            {
                previousRowCount = currentRowCount;
                noChangeCount = 0;
                await Clients.All.SendAsync("ShotCountChanged", currentRowCount);
            }
            else
            {
                noChangeCount++;
            }


        }
        else
        {
            await Clients.All.SendAsync("RedirectResult", gameId);
        }
        

    }





    public async Task DeclareWinner(int gameId, int winnerPlayerNumber)
    {
        if (gameId <= 0 || (winnerPlayerNumber != 1 && winnerPlayerNumber != 2))
        {
            Console.WriteLine($"Ogiltiga argument: gameId={gameId}, winnerPlayerNumber={winnerPlayerNumber}");
            await Clients.Caller.SendAsync("Error", $"Ogiltiga argument för DeclareWinner: gameId={gameId}, winnerPlayerNumber={winnerPlayerNumber}");
            return;
        }

        try
        {
            int winner = database.getNextPlayer(gameId);

            if (winner == 1)
            {
                winner = 2;
            }
            else if (winner == 2)
            {
                winner = 1;
            }
            string winnerName = database.getPlayerNamefromGame(gameId, winner); // Använder databasmetoden för att hämta spelarens namn
            if (string.IsNullOrEmpty(winnerName))
            {
                Console.WriteLine($"Spelare kunde inte hittas för gameId={gameId} och winnerPlayerNumber={winner}");
                await Clients.Caller.SendAsync("Error", $"Spelare kunde inte hittas.");
                return;
            }

            // Uppdaterar vinnaren i databasen
            database.declareWinner(gameId, winnerName);

            // Skickar meddelande till alla klienter i rätt grupp att vinnaren är deklarerad
            await Clients.Group(gameId.ToString()).SendAsync("WinnerDeclared", gameId, winnerName);
            Console.WriteLine($"Vinnare deklarerad för gameId={gameId}: {winnerName}");

            

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid deklarering av vinnare för gameId={gameId}: {ex.Message}");
            await Clients.Caller.SendAsync("Error", $"Fel inträffade vid deklarering av vinnare: {ex.Message}");
        }
    }



}