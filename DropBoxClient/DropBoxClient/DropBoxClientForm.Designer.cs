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
            this.usedAndAllocatedSpaceLabel = new System.Windows.Forms.Label();
            this.spaceLabel = new System.Windows.Forms.Label();
            this.currentStatusLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.logsPanel = new System.Windows.Forms.Panel();
            this.logsLabel = new System.Windows.Forms.Label();
            this.homePanel = new System.Windows.Forms.Panel();
            this.parametersPanel = new System.Windows.Forms.Panel();
            this.profileGroupBox = new System.Windows.Forms.GroupBox();
            this.connectedProfileLabel = new System.Windows.Forms.Label();
            this.logoutButton = new System.Windows.Forms.Button();
            this.appStartingGroupBox = new System.Windows.Forms.GroupBox();
            this.appStartingCheckBox = new System.Windows.Forms.CheckBox();
            this.logsGroupBox = new System.Windows.Forms.GroupBox();
            this.saveLogsLabel = new System.Windows.Forms.Label();
            this.saveLogsButton = new System.Windows.Forms.Button();
            this.parametersLabel = new System.Windows.Forms.Label();
            this.folderGroupBox = new System.Windows.Forms.GroupBox();
            this.chooseFolderButton = new System.Windows.Forms.Button();
            this.folderToSynchPathLabel = new System.Windows.Forms.Label();
            this.toSynchFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.logsSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loginPanel = new System.Windows.Forms.Panel();
            this.continueButton = new System.Windows.Forms.Button();
            this.dropboxCodeLabel = new System.Windows.Forms.Label();
            this.dropboxCodeTextBox = new System.Windows.Forms.TextBox();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.loginLabel = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.folderToSynchFileSystemWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.parametersHomePictureBox)).BeginInit();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.homePanel.SuspendLayout();
            this.parametersPanel.SuspendLayout();
            this.profileGroupBox.SuspendLayout();
            this.appStartingGroupBox.SuspendLayout();
            this.logsGroupBox.SuspendLayout();
            this.folderGroupBox.SuspendLayout();
            this.loginPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folderToSynchFileSystemWatcher)).BeginInit();
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
            this.displayNameLabel.Size = new System.Drawing.Size(117, 17);
            this.displayNameLabel.TabIndex = 2;
            this.displayNameLabel.Text = "Nom d\'utilisateur";
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
            this.bottomPanel.Controls.Add(this.usedAndAllocatedSpaceLabel);
            this.bottomPanel.Controls.Add(this.spaceLabel);
            this.bottomPanel.Controls.Add(this.currentStatusLabel);
            this.bottomPanel.Controls.Add(this.statusLabel);
            this.bottomPanel.Location = new System.Drawing.Point(0, 582);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(501, 46);
            this.bottomPanel.TabIndex = 4;
            // 
            // usedAndAllocatedSpaceLabel
            // 
            this.usedAndAllocatedSpaceLabel.AutoSize = true;
            this.usedAndAllocatedSpaceLabel.Location = new System.Drawing.Point(371, 15);
            this.usedAndAllocatedSpaceLabel.Name = "usedAndAllocatedSpaceLabel";
            this.usedAndAllocatedSpaceLabel.Size = new System.Drawing.Size(12, 17);
            this.usedAndAllocatedSpaceLabel.TabIndex = 6;
            this.usedAndAllocatedSpaceLabel.Text = "-";
            this.usedAndAllocatedSpaceLabel.Visible = false;
            // 
            // spaceLabel
            // 
            this.spaceLabel.AutoSize = true;
            this.spaceLabel.Location = new System.Drawing.Point(272, 15);
            this.spaceLabel.Name = "spaceLabel";
            this.spaceLabel.Size = new System.Drawing.Size(101, 17);
            this.spaceLabel.TabIndex = 2;
            this.spaceLabel.Text = "Espace utilisé :";
            this.spaceLabel.Visible = false;
            // 
            // currentStatusLabel
            // 
            this.currentStatusLabel.AutoSize = true;
            this.currentStatusLabel.Location = new System.Drawing.Point(63, 15);
            this.currentStatusLabel.Name = "currentStatusLabel";
            this.currentStatusLabel.Size = new System.Drawing.Size(328, 17);
            this.currentStatusLabel.TabIndex = 1;
            this.currentStatusLabel.Text = "En attente de connexion à un compte Dropbox...";
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
            this.homePanel.Controls.Add(this.logsPanel);
            this.homePanel.Controls.Add(this.logsLabel);
            this.homePanel.Location = new System.Drawing.Point(0, 53);
            this.homePanel.Name = "homePanel";
            this.homePanel.Size = new System.Drawing.Size(501, 523);
            this.homePanel.TabIndex = 7;
            this.homePanel.Visible = false;
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
            // profileGroupBox
            // 
            this.profileGroupBox.Controls.Add(this.connectedProfileLabel);
            this.profileGroupBox.Controls.Add(this.logoutButton);
            this.profileGroupBox.Location = new System.Drawing.Point(41, 396);
            this.profileGroupBox.Name = "profileGroupBox";
            this.profileGroupBox.Size = new System.Drawing.Size(419, 90);
            this.profileGroupBox.TabIndex = 4;
            this.profileGroupBox.TabStop = false;
            this.profileGroupBox.Text = "Compte";
            // 
            // connectedProfileLabel
            // 
            this.connectedProfileLabel.AutoSize = true;
            this.connectedProfileLabel.Location = new System.Drawing.Point(23, 37);
            this.connectedProfileLabel.Name = "connectedProfileLabel";
            this.connectedProfileLabel.Size = new System.Drawing.Size(117, 17);
            this.connectedProfileLabel.TabIndex = 1;
            this.connectedProfileLabel.Text = "Nom d\'utilisateur";
            // 
            // logoutButton
            // 
            this.logoutButton.Location = new System.Drawing.Point(294, 31);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(119, 28);
            this.logoutButton.TabIndex = 0;
            this.logoutButton.Text = "Se déconnecter";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // appStartingGroupBox
            // 
            this.appStartingGroupBox.Controls.Add(this.appStartingCheckBox);
            this.appStartingGroupBox.Location = new System.Drawing.Point(41, 283);
            this.appStartingGroupBox.Name = "appStartingGroupBox";
            this.appStartingGroupBox.Size = new System.Drawing.Size(419, 90);
            this.appStartingGroupBox.TabIndex = 3;
            this.appStartingGroupBox.TabStop = false;
            this.appStartingGroupBox.Text = "Démarrage de l\'application";
            // 
            // appStartingCheckBox
            // 
            this.appStartingCheckBox.AutoSize = true;
            this.appStartingCheckBox.Checked = true;
            this.appStartingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.appStartingCheckBox.Location = new System.Drawing.Point(26, 35);
            this.appStartingCheckBox.Name = "appStartingCheckBox";
            this.appStartingCheckBox.Size = new System.Drawing.Size(351, 21);
            this.appStartingCheckBox.TabIndex = 0;
            this.appStartingCheckBox.Text = "Démarrer l\'application au lancement de la session";
            this.appStartingCheckBox.UseVisualStyleBackColor = true;
            // 
            // logsGroupBox
            // 
            this.logsGroupBox.Controls.Add(this.saveLogsLabel);
            this.logsGroupBox.Controls.Add(this.saveLogsButton);
            this.logsGroupBox.Location = new System.Drawing.Point(41, 170);
            this.logsGroupBox.Name = "logsGroupBox";
            this.logsGroupBox.Size = new System.Drawing.Size(419, 90);
            this.logsGroupBox.TabIndex = 2;
            this.logsGroupBox.TabStop = false;
            this.logsGroupBox.Text = "Journal des opérations";
            // 
            // saveLogsLabel
            // 
            this.saveLogsLabel.AutoSize = true;
            this.saveLogsLabel.Location = new System.Drawing.Point(23, 37);
            this.saveLogsLabel.Name = "saveLogsLabel";
            this.saveLogsLabel.Size = new System.Drawing.Size(252, 17);
            this.saveLogsLabel.TabIndex = 1;
            this.saveLogsLabel.Text = "Sauvegarder le journal des opérations";
            // 
            // saveLogsButton
            // 
            this.saveLogsButton.Location = new System.Drawing.Point(294, 31);
            this.saveLogsButton.Name = "saveLogsButton";
            this.saveLogsButton.Size = new System.Drawing.Size(119, 28);
            this.saveLogsButton.TabIndex = 0;
            this.saveLogsButton.Text = "Sauvegarder";
            this.saveLogsButton.UseVisualStyleBackColor = true;
            this.saveLogsButton.Click += new System.EventHandler(this.saveLogsButton_Click);
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
            // folderGroupBox
            // 
            this.folderGroupBox.Controls.Add(this.chooseFolderButton);
            this.folderGroupBox.Controls.Add(this.folderToSynchPathLabel);
            this.folderGroupBox.Location = new System.Drawing.Point(41, 57);
            this.folderGroupBox.Name = "folderGroupBox";
            this.folderGroupBox.Size = new System.Drawing.Size(419, 90);
            this.folderGroupBox.TabIndex = 0;
            this.folderGroupBox.TabStop = false;
            this.folderGroupBox.Text = "Dossier à synchroniser";
            // 
            // chooseFolderButton
            // 
            this.chooseFolderButton.Location = new System.Drawing.Point(294, 31);
            this.chooseFolderButton.Name = "chooseFolderButton";
            this.chooseFolderButton.Size = new System.Drawing.Size(119, 28);
            this.chooseFolderButton.TabIndex = 1;
            this.chooseFolderButton.Text = "Choisir";
            this.chooseFolderButton.UseVisualStyleBackColor = true;
            this.chooseFolderButton.Click += new System.EventHandler(this.chooseFolderButton_Click);
            // 
            // folderToSynchPathLabel
            // 
            this.folderToSynchPathLabel.AutoEllipsis = true;
            this.folderToSynchPathLabel.Location = new System.Drawing.Point(23, 37);
            this.folderToSynchPathLabel.MaximumSize = new System.Drawing.Size(265, 17);
            this.folderToSynchPathLabel.Name = "folderToSynchPathLabel";
            this.folderToSynchPathLabel.Size = new System.Drawing.Size(265, 17);
            this.folderToSynchPathLabel.TabIndex = 0;
            this.folderToSynchPathLabel.Text = "Chemin du dossier";
            // 
            // loginPanel
            // 
            this.loginPanel.Controls.Add(this.continueButton);
            this.loginPanel.Controls.Add(this.dropboxCodeLabel);
            this.loginPanel.Controls.Add(this.dropboxCodeTextBox);
            this.loginPanel.Controls.Add(this.loginTextBox);
            this.loginPanel.Controls.Add(this.loginLabel);
            this.loginPanel.Controls.Add(this.loginButton);
            this.loginPanel.Location = new System.Drawing.Point(0, 0);
            this.loginPanel.Name = "loginPanel";
            this.loginPanel.Size = new System.Drawing.Size(501, 576);
            this.loginPanel.TabIndex = 9;
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(188, 458);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(125, 41);
            this.continueButton.TabIndex = 5;
            this.continueButton.Text = "Continuer";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Visible = false;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // dropboxCodeLabel
            // 
            this.dropboxCodeLabel.AutoSize = true;
            this.dropboxCodeLabel.Location = new System.Drawing.Point(26, 400);
            this.dropboxCodeLabel.Name = "dropboxCodeLabel";
            this.dropboxCodeLabel.Size = new System.Drawing.Size(179, 17);
            this.dropboxCodeLabel.TabIndex = 4;
            this.dropboxCodeLabel.Text = "Code fourni par Dropbox :";
            this.dropboxCodeLabel.Visible = false;
            // 
            // dropboxCodeTextBox
            // 
            this.dropboxCodeTextBox.Location = new System.Drawing.Point(29, 420);
            this.dropboxCodeTextBox.Name = "dropboxCodeTextBox";
            this.dropboxCodeTextBox.Size = new System.Drawing.Size(443, 23);
            this.dropboxCodeTextBox.TabIndex = 3;
            this.dropboxCodeTextBox.Visible = false;
            // 
            // loginTextBox
            // 
            this.loginTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.loginTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.loginTextBox.Location = new System.Drawing.Point(132, 122);
            this.loginTextBox.Multiline = true;
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(237, 133);
            this.loginTextBox.TabIndex = 2;
            this.loginTextBox.Text = "Pour utiliser l\'application une connexion avec Dropbox est nécessaire. \r\n\r\nL\'appl" +
    "ication doit être autorisée afin d\'interagir avec votre Dropbox.";
            // 
            // loginLabel
            // 
            this.loginLabel.AutoSize = true;
            this.loginLabel.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginLabel.Location = new System.Drawing.Point(171, 57);
            this.loginLabel.Name = "loginLabel";
            this.loginLabel.Size = new System.Drawing.Size(158, 33);
            this.loginLabel.TabIndex = 1;
            this.loginLabel.Text = "Connexion";
            // 
            // loginButton
            // 
            this.loginButton.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.Location = new System.Drawing.Point(109, 260);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(283, 58);
            this.loginButton.TabIndex = 0;
            this.loginButton.Text = "Connexion avec Dropbox";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // folderToSynchFileSystemWatcher
            // 
            this.folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
            this.folderToSynchFileSystemWatcher.IncludeSubdirectories = true;
            this.folderToSynchFileSystemWatcher.SynchronizingObject = this;
            this.folderToSynchFileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.folderToSynchFileSystemWatcher_Changed);
            this.folderToSynchFileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.folderToSynchFileSystemWatcher_Created);
            this.folderToSynchFileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.folderToSynchFileSystemWatcher_Deleted);
            this.folderToSynchFileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.folderToSynchFileSystemWatcher_Renamed);
            // 
            // DropBoxClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 628);
            this.Controls.Add(this.loginPanel);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DropBoxClientForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.parametersHomePictureBox)).EndInit();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.homePanel.ResumeLayout(false);
            this.homePanel.PerformLayout();
            this.parametersPanel.ResumeLayout(false);
            this.parametersPanel.PerformLayout();
            this.profileGroupBox.ResumeLayout(false);
            this.profileGroupBox.PerformLayout();
            this.appStartingGroupBox.ResumeLayout(false);
            this.appStartingGroupBox.PerformLayout();
            this.logsGroupBox.ResumeLayout(false);
            this.logsGroupBox.PerformLayout();
            this.folderGroupBox.ResumeLayout(false);
            this.loginPanel.ResumeLayout(false);
            this.loginPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.folderToSynchFileSystemWatcher)).EndInit();
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
        private System.Windows.Forms.Panel logsPanel;
        private System.Windows.Forms.Label logsLabel;
        private System.Windows.Forms.Panel homePanel;
        private System.Windows.Forms.Panel parametersPanel;
        private System.Windows.Forms.GroupBox folderGroupBox;
        private System.Windows.Forms.Label parametersLabel;
        private System.Windows.Forms.GroupBox logsGroupBox;
        private System.Windows.Forms.GroupBox appStartingGroupBox;
        private System.Windows.Forms.GroupBox profileGroupBox;
        private System.Windows.Forms.FolderBrowserDialog toSynchFolderBrowserDialog;
        private System.Windows.Forms.Label folderToSynchPathLabel;
        private System.Windows.Forms.Button chooseFolderButton;
        private System.Windows.Forms.Button saveLogsButton;
        private System.Windows.Forms.Label saveLogsLabel;
        private System.Windows.Forms.CheckBox appStartingCheckBox;
        private System.Windows.Forms.SaveFileDialog logsSaveFileDialog;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Label connectedProfileLabel;
        private System.Windows.Forms.Panel loginPanel;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Label loginLabel;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.TextBox dropboxCodeTextBox;
        private System.Windows.Forms.Label dropboxCodeLabel;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label usedAndAllocatedSpaceLabel;
        private System.IO.FileSystemWatcher folderToSynchFileSystemWatcher;
    }
}

