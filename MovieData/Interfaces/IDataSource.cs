/******************************************************************************
 *
 * File: IDataSource.cs
 *
 * Description: IDataSource.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using MovieData.Dtos;
using MovieData.Entitys;
using MovieData.Helpers;

namespace MovieData.Interfaces
{
    public interface IDataSource
    {
        /// <summary>
        /// Gets the categories async.
        /// </summary>
        /// <returns>A Task.</returns>
        Task<List<Category>> GetCategoriesAsync();

        /// <summary>
        /// Gets the movie list async.
        /// </summary>
        /// <param name="selectAndOrder">The select and order.</param>
        /// <returns>A Task.</returns>
        Task<List<MovieLightDto>> GetMovieListAsync(SelectAndOrder? selectAndOrder = null);
        /// <summary>
        /// Gets the movie dto by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A Task.</returns>
        Task<MovieDto> GetMovieDtoByIdAsync(int id);
        
    }
}
