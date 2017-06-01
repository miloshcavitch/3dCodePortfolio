var AllScreenBlinks = function(){
  this.blinks = [];
  this.active = false;
  this.new = function(val){
    this.blinks.push(new ScreenBlink(val) );
    this.active = true;
  }
  this.update = function(){
    var maxStar = 100;
    var maxColor = 60;
    var maxSpace = 0;
    for ( var i = 0; i < this.blinks.length; i++){
      if (this.blinks[i].update()){
        if (this.blinks[i].starGradient[this.blinks[i].currentFrame] < maxStar){
          maxStar = this.blinks[i].starGradient[this.blinks[i].currentFrame];
        }
        if (this.blinks[i].colorGradient[this.blinks[i].currentFrame] < maxColor){
          maxColor = this.blinks[i].colorGradient[this.blinks[i].currentFrame];
        }
        if (this.blinks[i].spaceGradient[this.blinks[i].currentFrame] > maxSpace){
          maxSpace = this.blinks[i].spaceGradient[this.blinks[i].currentFrame];
        }
      } else {
        this.blinks.splice(i, 1);
        i--;
      }
    }
    if (this.blinks.length === 0){
      this.active = false;
    } else {
      this.active = true;
    }
    return {star: maxStar, color: maxColor, space: maxSpace};
  }
}

var ScreenBlink = function(length){
  this.currentFrame = 0;
  this.active = false;
  if (length === undefined){
    this.frameCount = Math.floor((Math.random() * 100) + 10);
  } else {
    this.frameCount = length;
  }
  var colors = 60;
  var space = 60;
  var stars = 0;

  this.colorGradient = [];
  this.spaceGradient = [];
  this.starGradient = [];


  var spaceInc = 60/Math.floor(this.frameCount);
  var colors = 0;
  for ( var i = 0; i < this.frameCount; i++ ){
    space -= spaceInc;
    this.spaceGradient.push( Math.floor(space) );
  }

  var starInc = 100/Math.floor(this.frameCount/4);
  var colorInc = 60/Math.floor(this.frameCount/4);
  for ( var i = 0; i < Math.floor(this.frameCount * 3/4); i++){
    this.starGradient.push(stars);
    this.colorGradient.push( Math.floor(colors) );
  }
  for ( var i = 0; i < Math.ceil(this.frameCount/4); i++ ){
    stars += starInc;
    this.starGradient.push(stars);
    colors += colorInc;
    this.colorGradient.push( Math.floor(colors) );
  }

  ////end of constructor
  this.update = function(){
    this.currentFrame++;
    if (this.currentFrame >= this.frameCount){
      return false;
    } else {
      return true;
    }
  }
}

var saturationVal = 60;
var screenBlinks = new AllScreenBlinks();
var updateScreenBlinks = function(){
  if (screenBlinks.active === true){

    var values = screenBlinks.update();
    saturationVal = values.color;
    starColor = "hsl(" + game.hitColor + ", 100%," + values.star + "%)";
    spaceColor = "hsl(" + game.hitColor + ", 100%," + values.space + "%)";
    //background color = title.colorIndex hsl
  } else {
    saturationVal = 60;
    starColor = 'white';
    spaceColor = 'black';
  }
}
