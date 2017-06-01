var Engine = Matter.Engine,
  Render = Matter.Render,
  World = Matter.World,
  Bodies = Matter.Bodies,
  Vector = Matter.Vector,
  Mouse = Matter.Mouse,
  Body = Matter.Body,
  Composite = Matter.Composite,
  Constraint = Matter.Constraint,
  MouseConstraint = Matter.MouseConstraint;

// create an engine
var engine = Engine.create();//tried putting document.body in this but i get an error from matter.min.js
var mouse = Matter.Mouse.create(document.body);

var render = Render.create({
  element: document.body,
  engine: engine
});
var vel = Vector.create(0.00001,0.00001);
var mousePoint = Bodies.circle(10, 10, 2, {isStatic: true});
var mouseBall = Bodies.circle(10, 10, 100);
var ballConstraint = Constraint.create({bodyA: mousePoint, bodyB: mouseBall, length: 5, stiffness: 0.02});
World.add(engine.world, [mouseBall, ballConstraint]);
var staticPoints = [];
var renderPoints = [];
var constraints = [];
var chainOne = [];
var chainTwo = [];
for (var i = 0; i < 15; i++){
  staticPoints.push(Bodies.circle(20 + i * 20, 300, 20, {isStatic: true}));
  renderPoints.push(Bodies.circle(20 + i * 20, 400, 20, {mass: 0.00001}));
  constraints.push(Constraint.create({bodyA: staticPoints[i], bodyB: renderPoints[i], length: 2, stiffness: 0.05}));
}
var chainLength = 120
for (var i = 0; i < 7; i++){
  chainOne.push(Constraint.create({bodyA: renderPoints[i], bodyB: renderPoints[i+1], length: chainLength, stiffness: 1}));
}
for (var i = 7; i < 14; i++){
  var nextPoint;
  if (renderPoints[i+1] != undefined){
    nextPoint = renderPoints[0];
  }
  else {
    nextPoint = renderPoints[i+1];
  }
  chainTwo.push(Constraint.create({bodyA: renderPoints[i], bodyB: renderPoints[i+1], length: chainLength, stiffness: 1}));
}
chainTwo.push(Constraint.create({bodyA: renderPoints[14], bodyB: renderPoints[0], length: chainLength, stiffness: 0.5}));
World.add(engine.world, chainOne);
World.add(engine.world, chainTwo);
World.add(engine.world, renderPoints);

//World.add(engine.world, staticPoints);
World.add(engine.world, constraints);

Engine.run(engine);

//Render.run(render);
