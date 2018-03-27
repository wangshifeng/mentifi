using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Helpers
{
    public static class PageListMapper
    {
        public static PagedListModel<T> Map<T, TU>(this IEnumerable<T> items, IPagedList<TU> pagedList)
        {
            return new PagedListModel<T>()
            {
                Items = items,
                TotalCount = pagedList.TotalCount,
                PageIndex = pagedList.PageIndex,
                IndexFrom = pagedList.IndexFrom,
                PageSize = pagedList.PageSize,
                HasNextPage = pagedList.HasNextPage,
                HasPreviousPage = pagedList.HasPreviousPage,
                TotalPages = pagedList.TotalPages
            };
        }
        public static PagedListModel<T> Map<T>(this IPagedList<T> item)
        {
            return new PagedListModel<T>
            {
                Items = item.Items,
                TotalCount = item.TotalCount,
                PageIndex = item.PageIndex,
                IndexFrom = item.IndexFrom,
                PageSize = item.PageSize,
                HasNextPage = item.HasNextPage,
                HasPreviousPage = item.HasPreviousPage,
                TotalPages = item.TotalPages
            };
        }

    }
}
