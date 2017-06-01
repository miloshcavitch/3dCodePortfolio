
var ExhaustEmitter = function(x, y, color, type){
  this.x = x;
  this.y = y;
  this.color = color;
  this.exhaustParticles = [];
  this.newParticle = function(){
    this.exhaustParticles.push( new ExhaustParticle(this.color, type) )
  }
  this.renderParticles = function(){
    exCount++;
    for (var i = 0; i < this.exhaustParticles.length; i++){
      if (this.exhaustParticles[i].update(this.x, this.y) === true){
        this.exhaustParticles.splice(i, 1);
        i -= 1;
      }

    }
    this.newParticle();
  }
}

var ExhaustParticle = function(color, type){
  this.x = 0;
  this.y = 0;
  this.color = color;
  this.size = 1.2;
  this.globalAlpha = 1;
  this.alphaMinus = 0.05;
  this.dx = 0;
  this.dy = 1;
  this.type = type;
  if (this.type === 'tankBullet'){
    this.ddx = (Math.random() * 0.25) - 0.125;
    this.ddy = (Math.random() * 0.6) - 0.2;
  } else {
    this.ddx = (Math.random() * 0.1) - 0.05;
    this.ddy = (Math.random() * 0.1) - 0.05;
  }
  this.initCreation = function(equiz, egreeyega){
    this.x += this.dx;
    this.dx += this.ddx;
    this.y += this.dy;
    this.dy += this.ddy;
    this.size += 0.1;
    this.globalAlpha -= this.alphaMinus;
    if (this.globalAlpha <= 0.2){
      return true;
    } else {
      return false;
    }
  }
  this.update = function(equiz, egreeyega){
    this.x += this.dx;
    this.dx += this.ddx;
    this.y += this.dy;
    this.dy += this.ddy;
    this.size += 0.1;
    this.globalAlpha -= this.alphaMinus;

    ctx.beginPath();
    ctx.strokeStyle = ctx.fillStyle = "hsl(" + this.color + ", 100%, " + saturationVal+ "%)";
    ctx.lineWidth = this.size * 0.2 * unit;
    if (this.type === 'tankBullet'){
      ctx.fillStyle = "hsl(" + this.color + ", 100%, " + saturationVal+ "%)";
    } else {
      ctx.fillStyle = "hsl(" + this.color + ", 100%, " + saturationVal+ "%)";
    }
    ctx.globalAlpha = this.globalAlpha;
    ctx.rect( unit * (equiz + this.x - this.size/2) , unit * (egreeyega + this.y), unit * this.size, unit * this.size * 3);
    ctx.stroke();
    ctx.fill();
    ctx.closePath();
    ctx.fillStyle = 'black';
    if (this.globalAlpha <= 0.2){
      return true;
    } else {
      return false;
    }
  }
}
