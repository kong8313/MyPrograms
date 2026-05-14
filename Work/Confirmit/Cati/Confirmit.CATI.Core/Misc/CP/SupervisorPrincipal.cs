using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Core.Services;
using Firmglobal.Framework.Security;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;

namespace Confirmit.CATI.Core.Misc.CP
{
    /// <summary>
    /// Apollo IPrincipal implementation
    /// </summary>
    public class SupervisorPrincipal : IPrincipal
    {
        private SupervisorIdentity m_Identity = null;

        /// <summary>
        /// Person's name.
        /// </summary>
        public string Name
        {
            get { return m_Identity != null ? m_Identity.Name : null; }
        }

        /// <summary>
        /// Person's client key.
        /// </summary>
        public string ClientKey
        {
            get { return m_Identity != null ? m_Identity.ClientKey : null; }
        }

        /// <summary>
        /// Person's company.
        /// </summary>
        public string Company
        {
            get { return m_Identity != null ? m_Identity.Company : null; }
        }

        /// <summary>
        /// Person's company.
        /// </summary>
        public string CompanyName
        {
            get { return m_Identity != null ? m_Identity.m_CompanyName : string.Empty; }
        }

        public bool IsProsUser
        {
            get;
            private set;
        }

        public bool IsSystemAdministrator
        {
            get;
            private set;
        }

        public bool IsSuperviseMonitorOnly
        {
            get;
            private set;
        }
        
        private readonly bool _isCatiAdministrator;

        public bool IsCatiAdministratorOrPros
        {
            get
            {
                return IsProsUser || _isCatiAdministrator;
            }
        }

        public bool IsSystemProjectAdministrator
        {
            get;
            private set;
        }
        public bool IsCallCenterSupervisor
        {
            get;
            private set;
        }
        public bool IsCatiParentAdministrator { get; private set; }
        public bool IsCatiProjectAdministrator { get; private set; }
        public bool IsCatiDialerAdministrator { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clientKey">Security client key, passed from confirmit.</param>
        /// <param name="company">Company of current user.</param>
        /// <param name="companyName"></param>
        /// <param name="allowedTabs">Allowed tabs for current user.</param>
        /// <param name="isCatiAdministrator"></param>
        /// <param name="isProsUser"></param>
        /// <param name="useSsl"></param>
        public SupervisorPrincipal(
            string name, 
            string clientKey, 
            string company, 
            string companyName, 
            Tabs allowedTabs, 
            bool isCatiAdministrator, 
            bool isProsUser, 
            bool useSsl)
        {
            m_Identity = new SupervisorIdentity(name, clientKey, company, companyName);
            AllowedTabs = allowedTabs;
            IsSystemAdministrator = false;
            IsProsUser = isProsUser;
            _isCatiAdministrator = isCatiAdministrator;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="confirmitPrincipal"></param>
        /// <param name="allowedTabs">Allowed tabs for current user.</param>
        public SupervisorPrincipal(
            ConfirmitPrincipal confirmitPrincipal,
            Tabs allowedTabs)
        {
            IsSystemAdministrator = confirmitPrincipal.IsInRole(SystemPermissions.SystemAdministrate);
            IsProsUser = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiSupervisorAdmin) || confirmitPrincipal.IsInRole(SystemPermissions.AccountRead);
            _isCatiAdministrator = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiAdministrate);
            IsCatiParentAdministrator = _isCatiAdministrator && confirmitPrincipal.IsInRole(SystemPermissions.SystemParentCatiAdministrate);
            IsCatiProjectAdministrator = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiProjectAdministrate);
            IsCatiDialerAdministrator = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiDialerAdmin) || IsCatiAdministratorOrPros;
            IsSystemProjectAdministrator = confirmitPrincipal.IsInRole(SystemPermissions.SystemProjectAdministrate);
            IsCallCenterSupervisor = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiSuperviseCallCenter);

            m_Identity = new SupervisorIdentity(
                confirmitPrincipal.ConfirmitIdentity.Name,
                confirmitPrincipal.ConfirmitIdentity.ClientKey,
                confirmitPrincipal.ConfirmitIdentity.CompanyId.ToString(),
                confirmitPrincipal.ConfirmitIdentity.CompanyName);
            AllowedTabs = allowedTabs;

            IsSuperviseMonitorOnly = confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiSuperviseMonitor) && 
                                     !confirmitPrincipal.IsInRole(SystemPermissions.SystemCatiSuperviseActivity) &&
                                     !IsCatiAdministratorOrPros &&
                                     !IsSystemAdministrator;
        }

        /// <summary>
        /// Gets the current user principal object.
        /// </summary>
        public static SupervisorPrincipal Current
        {
            get
            {
                if (HttpContext.Current.User is SupervisorPrincipal)
                {
                    return (SupervisorPrincipal)HttpContext.Current.User;
                }

                return ServiceLocator.Resolve<IdentityService>().CreateSupervisorPrincipalByConfirmitIdentity((ConfirmitIdentity)HttpContext.Current.User.Identity);
            }
        }

        /// <summary>
        /// Returns ApolloIdentity object for the current person.
        /// </summary>
        public SupervisorIdentity ApolloIdentity
        {
            get { return (m_Identity); }
        }

        /// <summary>
        /// Gets tabs allowed for current user.
        /// </summary>
        public Tabs AllowedTabs
        {
            get;
            set;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return (m_Identity); }
        }

        /// <summary>
        /// Determines if user belongs to given role.
        /// </summary>
        /// <param name="role">Role name.</param>
        /// <returns>true, if user belongs to role; otherwise false.</returns>
        public bool IsInRole(string role)
        {
            bool result = false;

            if (role == "CatiAdministratorOrPros")
            {
                return IsCatiAdministratorOrPros;
            }

            if (role == "Pros")
            {
                return IsProsUser;
            }

            try
            {
                Tabs givenRole = (Tabs)Enum.Parse(typeof(Tabs), role);
                
                result = (AllowedTabs & givenRole) == givenRole;
            }
            catch (Exception /*ex*/)
            {
                Trace.TraceError("User role {0} is not supported.", role);
            }

            return result;
        }

        #endregion
    }
}