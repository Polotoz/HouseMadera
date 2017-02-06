﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseMadera.Vue_Modele
{
    public class CommuneViewModel
    {
        public string NomCommune { get; set; }
        public string CodePostal { get; set; }

        public CommuneViewModel(string value)
        {
            NomCommune = ExtractNomCommune(value);
            CodePostal = ExtractCodePostal(value);
        }

       

        private string ExtractNomCommune(string communeSelected)
        {
            var offset = 7;
            return communeSelected.Substring(offset, communeSelected.Length - offset);
        }

        private string ExtractCodePostal(string communeSelected)
        {
            var offset = 0;
            return communeSelected.Substring(offset, 5);
        }
    }
}
