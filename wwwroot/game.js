let turnTimer; // Global timer-referens
let turnTimeLeft = 60; // Initialt antal sekunder
let currentPlayer = 1; // Börjar med spelare 1, ändra vid behov
let isCurrentPlayerTurn = true; // Flagga för att hantera spelarens tur
let previousShotCount = 0;  


const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

const gameInfoElement = document.getElementById("game-info");
const gameId = gameInfoElement ? gameInfoElement.dataset.gameId : null;

if (!gameId) {
    console.error("Game ID saknas. Kontrollera att game-info-diven innehåller data-game-id-attributet.");
} else {
    console.log(`Game ID hämtat: ${gameId}`);
}

connection.start()
    .then(() => {
        console.log('SignalR connection established.');
        startSimpleTimer();
        startShotCountCheck(); // Starta skottkontrollen
        connection.invoke("CheckShotCountChange", gameId) 
            .then(() => console.log("Shot count checked."))
            .catch(err => console.error("Error checking shot count:", err));
    })
    .catch(err => console.error('SignalR connection error:', err));


window.onload = () => {
    if (isCurrentPlayerTurn) {
        startSimpleTimer();  
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
let shotCountTimer;
function startShotCountCheck(gameId) {
    shotCountTimer = setInterval(() => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            console.log("Kontrollerar antalet skott...");
            connection.invoke("CheckShotCountChange")
                .catch(err => console.error("Fel vid kontroll av skottantal:", err));
        } else {
            console.error("SignalR-anslutningen är inte ansluten.");
        }
    }, 5000);  // Kontrollera var femte sekund
}

connection.on("ShotCountChanged", (newShotCount) => {
    console.log(`Antal skott uppdaterat: ${newShotCount}`);

    if (newShotCount !== previousShotCount) {
        console.log(`Skottantal ändrades! Föregående: ${previousShotCount}, Nytt: ${newShotCount}`);
        previousShotCount = newShotCount;  // Uppdatera det föregående värdet
    } else {
        console.log("Inget skottantal förändrat.");
    }
});

connection.invoke("CheckShotCountChange", gameId)
    .then(() => console.log("Shot count checked."))
    .catch(err => {
        console.error("Error checking shot count:", err);
        alert(`Error: ${err.message}`); // Visa felmeddelande för att lättare identifiera orsaken
    });




function updateTimerDisplay(timeLeft) {
    const timerElement = document.getElementById("timer");
    if (timerElement) {
        timerElement.textContent = `Tid kvar: ${timeLeft} sekunder`;
    } else {
        console.error("Timer-elementet hittades inte!");
    }
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





