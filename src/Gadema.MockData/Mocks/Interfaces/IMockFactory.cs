namespace Gadema.MockData.Mocks.Interfaces;

/// <summary>
/// Defines a contract for creating mock entities of type TResponse.
/// </summary>
/// <typeparam name="TResponse">The type of the entity to be created and stored.</typeparam>
public interface IMockFactory<TResponse>
{
    /// <summary>
    /// Creates a new instance of the response DTO.
    /// </summary>
    /// <param name="input">The creation DTO containing initial data.</param>
    /// <returns>A fully populated instance of TResponse.</returns>
    TResponse Create(object input);
}
