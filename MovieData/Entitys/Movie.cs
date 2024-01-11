/******************************************************************************
 *
 * File: Movie.cs
 *
 * Description: Movie.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieData.Entitys
{
    public record Movie(int Id, string Title, string Description, int Rating, int Year, int CategoryId);
}
