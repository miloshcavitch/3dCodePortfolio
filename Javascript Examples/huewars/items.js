var floorItems = [];

var updateFloorItems = function(){
  floorItems.forEach(function(item){
    item.update();
    if ( Math.hypot(Math.abs(item.x - milo.x), Math.abs(item.y - milo.y) ) <= 60){
      item.use();
      floorItems.splice(floorItems.indexOf(item), 1 );
    }
  });
}
