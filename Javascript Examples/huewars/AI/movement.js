var checkSlopeSpeed = function(x , y){
  var c = Math.sqrt( ( x * x ) + (y * y) );
  return c;
}
var incSlopeSpeed = function(x, y, increment){
  var c = Math.sqrt( ( x * x ) + (y * y) );
  var xAngle = Math.asin(x/c);
  var yAngle = Math.asin(y/c);
  c += increment;
  var newX = Math.sin(xAngle) * c;
  var newY = Math.sin(yAngle) * c;
  return {x: newX, y: newY};
}

var setSlopeSpeed = function(x, y, size){
  var c = Math.sqrt( ( x * x ) + (y * y) );
  var xAngle = Math.asin(x/c);
  var yAngle = Math.asin(y/c);
  c = size
  var newX = Math.sin(xAngle) * c;
  var newY = Math.sin(yAngle) * c;
  return {x: newX, y: newY};
}
