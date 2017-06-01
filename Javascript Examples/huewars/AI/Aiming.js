var slopeToRadian = function(fromPos, toPos){
  var rise = fromPos.y - toPos.y;
  var run = fromPos.x - toPos.x;
  var slope = rise/run;
  var rad = Math.atan(slope) - Math.PI/2
  if (fromPos.x > toPos.x){
    rad += Math.PI;
  }
  return rad;

}



var rotateTowardsTarget = function(targetAngle, currentAngle, turnSpeed){

  var direction;
  if ( Math.abs(targetAngle - currentAngle) <= turnSpeed ){
    return targetAngle;
  }
  if ( Math.abs(targetAngle - currentAngle) <= Math.PI ){
    if (targetAngle > currentAngle){
      direction = 'counter-clockwise'
    } else {

      direction = 'clockwise'
    }

  } else {
      if ( targetAngle < currentAngle){
        direction = 'counter-clockwise';
      } else {
        direction = 'clockwise';
      }
  }
  if (direction === 'counter-clockwise'){
    turnSpeed *= -1;
  }
  currentAngle += turnSpeed;
  if (currentAngle > Math.PI * 2){
    currentAngle = Math.PI * 2 - currentAngle;
  }
  if ( Math.abs(currentAngle - targetAngle) < Math.abs(turnSpeed) ){
    return targetAngle;
  } else {
    return currentAngle;
  }
}
var radianToSlope = function(angle, multiplier){
  var rise = Math.sin(angle - Math.PI/2) * multiplier;
  var run = Math.cos(angle - Math.PI/2) * multiplier;
  return {dx: run, dy: rise};
}
