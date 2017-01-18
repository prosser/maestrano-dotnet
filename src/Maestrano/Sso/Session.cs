using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Web.SessionState;

namespace Maestrano.Sso
{
    [Flags]
    public enum SessionValidationSteps
    {
        /// <summary>
        /// No validation will be performed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Validation will succeed immediately if there HttpSessionState is null
        /// </summary>
        ValidIfNoHttpSessionState = 0x1,

        /// <summary>
        /// Validation will fail if HttpSessionState is null.
        /// </summary>
        RequireHttpSessionState = 0x2,

        /// <summary>
        /// Validation will fail if there are no event listeners subscribed to the Saved event.
        /// </summary>
        RequireEventListener = 0x4,

        /// <summary>
        /// IsValid will call PerformRemoteCheck (if required), and validation will fail if that call fails.
        /// </summary>
        PerformRemoteCheck = 0x5,

        Default = PerformRemoteCheck,
    }

    public class Session
    {
        private const string HttpSessionStateKey = "maestrano";

        /// <summary>
        /// Create an empty Maestrano SSO Session
        /// </summary>
        public Session()
        {
        }

        /// <summary>
        /// Create a Maestrano SSO session that uses an HttpSessionState object as its backing store
        /// </summary>
        /// <param name="httpSessionObj"></param>
        public Session(HttpSessionState httpSessionObj)
            : this(httpSessionObj, null)
        {
        }

        /// <summary>
        /// Create a Maestrano SSO session that uses an HttpSessionState object as its backing store and
        /// is initialized with the data from a User object
        /// </summary>
        /// <param name="httpSessionObj">HttpSessionState object to use as the backing store for the session</param>
        /// <param name="user">User object that contains the initialization data for the Session</param>
        /// <remarks>Updates to the User object will not be automatically reflected by the Session</remarks>
        public Session(HttpSessionState httpSessionObj, User user)
            : this(httpSessionObj == null ? null : httpSessionObj[HttpSessionStateKey] as string, user)
        {
            HttpSession = httpSessionObj;
        }

        /// <summary>
        /// Create a Maestrano SSO session that is initialized with the data from a User object
        /// </summary>
        /// <param name="user">User object that contains the initialization data for the Session</param>
        /// <remarks>Updates to the User object will not be automatically reflected by the Session</remarks>
        public Session(User user)
            : this((string)null, user)
        {
        }

        private Session(string encodedMnoSession, User user)
        {
            if (encodedMnoSession != null)
            {
                var enc = System.Text.Encoding.UTF8;
                JObject sessionObject = new JObject();
                try
                {
                    string decoded = enc.GetString(Convert.FromBase64String(encodedMnoSession));
                    sessionObject = JObject.Parse(decoded);
                }
                catch (Exception) { }

                // Assign attributes
                Uid = sessionObject.Value<String>("uid");
                GroupUid = sessionObject.Value<String>("group_uid");
                SessionToken = sessionObject.Value<String>("session");

                // Session Recheck
                try
                {
                    Recheck = sessionObject.Value<DateTime>("session_recheck");
                }
                catch (Exception) { }
            }

            if (Recheck == null)
                Recheck = DateTime.UtcNow.AddMinutes(-1);

            if (user != null)
            {
                Uid = user.Uid;
                GroupUid = user.GroupUid;
                SessionToken = user.SsoSession;
                Recheck = user.SsoSessionRecheck;
            }
        }

        public event EventHandler Saved;

        public string GroupUid { get; set; }

        public HttpSessionState HttpSession { get; set; }

        public DateTime Recheck { get; set; }

        public string SessionToken { get; set; }

        public string Uid { get; set; }

        public static Session FromEncodedString(string encodedMnoSession)
        {
            Session session = new Session(encodedMnoSession, null);
            return session;
        }

        /// <summary>
        /// Returns whether the session needs to be checked
        /// remotely from Maestrano or not
        /// </summary>
        /// <returns>True if the current time is past the recheck time
        /// or if any property is null. Otherwise, false.</returns>
        public Boolean isRemoteCheckRequired()
        {
            if (Uid != null && SessionToken != null && Recheck != null)
            {
                return (Recheck.CompareTo(DateTime.UtcNow) <= 0);
            }
            return true;
        }

        /// <summary>
        /// Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <returns>True if the Session should be treated as valid, otherwise false.</returns>
        public Boolean IsValid()
        {
            return IsValid(null, SessionValidationSteps.Default);
        }

        /// <summary>
        /// Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <param name="steps">Validation steps to perform</param>
        /// <returns>True if the Session should be treated as valid, otherwise false.</returns>
        public Boolean IsValid(SessionValidationSteps steps)
        {
            return IsValid(null, steps);
        }

        /// <summary>
        /// Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <param name="client">RestClient to use for Remote validation</param>
        /// <returns>True if the Session should be treated as valid, otherwise false.</returns>
        public Boolean IsValid(RestClient client)
        {
            return IsValid(client, SessionValidationSteps.Default);
        }

