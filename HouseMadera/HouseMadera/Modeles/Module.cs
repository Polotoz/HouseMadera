﻿using HouseMadera.DAL;
using System;

namespace HouseMadera.Modeles
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Module:ISynchronizable
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        /// <summary>
        /// Id du Module
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du Module
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Hauteur du Module
        /// </summary>
        public decimal Hauteur { get; set; }

        /// <summary>
        /// Largeur du Module
        /// </summary>
        public decimal Largeur { get; set; }

        /// <summary>
        /// Gamme associée
        /// </summary>
        public Gamme Gamme { get; set; }

        /// <summary>
        /// Type du Module associé
        /// </summary>
        public TypeModule TypeModule { get; set; }
        public DateTime? MiseAJour { get; set; }
        public DateTime? Suppression { get; set; }
        public DateTime? Creation { get; set; }


        #region OVERRIDE
        public override string ToString()
        {
            return string.Format("Nom {0} , Gamme {1} , Type {2}", Nom,Gamme.Nom,TypeModule.Nom);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Module m = (Module)obj;

            return (Nom == m.Nom) &&
                (Creation == m.Creation);
        }
        #endregion

        public bool IsUpToDate<TMODELE>(TMODELE modele) where TMODELE : ISynchronizable
        {
            if (modele.MiseAJour == null)
                return true;
            else
                return MiseAJour == modele.MiseAJour;
        }

        public bool IsDeleted<TMODELE>(TMODELE modele) where TMODELE : ISynchronizable
        {
            if (modele.Suppression != null && !Suppression.HasValue)
            {
                Suppression = modele.Suppression;
                return true;
            }

            return false;
        }

        public void Copy<TMODELE>(TMODELE modele) where TMODELE : ISynchronizable
        {
            Module module = modele as Module;
            Nom = module.Nom;
            Gamme = module.Gamme;
            TypeModule = module.TypeModule;
            MiseAJour = module.MiseAJour;
            Creation = module.Creation;
            Suppression = module.Suppression;
        }
        
    }
}