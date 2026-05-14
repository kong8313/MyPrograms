using System;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Class responsible for keeping arguments for functions that supports paging. This class supports sorting by several 
    /// columns.
    /// </summary>
    public class MultiSortPagingArgs
    {
        #region Fields

        private static int m_MaxPageSize = Int32.MaxValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class.
        /// </summary>
        /// <param name="pageIndex">Page's index (zero-based).</param>
        /// <param name="pageSize">Page's size (max rows count).</param>
        /// <param name="sortingArgs">Sorting arguments.</param>
        public MultiSortPagingArgs(int pageIndex, int pageSize, SortingArgsCollection sortingArgs) :
            this(pageIndex, pageSize, sortingArgs, new SearchParameterCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class. This constructor
        /// sets flag that all records should be returned (no paging is needed).
        /// </summary>
        /// <param name="sortingArgs">Sorting arguments.</param>
        public MultiSortPagingArgs(SortingArgsCollection sortingArgs)
        {
            PageIndex = 1;
            PageSize = m_MaxPageSize;
            SortArguments = sortingArgs;
            SearchParameters = new SearchParameterCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArgs"/> class.
        /// </summary>
        /// <param name="pageIndex">Page's index (zero-based).</param>
        /// <param name="pageSize">Page's size (max rows count).</param>
        /// <param name="sortingArgs">Sorting arguments.</param>
        /// <param name="searchParameters">Search parameters.</param>
        public MultiSortPagingArgs(int pageIndex, int pageSize, SortingArgsCollection sortingArgs, SearchParameterCollection searchParameters)
        {
            if (searchParameters == null)
            {
                throw new ArgumentNullException("searchParameters");
            }

            PageIndex = pageIndex;
            PageSize = pageSize;
            SortArguments = sortingArgs;
            SearchParameters = searchParameters;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the page's index (zero-based).
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
        /// Gets or sets the sorting arguments.
        /// </summary>
        public SortingArgsCollection  SortArguments
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