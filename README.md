![](http://i.imgur.com/bVM4POR.png)

ConvChain passes a sample image through a 1-layer lattice of small overlapping receptive fields. It then runs an MCMC simulation with obtained weights as coefficients in the energy functional.

In the language of cellular automata, ConvChain takes an input image and builds a probabilistic cellular automaton that is most likely to generate that image.

Besides universality, an important advantage of ConvChain is that it can "fill the gaps", i.e. organically continue pre-placed content created by humans or other algorithms. [Example](http://i.imgur.com/byyKHre.gif).

* `ConvChain.cs` — basic program.
* `ConvChainFast.cs` — equivalent faster program (~100 times faster on a 4-core CPU), but in a less human-readable form.

###### Ports:

* [Kevin Chapelier](https://github.com/kchapelier) made a vanilla JavaScript [port](http://www.kchapelier.com/convchain-demo/continuous.html) of the faster version. 
* [Amit](https://github.com/amitp) [Patel](https://github.com/redblobgames) made a [web port](http://www.redblobgames.com/x/1613-convchain/) of the older slower version, with the main algorithm ported to TypeScript.
* [buckle2000](https://github.com/buckle2000) made a [Processing (Java) port](https://github.com/buckle2000/ConvChainJava).

![](http://i.imgur.com/JKKt75D.gif)
![](http://i.imgur.com/ErTwOqr.png)
