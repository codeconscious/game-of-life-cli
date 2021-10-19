# Conway's Game of Life for the Terminal

## Overview

After reading the first part of [a blog post about Reddit interviews](https://alexgolec.dev/reddit-interview-problems-the-game-of-life/) by Alex Golec, I felt that creating my own iteration of [Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) in C# might be a fun practice exercise. Other than verifying the rules of the game, I opted not to reference other implementations in order to see how well I could do.

## Usage

This program runs on Linux, Windows, and Mac OS.

If you have the .NET 6 SDK installed, you can publish the program yourself by using the following command from the GameOfLife.Console project:
````dotnet publish -c Release --use-current-runtime````.

Then, simply run the executable from your command line. Pass in the ````--help```` or ````-h```` argument for more information.

## Technical

This console project targets the latest .NET 6 preview version (RC2) and uses some newer and still-unreleased features (upcoming in C# 10) that I wanted to try out (e.g., nullable reference types, record structs, init, switch expressions, global usings).

### Performance

* Larger grids (generated on larger or high-resolution screens) lead to slower speeds due to the sheer number of cells.

* The program runs much slower on Windows than on Mac OS and Linux. To my understanding, this is due to Windows limitations.

## 日本語の要約 (Japanese summary)

* Alex Golec氏が投稿された『[a blog post about Reddit interview strategy](https://alexgolec.dev/reddit-interview-problems-the-game-of-life/) (Redditの面接方法について)』というブログ記事の前半を読んでいたら、C#で[ライブゲーム](https://ja.wikipedia.org/wiki/%E3%83%A9%E3%82%A4%E3%83%95%E3%82%B2%E3%83%BC%E3%83%A0)を作ってみたいと思い、ルール以外何も参照しないで僕なりに開発してみました。

* 当プログラムはコンソールのプロジェクトで、せっかくなので未リリースの.NET 6 RC2とC# 10の新機能も試用しています。

## TODO

* ~~Save application settings to a JSON file (using the new System.Text.Json library)~~
* ~~Allow editing the settings via UI~~
* Allow saving and submitting seeds or grid states (maybe)
* Allow only single-byte characters to represent cells (Standard mode only)
* ~~Check the possibility of higher resolution~~