var renderPseudoSprite = function(object, context){
        context.translate(unit * object.xCenter, unit * object.yCenter);
        context.rotate( object.rotation);
        object.shapes.forEach(function(shape){
          switch (shape.type){
            case 'curvedline':
              renderCurvedLine(object, context, shape);
              //debugRender(object, context, shape);
              break;
            case 'polygon':
            case 'polyline':
              renderPolygon(object, context, shape);
              break;
            case 'circle':
              renderCircle(object, context, shape);
              break;
            case 'curvedshape':
              renderCurvedShape(object, context, shape);
              //renderPolygon(object, context, shape);
              //debugRender(object,context,shape);
              break;
          }
        });
        /*
        context.beginPath();
        context.moveTo(0,0);
        context.lineTo(0,-2000);
        context.strokeStyle = 'red';
        context.stroke();
        context.closePath();
        */
        context.rotate(object.rotation * -1);
        context.translate( unit * (object.xCenter * -1), unit * (object.yCenter * -1) );
      }
      var renderPolygon = function(object, context, shape){
        context.beginPath();
        context.moveTo( unit * (shape.positions[0].x * object.width), unit * (shape.positions[0].y * object.height));
        shape.positions.forEach(function(p){
          context.lineTo( unit * (p.x * object.width), unit * (p.y * object.height) );
        });
        context.globalAlpha = shape.globalAlpha;
        switch (shape.type){
          case 'polyline':
            context.strokeStyle = object.colorArray[shape.color];
            context.lineWidth = unit * shape.lineWidth * object.width;
            context.stroke();
            break;
          case 'polygon':
            context.fillStyle = object.colorArray[shape.color];
            context.fill();
            if (shape.globalAlpha >= 0.9){
              context.lineWidth = unit * 1;
              context.strokeStyle = object.colorArray[shape.color];
              context.stroke();
            }
            break;
        }
        context.closePath();
        if (shape.symmetryBool === true){
          symmetryRenderPoly(object, context, shape);
        }
      }
      var renderCircle = function(object, context, shape){
        context.beginPath();
        context.arc( unit * (shape.positions[0].x * object.width), unit * (object.height * shape.positions[0].y), unit *(shape.radius * object.width), 0, Math.PI * 2);
        context.globalAlpha = shape.globalAlpha;
        if (shape.line === true){
          context.strokeStyle = object.colorArray[shape.color];
          context.stroke();
        } else {
          context.fillStyle = object.colorArray[shape.color];
          context.fill()
        }
        context.closePath();
        if (shape.symmetryBool === true){
          var flippedX = Math.abs(shape.positions[0].x - object.symmetryLine);
          if (shape.positions[0].x > object.symmetryLine){
            flippedX = flippedX * -1;
          }
          context.beginPath();
          context.arc( unit * (flippedX * object.width), unit * (shape.positions[0].y * object.height), unit * (shape.radius * object.width), 0, Math.PI * 2)
          if (shape.line === true){
            context.stroke();
          } else {
            context.fill();
          }
          context.closePath();
        }
      }
      var renderPolyline = function(object, context, shape){
        context.beginPath();
        context.moveTo( unit * (shape.positions[0].x * object.width),  unit * (shape.positions[0].y * object.height));
        shape.positions.forEach(function(p){
          context.lineTo( unit * (p.x * object.width),  unit * (p.y * object.height));
        });
        context.lineWidth = unit * shape.lineWidth * object.width;
        context.globalAlpha = shape.globalAlpha;
        context.strokeStyle = object.colorArray[shape.color];
        context.stroke();
        context.closePath();
        if (shape.symmetryBool == true){
          symmetryRenderPoly(object, context, shape);
        }
      }
      var renderCurvedShape = function(object, context, shape){
        context.beginPath();
        context.moveTo( unit * (shape.positions[0].x * object.width) ,  unit * (shape.positions[0].y * object.height) );
        for (var i = 1; i < shape.positions.length; i+=3){
          var lastpoint = {x: undefined, y: undefined};
          if (i+3 > shape.positions.length){
            lastpoint.x = shape.positions[0].x;
            lastpoint.y = shape.positions[0].y;
          } else{
            lastpoint.x = shape.positions[i+2].x;
            lastpoint.y = shape.positions[i+2].y;
          }
          context.bezierCurveTo( unit * (shape.positions[i].x * object.width),  unit * (shape.positions[i].y * object.height),   unit * (shape.positions[i+1].x * object.width),  unit * (shape.positions[i+1].y * object.height),   unit * (lastpoint.x * object.width),  unit * (lastpoint.y * object.height) );
        }

        context.fillStyle = object.colorArray[shape.color];
        context.globalAlpha = shape.globalAlpha;
        context.fill();
        if (shape.globalAlpha >= 0.9){
          context.lineWidth = unit * 1;
          context.strokeStyle = object.colorArray[shape.color];
          context.stroke();
        }
        context.closePath();
        if (shape.symmetryBool === true){
          symmetryRenderCurvedShape(object, context, shape);
        }
      }

      var renderCurvedLine = function(object, context, shape){
        context.beginPath();
        context.moveTo( unit * (shape.positions[0].x * object.width), unit * (shape.positions[0].y * object.height) );
        for ( var i = 1; i < shape.positions.length; i+=3){
          context.bezierCurveTo( unit * (shape.positions[i].x * object.width), unit * (shape.positions[i].y * object.height), unit * (shape.positions[i+1].x * object.width), unit * (shape.positions[i+1].y * object.height), unit * (shape.positions[i+2].x * object.width), unit * (shape.positions[i+2].y * object.height))
        }
        context.strokeStyle = object.colorArray[shape.color];
        context.globalAlpha = shape.globalAlpha;
        context.lineWidth = unit * shape.lineWidth * object.width;
        context.stroke();
        context.closePath();
        if (shape.symmetryBool === true){
          symmetryRenderCurvedLine(object, context, shape);
        }

      }
      var symmetryRenderCurvedLine = function(object, context, shape){
        context.beginPath();
        var initFlippedX = Math.abs(shape.positions[0].x - object.symmetryLine);
        if (shape.positions[0].x > object.symmetryLine){
          initFlippedX = initFlippedX * -1;
        }
        context.moveTo( unit * (initFlippedX * object.width), unit * (shape.positions[0].y * object.height) );
        for ( var i = 1; i < shape.positions.length; i+=3){
          var flippedXOne = Math.abs(shape.positions[i].x - object.symmetryLine);
          if (shape.positions[i].x > object.symmetryLine){
            flippedXOne *= -1;
          }
          var flippedXTwo = Math.abs(shape.positions[i+1].x - object.symmetryLine);
          if (shape.positions[i+1].x > object.symmetryLine){
            flippedXTwo *= -1;
          }
          var flippedXThree = Math.abs(shape.positions[i+2].x - object.symmetryLine);
          if (shape.positions[i+2].x > object.symmetryLine){
            flippedXThree *= -1;
          }
          context.bezierCurveTo( unit * (flippedXOne * object.width), unit * (shape.positions[i].y * object.height), unit * (flippedXTwo * object.width), unit * (shape.positions[i+1].y * object.height), unit * (flippedXThree * object.width), unit * (shape.positions[i+2].y * object.height))
        }
        context.stroke();
        context.closePath();
      }
      var symmetryRenderPoly = function(object, context, shape){
        context.beginPath();
        var initFlippedX = Math.abs(shape.positions[0].x - object.symmetryLine);
        if (shape.positions[0].x > object.symmetryLine){
          initFlippedX = initFlippedX * -1;
        }
        context.moveTo( unit * (initFlippedX * object.width),  unit * (shape.positions[0].y * object.height))
        shape.positions.forEach(function(p){
          var flippedX = Math.abs(p.x - object.symmetryLine);
          if (p.x > object.symmetryLine){
            flippedX = flippedX * -1;
          }
          context.lineTo( unit * (flippedX * object.width),  unit * (p.y * object.height) );
        });
        switch (shape.type){
          //no need to set alpha or linewidth etc. already set
          case 'polyline':
            context.stroke();
            break;
          case 'polygon':
            context.fill();
            if (shape.globalAlpha >= 0.9){
              context.lineTo( unit *  (initFlippedX * object.width),  unit * (shape.positions[0].y * object.height));
              context.stroke();
            }
            break;
        }
        context.closePath();
      }
      var symmetryRenderCurvedShape = function(object, context, shape){
        context.beginPath();
        var initFlippedX = Math.abs(shape.positions[0].x - object.symmetryLine);
        if (shape.positions[0].x > object.symmetryLine){
          initFlippedX = initFlippedX * -1;
        }
        context.moveTo( unit * (initFlippedX * object.width) ,  unit * (shape.positions[0].y * object.height) );
        for (var i = 1; i < shape.positions.length; i+=3){
          var lastpoint = {x: undefined, y: undefined};
          if (i+3 > shape.positions.length){
            lastpoint.x = Math.abs(shape.positions[0].x - object.symmetryLine);
            if (shape.positions[0].x > object.symmetryLine){
              lastpoint.x = lastpoint.x * -1;
            }
            lastpoint.y = shape.positions[0].y;
          } else{
            lastpoint.x = Math.abs(shape.positions[i+2].x - object.symmetryLine);
            if (shape.positions[i+2].x > object.symmetryLine){
              lastpoint.x = lastpoint.x * -1;
            }
            lastpoint.y = shape.positions[i+2].y;
          }
          var oneX = Math.abs(shape.positions[i].x - object.symmetryLine);
          if (shape.positions[i].x > object.symmetryLine){
            oneX = oneX * -1;
          }
          var twoX = Math.abs(shape.positions[i+1].x - object.symmetryLine);
          if (shape.positions[i+1].x > object.symmetryLine){
            twoX = twoX * -1;
          }
          context.bezierCurveTo( unit * (oneX * object.width),  unit * (shape.positions[i].y * object.height),  unit * (twoX * object.width),  unit * (shape.positions[i+1].y * object.height),  unit * (lastpoint.x * object.width),  unit * (lastpoint.y * object.height) );
        }
        context.fill();
        if (shape.globalAlpha >= 0.9){
          context.stroke();
        }
        context.closePath();
      }
      var debugRender = function(object, context, shape){
        shape.positions.forEach(function(p){
          context.beginPath();
          context.arc( unit * (p.x * object.width),  unit * (p.y * object.height), 10, 0, Math.PI * 2);
          context.globalAlpha = 0.5;
          context.fillStyle = 'black';
          context.fill();
          context.closePath();
        });
      }
      var debugPoint = function(x, y){
        ctx.beginPath();
        ctx.arc(x,y,10,0,Math.PI * 2);
        ctx.globalAlpha = 0.5;
        ctx.fillStyle = 'red';
        ctx.fill();
        ctx.closePath();
        console.log(x + ", " + y);
      }
