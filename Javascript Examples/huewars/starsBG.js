var ahora = Date.now();// initialized here because this is first event
var s = 1;
function star(x){
  switch(x){
    case 16:
      this.thesize = Math.random()*0.25 + 0.3;
      this.speed = this.thesize * s;
      break;
    case 8:
      this.thesize = Math.random()*0.5 + 0.6;
      this.speed = this.thesize * s;
      break;
    case 4:
      this.thesize = Math.random() * 1 + 1.2;
      this.speed = this.thesize * s;
      break;
    case 2:
      this.thesize = Math.random() * 2 + 2.4;
      this.speed = this.thesize * s;
      break;
    case 1:
      this.thesize = Math.random() * 4 + 4.8;
      this.speed = this.thesize * s;
      break;
    default:
      this.thesize = Math.random() * 4.8;
      this.speed = this.thesize * s;
  }
  this.posY = -20;
  this.posX = Math.round(Math.random() * 1600);
  this.updateStar = function(){
    ctx.beginPath();
    ctx.rect(unit * this.posX, unit * this.posY, unit * this.thesize * 1.5, unit * this.thesize * 1.5);
    ctx.fillStyle = starColor;//colorRay[Math.round(Math.random() * 764)];// or colorRay[charColorIndex]; colorRay[charColorIndex];
    ctx.fill();
    ctx.closePath();
    this.posY += this.speed * (velocity.starRate * 5 );
  }
  this.initPosUpdate = function(){
    this.posY += this.speed * (velocity.starRate * 5 );
  }
}
var starColor = 'white';
var spaceColor = 'black';
var starLayers = [];
var starSpawnTimer = {spawnTimer: 0, then: 0};

//new one

function spacedSpawner(){
    var spaceStat = Math.random() * 31;
    var x;
    if (spaceStat > 15){
      x = 16;
    }
    if (spaceStat <= 15 && spaceStat > 7){
      x = 8;
    }
    if (spaceStat <= 7 && spaceStat > 3){
      x = 4;
    }
    if (spaceStat <= 3 && spaceStat > 1){
      x = 2;
    }
    if (spaceStat <= 1){
      x = 1;
    }
    if (Math.random() >= 0.7){
      starLayers.push(new star(x))
    }
}
function starVelocityController(){
  //console.log(vel.deltaSpeed);
  for (var i = 0; i < velocity.deltaInt; i++){
    spacedSpawner();
  }
}
var initLayerTraverse = function(el){
  el.initPosUpdate();
  if (el.posY > 900 + el.thesize){
    starLayers.splice(starLayers.indexOf(el), 1);
  }
}
function LayerTraverse(el){//runs updateStar for all the stars
    el.updateStar();
    if (el.posY > 900 + el.thesize){
      starLayers.splice(starLayers.indexOf(el), 1);
    }
}
var fpsCounter = 0;
var lastSecond = 0;
var fpsString = '';
var initSetStars = function(){
  velocity.updateSpeed();
  starVelocityController();
  starLayers.forEach(initLayerTraverse);
}
function updateStars(){
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.beginPath();
  ctx.rect(0, 0, canvas.width, canvas.height);
  velocity.updateSpeed();
//  spaceBackground();
  ctx.fillStyle = spaceColor;//variable can be found in rainbowray.js
  ctx.fill();
  ctx.closePath();
  starVelocityController();
  starLayers.forEach(LayerTraverse);
  //counts frames per second for debugging, should be 50
  fpsCounter++;
  if (Date.now() >= lastSecond + 1000){
    fpString = fpsCounter + " FPS";
    fpsCounter = 0;
    lastSecond = Date.now();
  }
  ctx.font = "15px Arial"
  ctx.fillText(fpString, 10, 20);
}
