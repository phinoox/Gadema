using System;
using System.Collections.Generic;

namespace Gadema.MockData.Mocks.Interfaces;

/// <summary>
/// Defines a contract for managing the lifecycle of a mock entity of type TResponse.
/// </summary>
/// <typeparam name="TResponse">The type of the entity to be managed.</typeparam>
public interface IMockService<TResponse>
{
    /// <summary>
    /// Creates a new instance of the response DTO.
    /// </summary>
    /// <param name="input">The creation data.</param>
    /// <returns>The created entity.</returns>
    TResponse Create(object input);

    /// <summary>
    /// Retrieves an existing entity by its ID.
    /// </summary>
    TResponse Get(Guid id);

    /// <summary>
    /// Updates an existing entity using a partial update DTO.
    /// </summary>
    void Update(Guid id, object updateDto);

    /// <summary>
    /// Removes an existing entity.
    /// </summary>
    void Delete(Guid id);

    /// <summary>
    /// Retrieves all entities of this type.
    /// </summary>
    IEnumerable<TResponse> GetAll();
}
