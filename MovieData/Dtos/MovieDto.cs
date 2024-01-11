/******************************************************************************
 *
 * File: MovieDto.cs
 *
 * Description: MovieDto.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieData.Dtos
{
    public record MovieDto(int Id, string Title, string Description, int Rating, int Year, string Category): MovieLightDto(Id, Title, Rating, Year, Category);
}
