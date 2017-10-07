# Cellular-Automata
Exploration of Cellular Automata concepts in C# with Unity3D

In Unity, place either CellularAutomata.cs (CA), CellularAutomata2D.cs (CA2D), or LangtonAnt.cs (LA) on a Quad gameobject.  Script will create a texture with dimensions of "Map Size" based on current rule.  Check scripts for input directions.  CA2D will automatically update texture based on a timer.

CA produces the standard results.  

CA2D is my own idea based on CA where iterations in two dimensions are shown through animation.  I'm not sure at the moment if the results have any meaning or usefullness.  It can be improved.

LA produces the standard results.  Press 'A' to switch texture to a heat map which shows hows frequent the ant landed on a certain cell.
