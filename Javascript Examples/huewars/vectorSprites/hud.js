var hud = {colorArray: ['#000000'], symmetryLine: 0, xCenter: 800, yCenter: 450, width: 140.625 * 2, height: 28.125 * 2, rotation: 0, shapes: [
{symmetryBool: false, color: 0, globalAlpha: 1, type: 'polygon',positions: [

{x: 2.85, y: -8},
{x: 1.85, y: -8},
{x: 1.85, y: -7},
{x: 2.85, y: -7},
{x: 2.85, y: -7.125},
{x: 1.8773333333333333, y: -7.111111111111111},
{x: 1.8773333333333333, y: -7.857777777777778},//point
{x: 2.85, y: -7.875},
]}
]}

var renderHUD = function(){
  hud.colorArray[0] = "hsl(" + milo.color + ", 100%, " + saturationVal + "%)";
  console.log(hud.colorArray[0]);
  renderPseudoSprite(hud, ctx);

}

var testLength = 100;
