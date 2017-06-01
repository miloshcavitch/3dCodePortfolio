var explosions = [];
var updateExplosions = function(){
  for ( var j = 0; j < explosions.length; j++){
    console.log(j);
    if ( explosions[j].update() ){
      explosions.splice(j, 1);
      i--;
    }
  }
}
var Explosion = function(x, y, color){
  this.x = x;
  this.y = y;
  this.particles = [];
  for (var i = 0; i < 40; i++){
    this.particles.push( new ExplosionParticle(x, y, color) );
  }
  this.update = function(){
    //ctx.translate(unit * this.x, unit * this.y);
    for ( var i = 0; i < this.particles.length; i++){
      if (this.particles[i].update() ){
        this.particles.splice(i , 1);
        i--;
      }
    }
    //ctx.translate(unit * this.x * -1, unit * this.y * -1);
    this.rotation += Math.PI/90;
    if (this.particles.length === 0){
      return true;
    } else {
      return false;
    }
  }
}


var ExplosionParticle = function(x, y, color){
  this.eX = x;
  this.eY = y;
  this.x = Math.random() * 8 - 4;
  this.y = Math.random() * 8 - 4;
  this.color = color;
  this.dx = Math.random() * 8 - 4;
  this.dy = Math.random() * 8 - 4;
  this.ddx = Math.random() * 1 - 0.5;
  this.ddy = Math.random() * 1 - 0.5;

  this.size = 4;
  this.alpha = 1;
  this.render = function(){
    ctx.beginPath();
    ctx.rect(unit * (this.eX + this.x), unit * (this.eY + this.y), unit * this.size, unit * this.size);
    ctx.globalAlpha = this.alpha;
    ctx.strokeStyle = "hsl(" + this.color + ", 100%, " + saturationVal + "%)";
    ctx.fillStyle = "hsl(" + this.color + ", 100%, " + saturationVal + "%)";
    ctx.lineWidth = this.size/10;
    ctx.fill();
    ctx.stroke();
    ctx.closePath();
  }
  this.update = function(){
    this.x += this.dx;
    this.y += this.dy;
    this.dx += this.ddx;
    this.dy += this.ddy;
    this.size *= 1.13;
    this.alpha -= 0.05;
    this.render();
    if (this.alpha <= 0.05){
      return true;
    } else {
      return false;
    }
  }
}
