using server_agent.Monitoring.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace server_agent.Data.Provider
{
    public class SqlProvider : IDataProvider
    {
        private SqlConnection conn;

        public SqlProvider()
        {
            conn = new SqlConnection();
        }

        public bool Open()
        {
            try
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["SqlConnectionLogin"].ConnectionString;
                conn.Open();
            }
            catch (Exception)
            {
                // error log
                return false;
            }

            return true;
        }

        public void Close()
        {
            if (conn?.State == ConnectionState.Open)
                conn?.Close();
            conn?.Dispose();
        }

        public List<ServerInfoModel> ServerInfo
        {
            get
            {
                SqlCommand cmd = new SqlCommand("GetServerBinarise", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });

                List<ServerInfoModel> serverInfo = new List<ServerInfoModel>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        serverInfo.Add(new ServerInfoModel
                        {
                            BinaryPath = reader["ProcessPath"].ToString(),
                            ServerName = reader["ServerName"].ToString()
                        });
                    }
                }

                return serverInfo;
            }
        }

        public DetectTimeModel DetectTime
        {
            get
            {
                DetectTimeModel detectTime = new DetectTimeModel();

                SqlCommand cmd = new SqlCommand("GetDeadlockTime", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });

                var deadlockMin = cmd.ExecuteScalar();
                if (deadlockMin == null)
                    return null;

                detectTime.DeadlockMin = uint.Parse(deadlockMin.ToString());

                cmd = new SqlCommand("GetStoppedTime", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });

                var stoppedMin = cmd.ExecuteScalar();
                if (stoppedMin == null)
                    return null;

                detectTime.StoppedMin = uint.Parse(stoppedMin.ToString());

                return detectTime;
            }
        }
    }
}
