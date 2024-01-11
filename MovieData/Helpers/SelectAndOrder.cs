/******************************************************************************
 *
 * File: SelectAndOrder.cs
 *
 * Description: SelectAndOrder.cs class and he's methods.
 *
 * Copyright (C) 2024 by Dmitry Sinitsyn
 *
 * Date: 11.1.2024	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace MovieData.Helpers
{
    public class SelectAndOrder
    {
        /// <summary>
        /// Gets the search term.
        /// </summary>
        public string SearchTerm { get; init; }
        /// <summary>
        /// Gets the selected categories.
        /// </summary>
        public List<int> SelectedCategories { get; init; } = new List<int>();
        /// <summary>
        /// Gets the order_by.
        /// </summary>
        public string Order_by { get; init; } = "title_asc";
    }
}
