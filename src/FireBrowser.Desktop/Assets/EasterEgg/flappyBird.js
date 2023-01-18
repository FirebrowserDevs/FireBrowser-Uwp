"use strict";

var cvs = document.getElementById("canvas");
var ctx = cvs.getContext("2d");

// load images
var bird = new Image();
var pipeNorth = new Image();
var pipeSouth = new Image();
var bg = new Image();

bg.src = "ms-appx-web:///Assets//EasterEgg/back.png";
bird.src = "ms-appx-web:///Assets//EasterEgg/bird.png";
pipeNorth.src = "ms-appx-web:///Assets//EasterEgg/pipeBottom.png";
pipeSouth.src = "ms-appx-web:///Assets//EasterEgg/pipeTop.png";

// some variables
var gap = 85;
var constant;
var bX = 10;
var bY = 150;
var gravity = 1.5;
var score = 0;

// on key down
document.addEventListener("keydown", moveUp);

function moveUp() {
    bY -= 25;

    fly.play();
}

// pipe coordinates
var pipe = [];

pipe[0] = {
    x: cvs.width,

    y: 0,
};

// draw images
function draw() {
    ctx.drawImage(bg, 0, 0);

    for (var i = 0; i < pipe.length; i++) {
        constant = pipeNorth.height + gap;

        ctx.drawImage(pipeNorth, pipe[i].x, pipe[i].y);
        ctx.drawImage(pipeSouth, pipe[i].x, pipe[i].y + constant);

        pipe[i].x--;

        if (pipe[i].x == 125) {
            pipe.push({
                x: cvs.width,
                y: Math.floor(Math.random() * pipeNorth.height) - pipeNorth.height,
            });
        }

        // detect collision
        if (
            (bX + bird.width >= pipe[i].x &&
                bX <= pipe[i].x + pipeNorth.width &&
                (bY <= pipe[i].y + pipeNorth.height ||
                    bY + bird.height >= pipe[i].y + constant)) ||
            bY + bird.height >= cvs.height - fg.height
        ) {
            location.reload(); // reload the page
        }

        if (pipe[i].x == 5) {
            score++;
            scor.play();
        }
    }

    ctx.drawImage(fg, 0, cvs.height - fg.height);
    ctx.drawImage(bird, bX, bY);

    bY += gravity;
    ctx.fillStyle = "#000";
    ctx.font = "20px Verdana";
    ctx.fillText("Score : " + score, 10, cvs.height - 20);
    requestAnimationFrame(draw);
}

draw();