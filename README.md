# Alligator
# Unity 3D Developer Assignment


The main scene path is in the Scenes folder.
In the main scene there is a Ground object. One of the aztec bases is walkable, while the other one is not. New walkable/unwalkable
terrains can be added by;
  -Adding a object resembling ground in the Scene ( preferably as a child to the Ground object ).
  -Setting it as NavMesh static and setting its Navigation Area walkable/not walkable from Navigation/Object. 
  -Baking all the objects from Navigation/Bake.
 
Obstacles object is for the further area of movement limitation. Using the obstacles in it will create unwalkable areas on 
walkable objects. One can change their transforms or duplicate them to define the area of movement as desired.

In Scenes/TestScene, a test scene is provided to show that the alligator can move on any navmesh baked surface. 

The alligator prefab has the alligator script attached. It currently has two different states (Idle, Walking) to execute randomly.
It changes its avatar depending on the state and executes the respective animation. The attached animation controller is in the
AnimationControllers folder.

The movement position is randomly calculated. It randomly gets a point in the sphere, which is shown with the red wire sphere gizmo.
The radius of the sphere can be increased or decreased as desired. 

The randomness variable controls the interpolation amount between maximum and minimum of the movement properties. A perlin noise 
value is set as speed, acceleration, idle state time and rotation time using the randomness value. For instance, if the randomness
is set to 0, there will be a single value returned as noise. Which means, variables mentioned will have one same value between
1 and its maximum value on every state execution. However, setting the randomness value larger will mean that a different value 
from the perlin noise on each execution can be returned. Thus, changing the interpolation value each time and making the movement 
properties changing. 

Lastly, a new alligator state can be added by;
  -Adding a new value to the AlligatorState enum.
  -Implementing a coroutine for the new state.
  -Adding the name of the coroutine to the coroutines array in the Awake function ( preferably, with the same order in the enum.).
The code starts executing the state automatically ( and randomly ).

Berke.
