var canvas = document.getElementById('game-canvas');
var ctx = canvas.getContext('2d');
var widthBool = false;
var setCanvasSize = function(){
  cWidth = window.innerWidth;
  cHeight = window.innerHeight;
  if (cWidth * 0.5625 < cHeight){
    canvas.width = window.innerWidth;
    cHeight = cWidth * 0.5625;
    canvas.height = cHeight;
    widthBool = true;
  } else {// temp
    canvas.height = cHeight;
    canvas.width = cHeight * 1.7778;
    widthBool = false;
  }
}


setCanvasSize();
var unit = canvas.width/1600;


var checkCanvasSize = function(){
  if (widthBool){
    if (window.innerWidth != canvas.width){
      setCanvasSize();
    }
  } else {
    if (window.innerHeight != canvas.height){
      setCanvasSize();
    }
  }
  unit = canvas.width/1600;
}