        /// <summary>
        /// Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <param name="client">RestClient to use for Remote validation</param>
        /// <param name="steps">Validation steps to perform</param>
        /// <returns>True if the Session should be treated as valid, otherwise false.</returns>
        public Boolean IsValid(RestClient client, SessionValidationSteps steps)
        {
            if (steps == SessionValidationSteps.None)
            {
                throw new ArgumentOutOfRangeException("steps", "IsValid should be called if you are not going to do any validation!");
            }

            if ((steps & SessionValidationSteps.ValidIfNoHttpSessionState) != SessionValidationSteps.ValidIfNoHttpSessionState)
            {
                throw new ArgumentOutOfRangeException("steps", "ValidIfNoHttpSessionState must be the only step if it is specified.");
            }

            // Return true automatically if Single Logout (SLO) is disabled
            if (!MnoHelper.Sso.SloEnabled)
                return true;

            // Return true if maestrano session not set
            // and only the HttpSessionState step is requested
            if (steps == SessionValidationSteps.ValidIfNoHttpSessionState && (HttpSession == null || HttpSession["maestrano"] == null))
                return true;

            // Perform local validation
            if ((steps & (SessionValidationSteps.RequireEventListener | SessionValidationSteps.RequireHttpSessionState)) == steps)
            {
                if (HttpSession == null && Saved == null)
                    return false;
            }
            else
            {
                if (steps.HasFlag(SessionValidationSteps.RequireHttpSessionState) && HttpSession == null)
                    return false;

                if (steps.HasFlag(SessionValidationSteps.RequireEventListener) && Saved == null)
                    return false;
            }

            if (steps.HasFlag(SessionValidationSteps.PerformRemoteCheck) && isRemoteCheckRequired())
            {
                if (client == null)
                {
                    client = new RestClient(MnoHelper.Sso.Idp);
                }

                if (PerformRemoteCheck(client))
                {
                    Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Obsolete. Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <param name="ifSession">If set to true then session returns
        /// false ONLY if Maestrano session exists and is invalid</param>
        /// <returns>True if the Session should be treated as valid, otherwise false.</returns>
        [Obsolete("Use IsValid(SessionValidationTypes) instead")]
        public Boolean IsValid(Boolean ifSession)
        {
            var client = new RestClient(MnoHelper.Sso.Idp);
            return IsValid(client, ifSession ? SessionValidationSteps.RequireHttpSessionState : SessionValidationSteps.Default);
        }

        /// <summary>
        /// Obsolete. Return whether the session is valid or not. Performs a
        /// remote check to Maestrano if required.
        /// </summary>
        /// <param name="client">RestClient initialized to point to
        /// a Maestrano API environment.</param>
        /// <param name="ifSession"></param>
        /// <returns></returns>
        [Obsolete("Use IsValid(RestClient, SessionValidationTypes) instead")]
        public Boolean IsValid(RestClient client, Boolean ifSession)
        {
            return IsValid(client, ifSession ? SessionValidationSteps.RequireHttpSessionState : SessionValidationSteps.Default);
        }

        /// <summary>
        /// Check whether the remote Maestrano session is still valid
        /// </summary>
        /// <param name="client">RestClient initialized to point to a Maestrano API environment.</param>
        /// <returns>True if the Maestrano API successfully validated this Session, otherwise false.</returns>
        public Boolean PerformRemoteCheck(RestClient client)
        {
            if (Uid != null && SessionToken != null && Uid.Length > 0 && SessionToken.Length > 0)
            {
                // Prepare request
                var request = new RestRequest("api/v1/auth/saml/{id}", Method.GET);
                request.AddUrlSegment("id", Uid);
                request.AddParameter("session", SessionToken);
                JObject resp = new JObject();
                try
                {
                    resp = JObject.Parse(client.Execute(request).Content);
                }
                catch (Exception) { }

                bool valid = Convert.ToBoolean(resp.Value<String>("valid"));
                string dateStr = resp.Value<String>("recheck");
                if (valid && dateStr != null && dateStr.Length > 0)
                {
                    Recheck = DateTime.Parse(dateStr);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check whether the remote Maestrano session is still valid.
        /// </summary>
        /// <returns></returns>
        public Boolean PerformRemoteCheck()
        {
            var client = new RestClient(MnoHelper.Sso.Idp);
            return PerformRemoteCheck(client);
        }

        /// <summary>
        /// Save the Maestrano session in HTTP Session
        /// </summary>
        public void Save()
        {
            // notify subscribers that things have changed
            if (Saved != null)
            {
                Saved(this, EventArgs.Empty);
            }

            // if an HttpSession is being used, update it
            if (HttpSession != null)
            {
                string encodedString = ToEncodedString();
                HttpSession[HttpSessionStateKey] = encodedString;
            }
        }

        public string ToEncodedString()
        {
            var enc = System.Text.Encoding.UTF8;
            JObject sessionObject = new JObject(
                new JProperty("uid", Uid),
                new JProperty("session", SessionToken),
                new JProperty("session_recheck", Recheck.ToString("s")),
                new JProperty("group_uid", GroupUid));

            return Convert.ToBase64String(enc.GetBytes(sessionObject.ToString()));
        }
    }
}