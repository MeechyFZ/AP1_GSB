﻿using AP_1_GSB.Administrateur;
using AP_1_GSB.Data.Models;
using AP_1_GSB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AP_1_GSB.Visiteur
{
    public partial class CreerModifierNoteFrais : Form
    {
        public event Action NoteDeFraisAjoutee;
        FraisForfait FraisForfaitAModifier;
        FraisHorsForfait FraisHorsForfaitAModifier;
        Utilisateur utilisateur;
        FicheFrais ficheEnCours;
        string version;
        byte[] fichierBinaire = null;
        int idType;

        //Trois constructeurs en fonction du besoin : créer ou modifier(forfait ou hors forfait 
        public CreerModifierNoteFrais(Utilisateur utilisateur, FicheFrais ficheEnCours, DateTime dtFin, string version)
        {
            this.utilisateur = utilisateur;
            this.ficheEnCours = ficheEnCours;
            this.version = version;
            InitializeComponent();
            ChargerCombobox();
            InitialiserForm(dtFin);
            MiseEnFormeBtn();
            fraisForfaitBindingSource.DataSource = new FraisForfait();
            fraisHorsForfaitBindingSource.DataSource = new FraisHorsForfait();
        }
        public CreerModifierNoteFrais(Utilisateur utilisateur, FicheFrais ficheEnCours, DateTime dtFin, string version, FraisForfait FraisForfaitAModifier)
        {
            this.utilisateur = utilisateur;
            this.version = version;
            this.ficheEnCours = ficheEnCours;
            this.FraisForfaitAModifier = FraisForfaitAModifier;
            InitializeComponent();
            ChargerCombobox();
            InitialiserForm(dtFin);
            MiseEnFormeBtn();
            fraisForfaitBindingSource.DataSource = FraisForfaitAModifier;

            if (FraisForfaitAModifier.justificatif != null )
                lblMiseAJourJustificatif.Text = "Justificatif ajouté";

            ComboBoxTypeForfait.Text = FraisForfaitAModifier.TypeForfait.Nom;

            ComboBoxSelectionForfaitHorsForfait.SelectedIndex = 0;
            ComboBoxSelectionForfaitHorsForfait.Enabled = false;
            ComboBoxTypeForfait.Visible = true;
            DtTimePickerForfait.Visible = true;
            quantiteNumericUpDown.Visible = true;
            LblQuantite.Visible = true;
            LblTypeForfait.Visible = true;
            BtnValider.Visible = true;
            lblMiseAJourJustificatif.Visible = true;
            BtnJustificatif.Visible = true;
            lblJustificatif.Visible = true;
            

        }
        public CreerModifierNoteFrais(Utilisateur utilisateur, FicheFrais ficheEnCours, DateTime dtFin, string version, FraisHorsForfait FraisHorsForfaitAModifier)
        {
            this.utilisateur = utilisateur;
            this.version = version;
            this.ficheEnCours = ficheEnCours;
            this.FraisHorsForfaitAModifier = FraisHorsForfaitAModifier;
            InitializeComponent();
            ChargerCombobox();
            InitialiserForm(dtFin);
            MiseEnFormeBtn();
            fraisHorsForfaitBindingSource.DataSource = FraisHorsForfaitAModifier;

            if (FraisHorsForfaitAModifier.Justificatif != null)
                lblMiseAJourJustificatif.Text = "Justificatif ajouté";

            ComboBoxSelectionForfaitHorsForfait.SelectedIndex = 1;
            ComboBoxSelectionForfaitHorsForfait.Enabled = false;

            descriptionTextBox.Visible = true;
            DtTimePickerHorsForfait.Visible = true;
            montantNumericUpDown.Visible = true;
            lblMiseAJourJustificatif.Visible = true;
            LblDate.Visible = true;
            LblDescription.Visible = true;
            LblMontant.Visible = true;
            BtnJustificatif.Visible = true;
            lblMiseAJourJustificatif.Visible = true;
            BtnValider.Visible = true;
            lblJustificatif.Visible = true;
        }

        //Applique le design des boutons
        private void MiseEnFormeBtn()
        {
            Design design = new Design();
            BtnValider.MouseEnter += design.Btn_EntrerCurseur;
            BtnValider.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(BtnValider);
            BtnJustificatif.MouseEnter += design.Btn_EntrerCurseur;
            BtnJustificatif.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(BtnJustificatif);
            btnQuitter.MouseEnter += design.Btn_EntrerCurseur;
            btnQuitter.MouseLeave += design.Btn_SortirCurseur;
            design.MiseEnFormeBoutons(btnQuitter);

        }

        //Met en forme la pop up 
        public void InitialiserForm(DateTime dtFin)
        {
            DateMinMaxDateTimePicker(DtTimePickerForfait, dtFin);
            DateMinMaxDateTimePicker(DtTimePickerHorsForfait, dtFin);
            DtTimePickerForfait.Value = DateTime.Now;
            DtTimePickerHorsForfait.Value = DateTime.Now;

            descriptionTextBox.Visible = false;
            DtTimePickerForfait.Visible = false;
            montantNumericUpDown.Visible = false;
            ComboBoxTypeForfait.Visible = false;
            DtTimePickerHorsForfait.Visible = false;
            lblMiseAJourJustificatif.Visible = false;
            LblDate.Visible = false;
            LblDescription.Visible = false;
            LblMontant.Visible = false;
            LblQuantite.Visible = false;
            LblTypeForfait.Visible = false;
            BtnJustificatif.Visible = false;
            BtnValider.Visible = false;
            quantiteNumericUpDown.Visible = false;
            lblMiseAJourJustificatif.Visible = false;
            lblJustificatif.Visible = false;

            if (version == "creer")
            {
                label1.Text = "Création d'une note de frais";
            }
            else
            {
                label1.Text = "Modification d'une note de frais";
            }
        }
        //Définit la période de sélection possible dans le DateTimePicker
        private void DateMinMaxDateTimePicker(DateTimePicker dtp, DateTime dtFin)
        {
            dtp.MinDate = ficheEnCours.Date;
            dtp.MaxDate = dtFin;
        }

        //Charge les valeurs du combobox
        private void ChargerCombobox()
        {
            List<TypeFraisForfait> typeFraisForfaits = TypeFraisForfaitService.RecupererTypeFraisForfait();
            ComboBoxTypeForfait.DataSource = typeFraisForfaits;
            ComboBoxTypeForfait.DisplayMember = "Nom";
            ComboBoxTypeForfait.ValueMember = "IdFraisForfait";
        }

        private void BtnJustificatif_Click(object sender, EventArgs e)
        {
            OpenFileDialog justificatif = new OpenFileDialog();
            justificatif.Filter = "Fichiers JPEG|*.jpg;*.jpeg|PNG Image|*.png";
            justificatif.Title = "Sélectionnez un justificatif";
            justificatif.Multiselect = false;

            if (justificatif.ShowDialog() == DialogResult.OK)
            {
                string CheminFichier = justificatif.FileName;
                FileInfo FichierInfo = new FileInfo(CheminFichier);

                if (FichierInfo.Length > 16L * 1024 * 1024)
                {
                    MessageBox.Show("Le fichier est trop grand. Veuillez sélectionner un fichier de moins de 16 Go.");
                    return;
                }
                fichierBinaire = File.ReadAllBytes(CheminFichier);
            }

            if (fichierBinaire != null)
            {
                lblMiseAJourJustificatif.Text = "Justificatif ajouté";
            }
        }

        //Met à jour les différents composants en fonction de l'item sélectionné
        private void ComboBoxSelectionForfaitHorsForfait_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isFraisForfait = ComboBoxSelectionForfaitHorsForfait.SelectedIndex == 0;

            LblTypeForfait.Visible = isFraisForfait;
            LblQuantite.Visible = isFraisForfait;
            ComboBoxTypeForfait.Visible = isFraisForfait;
            quantiteNumericUpDown.Visible = isFraisForfait;
            DtTimePickerForfait.Visible = isFraisForfait;
            LblCategorie.Visible = true;
            LblDate.Visible = true;
            lblMiseAJourJustificatif.Visible = true;
            BtnJustificatif.Visible = true;
            lblMiseAJourJustificatif.Visible = true;
            DtTimePickerHorsForfait.Visible = !isFraisForfait;
            montantNumericUpDown.Visible = !isFraisForfait;
            descriptionTextBox.Visible = !isFraisForfait;
            LblDescription.Visible = !isFraisForfait;
            LblMontant.Visible = !isFraisForfait;
            BtnValider.Visible = true;

        }

        //Creer un frais 
        private void CreerForfait()
        {
            if (ComboBoxSelectionForfaitHorsForfait.SelectedIndex == 0)
            {
                FraisForfait fraisForfait = fraisForfaitBindingSource.Current as FraisForfait;
                if (fraisForfait != null)
                {
                    ValidationContext context = new ValidationContext(fraisForfait, null, null);
                    IList<ValidationResult> errors = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(fraisForfait, context, errors, true))
                    {
                        if (errors.Count > 1)
                        {
                            MessageBox.Show("Plusieurs saisies sont incorrects. Veuillez recommencer");
                            return;
                        }
                        else
                        {
                            foreach (ValidationResult validationResult in errors)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        int idFiche = ficheEnCours.IdFicheFrais;
                        int idType = (int)ComboBoxTypeForfait.SelectedValue;
                        if (FraisForfaitService.AjouterFraisForfait(idFiche, idType, fraisForfait, fichierBinaire))
                        {
                            MessageBox.Show("Frais créé avec succés.");
                            utilisateur = FicheFraisService.RecupererNotesForfait(utilisateur, ficheEnCours);
                            NoteDeFraisAjoutee?.Invoke();
                            this.Close();
                        }
                    }
                }
            }
            else
            {
                FraisHorsForfait fraisHorsForfait = fraisHorsForfaitBindingSource.Current as FraisHorsForfait;
                if (fraisHorsForfait != null)
                {
                    ValidationContext context = new ValidationContext(fraisHorsForfait, null, null);
                    IList<ValidationResult> errors = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(fraisHorsForfait, context, errors, true))
                    {
                        if (errors.Count > 1)
                        {
                            MessageBox.Show("Plusieurs saisies sont incorrects. Veuillez recommencer");
                            return;
                        }
                        else
                        {
                            foreach (ValidationResult validationResult in errors)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        int idFiche = ficheEnCours.IdFicheFrais;
                        if (FraisHorsForfaitService.AjouterFraisHorsForfait(idFiche, fraisHorsForfait, fichierBinaire))
                        {
                            MessageBox.Show("Frais créé avec succés.");
                            utilisateur = FicheFraisService.RecupererNotesHorsForfait(utilisateur, ficheEnCours);
                            NoteDeFraisAjoutee?.Invoke();
                            this.Close();
                        }
                    }
                }
            }
        }

        //Méthode pour modifier un frais forfait 
        private void ModifierForfait()
        {
            FraisForfait fraisForfaitModifier = fraisForfaitBindingSource.Current as FraisForfait;
            if (fraisForfaitModifier != null)
            {
                ValidationContext context = new ValidationContext(fraisForfaitModifier, null, null);
                IList<ValidationResult> errors = new List<ValidationResult>();
                if (!Validator.TryValidateObject(fraisForfaitModifier, context, errors, true))
                {
                    if (errors.Count > 1)
                    {
                        MessageBox.Show("Plusieurs saisies sont incorrects. Veuillez recommencer");
                        return;
                    }
                    else
                    {
                        foreach (ValidationResult validationResult in errors)
                        {
                            MessageBox.Show(validationResult.ErrorMessage, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                else
                {
                    idType = (int)ComboBoxTypeForfait.SelectedValue;

                    fraisForfaitModifier.IdFraisForfait = FraisForfaitAModifier.IdFraisForfait;
                    if (FraisForfaitService.ModifierFraisForfait(idType, fraisForfaitModifier, fichierBinaire))
                    {
                        MessageBox.Show("Frais forfait modifié avec succés.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        utilisateur = FicheFraisService.RecupererNotesForfait(utilisateur, ficheEnCours);
                        NoteDeFraisAjoutee?.Invoke();
                        this.Close();
                    }
                }

            }
        }

        //Méthode pour modifier hors forfait 
        private void ModifierHorsForfait()
        {
            FraisHorsForfait fraisHorsForfaitModifie = fraisHorsForfaitBindingSource.Current as FraisHorsForfait;
            if (fraisHorsForfaitModifie != null)
            {
                ValidationContext context = new ValidationContext(fraisHorsForfaitModifie, null, null);
                IList<ValidationResult> errors = new List<ValidationResult>();
                if (!Validator.TryValidateObject(fraisHorsForfaitModifie, context, errors, true))
                {
                    if (errors.Count > 1)
                    {
                        MessageBox.Show("Plusieurs saisies sont incorrects. Veuillez recommencer");
                        return;
                    }
                    else
                    {
                        foreach (ValidationResult validationResult in errors)
                        {
                            MessageBox.Show(validationResult.ErrorMessage, "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                else
                {

                    fraisHorsForfaitModifie.IdFraisHorsForfait = FraisHorsForfaitAModifier.IdFraisHorsForfait;
                    if (FraisHorsForfaitService.ModifierFraisHorsForfait(fraisHorsForfaitModifie, fichierBinaire))
                    {
                        MessageBox.Show("Frais hors forfait modifié avec succés.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        utilisateur = FicheFraisService.RecupererNotesHorsForfait(utilisateur, ficheEnCours);
                        NoteDeFraisAjoutee?.Invoke();
                        this.Close();
                    }
                }

            }
        }

        //Définie la méthode à appeler en fonction de la version
        private void BtnValider_Click(object sender, EventArgs e)
        {
            switch (version)
            {
                case "creer":
                    CreerForfait();
                    break;
                case "modifierForfait":
                    ModifierForfait();
                    break;
                case "modifierHorsForfait":
                    ModifierHorsForfait();
                    break;
            }
        }

        private void BtnQuitter_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

