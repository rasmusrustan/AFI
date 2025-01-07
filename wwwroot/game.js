/*
let turnTimer;
let turnTimeLeft = 60;  // 60 sekunders timer

function startTurnTimer(gameId, currentPlayer) {
    turnTimeLeft = 60;
    updateTimerDisplay(turnTimeLeft);

    turnTimer = setInterval(() => {
        turnTimeLeft -= 1;
        updateTimerDisplay(turnTimeLeft);

        if (turnTimeLeft <= 0) {
            clearInterval(turnTimer);
            declareWinner(gameId, currentPlayer === 'Player1' ? 'Player2' : 'Player1');
        }
    }, 1000);  // Uppdatera varje sekund
}

function updateTimerDisplay(timeLeft) {
    const timerElement = document.getElementById("turn-timer");
    if (timerElement) {
        timerElement.textContent = `Tid kvar: ${timeLeft} sekunder`;
    }
}

function declareWinner(gameId, winner) {
    fetch(`/api/DatabaseMethods/declareWinner`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ gameId, winner })
    }).then(response => {
        if (!response.ok) {
            console.error("Fel vid deklarering av vinnare.");
        }
    }).catch(error => {
        console.error("Fel:", error);
    });
}
*/

function checkBoardsReady(gameId) {
    fetch(`/api/DatabaseMethods/isGameSetup/${gameId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Något gick fel med begäran.');
            }
            return response.json();
        })
        .then(data => {

            if (data.board1Ready && data.board2Ready) {
                clearInterval(checkInterval);  // Stoppa kontrollen när båda spelarna är redo
                startGame();  // Starta spelet
            } else {
                console.log("Väntar på att båda spelarna ska vara redo.");
            }
        })
        .catch(error => {
            console.error("Fel vid kontroll av brädstatus:", error);
        });
}

const checkInterval = setInterval(() => {
    const gameId = 1;
    checkBoardsReady(gameId);
}, 3000);


let currentPlayer = 1; //spelare som börjar.
let turnTimeLeft = 60;  // 60 sekunders timer
let turnTimer;
function startSimpleTimer() {
    let turnTimeLeft = 60;  // 60 sekunders timer

    updateTimerDisplay(turnTimeLeft); // Visa startvärdet

    turnTimer = setInterval(() => {
        turnTimeLeft -= 1;
        updateTimerDisplay(turnTimeLeft); // Uppdatera värdet varje sekund

        if (turnTimeLeft <= 0) {
            clearInterval(turnTimer); // Stoppa timern när tiden är slut
            //document.getElementById("message").textContent = "Tiden är ute!";
            endTurn();
        }
    }, 1000);
}
function endTurn() {
    document.getElementById("message").textContent = `Tiden är ute! Spelare ${currentPlayer} förlorade.`;
    declareWinner(currentPlayer === 1 ? 2 : 1); // Deklarera vinnaren
}

function updateTimerDisplay(timeLeft) {
    const timerElement = document.getElementById("timer");
    if (timerElement) {
        timerElement.textContent = `Tid kvar: ${timeLeft} sekunder`;
    }
}
function playerShot() {
    if (isCurrentPlayerTurn) {
        clearInterval(turnTimer); // Stoppa timern för den aktuella spelaren
        currentPlayer = currentPlayer === 1 ? 2 : 1; // Växla till nästa spelare
        isCurrentPlayerTurn = !isCurrentPlayerTurn; // Växla tur

        startSimpleTimer(); // Starta timern för den nya spelaren
    }
    return true; // Tillåt formuläret att skicka data
}

const gameInfoElement = document.getElementById("game-info");
const gameId = gameInfoElement.dataset.gameId;

function declareWinner(winnerPlayer) {
    fetch(`/game/DeclareWinner?gameId=${gameId}&playerName=Player${winnerPlayer}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById("message").textContent = `Spelare ${winnerPlayer} vann!`;
        });
}

window.onload = () => {
    if (isCurrentPlayerTurn) {
        startSimpleTimer();
    }
};



