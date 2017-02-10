﻿
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HouseMadera.DAL;
using HouseMadera.Modeles;
using HouseMadera.Utilites;
using HouseMadera.Vues;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HouseMadera.VueModele
{
    public class VueModeleClientList : ViewModelBase
    {
       

        #region PROPRIETES
        private string recherche;
        public string Recherche
        {
            get
            {
                return recherche;
            }
            set
            {
                if (Filtre != null && Clients != null)
                {

                    List<Client> clientsTrouves = new List<Client>();
                    //Si le textbox est vide on affiche tous les clients
                    if (string.IsNullOrEmpty(value))
                        clientsTrouves = AfficherClient();
                    else
                        clientsTrouves = AfficherClientFiltre(Filtre, value);
                    Clients = new ObservableCollection<Client>(clientsTrouves);
                    RaisePropertyChanged(() => Clients);
                    recherche = value;
                    RaisePropertyChanged(() => Recherche);
                }

            }
        }
        private string filtre;
        public string Filtre
        {
            get { return filtre; }
            set { filtre = value; }
        }
        public ICommand EditClient { get; private set; }
        public List<string> filtres { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
        public RegexUtilities reg { get; set; }
        #endregion

        public VueModeleClientList()
        {
            EditClient = new RelayCommand(EClient);
            reg = new RegexUtilities();
            Clients = new ObservableCollection<Client>(AfficherClient());
            filtres = new List<string>() { "Nom", "Adresse" };

        }

        #region METHODES
        /// <summary>
        /// Ferme la fenetre courante et affiche la nouvelle fenêtre
        /// </summary>
        private void EClient()
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
            VueClientEdit vce = new VueClientEdit();
            vce.Show();
            window.Close();
        }

        /// <summary>
        /// Recupère en base tous les clients
        /// </summary>
        /// <returns>La liste de tous les clients</returns>
        public List<Client> AfficherClient()
        {
            List<Client> clients = new List<Client>();
            using (var dal = new ClientDAL("SQLITE"))
            {
                clients = dal.GetAllClients();
            }
            return clients;
        }

        /// <summary>
        /// Récupère la liste des clients filtrés 
        /// </summary>
        /// <param name="filtre">nom de la colonne de la grille à filtrer</param>
        /// <param name="valeur">valeur saisie dans la textbox</param>
        /// <returns>Une liste de clients</returns>
        public List<Client> AfficherClientFiltre(string filtre, string valeur)
        {
            List<Client> clients = new List<Client>();
            //TODO SQLITE à remplacer par Bdd
            using (var dal = new ClientDAL("SQLITE"))
            {
                clients = dal.GetFilteredClient(filtre, valeur);
            }

            return clients;
        }
        #endregion

    }
}
