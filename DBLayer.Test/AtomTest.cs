using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBLayer.Persistence.Configuration;
using DBLayer.Persistence.Data;
using System.Data;


namespace DBLayer.Test
{
    [TestClass]
    public class AtomTest
    {
        [TestMethod]
        public void ConfigurationTest()
        {
            var databases = AtomSourceSettings.Instance.ConfigSection.AtomDataSources;
            var properties = PropertySettings.Instance.ConfigSection.AtomProperties;
            var providers = ProviderSettings.Instance.ConfigSection.AtomProviders;

            foreach (var item in databases)
            {

            }

            foreach (var item in properties)
            {

            }

            foreach (var item in providers)
            {

            }


        }
        [TestMethod]
        public void SqlServerTest()
        {

            var dsGt1 = DataSourceFactory.Instance.GetDataSource("SqlDataSource");
            var dicList = new List<IDictionary<string, object>>();
            using (var admin = dsGt1.DbProvider.GetDbProviderFactory().CreateConnection())
            {
                admin.ConnectionString = dsGt1.ConnectionString.ToString();
                admin.Open();
                var cmd = admin.CreateCommand();
                cmd.CommandText = "select top 10 * from sys_user";
                cmd.CommandType = CommandType.Text;

                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (read.Read())
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("user_id", read["user_id"]);
                        dicList.Add(dic);
                    }
                }
            }
           
        }

        [TestMethod]
        public void OracleTest()
        {
            var dsGt5 = DataSourceFactory.Instance.GetDataSource("OracleDataSource");

            Console.WriteLine(dsGt5.ConnectionString);
            var dicList = new List<IDictionary<string, object>>();
            using (var wgy = dsGt5.DbProvider.GetDbProviderFactory().CreateConnection())
            {
                wgy.ConnectionString = dsGt5.ConnectionString.ToString();
                wgy.Open();
                var cmd = wgy.CreateCommand();
                cmd.CommandText = "select * from t_d_user";
                cmd.CommandType = CommandType.Text;

                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (read.Read())
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("ID", read["ID"]);
                        dicList.Add(dic);
                    }
                }
            }
        }

        [TestMethod]
        public void MySqlTest()
        {
            var dsGt5 = DataSourceFactory.Instance.GetDataSource("MySqlDataSource");

            Console.WriteLine(dsGt5.ConnectionString);
            var dicList = new List<IDictionary<string, object>>();
            using (var wgy = dsGt5.DbProvider.GetDbProviderFactory().CreateConnection())
            {
                wgy.ConnectionString = dsGt5.ConnectionString.ToString();
                wgy.Open();
                var cmd = wgy.CreateCommand();
                cmd.CommandText = "select * from t_d_user";
                cmd.CommandType = CommandType.Text;

                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (read.Read())
                    {
                        var dic = new Dictionary<string, object>();
                        dic.Add("ID", read["ID"]);
                        dicList.Add(dic);
                    }
                }
            }
        }
    }
}
