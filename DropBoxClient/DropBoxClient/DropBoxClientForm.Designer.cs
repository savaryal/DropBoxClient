namespace DropBoxClient
{
    partial class DropBoxClientForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.parametersHomePictureBox = new System.Windows.Forms.PictureBox();
            this.userLabel = new System.Windows.Forms.Label();
            this.displayNameLabel = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.allocatedSpaceLabel = new System.Windows.Forms.Label();
            this.spaceLabel = new System.Windows.Forms.Label();
            this.usedSpaceLabel = new System.Windows.Forms.Label();
            this.currentStatusLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.logsPanel = new System.Windows.Forms.Panel();
            this.logsLabel = new System.Windows.Forms.Label();
            this.homePanel = new System.Windows.Forms.Panel();
            this.parametersPanel = new System.Windows.Forms.Panel();
            this.folderGroupBox = new System.Windows.Forms.GroupBox();
            this.parametersLabel = new System.Windows.Forms.Label();
            this.logsGroupBox = new System.Windows.Forms.GroupBox();
            this.appStartingGroupBox = new System.Windows.Forms.GroupBox();
            this.profileGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.parametersHomePictureBox)).BeginInit();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.homePanel.SuspendLayout();
            this.parametersPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // parametersHomePictureBox
            // 
            this.parametersHomePictureBox.Image = global::DropBoxClient.Properties.Resources.parametersIcon;
            this.parametersHomePictureBox.Location = new System.Drawing.Point(456, 3);
            this.parametersHomePictureBox.Name = "parametersHomePictureBox";
            this.parametersHomePictureBox.Size = new System.Drawing.Size(40, 40);
            this.parametersHomePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.parametersHomePictureBox.TabIndex = 0;
            this.parametersHomePictureBox.TabStop = false;
            this.parametersHomePictureBox.Click += new System.EventHandler(this.parametersHomePictureBox_Click);
            // 
            // userLabel
            // 
            this.userLabel.AutoSize = true;
            this.userLabel.Location = new System.Drawing.Point(12, 15);
            this.userLabel.Name = "userLabel";
            this.userLabel.Size = new System.Drawing.Size(81, 17);
            this.userLabel.TabIndex = 1;
            this.userLabel.Text = "Utilisateur : ";
            // 
            // displayNameLabel
            // 
            this.displayNameLabel.AutoSize = true;
            this.displayNameLabel.Location = new System.Drawing.Point(99, 15);
            this.displayNameLabel.Name = "displayNameLabel";
            this.displayNameLabel.Size = new System.Drawing.Size(86, 17);
            this.displayNameLabel.TabIndex = 2;
            this.displayNameLabel.Text = "placeholder";
            // 
            // topPanel
            // 
            this.topPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topPanel.Controls.Add(this.parametersHomePictureBox);
            this.topPanel.Controls.Add(this.userLabel);
            this.topPanel.Controls.Add(this.displayNameLabel);
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(501, 46);
            this.topPanel.TabIndex = 3;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomPanel.Controls.Add(this.separatorLabel);
            this.bottomPanel.Controls.Add(this.allocatedSpaceLabel);
            this.bottomPanel.Controls.Add(this.spaceLabel);
            this.bottomPanel.Controls.Add(this.usedSpaceLabel);
            this.bottomPanel.Controls.Add(this.currentStatusLabel);
            this.bottomPanel.Controls.Add(this.statusLabel);
            this.bottomPanel.Location = new System.Drawing.Point(0, 582);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(501, 46);
            this.bottomPanel.TabIndex = 4;
            // 
            // separatorLabel
            // 
            this.separatorLabel.AutoSize = true;
            this.separatorLabel.Location = new System.Drawing.Point(448, 15);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(14, 17);
            this.separatorLabel.TabIndex = 0;
            this.separatorLabel.Text = "/";
            // 
            // allocatedSpaceLabel
            // 
            this.allocatedSpaceLabel.AutoSize = true;
            this.allocatedSpaceLabel.Location = new System.Drawing.Point(459, 15);
            this.allocatedSpaceLabel.Name = "allocatedSpaceLabel";
            this.allocatedSpaceLabel.Size = new System.Drawing.Size(39, 17);
            this.allocatedSpaceLabel.TabIndex = 6;
            this.allocatedSpaceLabel.Text = "2 Go";
            // 
            // spaceLabel
            // 
            this.spaceLabel.AutoSize = true;
            this.spaceLabel.Location = new System.Drawing.Point(297, 15);
            this.spaceLabel.Name = "spaceLabel";
            this.spaceLabel.Size = new System.Drawing.Size(101, 17);
            this.spaceLabel.TabIndex = 2;
            this.spaceLabel.Text = "Espace utilisé :";
            // 
            // usedSpaceLabel
            // 
            this.usedSpaceLabel.AutoSize = true;
            this.usedSpaceLabel.Location = new System.Drawing.Point(396, 15);
            this.usedSpaceLabel.Name = "usedSpaceLabel";
            this.usedSpaceLabel.Size = new System.Drawing.Size(57, 17);
            this.usedSpaceLabel.TabIndex = 5;
            this.usedSpaceLabel.Text = "0.00 Go";
            // 
            // currentStatusLabel
            // 
            this.currentStatusLabel.AutoSize = true;
            this.currentStatusLabel.Location = new System.Drawing.Point(72, 15);
            this.currentStatusLabel.Name = "currentStatusLabel";
            this.currentStatusLabel.Size = new System.Drawing.Size(86, 17);
            this.currentStatusLabel.TabIndex = 1;
            this.currentStatusLabel.Text = "placeholder";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(54, 17);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Statut :";
            // 
            // logsPanel
            // 
            this.logsPanel.AutoScroll = true;
            this.logsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logsPanel.Location = new System.Drawing.Point(12, 45);
            this.logsPanel.Name = "logsPanel";
            this.logsPanel.Size = new System.Drawing.Size(477, 473);
            this.logsPanel.TabIndex = 5;
            // 
            // logsLabel
            // 
            this.logsLabel.AutoSize = true;
            this.logsLabel.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logsLabel.Location = new System.Drawing.Point(141, 11);
            this.logsLabel.Name = "logsLabel";
            this.logsLabel.Size = new System.Drawing.Size(218, 23);
            this.logsLabel.TabIndex = 6;
            this.logsLabel.Text = "Journal des opérations";
            // 
            // homePanel
            // 
            this.homePanel.Controls.Add(this.logsLabel);
            this.homePanel.Controls.Add(this.logsPanel);
            this.homePanel.Location = new System.Drawing.Point(0, 53);
            this.homePanel.Name = "homePanel";
            this.homePanel.Size = new System.Drawing.Size(501, 523);
            this.homePanel.TabIndex = 7;
            // 
            // parametersPanel
            // 
            this.parametersPanel.Controls.Add(this.profileGroupBox);
            this.parametersPanel.Controls.Add(this.appStartingGroupBox);
            this.parametersPanel.Controls.Add(this.logsGroupBox);
            this.parametersPanel.Controls.Add(this.parametersLabel);
            this.parametersPanel.Controls.Add(this.folderGroupBox);
            this.parametersPanel.Location = new System.Drawing.Point(0, 53);
            this.parametersPanel.Name = "parametersPanel";
            this.parametersPanel.Size = new System.Drawing.Size(501, 523);
            this.parametersPanel.TabIndex = 8;
            this.parametersPanel.Visible = false;
            // 
            // folderGroupBox
            // 
            this.folderGroupBox.Location = new System.Drawing.Point(41, 57);
            this.folderGroupBox.Name = "folderGroupBox";
            this.folderGroupBox.Size = new System.Drawing.Size(419, 90);
            this.folderGroupBox.TabIndex = 0;
            this.folderGroupBox.TabStop = false;
            this.folderGroupBox.Text = "Dossier à synchroniser";
            // 
            // parametersLabel
            // 
            this.parametersLabel.AutoSize = true;
            this.parametersLabel.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parametersLabel.Location = new System.Drawing.Point(193, 11);
            this.parametersLabel.Name = "parametersLabel";
            this.parametersLabel.Size = new System.Drawing.Size(115, 23);
            this.parametersLabel.TabIndex = 1;
            this.parametersLabel.Text = "Paramètres";
            // 
            // logsGroupBox
            // 
            this.logsGroupBox.Location = new System.Drawing.Point(41, 170);
            this.logsGroupBox.Name = "logsGroupBox";
            this.logsGroupBox.Size = new System.Drawing.Size(419, 90);
            this.logsGroupBox.TabIndex = 2;
            this.logsGroupBox.TabStop = false;
            this.logsGroupBox.Text = "Journal des opérations";
            // 
            // appStartingGroupBox
            // 
            this.appStartingGroupBox.Location = new System.Drawing.Point(41, 283);
            this.appStartingGroupBox.Name = "appStartingGroupBox";
            this.appStartingGroupBox.Size = new System.Drawing.Size(419, 90);
            this.appStartingGroupBox.TabIndex = 3;
            this.appStartingGroupBox.TabStop = false;
            this.appStartingGroupBox.Text = "Démarrage de l\'application";
            // 
            // profileGroupBox
            // 
            this.profileGroupBox.Location = new System.Drawing.Point(41, 396);
            this.profileGroupBox.Name = "profileGroupBox";
            this.profileGroupBox.Size = new System.Drawing.Size(419, 90);
            this.profileGroupBox.TabIndex = 4;
            this.profileGroupBox.TabStop = false;
            this.profileGroupBox.Text = "Compte";
            // 
            // DropBoxClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 628);
            this.Controls.Add(this.parametersPanel);
            this.Controls.Add(this.homePanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "DropBoxClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DropBoxClient";
            ((System.ComponentModel.ISupportInitialize)(this.parametersHomePictureBox)).EndInit();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.homePanel.ResumeLayout(false);
            this.homePanel.PerformLayout();
            this.parametersPanel.ResumeLayout(false);
            this.parametersPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox parametersHomePictureBox;
        private System.Windows.Forms.Label userLabel;
        private System.Windows.Forms.Label displayNameLabel;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label currentStatusLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label spaceLabel;
        private System.Windows.Forms.Label usedSpaceLabel;
        private System.Windows.Forms.Label allocatedSpaceLabel;
        private System.Windows.Forms.Panel logsPanel;
        private System.Windows.Forms.Label logsLabel;
        private System.Windows.Forms.Label separatorLabel;
        private System.Windows.Forms.Panel homePanel;
        private System.Windows.Forms.Panel parametersPanel;
        private System.Windows.Forms.GroupBox folderGroupBox;
        private System.Windows.Forms.Label parametersLabel;
        private System.Windows.Forms.GroupBox logsGroupBox;
        private System.Windows.Forms.GroupBox appStartingGroupBox;
        private System.Windows.Forms.GroupBox profileGroupBox;
    }
}

