﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using HouseMadera.Modeles;
using HouseMadera.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace HouseMadera.VueModele
{
    public class VueModeleGenererDevis : ViewModelBase
    {

        [PreferredConstructor]
        public VueModeleGenererDevis()
        {
            WindowLoaded = new RelayCommand(WindowLoadedEvent);
            Retour = new RelayCommand(RetourDetailsProjets);
            OuvrirDevis = new RelayCommand(OuvertureDevis);
            EnvoiDevis = new RelayCommand(EnvoyerDevis);
        }

        public ICommand WindowLoaded { get; private set; }
        public ICommand Retour { get; private set; }
        public ICommand EnvoiDevis { get; private set; }
        public ICommand OuvrirDevis { get; private set; }

        private MetroWindow vuePrecedente;
        public MetroWindow VuePrecedente
        {
            get { return vuePrecedente; }
            set
            {
                vuePrecedente = value;

            }
        }

        private string titreVue;
        public string TitreVue
        {
            get { return titreVue; }
            set
            {
                titreVue = value;
                RaisePropertyChanged(() => TitreVue);
            }
        }

        private List<String> listeModules;
        public List<String> ListeModules
        {
            get { return listeModules; }
            set
            {
                listeModules = value;
                RaisePropertyChanged("ListeModules");
            }
        }

        private string prixHT;
        public string PrixHT
        {
            get { return prixHT; }
            set
            {
                prixHT = value;
                RaisePropertyChanged(() => PrixHT);
            }
        }

        private string prixTTC;
        public string PrixTTC
        {
            get { return prixTTC; }
            set
            {
                prixTTC = value;
                RaisePropertyChanged(() => PrixTTC);
            }
        }

        private DevisGenere dGen;
        public DevisGenere DGen
        {
            get { return dGen; }
            set { dGen = value; }
        }

        private string devisActuel;

        public string DevisActuel
        {
            get { return devisActuel; }
            set { devisActuel = value; }
        }

        private async void OuvertureDevis()
        {
            try
            {
                Process.Start(AppInfo.AppPath + @"\Devis\" + DevisActuel);
            }
            catch (Exception ex)
            {
                Logger.WriteEx(ex);
                var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                if (window != null)
                {
                    await window.ShowMessageAsync("Erreur", "Le devis n'a pas pu être ouvert.");
                }
            }
        }

        private async void EnvoyerDevis()
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
            try
            {
                // envoi devis au client.
                SmtpClient client = new SmtpClient()
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential("serviceclient.madera@gmail.com", "Rila2016")
                };

                MailMessage mm = new MailMessage("serviceclient.madera@gmail.com", DGen.client.Email)
                {
                    Subject = @"Votre Devis pour votre maison modulaire - Madera",
                    Body = @"Cher client, " + Environment.NewLine +
                        "vous trouverez ci-joint le devis pour votre maison modulaire réalisée le " + DateTime.Now.ToLongDateString() + Environment.NewLine + @"." +
                        "N'hésitez pas à nous contacter pour toute information complémentaire dont vous auriez besoin." + Environment.NewLine +
                        "Cordialement," + Environment.NewLine +
                        "La société Madera."
                };
                Attachment attachment = new Attachment(@"Devis\" + DevisActuel, new System.Net.Mime.ContentType("application/pdf"));
                mm.Attachments.Add(attachment);
                mm.BodyEncoding = Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mm);
                mm.Dispose();
                if (window != null)
                {
                    await window.ShowMessageAsync("Information", String.Format("Le devis a bien été envoyé au client ({0})", DGen.client.Email));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteEx(ex);
                if (window != null)
                {
                    await window.ShowMessageAsync("Erreur", "Le devis n'a pas pu être envoyé");
                }
            }
        }

        private void WindowLoadedEvent()
        {
            Console.WriteLine("window loaded event");
            // Actions à effectuer au lancement du form :
            ActualiserTitreForm();
            ListeModules = new List<String>();
            ListeModules = DGen.Modules;
            PrixHT = DGen.PrixHT + @" €";
            PrixTTC = DGen.PrixTTC + @" €";
            GenererPdfDevis();
        }

        private async void RetourDetailsProjets()
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().Last();
            if (window != null)
            {
                var result = await window.ShowMessageAsync("Avertissement", "Voulez-vous vraiment fermer ce devis ?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                {
                    AffirmativeButtonText = "Oui",
                    NegativeButtonText = "Non",
                    AnimateHide = false,
                    AnimateShow = true
                });

                if (result == MessageDialogResult.Affirmative)
                {
                    window.Close();
                }
            }
        }

        private void GenererPdfDevis()
        {
            DevisActuel = @"devis" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
            Console.WriteLine(AppInfo.AppPath + @"\Devis\" + DevisActuel);
            if (!Directory.Exists(AppInfo.AppPath + @"\Devis"))
            {
                Directory.CreateDirectory(AppInfo.AppPath + @"\Devis");
            }
            FileStream fs = new FileStream(AppInfo.AppPath + @"\Devis\" + DevisActuel, FileMode.Create);
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);

            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.AddAuthor("Madera");
            document.AddCreator("Société Madera");
            document.AddKeywords("Devis généré par l'application Mader'house");
            document.AddTitle("Devis généré le " + DateTime.Now.ToLongDateString());
            document.Open();
            document.Add(new Paragraph(DGen.Output));
            document.Close();
            writer.Close();
            fs.Close();
        }

        private void ActualiserTitreForm()
        {
            TitreVue = string.Format(@"Devis client - {0} {1}", DGen.client.Prenom, DGen.client.Nom);
            RaisePropertyChanged(() => TitreVue);
        }

    }
}
