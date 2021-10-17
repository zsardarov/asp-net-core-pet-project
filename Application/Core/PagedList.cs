using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Followers.UseCases;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    public class PagedList<T> : List<T>
    {
        private PagedList(IEnumerable<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            var slice = await queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var count = await queryable.CountAsync();
            return new PagedList<T>(slice, count, pageNumber, pageSize);
        }
    }
}