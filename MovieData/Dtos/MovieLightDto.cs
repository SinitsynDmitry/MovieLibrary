/******************************************************************************
 *
 * File: MovieLightDto.cs
 *
 * Description: MovieLightDto.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieData.Dtos
{
    public record MovieLightDto(int Id, string Title, int Rating, int Year, string Category);
}
