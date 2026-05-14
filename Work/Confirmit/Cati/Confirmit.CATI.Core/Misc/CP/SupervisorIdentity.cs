using System.Security.Principal;

namespace Confirmit.CATI.Core.Misc.CP
{
    /// <summary>
    /// Apollo IIdentity implementation
    /// </summary>
    public class SupervisorIdentity : IIdentity
    {
        public string m_CompanyName { get; set; }

        private string m_ClientKey;
        private string m_Company;
        private string m_Name;

        /// <summary>
        /// Gets supervisor's client key.
        /// </summary>
        public string ClientKey
        {
            get { return m_ClientKey; }
        }

        /// <summary>
        /// Gets supervisor's company. Contains number.
        /// </summary>
        public string Company
        {
            get { return m_Company; }
            set { m_Company = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Person's name.</param>
        /// <param name="clientKey">Person's client key.</param>
        /// <param name="company">Person's company.</param>
        public SupervisorIdentity(string name, string clientKey, string company, string mCompanyName)
        {
            m_CompanyName = mCompanyName;
            m_Name = name;
            m_ClientKey = clientKey;
            m_Company = company;
        }

        #region IIdentity Members
        public bool IsAuthenticated
        {
            get{ return true; }
        }

        /// <summary>
        /// Person's name.
        /// </summary>
        public string Name
        {
            get{ return m_Name; }
        }

        public string AuthenticationType
        {
            get{ return( "LDAP authentication" ); }
        }
        #endregion
    }
}