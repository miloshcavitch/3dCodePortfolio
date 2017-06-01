mouse = {x: 0, y: 0};

$(document).bind('mousewheel', function(e){
    if(e.originalEvent.wheelDelta /120 > 0) {
        game.scrollColor += game.scrollSensitivity;
        if (game.scrollColor > 360){
          game.scrollColor = game.scrollSensitivity;
        }
    }
    else{
        game.scrollColor -= game.scrollSensitivity;
        if (game.scrollColor < game.scrollSensitivity){
          game.scrollColor = 360;
        }
    }
    $('#scroll-test').css('color', 'hsl(' + game.scrollColor + ', 100%, 50%)')
    e.preventDefault();
});
$(document).on('mousemove', function(event){
  mouse.x = event.pageX;
  mouse.y = event.pageY;
});
$(document).on('click', function(){
  activeClick();
});
var returnKeyFunction = function(){
  activeMode = function(){
    consistentUpdate();//to be changed to initLevel();
    level();
  }
  returnKeyFunction = function(){
  }
  game.currentClick = 1;
  activeClick = function(){
    game.clickRay[game.currentClick]();
  }
}
input = {l: false, u: false, r: false, d: false};
$(document).keydown(function(e) {
    switch(e.which) {
        case 190://greater than key
          game.scrollColor += 20;
          if (game.scrollColor >= 360){
            game.scrollColor -= 360;
          }
          break;
        case 188://less than key
          game.scrollColor -= 20;
          if (game.scrollColor <= 0){
            game.scrollColor += 360;
          }
          break;

        case 27://escape key
          game.paused = !game.paused;
          if (game.paused){
            game.pauseClick = activeClick;
            clearInterval(running);
            $('#mouse-sensitivity-menu').css('display', 'block');
            game.pauseColor = game.scrollColor;
            activeClick = function(){

            }
          } else {
            clearInterval(running);
            activeClick = function(){
              game.clickRay[game.currentClick]();
            }
            running = setInterval(update, 20);
            $('#mouse-sensitivity-menu').css('display', 'none');
            game.scrollColor = game.pauseColor;
          }
          break;

        case 65:
        case 37: // left
        input.l = true;
        break;

        case 87:
        case 38:
        input.u = true; // up
        break;

        case 68:
        case 39:
        input.r = true; // right
        break;

        case 40:
        case 83:
        input.d = true; // down
        break;

        case 13:
        returnKeyFunction();
        break;

        default: return; // exit this handler for other keys
    }
  });


  $(document).keyup(function(e) {
      switch(e.which) {
          case 65:
          case 37: // left
            input.l = false;
            break;

          case 68:
          case 39://right
            input.r = false;
            break;

          case 87:
          case 38:
            input.u = false;
            break;
          case 40:
          case 83:
          input.d = false; // up
          break;
          default: return; // exit this handler for other keys
      }
});
/////////////////
////////////////
//init star setup
for ( var i = 0; i < 2000; i++){
  initSetStars();
}
var exCount;
var consistentUpdate = function(){
  updateScreenBlinks();
  checkCanvasSize();
  exCount = 0;
  updateStars();//
  updateTankBullets();//
  updatePlayerBullets();//
  updateExplosions();//
  ai.enemies.forEach(function(enemy){//
    enemy.applyMovement();
    enemy.render();
  });
  ai.enemies.forEach(function(tank){
    tank.targetRotation = tank.headRotation = slopeToRadian(tank, milo) + Math.PI;
    tank.headRotation = tank.targetRotation;
  });
  updateFloorItems();
}
var testUpdate = function(){

  updateScreenBlinks();//
  checkCanvasSize();//
  exCount = 0;
  updateStars();//
  updateTankBullets();//
  updatePlayerBullets();//
  updateExplosions();//
  ai.update();
  ai.enemies.forEach(function(enemy){//
    enemy.applyMovement();
    enemy.render();
  });
  milo.render();
  milo.checkCollision();
  ai.enemies.forEach(function(tank){
    tank.targetRotation = tank.headRotation = slopeToRadian(tank, milo) + Math.PI;
    tank.headRotation = tank.targetRotation;
  });
  renderPoints();//
}
/////////////////////
////////////////////
var activeMode = function(){
  titleScreen();
}
//////////////////
/////////////////
var title = {colorIndex: 100, colorCount: 10, startFrame: 0, startSwitch: true};
var titleScreen = function(){
  if (Math.random() * 200 > 198) {
    screenBlinks.new();
  }
  renderPseudoSprite(logoHUE, ctx);
  //renderPseudoSprite(gmu, ctx);
  renderPseudoSprite(msShape, ctx);
  title.startFrame++;
  if (title.startFrame >= 30){
    title.startSwitch = !title.startSwitch;
    title.startFrame = 0;
  }
  if (title.startSwitch){
    renderPseudoSprite(startBTN, ctx);
  }
  updateScreenBlinks();

  logoHUE.colorArray[0] = startBTN.colorArray[0] = msShape.colorArray[0] = "hsl(" + title.colorIndex + ", 100%, " + saturationVal + "%)";
  gmu.colorArray[0] = "hsl(" + title.colorIndex + ", 100%, " + saturationVal + "%)";
  title.colorIndex += 3;
  game.hitColor = title.colorIndex;

}
/////////////////////
/////////////////////
var game = {currentLevel: 1, points: 0, multiplier: 1, lives: 3, hitColor: 100, scrollColor: 0, scrollSensitivity: 5, startGameOver: 0, paused: false, pauseColor: 0, currentClick: 0, clickRay: [titleClick, gameClick]};
var initLevel = function(){
  //starts level
}
var level = function(){
  ai.update();
  milo.render();
  milo.checkCollision();
  renderPoints();
}
/////////////
///////////////
var startRespawn = function(){
  ai.betweenDeath();
  ctx.fillStyle = "hsl(" + milo.color + ", 100%, " + saturationVal + "%)";
  renderPoints();
  velocity.starRate *= 0.99;
  if (velocity.starRate <= 0.025){
    velocity.starRate = 0.09;
    activeMode = function(){
      consistentUpdate();
      endRespawn();
    }
  }
}
var endRespawn = function(){
  ai.update();
  ctx.fillStyle = "hsl(" + milo.color + ", 100%, " + saturationVal + "%)";
  milo.render();
  milo.checkCollision();
  renderPoints();
  velocity.starRate *= 1.01;
  if (velocity.starRate >= 0.3){
    activeMode = function(){
      consistentUpdate();
      level();
    }
  }
}
var preGameOver = function(){
  ai.betweenDeath();
  game.startGameOver++;
  if (game.startGameOver >= 100){
    activeMode = function(){
      consistentUpdate();
      gameOver();
    }
  }
}
var gameOver = function(){
  ai.betweenDeath();
  title.colorIndex += 3;
  renderPoints();
  gameLogo.colorArray[0] = overLogo.colorArray[0] = "hsl(" + title.colorIndex + ", 100%, " + saturationVal + "%)";
  renderPseudoSprite(gameLogo, ctx);
  renderPseudoSprite(overLogo, ctx);
}

