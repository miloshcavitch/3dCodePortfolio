var Velocity = function(){
  this.starRate = 0.3;
  this.deltaInt = 0;
  this.deltaFloat = 0;
  this.testD = 1;
  this.lastTime = Date.now();
  this.updateSpeed = function(){
    /*test section
    this.starRate += 0.01 * this.testD;
    if (this.starRate >= 3 || this.starRate <= 0.2){
      this.testD *= -1;
    }
    */
    this.deltaInt = Math.floor(this.starRate);
    this.deltaFloat += ( this.starRate - Math.floor(this.starRate) );
    if (this.deltaFloat >= 1){
      this.deltaInt += 1
      this.deltaFloat -= 1;
    }
  }
}


var velocity = new Velocity();
