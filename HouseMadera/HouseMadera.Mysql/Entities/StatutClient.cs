﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseMadera.Mysql.Entities
{
    public class StatutClient
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Nom { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
