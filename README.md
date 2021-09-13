# Conway's Game of Life for the Terminal

## Overview

After reading the first part of [a blog post about Reddit interviews](https://alexgolec.dev/reddit-interview-problems-the-game-of-life/) by Alex Golec, I felt that creating my own iteration of [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) in C# might be a fun practice exercise. Other than verifying the rules of the game, I opted not to reference other implementations in order to see how well I could (or could not) do.

## Technical

This console project targets the latest .NET 6 preview version and uses some still-unreleased features in C# 10 that I simply wanted to use.

### Performance

* Larger grids (generated due to larger or high-resolution screens) will run a slower than smaller ones.
* The program runs much slower on Windows than on Mac OS and Linux. To my understanding, this is due to limitations of the Windows command line.

## Usage

Simply build it and run it in your terminal. Certain arguments are necessary, but the program will explain them if you run it with no arguments.

## TODO

* Adding and editing application settings
* Allow submitting seeds manually (maybe)
* Check the possibility of higher resolution