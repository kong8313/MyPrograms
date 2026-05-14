using System;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Class responsible for keeping arguments for functions that supports paging.
    /// </summary>
    public class PagingArgs
    {
        #region Fields

        private static int m_MaxPageSize = Int32.MaxValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class.
        /// </summary>
        public PagingArgs()
        {
            SearchParameters = new SearchParameterCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class.
        /// </summary>
        /// <param name="pageIndex">Page's index (zero-based).</param>
        /// <param name="pageSize">Page's size (max rows count).</param>
        /// <param name="sortField">Sorting field name.</param>
        /// <param name="sortOrderAsc">If set to <c>true</c> sorting order is ascending.</param>
        public PagingArgs(int pageIndex, int pageSize, string sortField, bool sortOrderAsc):
            this(pageIndex, pageSize, sortField, sortOrderAsc, new SearchParameterCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class. This constructor
        /// sets flag that all records should be returned (no paging is needed).
        /// </summary>
        /// <param name="sortField">Sorting field name.</param>
        /// <param name="sortOrderAsc">if set to <c>true</c> sorting order is ascending.</param>
        public PagingArgs(string sortField, bool sortOrderAsc)
        {
            PageIndex = 1;
            PageSize = m_MaxPageSize;
            SortField = sortField;
            SortOrderAsc = sortOrderAsc;
            SearchParameters = new SearchParameterCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class.
        /// </summary>
        /// <param name="pageIndex">Page's index (zero-based).</param>
        /// <param name="pageSize">Page's size (max rows count).</param>
        /// <param name="sortField">Sorting field name.</param>
        /// <param name="sortOrderAsc">If set to <c>true</c> sorting order is ascending.</param>
        /// <param name="searchParameters">Search parameters.</param>
        public PagingArgs(int pageIndex, int pageSize, string sortField, bool sortOrderAsc, SearchParameterCollection searchParameters)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException("searchParameters");
            }

            PageIndex = pageIndex;
            PageSize = pageSize;
            SortField = sortField;
            SortOrderAsc = sortOrderAsc;
            SearchParameters = searchParameters;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the page's index (one-based).
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the page (max rows count).
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sorting field name.
        /// </summary>
        public string SortField
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting order is ascending.
        /// </summary>
        public bool SortOrderAsc
        {
            get;
            set;
        }

        /// <summary>
        /// Gets flag indication that paging should be performed.
        /// </summary>
        public bool NeedPaging
        {
            get
            {
                return !(PageIndex == 1 && PageSize == m_MaxPageSize);
            }
        }

        /// <summary>
        /// Gets one-based index of first element of given page in collection.
        /// </summary>
        public int StartElementIndex
        {
            get
            {
                if (NeedPaging)
                {
                    return (PageIndex - 1) * PageSize + 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Gets count of elements.
        /// </summary>
        public int ElementsCount
        {
            get
            {
                if (NeedPaging)
                {
                    return PageSize;
                }
                else
                {
                    return m_MaxPageSize;
                }
            }
        }

        /// <summary>
        /// Gets/sets search parameters.
        /// </summary>
        public SearchParameterCollection SearchParameters
        {
            get;
            set;
        }

        #endregion
    }
}