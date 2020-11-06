using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VetCoin.Services
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum PageIndexOrigen
    {
        ZeroOrigen,
        OneOrigen
    }

    public class UrlQueryService
    {
        public UrlQueryService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public string PageKey { get; set; } = "page";

        public int RowsPerPage { get; set; } = 10;

        public string SortKey { get; set; } = "sortby";

        public string OrderKey { get; set; } = "orderby";

        public SortDirection DefaultSortDirection { get; set; } = SortDirection.Ascending;

        public PageIndexOrigen PageIndexOrigen { get; set; } = PageIndexOrigen.OneOrigen;

        public int PageToSkip(string pagekey)
        {
            var idx = 0;

            if (string.IsNullOrEmpty(pagekey) || !int.TryParse(pagekey, out idx))
                return 0;

            switch (PageIndexOrigen)
            {
                case PageIndexOrigen.ZeroOrigen:
                    return idx;
                case PageIndexOrigen.OneOrigen:
                    return idx - 1;
                default:
                    break;
            }
            throw new ArgumentException("想定外の文字列がわたってきました", nameof(pagekey));
        }

        public IEnumerable<T> PageRows<T>(IQueryable<T> source)
        {
            source = Sort(source);
            source = Pagination(source);

            return source.ToArray();
        }

        public IQueryable<T> Pagination<T>(IQueryable<T> source)
        {
            var pageKey = HttpContextAccessor.HttpContext.Request.Query[PageKey];
            var skipCount = PageToSkip(pageKey);

            return source.Skip(skipCount * RowsPerPage).Take(RowsPerPage);
        }


        public IQueryable<T> Sort<T>(IQueryable<T> source)
        {
            var sortKey = HttpContextAccessor.HttpContext.Request.Query[SortKey];
            var orderKey = HttpContextAccessor.HttpContext.Request.Query[OrderKey];
            var sortDir = ToSortDirection(orderKey);

            if (string.IsNullOrEmpty(sortKey))
            {
                return source;
            }
            return Sort(source, sortKey, sortDir);
        }

        public SortDirection ToSortDirection(string orderKey)
        {
            if (string.IsNullOrEmpty(orderKey))
                return DefaultSortDirection;

            switch (orderKey.ToLower())
            {
                case "asc":
                    return SortDirection.Ascending;
                case "desc":
                    return SortDirection.Descending;
                default:
                    break;
            }
            throw new ArgumentException("想定外の文字列がわたってきました", nameof(orderKey));
        }

        public string ToSortQuery(SortDirection sortDir)
        {
            switch (sortDir)
            {
                case SortDirection.Ascending:
                    return "asc";
                case SortDirection.Descending:
                    return "desc";
                default:
                    break;
            }

            throw new ArgumentException("想定外のSortDirがわたってきました", nameof(sortDir));
        }


        private static IQueryable<T> Sort<T>(IQueryable<T> data, string sortColumn, SortDirection sortDirection)
        {

            Expression sorterFunctionBody;
            ParameterExpression sorterFunctionParameter;
            // The IQueryable<dynamic> data source is cast as IQueryable<object> at runtime. We must call
            // SortGenericExpression using reflection so that the LINQ expressions use the actual element type.
            // Lambda: o => o.Property[.NavigationProperty,etc]
            sorterFunctionParameter = Expression.Parameter(typeof(T), "o");
            Expression member = sorterFunctionParameter;
            var type = typeof(T);
            var sorts = sortColumn.Split('.');
            foreach (var name in sorts)
            {
                PropertyInfo prop = type.GetProperty(name);
                if (prop == null)
                {
                    return data;
                }
                member = Expression.Property(member, prop);
                type = prop.PropertyType;
            }
            sorterFunctionBody = member;


            var actualSortMethod = SortGenericExpressionMethod.MakeGenericMethod(typeof(T), sorterFunctionBody.Type);
            return (IQueryable<T>)actualSortMethod.Invoke(null, new object[] { data, sorterFunctionBody, sorterFunctionParameter, sortDirection });
        }

        private static readonly MethodInfo SortGenericExpressionMethod = typeof(UrlQueryService).GetMethod("SortGenericExpression", BindingFlags.Static | BindingFlags.NonPublic);

        private static IQueryable<TElement> SortGenericExpression<TElement, TProperty>(IQueryable<TElement> data, Expression body, ParameterExpression param, SortDirection sortDirection)
        {
            //data.ToArray();

            IQueryable<TElement> data2 = data.Cast<TElement>();
            Expression<Func<TElement, TProperty>> lambda = Expression.Lambda<Func<TElement, TProperty>>(body, param);
            if (sortDirection == SortDirection.Descending)
            {
                return data2.OrderByDescending(lambda);
            }
            else
            {
                return data2.OrderBy(lambda);
            }
        }

        public PageUrlContext<T> GetPageUrlContext<T>(IQueryable<T> source)
        {
            return new PageUrlContext<T>(HttpContextAccessor, this, source);
        }
    }

    public class PageUrlContext<T>
    {
        public int TotalRowCount { get; private set; }

        public int PageCount { get; private set; }

        public int RowsPerPage { get; private set; }

        public int PageIdx { get; private set; }

        public string SortColumn { get; private set; }

        public IHttpContextAccessor HttpContextAccessor { get; }
        public UrlQueryService UrlQueryService { get; }

        public PageUrlContext(IHttpContextAccessor httpContextAccessor, UrlQueryService urlQueryService, IQueryable<T> data)
        {
            HttpContextAccessor = httpContextAccessor;
            UrlQueryService = urlQueryService;

            TotalRowCount = data.Count();
            RowsPerPage = urlQueryService.RowsPerPage;
            PageCount = (int)Math.Ceiling((double)TotalRowCount / RowsPerPage);

            PageIdx = GetPageIndex();

            SortColumn = HttpContextAccessor.HttpContext.Request.Query[UrlQueryService.SortKey];

        }

        private int GetPageIndex()
        {
            var pagekey = HttpContextAccessor.HttpContext.Request.Query[UrlQueryService.PageKey];

            var idx = 0;
            if (string.IsNullOrEmpty(pagekey) || !int.TryParse(pagekey, out idx))
                return 1;

            return idx;
        }


        public string GetSortUrlFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            List<string> memberNames = new List<string>();
            Expression currentExpression = expression.Body;
            while (currentExpression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)currentExpression;

                memberNames.Add(memberExpression.Member.Name);

                currentExpression = memberExpression.Expression;
            }

            memberNames.Reverse();
            var sortTarget = string.Join(".", memberNames.ToArray());

            return GetSortUrl(sortTarget);
        }


        public string GetPageUrl(int pageIndex)
        {
            var qs = GetQueryDic();
            qs[UrlQueryService.PageKey] = pageIndex.ToString();
            return GetPath(qs);
        }

        public string GetSortUrl(string column)
        {
            var sort = SortColumn;
            var qs = GetQueryDic();
            var qsOrderKey = qs.GetValueOrDefault(UrlQueryService.OrderKey);
            var qsSortDir = UrlQueryService.ToSortDirection(qsOrderKey);
            var sortDir = UrlQueryService.DefaultSortDirection;

            if (column.Equals(sort, StringComparison.OrdinalIgnoreCase))
            {
                if (qsSortDir == sortDir)
                {
                    sortDir = OtherSortDirection(sortDir);
                }
            }

            qs[UrlQueryService.SortKey] = column;
            qs[UrlQueryService.OrderKey] = UrlQueryService.ToSortQuery(sortDir);
            return GetPath(qs);
        }

        private SortDirection OtherSortDirection(SortDirection sortDirection)
        {
            if (sortDirection == SortDirection.Ascending)
                return SortDirection.Descending;
            return SortDirection.Ascending;

        }




        private Dictionary<string, string> GetQueryDic()
        {
            return HttpContextAccessor.HttpContext.Request.Query.ToDictionary(c => c.Key, c => c.Value.ToString());
        }

        internal string GetPath(Dictionary<string, string> queryDic)
        {
            var qs = new QueryString();
            foreach (var item in queryDic)
            {
                foreach (var item2 in item.Value.Split(','))
                {
                    qs = qs.Add(item.Key, item2);
                }
            }
            StringBuilder sb = new StringBuilder(HttpContextAccessor.HttpContext.Request.Path);
            sb.Append(qs.ToString());
            return sb.ToString();
        }

        public PagerContext GetPagerContext(int numericLinksCount = 10)
        {
            var pi = new PagerContext();
            pi.TotalCount = TotalRowCount;
            pi.TotalPageCount = PageCount;
            pi.CurrentPage = PageIdx;
            pi.EnablePaging = pi.TotalPageCount > 1;

            if (TotalRowCount > 0)
            {
                pi.FirstLink = GetPageUrl(1);
            }
            if (TotalRowCount > 1)
            {
                pi.LastLink = GetPageUrl(PageCount);
            }

            if (PageIdx != 1)
            {
                pi.HasPrev = true;
            }
            if (PageIdx != pi.TotalPageCount)
            {
                pi.HasNext = true;
            }

            pi.PrevLink = PageIdx == 1 ? string.Empty : GetPageUrl(PageIdx - 1);
            pi.NextLink = (PageIdx == PageCount) ? string.Empty : GetPageUrl(PageIdx + 1);


            int lastPage = PageCount;

            List<PageLink> list = new List<PageLink>();

            if (PageCount >= 1)
            {
                int last = PageIdx + (numericLinksCount / 2);
                int first = last - numericLinksCount;
                if (last > lastPage)
                {
                    first -= last - lastPage;
                    last = lastPage;
                }
                if (first < 0)
                {
                    last = Math.Min(last + (0 - first), lastPage);
                    first = 0;
                }
                for (int i = first; i < last; i++)
                {
                    list.Add(new PageLink { LinkUrl = GetPageUrl(i + 1), PageNumber = i + 1 });
                }
            }
            pi.PageLinks = list.ToArray();
            return pi;
        }

    }

    public class PageLink
    {
        public int PageNumber { get; set; }
        public string LinkUrl { get; set; }
    }

    public class PagerContext
    {
        public int TotalCount { get; set; }

        public bool HasNext { get; set; }
        public bool HasPrev { get; set; }

        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
        public PageLink[] PageLinks { get; set; }
        public string FirstLink { get; set; }
        public string LastLink { get; set; }
        public string PrevLink { get; set; }
        public string NextLink { get; set; }
        public bool EnablePaging { get; set; }
    }

}
