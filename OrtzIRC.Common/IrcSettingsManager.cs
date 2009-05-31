﻿namespace OrtzIRC.Common
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data.Common;

    public sealed class IRCSettingsManager
    {
        private static IRCSettingsManager instance;
        private static DbConnection db;

        private IRCSettingsManager()
        {
            //this is just here to make the class inconstructible
        }

        public static IRCSettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IRCSettingsManager();

                    DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
                    db = fact.CreateConnection();
                    
                    db.ConnectionString = "Data Source=ircsettings.db;";
                    db.Open();
                    
                    CheckDatabase();
                }
                return instance;
            }
        }

        private static void CheckDatabase()
        {
            var cmd = db.CreateCommand();
            
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS networks (
                                id integer PRIMARY KEY AUTOINCREMENT NOT NULL, 
                                name varchar(50) UNIQUE COLLATE NOCASE NOT NULL)";

            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS networks (
                                id integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                                uri varchar(50) NOT NULL,
                                ports varchar(50),
                                network_id integer NOT NULL,
                                CONSTRAINT fk_servers FOREIGN KEY (network_id) REFERENCES networks (id))";

            cmd.ExecuteNonQuery();
            //TODO: Better way to execute multiple queries?
        }

        public bool AddNetwork(string networkName)
        {
            var cmd = db.CreateCommand();
            
            cmd.CommandText = string.Format("INSERT INTO networks (Name) VALUES ('{0}')", networkName); //TODO: Sanitize?

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<NetworkSettings> GetNetworks()
        {
            var set = new List<NetworkSettings>();
            var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT * FROM networks";

            DbDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                var network = new NetworkSettings();
                network.Name = (string)rdr["Name"];
                set.Add(network);
            }
            return set;
        }

        public NetworkSettings GetNetwork(int id)
        {
            var cmd = db.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM networks WHERE id = {0}", id);

            DbDataReader rdr = cmd.ExecuteReader();

            rdr.Read();

            return new NetworkSettings { Name = (string)rdr["Name"] };
        }
    }
}