using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Represents single session variable and provides strongly-typed access to it via <c>Value</c> property.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public class SessionVariable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionVariable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="key">The key to use when variable is stored in the ASP.NET session.</param>
        public SessionVariable(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Gets or sets the key that is used when variable is stored in the ASP.NET session.
        /// </summary>
        protected string Key { get; set; }

        /// <summary>
        /// Gets or sets the value of the ASP.NET session variable.
        /// </summary>
        public T Value
        {
            get
            {
                return (T) (HttpContext.Current.Session[Key] ?? default(T));
            }

            set
            {
                HttpContext.Current.Session[Key] = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}