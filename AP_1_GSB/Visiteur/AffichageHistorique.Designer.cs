﻿namespace AP_1_GSB.Visiteur
{
    partial class AffichageHistorique
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewFicheFrais = new System.Windows.Forms.ListView();
            this.Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Montant = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Etat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.BtnRetour = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewFicheFrais
            // 
            this.listViewFicheFrais.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Date,
            this.Montant,
            this.Etat});
            this.listViewFicheFrais.GridLines = true;
            this.listViewFicheFrais.HideSelection = false;
            this.listViewFicheFrais.HoverSelection = true;
            this.listViewFicheFrais.Location = new System.Drawing.Point(238, 128);
            this.listViewFicheFrais.Name = "listViewFicheFrais";
            this.listViewFicheFrais.Size = new System.Drawing.Size(740, 400);
            this.listViewFicheFrais.TabIndex = 0;
            this.listViewFicheFrais.UseCompatibleStateImageBehavior = false;
            this.listViewFicheFrais.View = System.Windows.Forms.View.Details;
            // 
            // Date
            // 
            this.Date.Text = "Date";
            this.Date.Width = 150;
            // 
            // Montant
            // 
            this.Montant.Text = "Montant";
            this.Montant.Width = 100;
            // 
            // Etat
            // 
            this.Etat.Text = "Etat";
            this.Etat.Width = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(525, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Historique de vos fiches de frais";
            // 
            // BtnRetour
            // 
            this.BtnRetour.Location = new System.Drawing.Point(1039, 716);
            this.BtnRetour.Name = "BtnRetour";
            this.BtnRetour.Size = new System.Drawing.Size(118, 39);
            this.BtnRetour.TabIndex = 2;
            this.BtnRetour.Text = "Retour";
            this.BtnRetour.UseVisualStyleBackColor = true;
            this.BtnRetour.Click += new System.EventHandler(this.BtnRetour_Click);
            // 
            // AffichageHistorique
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 874);
            this.Controls.Add(this.BtnRetour);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewFicheFrais);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AffichageHistorique";
            this.Text = "AffichageHistorique";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewFicheFrais;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader Date;
        private System.Windows.Forms.ColumnHeader Montant;
        private System.Windows.Forms.ColumnHeader Etat;
        private System.Windows.Forms.Button BtnRetour;
    }
}