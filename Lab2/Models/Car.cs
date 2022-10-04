using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Lab2.Models
{

    public class Car
    {
        public ObjectId Id { get; set; }

        public int CarId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }
    }
}
