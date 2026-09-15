using System;

public class CouldNotFindGameObjectException : Exception
{ 
    public CouldNotFindGameObjectException(params string[] MissingGameObjects) 
        : base("Could not find the following GameObjects on the scene. " +
               $"Missing Game Objects: {CollectionHelper.ConvertCollectionToProperlyFormattedString(MissingGameObjects)}")
    {}
}