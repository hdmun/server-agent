using log4net;
using ServerAgent.Monitoring.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace ServerAgent.Data.Provider
{
    public class SqlProvider : IDataProvider
    {
        private readonly ILog logger;
        private SqlConnection conn;

        public SqlProvider()
        {
            logger = LogManager.GetLogger(typeof(SqlProvider));
            conn = new SqlConnection();
        }

        public bool Open()
        {
            try
            {
                var connectionString = ConfigurationManager.AppSettings["SqlConnection"];
                conn.ConnectionString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
                conn.Open();
            }
            catch (Exception ex)
            {
                logger.Error($"Exception `SqlConnection.Open`", ex);
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
                SqlCommand cmd = new SqlCommand("GetServerProcess", conn);
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

                SqlCommand cmd = new SqlCommand("GetConfigValue", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@key",
                    SqlDbType = SqlDbType.VarChar,
                    Value = "DeadlockTime"
                });

                var deadlockMin = cmd.ExecuteScalar();
                if (deadlockMin == null)
                    return null;

                detectTime.DeadlockMin = uint.Parse(deadlockMin.ToString());

                cmd = new SqlCommand("GetConfigValue", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@key",
                    SqlDbType = SqlDbType.VarChar,
                    Value = "StoppedTime"
                });

                var stoppedMin = cmd.ExecuteScalar();
                if (stoppedMin == null)
                    return null;

                detectTime.StoppedMin = uint.Parse(stoppedMin.ToString());

                cmd = new SqlCommand("GetConfigValue", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@hostName",
                    SqlDbType = SqlDbType.VarChar,
                    Value = Dns.GetHostName()
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    ParameterName = "@key",
                    SqlDbType = SqlDbType.VarChar,
                    Value = "Checker"
                });

                var checkerName = cmd.ExecuteScalar();
                if (checkerName == null)
                    return null;

                detectTime.Checker = checkerName.ToString();

                return detectTime;
            }
        }
    }
}
