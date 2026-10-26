# Alfa-college Arcade game

1. [About](#about)
2. [Development Instructions](#development)

<h2 id="about">About</h2>
This is a simple platformer/puzzle game made for the school "Alfa-college". This game was developed in Unity 6000.0.58F2 with DotNet version 9.0.
This game was made so that new students can learn a bit about the course "Software-development" in a more intresting manner. It is written to run on a raspbery pi 4.

<h2 id="development">Development Instructions</h2>
For developers do your work on your own branches and make a new one for each feature.
Follow the conventions in conventions.md.
Any field that will look for a component on the gameobject must be marked with the Component attribute, this will handle the checking if it exists and the initialization by itself.
Any field marked with SerializeField that can be set to null must also be given the RequiredSerializeField attribute, this will automatically check if the field is initialized and throw an exception if not.
