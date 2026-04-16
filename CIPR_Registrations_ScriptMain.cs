#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Data.Odbc;
using System.Runtime.Remoting.Messaging;
using System.IO;
#endregion

namespace ST_c3d0aa7106d9493a87e7c3708ac08bfc
{
    /// <summary>
    /// ScriptMain is the entry point class of the script.  Do not change the name, attributes,
    /// or parent of this class.
    /// </summary>
    /// 


    [Microsoft.SqlServer.Dts.Tasks.ScriptTask.SSISScriptTaskEntryPointAttribute]
    public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase
    {
        #region Help:  Using Integration Services variables and parameters in a script
        /* To use a variable in this script, first ensure that the variable has been added to 
         * either the list contained in the ReadOnlyVariables property or the list contained in 
         * the ReadWriteVariables property of this script task, according to whether or not your
         * code needs to write to the variable.  To add the variable, save this script, close this instance of
         * Visual Studio, and update the ReadOnlyVariables and 
         * ReadWriteVariables properties in the Script Transformation Editor window.
         * To use a parameter in this script, follow the same steps. Parameters are always read-only.
         * 
         * Example of reading from a variable:
         *  DateTime startTime = (DateTime) Dts.Variables["System::StartTime"].Value;
         * 
         * Example of writing to a variable:
         *  Dts.Variables["User::myStringVariable"].Value = "new value";
         * 
         * Example of reading from a package parameter:
         *  int batchId = (int) Dts.Variables["$Package::batchId"].Value;
         *  
         * Example of reading from a project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].Value;
         * 
         * Example of reading from a sensitive project parameter:
         *  int batchId = (int) Dts.Variables["$Project::batchId"].GetSensitiveValue();
         * */

        #endregion

        #region Help:  Firing Integration Services events from a script
        /* This script task can fire events for logging purposes.
         * 
         * Example of firing an error event:
         *  Dts.Events.FireError(18, "Process Values", "Bad value", "", 0);
         * 
         * Example of firing an information event:
         *  Dts.Events.FireInformation(3, "Process Values", "Processing has started", "", 0, ref fireAgain)
         * 
         * Example of firing a warning event:
         *  Dts.Events.FireWarning(14, "Process Values", "No values received for input", "", 0);
         * */
        #endregion

        #region Help:  Using Integration Services connection managers in a script
        /* Some types of connection managers can be used in this script task.  See the topic 
         * "Working with Connection Managers Programatically" for details.
         * 
         * Example of using an ADO.Net connection manager:
         *  object rawConnection = Dts.Connections["Sales DB"].AcquireConnection(Dts.Transaction);
         *  SqlConnection myADONETConnection = (SqlConnection)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Sales DB"].ReleaseConnection(rawConnection);
         *
         * Example of using a File connection manager
         *  object rawConnection = Dts.Connections["Prices.zip"].AcquireConnection(Dts.Transaction);
         *  string filePath = (string)rawConnection;
         *  //Use the connection in some code here, then release the connection
         *  Dts.Connections["Prices.zip"].ReleaseConnection(rawConnection);
         * */
        #endregion

        /// <summary>
        /// This method is called when this script task executes in the control flow.
        /// Before returning from this method, set the value of Dts.TaskResult to indicate success or failure.
        /// To open Help, press F1.
        /// </summary>
        /// 
        public class Registration
        {
            public string registrant { get; set; }
            public string ciprContact { get; set; }
            public string program { get; set; }
            public string provider { get; set; }
            public string ciprProvider { get; set; }
            public string acceptedDate { get; set; }
            public string expiryDate { get; set; }
            public string certificationDate { get; set; }
            public string certified { get; set; }
            public string active { get; set; }
            public string exported { get; set; }
            public string registrationDate { get; set; }
            public string modifiedOn { get; set; }
            public string createdOn { get; set; }
            public Registration()
            {

            }
        }
        public void Main()
        {

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                bool fireAgain = true; // Not used for anything but logging
                HttpClient http = new HttpClient();

                DateTime startDate = (DateTime)Dts.Variables["User::lastSuccessfulRun"].Value;
                var endDate = startDate.AddDays(2);

                string sessionId = Guid.NewGuid().ToString();
                string packageName = Dts.Variables["System::PackageName"].Value.ToString();
                string logFilePath = Dts.Variables["User::SSIS_LogDir"].Value.ToString() + $"/{packageName}.log";
                int retentionDays = (int)Dts.Variables["User::SSIS_Log_retentionDays"].Value;

                string apiPath = "CIPRWebApi/api/Registration/GetRegistrationUpdates";
                string apiParametersValue = $"StartDate={startDate.ToUniversalTime():s}&EndDate={endDate.ToUniversalTime():s}";
                string apiServerURL = Dts.Variables["User::HTTP_CIPR_apiServerURL"].Value.ToString();
                string apiKeyName = Dts.Variables["User::HTTP_CIPR_apiKeyName"].Value.ToString();  //"Ocp-Apim-Subscription-Key";
                string apiKeyValue = Dts.Variables["User::HTTP_CIPR_apiKeyValue"].Value.ToString();
                string apiAuthorizationName = Dts.Variables["User::HTTP_CIPR_apiAuthorizationName"].Value.ToString(); //"Authorization";
                string apiAuthorizationValue = Dts.Variables["User::HTTP_CIPR_apiAuthorizationValue"].Value.ToString();

                string odbcConnString = Dts.Connections["SYBASE_AIC"].ConnectionString;

                string apiUrl = apiServerURL + "/" + apiPath + "?" + apiParametersValue;

                DateTime startTime = DateTime.Now;
                LogMessage(logFilePath, "INFO", "Session", $"SessionID={sessionId}, Package={packageName}, Msg=START ScriptTask");

                http.DefaultRequestHeaders.Add("Accept", "application/json");
                string apiParamsCorrect = HttpUtility.UrlEncode(apiUrl);
                Dts.Events.FireInformation(0, "ScriptTask", $"{apiUrl}", "", 0, ref fireAgain);
                LogMessage(logFilePath, "INFO", "Session", $"SessionID={sessionId}, Package={packageName}, Msg=apiUrl: {apiUrl}");

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add(apiKeyName, apiKeyValue);
                request.Headers.Add(apiAuthorizationName, apiAuthorizationValue);
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                string registrationData = response.Content.ReadAsStringAsync().Result;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var registrations = serializer.Deserialize<List<Registration>>(registrationData);

                Dts.Events.FireInformation(0, "ScriptTask", $"Registration Count {registrations.Count}", "", 0, ref fireAgain);
                LogMessage(logFilePath, "INFO", "Session", $"SessionID={sessionId}, Package={packageName}, Msg=Registration Count: {registrations.Count}");

                string orgQuery = $@"SELECT org_profile_seq FROM Profile.OrgProfile WHERE org_cipr_seq = ?";

                string userQuery = $@"SELECT user_seq FROM Profile.[User] WHERE national_id_seq = ?";

                string programTypeQuery = $@"SELECT llqp_program_type_cd FROM Profile.llqp_program_type WHERE llqp_program_name = ?";

                string llqpQuery = $@"INSERT INTO Profile.LLQP (program_type, user_seq, org_profile_seq, accept_dt, expiry_dt, completion_dt, completed_yn, active_yn, start_dt, version)
                                      VALUES (?,?,?,?,?,?,?,?,?,0)";

                var programMapping = new Dictionary<string, string>
                {
                    { "A_S(Canada)", "A_S(Canada)" },
                    { "A_S(Civil Code)", "A_S(Civil Code)" },
                    { "A_S(Common Law)", "A_S(Common Law)" },
                    { "Accident and Sickness Only - 2015", "Accident and Sickness Only - 2015" },
                    { "Ethics & PP(Civil Code)", "Ethics & PP(Civil Code)" },
                    { "Ethics & PP(Common Law)", "Ethics & PP(Common Law)" },
                    { "Full LLQP - 2015", "Full LLQP - 2015" },
                    { "LLQP(Canada)", "LLQP(Canada)" },
                    { "LLQP(Civil Code)", "LLQP(Civil Code)" },
                    { "LLQP(Common Law)", "LLQP(Common Law)" },
                    { "Top Up From A_S", "Top Up From A_S" },
                    { "Top Up From A_S plus Civil Code", "Top Up From A_S plus Civil Code" },
                    { "Top Up From A_S plus Common Law", "Top Up From A_S plus Common Law" },
                };

                OdbcConnection odbc = new OdbcConnection(odbcConnString);
                odbc.Open();
                LogMessage(logFilePath, "INFO", "Database Connection", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Connection START");
                LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"ODBC START", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                foreach (var registration in registrations)
                {
                    OdbcTransaction transaction = odbc.BeginTransaction();
                    OdbcCommand command = odbc.CreateCommand();
                    command.Transaction = transaction;
                    command.Parameters.Clear();

                    var missingInfo = MissingInfo(registration);

                    if (!missingInfo)
                    {
                        try
                        {
                            // Find organization

                            command.CommandText = orgQuery;
                            command.Parameters.AddWithValue("?", registration.ciprProvider);
                            var orgId = Convert.ToInt32(command.ExecuteScalar());
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=orgId get: {orgId}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"orgId get: {orgId}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            // Find user
                            command.CommandText = userQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?", registration.ciprContact);
                            var userId = Convert.ToInt32(command.ExecuteScalar());
                            Dts.Events.FireInformation(0, "Cipr", $"{registration.ciprContact}", "", 0, ref fireAgain);
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=CIPR: {registration.ciprContact}, userId: {userId}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"CIPR: {registration.ciprContact}, userId: {userId}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            // Find program type
                            var programCode = "";
                            if (programMapping.TryGetValue(registration.program, out var prog))
                            {

                                command.CommandText = programTypeQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("?", prog);
                                programCode = command.ExecuteScalar().ToString();
                                Dts.Events.FireInformation(0, "ProgramCode", $"{programCode}", "", 0, ref fireAgain);

                            }
                            else
                            {
                                LogMessage(logFilePath, "ERROR", "Data Operation", $"SessionID={sessionId}, Package={packageName}, Msg=Key '{registration.program}' was not found in the program dictionary.");
                                LogToSYBASELog(odbc, sessionId, packageName, "ERROR", $"Key '{registration.program}' was not found in the program dictionary.", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");
                                throw new KeyNotFoundException($"Key '{registration.program}' was not found in the program dictionary.");
                            }

                            Dts.Events.FireInformation(0, "ScriptTask", $"Registration certified (misspell) {registration.program} | {registration.ciprContact} | {userId} | {orgId} | {registration.acceptedDate} | {registration.expiryDate} | {registration.certificationDate} | {registration.certified} | {registration.active} | {registration.registrationDate}", "", 0, ref fireAgain);
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=Registration certified (misspell) {registration.program} | {registration.ciprContact} | {userId} | {orgId} | {registration.acceptedDate} | {registration.expiryDate} | {registration.certificationDate} | {registration.certified} | {registration.active} | {registration.registrationDate}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"Registration certified (misspell) {registration.program} | {registration.ciprContact} | {userId} | {orgId} | {registration.acceptedDate} | {registration.expiryDate} | {registration.certificationDate} | {registration.certified} | {registration.active} | {registration.registrationDate}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            // Insert into Profile.LLQP
                            command.CommandText = llqpQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?", programCode);
                            command.Parameters.AddWithValue("?", userId);
                            command.Parameters.AddWithValue("?", orgId);
                            command.Parameters.AddWithValue("?", registration.acceptedDate);
                            command.Parameters.AddWithValue("?", registration.expiryDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("?", registration.certificationDate ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("?", registration.certified);
                            command.Parameters.AddWithValue("?", registration.active);
                            command.Parameters.AddWithValue("?", registration.registrationDate ?? (object)DBNull.Value);
                            int rowsAffected = command.ExecuteNonQuery();
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=Rows inserted to Profile.LLQP: {rowsAffected}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"Rows inserted to Profile.LLQP: {rowsAffected}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            transaction.Commit();
                            command.Dispose();
                        }

                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Dts.Events.FireInformation(0, "Failed to Execute Query", $"{ex.Message} ${ex.StackTrace}", "", 0, ref fireAgain);
                            LogMessage(logFilePath, "ERROR", "SQL_EXECUTION_FAILURE", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Query Failed to Execute");
                        }
                    }

                }

                LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Connection END");
                LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"ODBC END", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");
                odbc.Close();


                Dts.Variables["User::apiResponse"].Value = registrations;

                LogMessage(logFilePath, "INFO", "Session", $"SessionID={sessionId}, Package={packageName}, Msg=END ScriptTask");
                CleanOldLogs(logFilePath, retentionDays, fireAgain);

                Dts.TaskResult = (int)ScriptResults.Success;
            }
            catch (Exception ex)
            {
                Dts.Events.FireError(0, "HTTP Request Failed", ex.Message + "\n" + ex.StackTrace, "", 0);

                Dts.TaskResult = (int)ScriptResults.Failure;
            }

        }

        public bool MissingInfo(Registration registration)
        {
            return registration.program == null || registration.certified == null || registration.active == null;
        }

        // Log to file with structured format
        public void LogMessage(string filePath, string level, string source, string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff%K}, [{level}], [{source}]={message}");
                }
            }
            catch (Exception ex)
            {
                Dts.Events.FireError(0, "File Logging", $"Failed to write log: {ex.Message}", "", 0);
            }
        }

        // Log to SYBASE LOG with metadata
        public void LogToSYBASELog(OdbcConnection connection, string sessionId, string packageName, string level, string message, string eventTime)
        {
            if (connection == null) return;

            string logQuery = "MESSAGE STRING('SSIS: ','SessionID=',@SessionID,', PackageName=',@PackageName,', LogLevel=',@LogLevel,', LogTime=',@LogTime,', LogMessage=',@LogMessage) TO CONSOLE";
            logQuery = logQuery.Replace("@SessionID", $"'{sessionId}'");
            string sPackageName = packageName.Replace("'", "''"); // sanitize string
            logQuery = logQuery.Replace("@PackageName", $"'{sPackageName}'");
            logQuery = logQuery.Replace("@LogLevel", $"'{level}'");
            logQuery = logQuery.Replace("@LogTime", $"'{eventTime}'");
            string sMessage = message.Replace("'", "''"); // sanitize string
            logQuery = logQuery.Replace("@LogMessage", $"'{sMessage}'");

            using (OdbcCommand logCommand = new OdbcCommand(logQuery, connection))
            {
                try
                {
                    logCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Dts.Events.FireError(0, "SYBASE LOG Logging", $"Failed to insert log into SYBASE LOG: {ex.Message}", "", 0);
                }
            }
        }

        // Function to clean old logs beyond retention period
        public void CleanOldLogs(string filePath, int retentionDays, bool fireAgain)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] logs = File.ReadAllLines(filePath);
                    DateTime cutoffDate = DateTime.Now.AddDays(-retentionDays);

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (string log in logs)
                        {
                            if (log.Contains("[INFO]") || log.Contains("[WARNING]") || log.Contains("[ERROR]"))
                            {
                                string[] parts = log.Split(' ');
                                if (parts.Length > 1 && DateTime.TryParse(parts[0] + " " + parts[1], out DateTime logTime))
                                {
                                    if (logTime >= cutoffDate)
                                    {
                                        writer.WriteLine(log);
                                    }
                                }
                            }
                        }
                    }
                    Dts.Events.FireInformation(0, "Log Cleanup", $"Old logs older than {retentionDays} days deleted.", "", 0, ref fireAgain);
                }
            }
            catch (Exception ex)
            {
                Dts.Events.FireError(0, "Log Cleanup", $"Failed to clean old logs: {ex.Message}", "", 0);
            }
        }

        #region ScriptResults declaration
        /// <summary>
        /// This enum provides a convenient shorthand within the scope of this class for setting the
        /// result of the script.
        /// 
        /// This code was generated automatically.
        /// </summary>
        enum ScriptResults
        {
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        };
        #endregion

    }
}