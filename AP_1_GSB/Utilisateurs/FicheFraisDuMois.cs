﻿using AP_1_GSB.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Globalization;
using iText.IO.Image;
using iText.Layout.Properties;
using AP_1_GSB.Services;

using AP_1_GSB.Comptable;
namespace AP_1_GSB.Visiteur
{
    public partial class FicheFraisDuMois : Form
    {
        readonly Utilisateur utilisateur;
        readonly FicheFrais ficheEnCours;
        readonly string version;
        public event Action DataGridsVides;
        public event Action GriserBouton;
        public event Action RetourChoixVisiteur;
        public DataGridView dataGridViewForfait => this.DataGridFraisForfait;
        public DataGridView dataGridViewHorsForfait => this.DataGridHorsForfait;

        public FicheFraisDuMois(Utilisateur utilisateur, FicheFrais ficheEnCours, DateTime dtFin, string version)
        {
            this.utilisateur = utilisateur;
            this.ficheEnCours = ficheEnCours;
            this.version = version;
            InitializeComponent();
            MettreAJourDataGrids();
            AfficherInformationForm();
            MiseEnFormeBoutons();
            DateFicheFrais.Text = "Fiche de frais du " + ficheEnCours.Date.ToString("dd MMMM yyyy") + " au " + dtFin.ToString("dd MMMM yyyy");
        }

        //Met en forme le design des boutons 
        private void MiseEnFormeBoutons()
        {
            Design design = new Design();
            BtnPDF.MouseEnter += design.Btn_EntrerCurseur;
            BtnPDF.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(BtnPDF);
            btnRetour.MouseEnter += design.Btn_EntrerCurseur;
            btnRetour.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(btnRetour);
            BtnAccepterFiche.MouseEnter += design.Btn_EntrerCurseur;
            BtnAccepterFiche.MouseLeave += design.Btn_SortirCurseur;
            BtnRefuserFicheComptable.MouseEnter += design.Btn_EntrerCurseur;
            BtnRefuserFicheComptable.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(BtnAccepterFiche);
            design.MiseEnFormeBoutons(BtnRefuserFicheComptable);
        }


        #region General

