using CheckersOnlineSPA.CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace CheckersOnlineSPA.CheckersEngine.BotCore
{
    public abstract class AbstractScoreStorage
    {
        abstract public FieldScoreResult? GetResult(BigInteger index, bool IsWhiteStep);
        abstract public void StoreResult(BigInteger fieldIndentify, FieldScoreResult result, bool IsWhiteStep, int complexity);
    }

    public class ScoreStorage : AbstractScoreStorage
    {
        public Dictionary<BigInteger, FieldScoreResult> WhiteBestActions { get; set; }
        public Dictionary<BigInteger, FieldScoreResult> BlackBestActions { get; set; }


        public ScoreStorage()
        {
            WhiteBestActions = new Dictionary<BigInteger, FieldScoreResult>();
            BlackBestActions = new Dictionary<BigInteger, FieldScoreResult>();
        }

        public void LoadFromDatabase()
        {
            var table = "best_actions";
            string connectionString = "server=192.168.0.101;database=checkers_actions;uid=dimon;pwd=_;";
            using (var connection = new MySqlConnection(connectionString))
            {
                var queryString = $"SELECT * FROM {table}";
                connection.Open();
                var myCommand = connection.CreateCommand();
                myCommand.CommandText = queryString;
                MySqlDataReader rdr = myCommand.ExecuteReader();
                while( rdr.Read())
                {
                    var fieldState = BigInteger.Parse(rdr[0].ToString());
                    var resultObj = FieldScoreResult.DeserializeFromJson(rdr[1].ToString());
                    bool isPlayerWhite = rdr[2].ToString().Equals("True");
                    if (isPlayerWhite)
                        WhiteBestActions[fieldState] = resultObj;
                    else
                        BlackBestActions[fieldState] = resultObj;
                }
                rdr.Close();
            }
        }

        public override FieldScoreResult? GetResult(BigInteger fieldIndentify, bool IsWhiteStep )
        {
            var dict = IsWhiteStep ? WhiteBestActions : BlackBestActions;
            if (dict.ContainsKey(fieldIndentify) == false)
                return null;
            return dict[fieldIndentify];
        }

        public override void StoreResult(BigInteger fieldIndentify, FieldScoreResult result, bool IsWhiteStep, int complexity)
        {
            var dict = IsWhiteStep ? WhiteBestActions : BlackBestActions;
            if (dict.ContainsKey(fieldIndentify))
                return;

            dict[fieldIndentify] = result;
            var table = "best_actions";
            string connectionString = "server=192.168.0.101;database=checkers_actions;uid=dimon;pwd=_;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var myCommand = connection.CreateCommand();
                var queryString = $"INSERT IGNORE INTO {table} (field_state, best_action, is_playerWhite, complexity)\n" +
                       $"VALUES ({fieldIndentify.ToString()}, @jsObject, {Convert.ToInt32(IsWhiteStep)}, {complexity})";
                myCommand.CommandText = queryString;
                myCommand.Parameters.AddWithValue("@jsObject", result.SerializeToJson());
                myCommand.ExecuteNonQuery();
            }
        }
    }
}
