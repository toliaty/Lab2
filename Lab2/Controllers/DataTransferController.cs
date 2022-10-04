using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using Npgsql;
using System.Data;
using Lab2.Models;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTransferController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DataTransferController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            string query = @"
                select brand as ""Brand"",
                       model as ""Model""                
                from car
                where carid = @id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("CarAppCon");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("MongoDbCon"));
            int LastCarId = dbClient.GetDatabase("TestDB").GetCollection<Car>("Car").AsQueryable().Count();
            table.Columns.Add("CarId");
            table.Rows[0]["CarId"] = LastCarId + 1;

            Car release = new Car();
            release.CarId = Convert.ToInt32(table.Rows[0]["CarId"]);
            release.Brand = Convert.ToString(table.Rows[0]["Brand"]);
            release.Model = Convert.ToString(table.Rows[0]["Model"]);

            dbClient.GetDatabase("TestDB").GetCollection<Car>("Car").InsertOne(release);

            return new JsonResult("Элемент таблицы Car с id = " + id + " успешно перемещен из PostgreSQL в MongoDB");
        }
    }
}

