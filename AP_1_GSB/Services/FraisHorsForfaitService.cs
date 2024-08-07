﻿using AP_1_GSB.Data.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using iText.Kernel.Pdf.Action;

namespace AP_1_GSB.Services
{
    internal class FraisHorsForfaitService
    {

        //Supression d'un frais hors forfait en base
        public static bool SupprimerFraisHorsForfait(FraisHorsForfait frais)
        {
            int IdFrais = frais.IdFraisHorsForfait;

            if (frais == null && IdFrais < 1)
            {
                return false;
            }

            Data.SqlConnection.ConnexionSql();

            string requete = "DELETE FROM frais_hors_forfait  WHERE id_hors_forfait = @IdFraisHorsForfait";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(requete, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@IdFraisHorsForfait", IdFrais);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Erreur lors de la connexion à la BD : " + ex.Message);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }

            return false;
        }
        //Insertion d'un frais hors forfait en base
        public static bool AjouterFraisHorsForfait(int IdFiche, FraisHorsForfait frais, byte[] FichierBinaire)
        {

            if (frais.Date == new DateTime(1, 1, 1))
            {
                frais.Date = DateTime.Now;
            }

            int idJustificatif = 0;
            if (FichierBinaire != null)
            {
                Justificatif justi = JustificatifService.AjouterJustificatif(FichierBinaire);
                idJustificatif = justi.IdJustificatif;
            }

            Data.SqlConnection.ConnexionSql();

            string RequeteCreationFraisHorsForfait = "INSERT INTO `frais_hors_forfait` (`description`, `montant`, `date`, `etat`, `id_fiche_frais`, `id_justificatif`) " +
                                                     "VALUES (@description, @montant, @date, @etat, @IdFiche, @idJustificatif)";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(RequeteCreationFraisHorsForfait, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@description", frais.Description);
                    cmd.Parameters.AddWithValue("@montant", frais.Montant);
                    cmd.Parameters.AddWithValue("@date", frais.Date);
                    cmd.Parameters.AddWithValue("@etat", "ATTENTE");
                    cmd.Parameters.AddWithValue("@IdFiche", IdFiche);
                    cmd.Parameters.AddWithValue("@idJustificatif", idJustificatif == 0 ? (object)DBNull.Value : idJustificatif);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }

            return false;
        }
        //Modification d'un frais hors forfait en base
        public static bool ModifierFraisHorsForfait(FraisHorsForfait frais, byte[] FichierBinaire)
        {

            if (frais.Date == new DateTime(1, 1, 1))
            {
                frais.Date = DateTime.Now;
            }
            int idJustificatif = 0;
            if (FichierBinaire != null)
            {
                Justificatif justi = JustificatifService.AjouterJustificatif(FichierBinaire);
                idJustificatif = justi.IdJustificatif;
            }

            Data.SqlConnection.ConnexionSql();

            string RequeteModificationFraisHorsForfait = "UPDATE `frais_hors_forfait` SET `description` = @description, `montant` = @montant, `date` = @date, `etat` = @etat, `id_justificatif` = @idJustificatif " +
                                                     "WHERE `id_hors_forfait` = @IdFrais";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(RequeteModificationFraisHorsForfait, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@description", frais.Description);
                    cmd.Parameters.AddWithValue("@montant", frais.Montant);
                    cmd.Parameters.AddWithValue("@date", frais.Date);
                    cmd.Parameters.AddWithValue("@etat", "ATTENTE");
                    cmd.Parameters.AddWithValue("@idJustificatif", idJustificatif == 0 ? (object)DBNull.Value : idJustificatif);
                    cmd.Parameters.AddWithValue("@IdFrais", frais.IdFraisHorsForfait);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de données : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }

            return false;
        }
        //Calcul du total hors forfait 
        public static float CalculerTotalHorsForfait(FicheFrais ficheEncours)
        {
            float total = 0;
            foreach (FraisHorsForfait fraisHorsForfait in ficheEncours.FraisHorsForfaits)
            {
                total += fraisHorsForfait.Montant;

            }
            return total;
        }
        //Changement de l'état d'un frais hors forfait
        public static void ChangerEtatHorsForfait(int idFrais, string etat)
        {
            Data.SqlConnection.ConnexionSql();
            string requete = "UPDATE frais_hors_forfait SET etat = @etat WHERE id_hors_forfait = @idFrais;";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(requete, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@idFrais", idFrais);
                    cmd.Parameters.AddWithValue("@etat", etat);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erreur lors du refus de frais : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }
        }

        //Permet l'écriture correcte de l'état d'un frais dans l'application 
        public static string EcrireEtatFraiHorsForfait(FraisHorsForfait frais)
        {
            string etat = "";
            switch (frais.Etat)
            {
                case EtatFraisHorsForfait.Attente:
                    etat = "En cours";
                    break;
                case EtatFraisHorsForfait.Accepter:
                    etat = "Accepté";
                    break;
                case EtatFraisHorsForfait.Refuser:
                    etat = "Refusé";
                    break;
            }
            return etat;
        }

    }
}
