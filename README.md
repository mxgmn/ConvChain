<<<<<<< HEAD
!{}(http://i.imgur.com/fJ8Olxk.png)

ConvChain passes a sample image through a 1-layer lattice of small overlapping receptive fields. It then runs an MCMC simulation with obtained weights as coefficients in the energy functional.

Besides universality, an important advantage of ConvChain is that it can "fill the gaps", i.e. organically continue pre-placed content created by humans or other algorithms. [Example](http://i.imgur.com/byyKHre.gif).

Amit Patel made a [web port](http://www.redblobgames.com/x/1613-convchain/) of ConvChain.
=======
![64](http://i.imgur.com/H7urFch.png)
![24](http://i.imgur.com/sObK54r.png)

How it works. ConvChain passes a sample image through a 1-layer lattice of small overlapping receptive fields. It then runs an MCMC simulation with obtained weights as coefficients in the energy functional.

I used the Heat Bath algorithm for MCMC, but someone might want to use Metropolis, since it's faster when the number of colors is small. Note that the current version treats sample and output images as if they are drawn on a torus (periodical).

Besides universality, an important advantage of ConvChain is that it can "fill the gaps", i.e. organically continue pre-placed content created by humans or other algorithms. [Example](http://i.imgur.com/byyKHre.gif).

The obvious thing to do next is to add further layers, so the net could grasp higher-level concepts.
>>>>>>> origin/master
