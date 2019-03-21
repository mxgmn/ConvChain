ConvChain is a Markov chain of images that converges to input-like images. That is, the distribution of NxN patterns in the outputs converges to the distribution of NxN patterns in the input as the process goes on.

In the examples a typical value of N is 3.

<p align="center"><img alt="main collage" src="https://raw.githubusercontent.com/mxgmn/Blog/master/resources/convchain.png"></p>
<p align="center"><img alt="main gif" src="https://raw.githubusercontent.com/mxgmn/Blog/master/resources/convchain.gif"></p>

How to construct such a process? We want the final probability of a given pattern to be proportional to the pattern's weight, where pattern's weight is the number of such patterns in the input. For this it is sufficient that a stronger condition is satisfied: the probability of a given state (image) `S` should be proportional to the product of pattern weights over all patterns in `S`.
```
p(S) ~ product of pattern weights over all patterns in S
```
Fortunately, there are [general methods](https://en.wikipedia.org/wiki/Markov_chain_Monte_Carlo) to build a Markov chain that has the desired probability distribution over states as its stationary distribution.

Additional definitions:
1. For the ease of reasoning about the algorithm, it's convenient to introduce an energy function `E`, `E(S) := - sum over all patterns P in S of log(weight(P))` so the probability distribution over states becomes `p(S) ~ exp(-E(S))`. Note that this energy function is a generalization of the [Ising model](https://en.wikipedia.org/wiki/Ising_model) energy. In the Ising model patterns are 1x2 instead of NxN.
2. To expand possible applications, it's convenient to introduce a [temperature](https://en.wikipedia.org/wiki/Boltzmann_distribution) parameter `T` so the probability distribution over states becomes `p(S) ~ exp(-E(S)/T)`. Low temperatures make the distribution more concentrated in energy wells, high temperatures make the distribution more uniform. If one uses ConvChain to generate dungeons, low temperatures correspond to accurate newly built dungeons while high temperatures correspond to ruins.
3. For the speed of convergence, it's convenient for weights of all patterns to be nonzero. So let's redefine the `weight(P)` of a pattern `P` to be the number of patterns `P` in the input if that number is more than zero and some small number `eps` otherwise, `0 < eps < 1`.

## Algorithm
1. Read the input image and count NxN patterns.
	1. (optional) Augment pattern data with rotations and reflections.
2. Initialize the image (for example, with independent random values) in some state `S0`.
3. Repeat the Metropolis step:
	1. Compute the energy `E` of the current state `S`.
	2. Choose a random pixel and change its value. Let's call the resulting state `S'`.
	3. Compute the energy `E'` of the state `S'`.
	4. Compare `E'` to `E`. If `E' < E` assign the current state to be `E'`. Otherwise, assign the current state to be `E'` with probability `exp(-(E'-E)/T)`.

If there are more than 2 colors, Gibbs sampling may converge faster than Metropolis:

3. Repeat the Gibbs step: change the current state `S` to a state `S'` according to the probability distribution `p(S'|S) ~ exp(-E'/T)`.

## Comments
ConvChain supports constraints, so you can easily combine it with other generators or [handcrafted content](http://i.imgur.com/byyKHre.gif).

<p align="center"><img alt="constrained-convchain" src="https://raw.githubusercontent.com/mxgmn/Blog/master/resources/constrained-convchain.gif"></p>

In the language of [WFC readme](https://github.com/mxgmn/WaveFunctionCollapse) ConvChain satisfies strong condition 2 (Strong C2), but not condition 1 (C1).

If you freeze out the system as the Metropolis simulation goes on, you'll get a variant of the [simulated annealing](https://en.wikipedia.org/wiki/Simulated_annealing#Acceptance_probabilities_2) algorithm.

The [detailed balance](https://en.wikipedia.org/wiki/Detailed_balance#Reversible_Markov_chains) condition for ConvChain is `exp(-E1/T)p(S2|S1) = exp(-E2/T)p(S1|S2)`, so both Gibbs `p(S2|S1) ~ exp(-E2/T)` and Metropolis `p(S2|S1) = min(1, exp(-(E2-E1)/T))` chains converge to the desired distribution over states.

## Related work
1. Stuart Geman and Donald Geman, [Stochastic Relaxation, Gibbs Distributions, and the Bayesian Restoration of Images](http://image.diku.dk/imagecanon/material/GemanPAMI84.pdf), 1984.
2. Kris Popat and Rosalind W. Picard, [Novel cluster-based probability model for texture synthesis, classiffcation, and compression](https://pdfs.semanticscholar.org/9929/e48e11e7fa6a8f8f78889798b2b1ccd68a36.pdf), 1993.
3. Rupert Paget and I. Dennis Longstaf, [Texture Synthesis via a Non-parametric Markov Random Field](http://www.texturesynthesis.com/papers/Paget_DICTA_1995.pdf), 1995.
4. Vivek Kwatra, Irfan Essa, Aaron Bobick and Nipun Kwatra, [Texture Optimization for Example-based Synthesis](https://www.cc.gatech.edu/cpl/projects/textureoptimization/), 2005.

## How to build
ConvChain is a console application that depends only on the standard library. Get .NET Core for Windows, Linux or macOS and run
```
dotnet run ConvChain.csproj
```
`ConvChain.cs` contains the basic program, `ConvChainFast.cs` contains an equivalent faster program (~100 times faster on a 4-core CPU), but in a less human-readable form.

## Notable ports, forks and spinoffs
* [Kevin Chapelier](https://github.com/kchapelier) made an interactive vanilla JavaScript [port](http://www.kchapelier.com/convchain-demo/continuous.html) of the faster version and a [1KB adaptation](https://js1k.com/2019-x/demo/4069) of it.
* [Amit](https://github.com/amitp) [Patel](https://github.com/redblobgames) made a [web port](http://www.redblobgames.com/x/1613-convchain/) of the older slower version, with the main algorithm ported to TypeScript.
* [buckle2000](https://github.com/buckle2000) made a [Processing (Java) port](https://github.com/buckle2000/ConvChainJava) and a [MoonScript port](https://github.com/buckle2000/ConvChainMoon).

<p align="center"><img alt="mix" src="https://raw.githubusercontent.com/mxgmn/Blog/master/resources/convchain-mix.png"></p>
<p align="center"><img alt="convchain-3d-collage" src="https://raw.githubusercontent.com/mxgmn/Blog/master/resources/convchain-3d.png"></p>