        //Génère la création d'un PDF  
        private void BtnPDF_Click(object sender, EventArgs e)
        {
            FicheFraisService.CreerPDF(utilisateur, ficheEnCours, DataGridFraisForfait, DataGridHorsForfait);
        }
        //Clear la sélection d'un grid au clique sur l'autre grid
        //Evite d'avoir une ligne sélectionnée dans chacun des DataGridView
        private void DataGridFraisForfait_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridHorsForfait.ClearSelection();
        }
        private void DataGridHorsForfait_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridFraisForfait.ClearSelection();
        }

        //Met en forme le formulaire selon le profil
        private void AfficherInformationForm()
        {
            if (version == "utilisateur")
            {
                BtnAccepterFiche.Visible = false;
                BtnRefuserFicheComptable.Visible = false;
                LblEmployeInfo.Visible = false;
                btnRetour.Visible = false;
                LblEtatFiche.Location = new Point(45, 643);   
                VerifierEtatFiche();
            }
            else
            {
                LblEmployeInfo.Text = "Employé : " + utilisateur.Nom + " " + utilisateur.Prenom;
                lblEtat.Visible = false;
                VerifierEtatFiche();
            }
        }

        //Méthode de mise à jour des data grids et des champs "total" lors de modifications effectuées 
        public void MettreAJourDataGrids()
        {
            DataGridFraisForfait.Rows.Clear();
            DataGridFraisForfait.Columns["IdFraisforfait"].Visible = false;

            foreach (FraisForfait fraisForfait in ficheEnCours.FraisForfaits)
            {
                float montantForfait = fraisForfait.Quantite * fraisForfait.TypeForfait.Montant;
                string etatJustificatif = "Pas de justificatif";
                if (fraisForfait.justificatif != null)
                {
                    etatJustificatif = "Justificatif présent";
                }
                string etat = Services.FraisForfaitService.EcrireEtatFraisForfait(fraisForfait);
                string[] row = new string[]
                {
                    fraisForfait.TypeForfait.Nom,
                    fraisForfait.Quantite.ToString(),
                    montantForfait.ToString() + " €",
                    fraisForfait.Date.ToString("dd/MM/yyyy"),
                    etat,
                    etatJustificatif,
                    fraisForfait.IdFraisForfait.ToString()
                };

                DataGridFraisForfait.Rows.Add(row);
            }
            DataGridFraisForfait.ClearSelection();
            float totalForfait = FraisForfaitService.CalculerTotalForfait(ficheEnCours);
            LblTotalForfait.Text = "Total forfait : " + totalForfait.ToString("F2") + " €";

            DataGridHorsForfait.Rows.Clear();
            DataGridHorsForfait.Columns["IdFraisHorsForfait"].Visible = false;

            foreach (FraisHorsForfait fraisHorsForfait in ficheEnCours.FraisHorsForfaits)
            {
                string etatJustificatif = "Pas de justificatif";
                if (fraisHorsForfait.Justificatif != null)
                {
                    etatJustificatif = "Justificatif présent";
                }
                string etat = Services.FraisHorsForfaitService.EcrireEtatFraiHorsForfait(fraisHorsForfait);
                string[] row = new string[]
                {
                    fraisHorsForfait.Description,
                    fraisHorsForfait.Montant.ToString(),
                    fraisHorsForfait.Date.ToString("dd/MM/yyyy"),
                    etat,
                    etatJustificatif,
                    fraisHorsForfait.IdFraisHorsForfait.ToString()

                };

                DataGridHorsForfait.Rows.Add(row);
            }
            DataGridHorsForfait.ClearSelection();
            float totalHorsForfait = FraisHorsForfaitService.CalculerTotalHorsForfait(ficheEnCours);
            LblTotalHorsForfait.Text = "Total hors forfait : " + totalHorsForfait.ToString("F2") + " €";
            float totalFiche = FicheFraisService.CalculerTotalFiche(ficheEnCours);
            LblTotalFiche.Text = "Total fiche : " +totalFiche.ToString("F2") + " €";

        }
        #endregion


        #region utilisateur 

        //Méthode qui permet de griser les boutons si DataGrid sont vides 
        public void VerifierDataGridsVides()
        {
            if (DataGridFraisForfait.Rows.Count == 0 && DataGridHorsForfait.Rows.Count == 0)
            {
                DataGridsVides?.Invoke();
            }
        }

        public void SupprimerSelectionLigne()
        {
            if (DataGridFraisForfait.SelectedRows.Count > 0)
            {
                SupprimerFraisForfait();
            }
            else if (DataGridHorsForfait.SelectedRows.Count > 0)
            {
                SupprimerFraisHorsForfait();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un frais à supprimer.", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void SupprimerFraisForfait()
        {

            int idFraisASupprimer = int.Parse(DataGridFraisForfait.SelectedRows[0].Cells[6].Value.ToString());
            DataGridFraisForfait.Rows.RemoveAt(DataGridFraisForfait.SelectedRows[0].Index);

            FraisForfait FraisASupprimer = ficheEnCours.FraisForfaits.Find(f => f.IdFraisForfait == idFraisASupprimer);


            if (FraisASupprimer == null)
            {
                MessageBox.Show("Erreur : frais forfait non trouvé", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Services.FraisForfaitService.SupprimerFraisForfait(FraisASupprimer))
            {
                MessageBox.Show("Note de frais forfait supprimée", "Supression", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ficheEnCours.FraisForfaits.Remove(FraisASupprimer);
                float total = FraisForfaitService.CalculerTotalForfait(ficheEnCours);
                LblTotalForfait.Text = "Total forfait : " + total.ToString("F2") + " €";
                float totalFiche = FicheFraisService.CalculerTotalFiche(ficheEnCours);
                LblTotalFiche.Text = "Total fiche : " + totalFiche.ToString("F2") + " €";
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression de la note de frais", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MettreAJourDataGrids();
            VerifierDataGridsVides();
        }

        public void SupprimerFraisHorsForfait()
        {
            int idFraisASupprimer = 0;
            FraisHorsForfait FraisASupprimer = null;
            idFraisASupprimer = int.Parse(DataGridHorsForfait.SelectedRows[0].Cells[5].Value.ToString());
            DataGridHorsForfait.Rows.RemoveAt(DataGridHorsForfait.SelectedRows[0].Index);
            FraisASupprimer = ficheEnCours.FraisHorsForfaits.Find(f => f.IdFraisHorsForfait == idFraisASupprimer);


            if (Services.FraisHorsForfaitService.SupprimerFraisHorsForfait(FraisASupprimer))
            {
                ficheEnCours.FraisHorsForfaits.Remove(FraisASupprimer);
                float totalHorsForfait = FraisHorsForfaitService.CalculerTotalHorsForfait(ficheEnCours);
                LblTotalHorsForfait.Text = "Total hors forfait :" +totalHorsForfait.ToString("F2") + " €";
                float totalFiche = FicheFraisService.CalculerTotalFiche(ficheEnCours);
                LblTotalFiche.Text ="Total fiche : " + totalFiche.ToString("F2") + " €";
                MessageBox.Show("Note de frais hors forfait supprimée", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression de la note de frais", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MettreAJourDataGrids();
            VerifierDataGridsVides();
        }
        #endregion


        #region Comptable

        // En base, pour id_etat : 1 = attente / 2 = en_cours / 3 = accepter / 4 = refuser / 5 = refus_partiel

        //Gère l'affichage de l'état de la fiche
        public void VerifierEtatFiche()
        {
            switch (ficheEnCours.Etat)
            {
                case EtatFicheFrais.Accepter:
                    LblEtatFiche.Text = "Etat fiche : Acceptée";
                    LblEtatFiche.ForeColor = Color.Green;

                    break;
                case EtatFicheFrais.Refuser:
                    LblEtatFiche.Text = "Etat fiche : Refusée";
                    LblEtatFiche.ForeColor = Color.Red;
                    break;
                case EtatFicheFrais.EnCours:
                    LblEtatFiche.Text = "Etat fiche : En cours";
                    LblEtatFiche.ForeColor = Color.Black;
                    break;
                case EtatFicheFrais.RefusPartiel:
                    LblEtatFiche.Text = "Etat fiche : Refusée partiellement";
                    LblEtatFiche.ForeColor = Color.Orange;
                    break;
                case EtatFicheFrais.Attente:
                    LblEtatFiche.Text = "Etat fiche : En attente";
                    LblEtatFiche.ForeColor = Color.Gray;
                    break;
            }
        }

        //Méthode pour refuser un frais et mettre à jour en base, en objet et le datagrid
        public void RefuserFrais()
        {
            string etat = "REFUSER";
            if (DataGridFraisForfait.SelectedRows.Count > 0)
            {
                int idFrais = int.Parse(DataGridFraisForfait.SelectedRows[0].Cells[6].Value.ToString());
                FraisForfait fraisForfait = null;
                foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
                {
                    if (frais.IdFraisForfait == idFrais)
                    {
                        fraisForfait = frais;
                        break;
                    }
                }

                if (fraisForfait.Etat == EtatFraisForfait.Refuser)
                {
                    MessageBox.Show("Le frais est déjà refusé", "Frais refusé", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    fraisForfait.Etat = EtatFraisForfait.Refuser;
                }

                FraisForfaitService.ChangerEtatFraisForfat(idFrais, etat);

                FicheFraisService.ChangerEtatFiche(ficheEnCours, 5);
                ficheEnCours.Etat = EtatFicheFrais.RefusPartiel;
                MettreAJourDataGrids();
                VerifierEtatRefuser();
                VerifierEtatFiche();
            }
            else if (DataGridHorsForfait.SelectedRows.Count > 0)
            {
                int idFrais = int.Parse(DataGridHorsForfait.SelectedRows[0].Cells[5].Value.ToString());

                FraisHorsForfaitService.ChangerEtatHorsForfait(idFrais, etat);
                foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
                {
                    if (frais.IdFraisHorsForfait == idFrais)
                        frais.Etat = EtatFraisHorsForfait.Refuser;
                }

                FicheFraisService.ChangerEtatFiche(ficheEnCours, 5);
                ficheEnCours.Etat = EtatFicheFrais.RefusPartiel;
                MettreAJourDataGrids();
                VerifierEtatRefuser();
                VerifierEtatFiche(); 
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un frais à refuser", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Méthode pour accepter un frais et mettre à jour en base, en objet et le datagrid
        public void AccepterFrais()
        {
            string etat = "ACCEPTER";
            if (DataGridFraisForfait.SelectedRows.Count > 0)
            {
                int idFrais = int.Parse(DataGridFraisForfait.SelectedRows[0].Cells[6].Value.ToString());
                FraisForfaitService.ChangerEtatFraisForfat(idFrais, etat);

                FraisForfait fraisForfait = null;
                foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
                {
                    if (frais.IdFraisForfait == idFrais)
                        fraisForfait = frais;
                }

                if (fraisForfait.Etat == EtatFraisForfait.Accepter)
                {
                    MessageBox.Show("Le frais est déjà accepté", "Frais accepté", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    fraisForfait.Etat = EtatFraisForfait.Accepter;
                }

                MettreAJourDataGrids();
                VerifierEtatAccepter();
                VerifierEtatFiche();


            }
            else if (DataGridHorsForfait.SelectedRows.Count > 0)
            {
                int idFrais = int.Parse(DataGridHorsForfait.SelectedRows[0].Cells[5].Value.ToString());
                FraisHorsForfaitService.ChangerEtatHorsForfait(idFrais, etat);

                FraisHorsForfait fraisHorsForfait = null;
                foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
                {
                    if (frais.IdFraisHorsForfait == idFrais)
                        fraisHorsForfait = frais;
                }

                if (fraisHorsForfait.Etat == EtatFraisHorsForfait.Accepter)
                {
                    MessageBox.Show("Le frais est déjà accepté", "Frais accepté", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    fraisHorsForfait.Etat = EtatFraisHorsForfait.Accepter;
                }
                MettreAJourDataGrids();
                VerifierEtatAccepter();
                VerifierEtatFiche();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un frais à accepter", "Aucune sélection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Vérifie si l'état de tous les frais forfait et hors forfait est accepté et change l'état de la fiche en fonction
        private void VerifierEtatAccepter()
        {
            List<FraisForfait> listeForfait = new List<FraisForfait>();
            foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
            {
                if (frais.Etat == EtatFraisForfait.Accepter)
                    listeForfait.Add(frais);
            }
            List<FraisHorsForfait> listeHorsForfait = new List<FraisHorsForfait>();
            foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
            {
                if (frais.Etat == EtatFraisHorsForfait.Accepter)
                    listeHorsForfait.Add(frais);
            }
            if (listeForfait.Count == dataGridViewForfait.RowCount && listeHorsForfait.Count == dataGridViewHorsForfait.RowCount)
            {
                ficheEnCours.Etat = EtatFicheFrais.Accepter;
                FicheFraisService.ChangerEtatFiche(ficheEnCours, 3);

            }
        }
        //Vérifie si l'état de tous les frais forfait et hors forfait est refusé et change l'état de la fiche en fonction
        private void VerifierEtatRefuser()
        {
            List<FraisForfait> listeForfait = new List<FraisForfait>();
            foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
            {
                if (frais.Etat == EtatFraisForfait.Refuser)
                    listeForfait.Add(frais);
            }
            List<FraisHorsForfait> listeHorsForfait = new List<FraisHorsForfait>();
            foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
            {
                if (frais.Etat == EtatFraisHorsForfait.Refuser)
                    listeHorsForfait.Add(frais);
            }
            if (listeForfait.Count == dataGridViewForfait.RowCount && listeHorsForfait.Count == dataGridViewHorsForfait.RowCount)
            {
                ficheEnCours.Etat = EtatFicheFrais.Refuser;
                FicheFraisService.ChangerEtatFiche(ficheEnCours, 4);
            }
        }
        //En base, pour id_etat : 1 = attente / 2 = en_cours / 3 = accepter / 4 = refuser / 5 = refus_partiel


        private void BtnRetour_Click(object sender, EventArgs e)
        {
            RetourChoixVisiteur?.Invoke();
            GriserBouton?.Invoke();
            this.SendToBack();
        }

        //Méthode pour accepter l'état général de la fiche et mettre à jour l'état de tous les frais
        private void BtnAccepterFiche_Click(object sender, EventArgs e)
        {
                foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
            {
                frais.Etat = EtatFraisForfait.Accepter;
                FraisForfaitService.ChangerEtatFraisForfat(frais.IdFraisForfait, "ACCEPTER");
            }

            foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
            {
                frais.Etat = EtatFraisHorsForfait.Accepter;
                FraisHorsForfaitService.ChangerEtatHorsForfait(frais.IdFraisHorsForfait, "ACCEPTER");
            }
            MettreAJourDataGrids();
            FicheFraisService.ChangerEtatFiche(ficheEnCours, 3);
            ficheEnCours.Etat = EtatFicheFrais.Accepter;
            VerifierEtatFiche();
            MessageBox.Show("La fiche est acceptée");
        }
        //Méthode pour refuser l'état général de la fiche et mettre à jour l'état de tous les frais
        private void BtnRefuserFiche_Click(object sender, EventArgs e)
        {
            foreach (FraisForfait frais in ficheEnCours.FraisForfaits)
            {
                frais.Etat = EtatFraisForfait.Refuser;
                FraisForfaitService.ChangerEtatFraisForfat(frais.IdFraisForfait, "REFUSER");
            }

            foreach (FraisHorsForfait frais in ficheEnCours.FraisHorsForfaits)
            {
                frais.Etat = EtatFraisHorsForfait.Refuser;
                FraisHorsForfaitService.ChangerEtatHorsForfait(frais.IdFraisHorsForfait, "REFUSER");
            }

            MettreAJourDataGrids();
            FicheFraisService.ChangerEtatFiche(ficheEnCours, 4);
            ficheEnCours.Etat = EtatFicheFrais.Refuser;
            VerifierEtatFiche();
            MessageBox.Show("La fiche est refusée");
        }
        #endregion

    }

}