
let turnTimer; // Global timer-referens
let turnTimeLeft = 60; // Initialt antal sekunder
let currentPlayer = 1; // Börjar med spelare 1, ändra vid behov
let isCurrentPlayerTurn = true; // Flagga för att hantera spelarens tur


const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();


connection.start()
    .then(() => {
        console.log('SignalR connection established.');
        startSimpleTimer(); // Starta timern när anslutningen är klar
    })
    .catch(err => console.error('SignalR connection error:', err));

window.onload = () => {
    if (isCurrentPlayerTurn) {
        startSimpleTimer();  // Startar timern när spelet laddas
    }
};

function startSimpleTimer() {
    // Stoppa eventuell tidigare timer
    if (turnTimer) {
        clearInterval(turnTimer); // Rensa tidigare timer
    }

    turnTimeLeft = 60; // Startvärde
    updateTimerDisplay(turnTimeLeft); // Uppdatera visningen

    turnTimer = setInterval(() => {
        turnTimeLeft -= 1;
        updateTimerDisplay(turnTimeLeft);

        if (turnTimeLeft <= 0) {
            clearInterval(turnTimer); // Stoppa timern
            document.getElementById("message").textContent = `Tiden är ute! Spelare ${currentPlayer} förlorade.`;

            // Deklarera den andra spelaren som vinnare när timern når 0
            declareWinner(currentPlayer === 1 ? 2 : 1);

        }
    }, 1000); // 1000 ms = 1 sekund
}

function updateTimerDisplay(timeLeft) {
    const timerElement = document.getElementById("timer");
    if (timerElement) {
        timerElement.textContent = `Tid kvar: ${timeLeft} sekunder`;
    } else {
        console.error("Timer-elementet hittades inte!");
    }
}

const gameInfoElement = document.getElementById("game-info");
const gameId = gameInfoElement ? gameInfoElement.dataset.gameId : null;

if (!gameId) {
    console.error("Game ID saknas. Kontrollera att game-info-diven innehåller data-game-id-attributet.");
} else {
    console.log(`Game ID hämtat: ${gameId}`);
}


function declareWinner(winnerPlayer) {
    const numericGameId = parseInt(gameId, 10);
    if (isNaN(numericGameId)) {
        console.error("Ogiltigt gameId-värde.");
        return;
    }

    console.log(`Declaring winner player ${winnerPlayer} for game ${numericGameId}`);
    connection.invoke("DeclareWinner", numericGameId, winnerPlayer)
        .catch(err => {
            console.error("Error invoking DeclareWinner:", err.toString());
        });
}



function playerShot(x, y) {
    if (isCurrentPlayerTurn) {
        clearInterval(turnTimer); // Stop timer for current player
        isCurrentPlayerTurn = false;

        connection.invoke("Shoot", gameId, x, y)
            .catch(err => console.error(err.toString()));

        currentPlayer = currentPlayer === 1 ? 2 : 1; // Switch player
        isCurrentPlayerTurn = true; // Enable next player's turn
        startSimpleTimer(); // Start timer for the new player
    }
}



connection.on("ReceiveShot", (gameId, x, y) => {
    console.log(`Shot received at (${x}, ${y}) in game ${gameId}`);
});