var renderPoints = function(){
  var fontSize = Math.floor(20 * unit);
  ctx.font = fontSize + "Arial";
  ctx.fillColor = 'white';
  ctx.fillText(game.points + " points         " + game.multiplier.toFixed(2) + "x             " + game.lives + " lives left", 150 * unit, 30 * unit);
}
var renderReticule = function(){
  var initRay = [{x: mouse.x + (10 * unit), y: mouse.y},{x: mouse.x, y: mouse.y + (10 * unit)},{x: mouse.x - (10 * unit), y: mouse.y},{x: mouse.x, y: mouse.y - (10 * unit)}];
  var endRay = [{x: mouse.x + (20 * unit), y: mouse.y},{x: mouse.x, y: mouse.y + (20 * unit)},{x: mouse.x - (20 * unit), y: mouse.y},{x: mouse.x, y: mouse.y - (20 * unit)}];
  ctx.lineWidth = 5;
  ctx.globalAlpha = 1;
  ctx.strokeStyle = "hsl(" + milo.color + ", 100%, " + saturationVal + "%)";
  for ( var i = 0; i < 4; i++){
    ctx.beginPath();
    ctx.moveTo(initRay[i].x, initRay[i].y);
    ctx.lineTo(endRay[i].x, endRay[i].y);
    ctx.stroke();
    ctx.closePath();
  }
}
var update = function(){
  checkCanvasSize();
  updateStars()
  activeMode();
  //activeUpdate();
  renderReticule();
}
var running = setInterval(update, 20);
