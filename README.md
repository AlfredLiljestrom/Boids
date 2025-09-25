# Boids
Simulation of the behaviour of boids such as coherence, alignment, seperation and a small random jitter in direction. The calculations is done in the CPU which works well up to about 500 birds at the same time. Much of the algorithm is optimized fully to require as few computations and to loop through as few other boid objects as possible. A boundary structure helps much in this regard and the amount of cells can the changed depending on the needs. For around 250 birds the frames are around 400 - 500 which should make it possible to drag and drop into any project. 

### Things Learned: 
- In general about the boids-algorithm which can be applied to many different systems of creatures. 
- The Quaternions functions *LookRotation* and *Slerp* has been used extensively in the project which are quick enough to be run after another for every turn that the boid makes. 
- It has been very educational to create a boundary structure and to make sure that each boid is in the correct boundary at all times. 
- Lastly it is my first experience with Scriptable Objects which I use as combined settings for the different scripts. Very neat since the things changed in the settings lasts even one changes it while the program is running. 