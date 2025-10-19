# Microbonk
A sample DOTS horde survivor

![Sample.gif](Docs/Sample.gif)

This demo runs at 70+ FPS with ~1000 enemies, ~10000 collectibles, ~100 projectiles on my laptop with i7-1260p and no discrete GPU

## Features
- Player - full physics, third person movement & camera
- Enemies - full physics, move towards the player, can climb obstacles, have collision avoidance
- Collectibles - dropped when enemies die, picked up when a player is nearby
- Projectiles - kill enemies on collision, have limited lifetime

## Tests
This repo also showcases:
- How to write tests for DOTS code, with two sample PlayMode tests that run inside ECS world 
- GitHub CI that executes and reports these tests

## Will not implement
These are not supported by DOTS natively and would require writing supplementary MonoBehaviours; this repo aims to be a sample of ECS only
- Sounds
- Animations
- UI