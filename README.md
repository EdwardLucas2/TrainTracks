# TrainTracks

This program solves the train track puzzle in The Times, you can see a puzzle here: https://extras.thetimes.co.uk/web/public/pdfs/aedc0482032bb812a54b5773e54e11d6.pdf

## Quick Start Guide
There are pre-built executables for mac (intel and apple silicon) and windows in the /Builds folder. There are two different versions of the executable, Puzzle Input and Randomly Generated.

### Puzzle Input
Input the column data (the number of tracks in each column) from left to right, and the row data from bottom to top. Input the start and end track position via its 0-indexed coordinates, with the track position (0,0) being the bottom left position. You can input an optional mid track coordinate (leave it blank if you don't want to).

### Randomly generated
A randomly generated puzzle will be gerated on the left of the screen, and the solved puzzle will be shown, once calculated, on the right.


## Project Info
It's built with C# and Unity game engine, but the C# code in the Assets/Scripts folder should be very reusable. It uses a minimax algorithm, but it doesn't have any optimisations so it's quite slow. I wrote this when I was 15 so the code is my no means perfect and the design could definitely be redone, but, given enough time, it will generate and solve puzzles.