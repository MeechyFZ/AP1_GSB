﻿using AP_1_GSB.Data.Models;
using AP_1_GSB.Visiteur;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AP_1_GSB
{
    public partial class Dashboard : Form
    {
        public Dashboard(Data.Models.Utilisateur utilisateur)
        {
            InitializeComponent();
            NomPrenom.Text = "Bienvenue " + utilisateur.Nom + " " + utilisateur.Prenom;
            DateFicheFrais.Text = "Fiche de frais du 10 " + DateTime.Now.ToString("MMMM yyyy") + " au 9 " + DateTime.Now.AddMonths(1).ToString("MMMM yyyy");
        }
        

        private void BtnAjouterNoteFrais(object sender, EventArgs e)
        {
            AjouterNoteFrais ajouterNoteFrais = new AjouterNoteFrais();
            ajouterNoteFrais.TopLevel = false;
            myPanel.Controls.Add(ajouterNoteFrais);
            ajouterNoteFrais.FormBorderStyle = FormBorderStyle.None;
            ajouterNoteFrais.Dock = DockStyle.Fill;
            ajouterNoteFrais.Show();

        }

        private void BtnQuitter_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}