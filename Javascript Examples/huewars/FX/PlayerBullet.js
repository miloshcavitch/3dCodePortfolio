var playerBullets = [];
var updatePlayerBullets = function(){
  for (var i = 0; i < playerBullets.length; i++){
    playerBullets[i].update();

    if ( playerBullets[i].x > (1600 + playerBullets[i].size) || playerBullets[i].x < (0 - playerBullets[i].size) || playerBullets[i].y > (900 + playerBullets[i].size) || playerBullets[i].y < (0 - playerBullets[i].size) ){
      playerBullets.splice(i, 1);
      i -= 1;
      break;
    }
    for (var j = 0; j < ai.enemies.length; j++){
      if (Math.hypot(Math.abs(ai.enemies[j].x - playerBullets[i].x), Math.abs(ai.enemies[j].y - playerBullets[i].y) ) <= 60){
        if ( Math.abs(game.scrollColor - ai.enemies[j].color) < 40 || Math.abs( (game.scrollColor + 360) - ai.enemies[j].color ) < 40){
          explosions.push( new Explosion(ai.enemies[j].x, ai.enemies[j].y, ai.enemies[j].color) );
          var rand = Math.random();
          if (rand >= 0.8){
            if (milo.health < 0.9){
              floorItems.push( new HealthPack(ai.enemies[j].x, ai.enemies[j].y, ai.enemies[j].color) );
            } else {
              floorItems.push( new OneUp(ai.enemies[j].x, ai.enemies[j].y, ai.enemies[j].color) );
            }
          }
          ai.enemies.splice(j, 1);
          console.log('kablooie!' + j);
          game.points += Math.floor(40 * game.multiplier);
          game.multiplier += 0.1;
        }
        //place enemy explosion here
        playerBullets.splice(i , 1);
        i -= 1;
        if ( i < 0){
          i = 0;
        }
        break;
      }
    }
  }
}

var PlayerBullet = function(size, angle, initColor, x, y, speed){
  this.size = size;
  this.multiplierBool = false;
  this.x = x;
  this.y = y;
  this.speed = speed;
  var slope = radianToSlope(angle, this.speed);
  this.dx = slope.dx;
  this.dy = slope.dy;
  this.angle = angle;
  this.color = initColor;
  this.exhaustEmitters = [];

  for (var i = 0; i < 2; i++){
    this.exhaustEmitters.push(new ExhaustEmitter(0,0, this.color, 'tankBullet') );
  }
  this.setEmitterPos = function(){
    var angle =  90;
    for (var i = 0; i < this.exhaustEmitters.length; i++){
      this.exhaustEmitters[i].x = ( (this.size * 0.8) * Math.cos(Math.PI/180 * angle));
      this.exhaustEmitters[i].y = ( (this.size * 0.8) * Math.sin(Math.PI/180 * angle));

      angle -= 360/this.exhaustEmitters.length;
      this.exhaustEmitters[i].newParticle();
    }
  }
  this.setEmitterPos();
  this.exhaustEmitters.forEach(function(emitter){
    for (var i = 0; i < 15; i++){
      emitter.newParticle();
      for ( var i = 0; i < emitter.exhaustParticles.length; i++){
        emitter.exhaustParticles[i].initCreation();
      }
    }

  });
  this.renderExhaust = function(){
    this.exhaustEmitters.forEach(function(emitter){
      emitter.renderParticles();
    });
  }

  this.update = function(){
    this.multiplierBool = !this.multiplierBool;
    var size = this.size;
    this.x += this.dx;
    this.y += this.dy;
    ctx.translate(unit * this.x, unit * this.y);
    ctx.rotate(this.angle);
    //this.color += 10;
    this.renderExhaust();
    ctx.beginPath();
    ctx.arc(0,0, unit * size, 0, Math.PI* 2);
    ctx.strokeStyle = "hsl(" + this.color + ", 100%,"+ saturationVal + "%)";
    ctx.lineWidth = unit * 6;
    ctx.stroke();
    ctx.strokeStyle = "hsl(" + this.color + ", 100%,"+ saturationVal + "%)";
    ctx.fillStyle = spaceColor;
    ctx.lineWidth = unit * 3;
    ctx.globalAlpha = 0.7;
    ctx.stroke();
    ctx.globalAlpha = 1;
    ctx.fill();
    ctx.closePath();
    ctx.rotate(-1 * this.angle);
    ctx.translate( unit * (-1 * this.x) , unit * (-1 * this.y) );
  }
}
