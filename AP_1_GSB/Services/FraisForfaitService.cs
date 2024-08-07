﻿using AP_1_GSB.Data.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Prepare;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Pqc.Crypto.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace AP_1_GSB.Services
{
    public class FraisForfaitService
    {

        //Supression d'un frais forfait en base
        public static bool SupprimerFraisForfait(FraisForfait frais)
        {
            int IdFrais = frais.IdFraisForfait;

            if (frais == null && IdFrais < 1)
            {
                return false;
            }

            Data.SqlConnection.ConnexionSql();
            string requete = "DELETE FROM frais_forfait WHERE id_frais_forfait = @IdFraisForfait";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(requete, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@IdFraisForfait", IdFrais);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
                return false;

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de donénes : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }
        }
        //Insertion d'un frais forfait en base
        public static bool AjouterFraisForfait(int IdFiche, int idType, FraisForfait frais, byte[] justificatif)
        {
            if (frais.Date == new DateTime(1, 1, 1)) 
            {
                frais.Date = DateTime.Now;
            }
            int idJustificatif = 0;
            if (justificatif != null)
            {
                Justificatif justi;
                justi = Services.JustificatifService.AjouterJustificatif(justificatif);
                idJustificatif = justi.IdJustificatif;
            }

            Data.SqlConnection.ConnexionSql();

            string RequeteCreationFraisForfait = "INSERT INTO frais_forfait (quantite, date, etat, id_type_forfait, id_fiche_frais, id_justificatif) " +
                                                "VALUES (@quantite, @Date, @etat, @id_type_forfait, @id_fiche_frais, @id_justificatif)";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(RequeteCreationFraisForfait, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@quantite", frais.Quantite);
                    cmd.Parameters.AddWithValue("@Date", frais.Date);
                    cmd.Parameters.AddWithValue("@etat", "ATTENTE");
                    cmd.Parameters.AddWithValue("@id_type_forfait", idType);
                    cmd.Parameters.AddWithValue("@id_fiche_frais", IdFiche);
                    cmd.Parameters.AddWithValue("@id_Justificatif", idJustificatif == 0 ? (object)DBNull.Value : idJustificatif);

                    if (cmd.ExecuteNonQuery() > 0)
                    { return true; }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de donné : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }
            return false;
        }
        //Modification d'un frais forfait en base
        public static bool ModifierFraisForfait(int idType, FraisForfait frais, byte[] justificatif)
        {
            if (frais.Date == new DateTime(1, 1, 1)) 
            {
                frais.Date = DateTime.Now;
            }
            int idJustificatif = 0;
            if (justificatif != null)
            {
                Justificatif justi;
                justi = Services.JustificatifService.AjouterJustificatif(justificatif);
                idJustificatif = justi.IdJustificatif;
            }

            Data.SqlConnection.ConnexionSql();

            string RequeteModificationFraisForfait = "UPDATE frais_forfait SET quantite = @quantite, date = @Date, etat = @etat, id_type_forfait = @id_type_forfait, id_justificatif = @id_justificatif" +
                " WHERE id_frais_forfait = @IdFraisForfait";

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(RequeteModificationFraisForfait, Data.SqlConnection.Connection))
                {
                    cmd.Parameters.AddWithValue("@quantite", frais.Quantite);
                    cmd.Parameters.AddWithValue("@Date", frais.Date);
                    cmd.Parameters.AddWithValue("@etat", "ATTENTE");
                    cmd.Parameters.AddWithValue("@id_type_forfait", idType);
                    cmd.Parameters.AddWithValue("@id_justificatif", idJustificatif == 0 ? (object)DBNull.Value : idJustificatif);
                    cmd.Parameters.AddWithValue("@IdFraisForfait", frais.IdFraisForfait);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur lors de la connexion à la base de donné : " + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Data.SqlConnection.DeconnexionSql();
            }
            return false;
        }


        //Calcul du total des frais forfait
        public static float CalculerTotalForfait(FicheFrais ficheEncours)
        {
            float Total = 0;
            foreach (FraisForfait frais in ficheEncours.FraisForfaits)
            {
                Total += frais.Quantite * frais.TypeForfait.Montant;
            }
            return Total;
        }

        public static void ChangerEtatFraisForfat(int idFrais, string etat)
        {
            Data.SqlConnection.ConnexionSql();
            string requete = "UPDATE frais_forfait SET etat = @etat WHERE id_frais_forfait = @idFrais;";

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
        //Ecriture des frais forfait 
        public static string EcrireEtatFraisForfait(FraisForfait frais)
        {
            string etat = "";
            switch (frais.Etat)
            {
                case EtatFraisForfait.Attente:
                    etat = "En cours";
                    break;
                case EtatFraisForfait.Accepter:
                    etat = "Accepté";
                    break;
                case EtatFraisForfait.Refuser:
                    etat = "Refusé";
                    break;
            }
            return etat;
        }
    }
}


