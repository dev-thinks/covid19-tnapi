using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Service.Common.Utils
{
    /// <summary>
    /// Service helper methods
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        /// Parses the input Data table, check for the existence of the input column
        /// and returns the value of the identified column for the first Row 
        /// in the input data table
        /// </summary>
        /// <param name="data">Input Data table to be processed</param>
        /// <param name="columnName">Column whose value is to be fetched</param>
        /// <returns>Value of the column, Empty string if the column 
        /// does not exist or the value is null</returns>
        public static string GetValueIfColumnExists(DataTable data, string columnName)
        {
            if (data.Columns.IndexOf(columnName) != -1)
            {
                return data.Rows[0][columnName].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Parses the input Data row, check for the existence of the input column
        /// and returns the value of the identified column 
        /// </summary>
        /// <param name="dataRow">Input Data row to be processed</param>
        /// <param name="columnName">Column whose value is to be fetched</param>
        /// <returns>Value of the column, Empty string if the column 
        /// does not exist or the value is null</returns>
        public static string GetValueIfColumnExists(DataRow dataRow, string columnName)
        {
            if (dataRow.Table.Columns.IndexOf(columnName) != -1)
            {
                return dataRow[columnName].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Check for the existence of the First table in the input data set
        /// </summary>
        /// <param name="data">Input Data set to be processed</param>
        /// <returns>Returns True if a valid table with data exists in the input 
        /// data set, else False</returns>
        public static bool IsFirstTableNotEmpty(DataSet data)
        {
            if (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the input Data table and checks for the existence of 
        /// a valid data row
        /// </summary>
        /// <param name="data">Input data table to be processed</param>
        /// <returns>Returns true if a valid data exists in the input table
        /// else False</returns>
        public static bool IsTableNotEmpty(DataTable data)
        {
            if (data != null && data.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check whether a valid Data table exists in the input Data set
        /// </summary>
        /// <param name="data">Data set to be validated</param>
        /// <returns>True if a valid data table exists in the input data set,
        /// else False</returns>
        public static bool IsDataSetNotEmpty(DataSet data)
        {
            if (data != null && data.Tables.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the Input parameter and returns the Int value stored in 
        /// the input and returns '-1' if the input is not a valid Integer
        /// </summary>
        /// <param name="parameter">Input object to be converted</param>
        /// <returns>Returns the Integer value passed as parameter, '-1' 
        /// when the input is not a valid input</returns>
        public static int GetWebSafeInt(object parameter)
        {
            if (parameter != null && parameter != DBNull.Value
               && parameter.ToString().Length > 0)
            {
                IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                return Convert.ToInt32(parameter, format);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Validates the Input parameter and returns the Int value stored in 
        /// the input and returns '-1' if the input is not a valid Integer
        /// </summary>
        /// <param name="parameter">Input object to be converted</param>
        /// <returns>Returns the Integer value passed as parameter, '-1' 
        /// when the input is not a valid input</returns>
        public static long GetWebSafeLongInt(object parameter)
        {
            if (parameter != null && parameter != DBNull.Value
               && parameter.ToString().Length > 0)
            {
                IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                return Convert.ToInt64(parameter, format);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Validates the Input parameter and returns the Boolean value stored in 
        /// the input and returns 'False' if the input is not a valid Boolean value
        /// </summary>
        /// <param name="parameter">Input object to be converted</param>
        /// <returns>Returns the Boolean value passed as parameter, 'False' 
        /// when the input is not a valid Boolean</returns>
        public static bool GetWebSafeBool(object parameter)
        {
            if (parameter != null && parameter != DBNull.Value
               && parameter.ToString().Length > 0)
            {
                IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                return Convert.ToBoolean(parameter, format);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the input Data set and check for the existence of a
        /// particular table identified by the input parameter tableIndex
        /// </summary>
        /// <param name="ds">Data set to be processed</param>
        /// <param name="tableIndex">Index of the table to be checked</param>
        /// <returns>True if the table exists, else False</returns>
        public static bool IsTableNotEmpty(DataSet ds, int tableIndex)
        {
            if (ds != null && ds.Tables.Count > tableIndex
               && ds.Tables[tableIndex].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the corresponding table has rows or not
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsTableNotEmpty(DataSet ds, string tableName)
        {
            if (ds != null && ds.Tables.Count > 0
               && ds.Tables.Contains(tableName)
               && ds.Tables[tableName].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get token value for ooid and aoid based on the session id
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="orgOid"></param>
        /// <param name="assocOid"></param>
        /// <returns></returns>
        public static string GetTokenValue(string sessionId, string orgOid, string assocOid)
        {
            HMAC hmac = new HMACSHA256();
            hmac.Key = Encoding.ASCII.GetBytes(sessionId);
            string value = orgOid + "." + assocOid;
            byte[] hmres = hmac.ComputeHash(Encoding.ASCII.GetBytes(value));
            string hex = BitConverter.ToString(hmres).Replace("-", string.Empty).ToLower();
            return hex;
        }

        /// <summary>
        /// Is activity associated with associate or user
        /// </summary>
        /// <param name="activityOwner"></param>
        /// <returns></returns>
        public static bool IsAssociateActivity(string activityOwner)
        {
            List<string> siebelUserIds = new List<string> { "serviceconnect", "sbsformbuilder" };

            bool returnValue = !siebelUserIds.Exists(s => string.Equals(s, activityOwner, StringComparison.OrdinalIgnoreCase));

            // activityOwner != "serviceconnect";

            //  If the activity didn't get created by the service connect
            //  app / user, then it must be an associate created activity

            return returnValue;
        }
    }
}
