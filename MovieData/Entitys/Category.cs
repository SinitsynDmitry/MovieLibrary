/******************************************************************************
 *
 * File: Category.cs
 *
 * Description: Category.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieData.Entitys
{
    public record Category(int Id, string Name);

    public class CategoryList : List<Category> { }
}
