let turnTimer; // Global timer-referens
let turnTimeLeft = 60; // Initialt antal sekunder
let currentPlayer = 1; // sätt nuvarande spelare till 1
let isCurrentPlayerTurn = true;  // Sätt nuarande spelare till sant
let previousShotCount = 0;  // gör en tidigare shotcount för att jämföra med.

let shotCountTimer;

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

const gameInfoElement = document.getElementById("game-info");
const gameIdString = gameInfoElement ? gameInfoElement.dataset.gameId : null;
const gameId = gameIdString ? parseInt(gameIdString, 10) : null;



if (gameId== null) {
    console.error("Game ID saknas. Kontrollera att game-info-diven innehåller data-game-id-attributet.");
} else {
    console.log(`Game ID hämtat: ${gameId}`);
}

connection.start()
    .then(() => {        
        startSimpleTimer();
        startShotCountCheck(); // Starta skottkontrollen
        connection.invoke("CheckShotCountChange", gameId) 
            .then(() => console.log("Shot count checked."))
            .catch(err => console.error("Error checking shot count:", err));
    })


function startSimpleTimer() {
    if (turnTimer) {
        clearInterval(turnTimer);
    }

    turnTimeLeft = 60; 
    updateTimerDisplay(turnTimeLeft); 

    turnTimer = setInterval(() => {
        turnTimeLeft -= 1;
        updateTimerDisplay(turnTimeLeft);

        if (turnTimeLeft <= 0) {
            clearInterval(turnTimer); // Stoppa timern
            document.getElementById("message").textContent = `Tiden är ute! Spelare ${currentPlayer} förlorade.`;

            // Deklarera den andra spelaren som vinnare
            declareWinner(currentPlayer === 1 ? 2 : 1);
        }
    }, 1000); // 1000 ms = 1 sekund
}

function startShotCountCheck() {
    shotCountTimer = setInterval(() => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            console.log("Kontrollerar antalet skott...");
                connection.invoke("CheckShotCountChange",gameId)
           
                .catch(err => console.error("Fel vid kontroll av skottantal:", err));
        } else {
            console.warn("Anslutningen är inte redo.");

        }
    }, 5000);  // Kontrollera var femte sekund
}

connection.on("ShotCountChanged", (newShotCount) => {
    console.log(`Antal skott uppdaterat: ${newShotCount}`);

    if (newShotCount !== previousShotCount) {
        console.log(`Skottantal ändrades! Föregående: ${previousShotCount}, Nytt: ${newShotCount}`);
        previousShotCount = newShotCount;  // Uppdatera det föregående värdet

        if (isCurrentPlayerTurn) {
            startSimpleTimer();
        }
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
    connection.invoke("DeclareWinner", numericGameId, winnerPlayer).catch(err => {
            console.error("Error invoking DeclareWinner:", err.toString());
    });
    window.location.href = `/BattleField2/Result?gameId=${numericGameId}`

}

function RedirectResult() {
    window.location.href = `/BattleField2/Result?gameId=${numericGameId}`

}









