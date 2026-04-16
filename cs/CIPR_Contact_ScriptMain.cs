#region Help:  Introduction to the script task
/* The Script Task allows you to perform virtually any operation that can be accomplished in
 * a .Net application within the context of an Integration Services control flow. 
 * 
 * Expand the other regions which have "Help" prefixes for examples of specific ways to use
 * Integration Services features within this script task. */
#endregion


#region Namespaces
using System;
using System.Data;
using System.Net.Http;
using System.Web;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Data.Odbc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Linq;
using System.Security.Policy;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using System.IO;
using static ST_c3d0aa7106d9493a87e7c3708ac08bfc.ScriptMain;
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
        public class Contact
        {
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string ciprNumber { get; set; }
            public string email { get; set; }
            public string street { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string postalCode { get; set; }
            public string province { get; set; }
            public string birthday { get; set; }
            public string cell { get; set; }
            public string phone { get; set; }
            public string fax { get; set; }
            public string jursidiction { get; set; }
            public string active { get; set; }
            public string isVoid { get; set; }
            public string emailConfirmed { get; set; }
            public string registrationDate { get; set; }
            public string modifiedOn { get; set; }
            public string createdOn { get; set; }
            public Contact()
            {

            }
        }
        public void Main()
        {

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                HttpClient http = new HttpClient();

                DateTime startDate = (DateTime)Dts.Variables["User::lastSuccessfulRun"].Value;
                var endDate = startDate.AddDays(2);

                bool fireAgain = true;
                string sessionId = Guid.NewGuid().ToString();
                string packageName = Dts.Variables["System::PackageName"].Value.ToString();
                string logFilePath = Dts.Variables["User::SSIS_LogDir"].Value.ToString() + $"/{packageName}.log";
                int retentionDays = (int)Dts.Variables["User::SSIS_Log_retentionDays"].Value;

                string apiPath = "CIPRWebApi/api/Contact/GetContactUpdates";
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
                string contactData = response.Content.ReadAsStringAsync().Result;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var contacts = serializer.Deserialize<List<Contact>>(contactData).Where(contact => contact.emailConfirmed == "Yes");
                Dts.Events.FireInformation(0, "ScriptTask", $"ContactCount: {contacts.Count()}", "", 0, ref fireAgain);
                LogMessage(logFilePath, "INFO", "Session", $"SessionID={sessionId}, Package={packageName}, Msg=ContactCount: {contacts.Count()}");
                var currentDate = DateTime.UtcNow;

                string orgQuery = $@"SELECT org_profile_seq FROM Profile.OrgProfile WHERE legal_name = ?;";

                string addressQuery = $@"INSERT INTO Profile.Address (street, postal_code, city, province, country, version)
                                        VALUES (?,?,?,?,?,0); SELECT @@identity AS inserted_id;";

                string userQuery = $@"
                                    INSERT INTO Profile.[User] (national_id_seq, username, status, voided_ts, verified_ts, version, last_updated, date_created, password)
                                    VALUES (?,?,?,?,?, 0, ?,?,?); SELECT @@identity AS inserted_id;";

                string userExistsQuery = $@"SELECT user_seq FROM Profile.[User] WHERE national_id_seq = ?";

                string userProfileQuery = $@"
                        INSERT INTO Profile.UserProfile (first_name, middle_name, last_name, email, birth_date, address_id, user_id, date_created, jurisdiction_profile_id, version, last_updated)
                        VALUES(?,?,?,?,?,?,?,?,?, 0, ?); SELECT @@identity AS inserted_id;";

                var orgMapping = new Dictionary<string, string>
                {
                    { "Alberta Insurance Council (AIC)", "Alberta Insurance Council" },
                    { "Insurance Council of British Columbia", "Insurance Council of British Columbia " },
                    { "Insurance Council of Manitoba", "Insurance Council of Manitoba" },
                    { "New Brunswick", "New Brunswick" },
                    { "Newfoundland & Labrador", "Newfoundland & Labrador" },
                    { "Northwest Territories", "Northwest Territories" },
                    { "Nova Scotia Financial Institutions", "Nova Scotia Financial Institutions" },
                    { "Nunavut", "Nunavut" },
                    { "Financial Services Regulatory Authority of Ontario (FSRA)", "Financial Services Regulatory Authority of Ontario" },
                    { "Registered Insurance Brokers of Ontario", "Registered  Insurance Brokers of Ontario" },
                    { "Prince Edward Island", "Prince Edward Island" },
                    { "The Autorité des marchés financiers (AMF)", "The Autorité des marchés financiers (AMF)" },
                    { "Chambre de la sécurité financière (CSF)", "Chambre de la sécurité financière (CSF)" },
                    { "Chambre de l'assurance de dommages (ChAD)", "Chambre de l'assurance de dommages (ChAD)" },
                    { "Insurance Councils of Saskatchewan", "Insurance Councils of Saskatchewan" },
                    { "Yukon", "Yukon" }
                };


                OdbcConnection odbc = new OdbcConnection(odbcConnString);
                odbc.Open();
                LogMessage(logFilePath, "INFO", "Database Connection", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Connection START");
                LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"ODBC START", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                foreach (var contact in contacts)
                {

                    OdbcTransaction transaction = odbc.BeginTransaction();
                    OdbcCommand command = odbc.CreateCommand();
                    command.Transaction = transaction;
                    command.Parameters.Clear();

                    try
                    {

                        command.Parameters.Clear();
                        var userExists = false;
                        command.CommandText = userExistsQuery;
                        command.Parameters.AddWithValue("?", contact.ciprNumber);
                        userExists = command.ExecuteScalar() != null ? true : false;
                        var invalidContact = IsMissingInfo(contact);
                        if (!userExists && !invalidContact)
                        {
                            var orgId = 0;

                            // Select org
                            if (contact.jursidiction != null)
                            {

                                if (orgMapping.TryGetValue(contact.jursidiction, out var orgValue))
                                {
                                    command.CommandText = orgQuery;
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("?", orgValue);
                                    orgId = Convert.ToInt32(command.ExecuteScalar());
                                }
                                else
                                {
                                    LogMessage(logFilePath, "ERROR", "Prepare Data", $"SessionID={sessionId}, Package={packageName}, Msg=Key '{contact.jursidiction}' was not found in the org dictionary");
                                    LogToSYBASELog(odbc, sessionId, packageName, "ERROR", $"Key '{contact.jursidiction}' was not found in the org dictionary", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");
                                    throw new KeyNotFoundException($"Key '{contact.jursidiction}' was not found in the org dictionary.");
                                }
                            }

                            // Insert into Profile.Address
                            command.CommandText = addressQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?", contact.street);
                            command.Parameters.AddWithValue("?", contact.postalCode ?? "");
                            command.Parameters.AddWithValue("?", contact.city ?? "");
                            command.Parameters.AddWithValue("?", contact.province ?? "");
                            command.Parameters.AddWithValue("?", contact.country ?? "");
                            var addressId = Convert.ToInt32(command.ExecuteScalar());
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=addressId inserted: {addressId}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"addressId inserted: {addressId}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            // Insert into Profile.User
                            command.CommandText = userQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?", contact.ciprNumber);
                            command.Parameters.AddWithValue("?", contact.email);

                            var status = contact.active == "Yes" ? "ACTIVE" : "INACTIVE";
                            command.Parameters.AddWithValue("?", status);

                            if (contact.isVoid == "Yes")
                            {
                                command.Parameters.AddWithValue("?", currentDate);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("?", (object)DBNull.Value);
                            }

                            if (contact.emailConfirmed == "Yes")
                            {
                                command.Parameters.AddWithValue("?", currentDate);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("?", (object)DBNull.Value);
                            }

                            command.Parameters.AddWithValue("?", currentDate);
                            command.Parameters.AddWithValue("?", currentDate);
                            command.Parameters.AddWithValue("?", "");
                            var result = command.ExecuteScalar();
                            var userId = Convert.ToInt32(result);
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=userId inserted: {userId}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"userId inserted: {userId}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");

                            // Insert into Profile.UserProfile
                            command.CommandText = userProfileQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?", contact.firstName);
                            command.Parameters.AddWithValue("?", contact.middleName);
                            command.Parameters.AddWithValue("?", contact.lastName);
                            command.Parameters.AddWithValue("?", contact.email);
                            command.Parameters.AddWithValue("?", contact.birthday);
                            command.Parameters.AddWithValue("?", addressId);
                            command.Parameters.AddWithValue("?", userId);
                            command.Parameters.AddWithValue("?", contact.registrationDate);

                            if (orgId == 0)
                            {
                                command.Parameters.AddWithValue("?", null);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("?", orgId);
                            }

                            command.Parameters.AddWithValue("?", currentDate);
                            int rowsAffected = command.ExecuteNonQuery();
                            LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=Rows inserted to userProfile: {rowsAffected}");
                            LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"Rows inserted to userProfile: {rowsAffected}", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");
                        }
                        transaction.Commit();
                        command.Dispose();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Dts.Events.FireInformation(0, "Query Failed to Execute", $"{ex.Message} {ex.StackTrace}", "", 0, ref fireAgain);
                        LogMessage(logFilePath, "ERROR", "SQL_EXECUTION_FAILURE", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Query Failed to Execute");
                    }
                }

                LogMessage(logFilePath, "INFO", "Database Operation", $"SessionID={sessionId}, Package={packageName}, Msg=ODBC Connection END");
                LogToSYBASELog(odbc, sessionId, packageName, "INFO", $"ODBC END", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff%K}");
                odbc.Close();

                Dts.Variables["User::apiResponse"].Value = contacts;

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

        public bool IsMissingInfo(Contact contact)
        {
            return (contact.firstName == null || contact.lastName == null || contact.email == null || contact.birthday == null);
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