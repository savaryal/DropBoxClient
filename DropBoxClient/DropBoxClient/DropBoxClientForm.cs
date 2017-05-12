///ETML
///Auteur : Alison Savary
///Date   : 11.05.2017
///Description : Client Dropbox en C#
///   
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropBoxClient
{
    public partial class DropBoxClientForm : Form
    {
        //////////// Constantes ////////////
        private const string STR_APP_KEY = "5zpx9nyjaxmksiy";
        private const string STR_APP_SECRET = "c395jxtgfj42pjw";

        // Titre et message des MessageBox
        private const string STR_QUIT_BOX_MESSAGE = "Voulez-vous vraiment quitter l'application ? La synchronisation automatique ne sera plus effectuée.";
        private const string STR_QUIT_BOX_TITLE = "Fermeture de l'application";
        private const string STR_SAVE_BOX_TITLE = "Sauvegarde";
        private const string STR_SAVE_BOX_MESSAGE_OK = "Sauvegarde du journal des opérations effectuée.";
        private const string STR_SAVE_BOX_MESSAGE_KO = "La sauvegarde du journal des opérations n'a pas été effectuée.";
        
        // Status
        private const string STR_WAITING_LOGIN_STATUS = "En attente de connexion à un compte Dropbox...";
        private const string STR_CONNECTED_STATUS = "Connecté";

        //////////// Icônes ////////////
        private Bitmap bitmapHomeIcon = DropBoxClient.Properties.Resources.homeIcon;
        private Bitmap bitmapParametersIcon = DropBoxClient.Properties.Resources.parametersIcon;

        //////////// URLs ressources ////////////
        private string strGetOauth2Authorize = "https://www.dropbox.com/oauth2/authorize";
        private string strPostOauth2Token = "https://api.dropboxapi.com/oauth2/token";
        private string strPostGetAccount = "https://api.dropboxapi.com/2/users/get_current_account";
        private string strPostGetSpaceUsage = "https://api.dropboxapi.com/2/users/get_space_usage";

        private string strCode;
        private string strToken; 

        /// <summary>
        /// Constructeur de la Form
        /// </summary>
        public DropBoxClientForm()
        {
            InitializeComponent();

            // Initialise des paramètres de logsSaveFileDialog
            logsSaveFileDialog.Filter = "txt files (*.txt)|*.txt";
            logsSaveFileDialog.RestoreDirectory = true;
            logsSaveFileDialog.DefaultExt = ".txt";
            logsSaveFileDialog.AddExtension = true;
        }

        //////////// INTERACTIONS PROVENANT D'ELEMENTS DU GUI ////////////

        /// <summary>
        ///  Passe de l'interface principale aux paramètres et inversement
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void parametersHomePictureBox_Click(object sender, EventArgs e)
        {
            // Si l'image correspond à l'icône Paramètres
            if(parametersHomePictureBox.Image == bitmapParametersIcon)
            {   // Change l'icône en Home
                parametersHomePictureBox.Image = bitmapHomeIcon;
                // Cache l'interface principale et affiche celle des paramètres
                homePanel.Visible = false;
                parametersPanel.Visible = true;
            }
            else
            {   // Change l'icône en celui des paramètres 
                parametersHomePictureBox.Image = bitmapParametersIcon;
                // Cache l'interface des paramètres et affiche l'interface principale
                parametersPanel.Visible = false;
                homePanel.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(strGetOauth2Authorize += "?client_id=" + STR_APP_KEY + "&response_type=code&force_reauthentication=true");
            dropboxCodeLabel.Visible = true;
            dropboxCodeTextBox.Visible = true;
            continueButton.Visible = true;

        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            strCode = dropboxCodeTextBox.Text;

            // OBTENTION TOKEN //
            // Création de la requête avec l'url
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(strPostOauth2Token += "?code=" + Uri.EscapeDataString(strCode) + "&grant_type=" + Uri.EscapeDataString("authorization_code") + "&client_id="+ Uri.EscapeDataString(STR_APP_KEY) + "&client_secret="+ Uri.EscapeDataString(STR_APP_SECRET));
            // Ajout de la méthode, verbe http
            postRequest.Method = "POST";
            // Spécification du type de contenu
            postRequest.ContentType = "application/x-www-form-urlencoded";
            // Envoi de la requête
            WebResponse postResponse = postRequest.GetResponse();
            // Récupération des données reçues et stockage dans un string
            StreamReader reader = new StreamReader(postResponse.GetResponseStream());
            string strJson = reader.ReadToEnd();
            postResponse.Close();

            // Création d'un objet JSON avec le string des données reçues
            JObject joToken = JObject.Parse(strJson);
            strToken = Convert.ToString(joToken["access_token"]);

            // OBTENTION DISPLAY_NAME
            // Création de la requête avec l'url
            HttpWebRequest postRequest2 = (HttpWebRequest)WebRequest.Create(strPostGetAccount);
            postRequest2.Headers.Add("Authorization", "Bearer "+ Uri.EscapeDataString(strToken));
            // Ajout de la méthode, verbe http
            postRequest2.Method = "POST";
            // Envoi de la requête
            WebResponse postResponse2 = postRequest2.GetResponse();
            // Récupération des données reçues et stockage dans un string
            StreamReader reader2 = new StreamReader(postResponse2.GetResponseStream());
            strJson = reader2.ReadToEnd();
            postResponse.Close();

            JObject joAccount = JObject.Parse(strJson);

            string strDisplayName = Convert.ToString(joAccount["name"]["display_name"]);

            displayNameLabel.Text = strDisplayName;
            connectedProfileLabel.Text = strDisplayName;

            // Cache l'interface de connexion et affiche l'interface principale
            loginPanel.Visible = false;
            homePanel.Visible = true;

            // Création de la requête avec l'url
            HttpWebRequest postRequest3 = (HttpWebRequest)WebRequest.Create(strPostGetSpaceUsage);
            postRequest3.Headers.Add("Authorization", "Bearer " + Uri.EscapeDataString(strToken));
            // Ajout de la méthode, verbe http
            postRequest3.Method = "POST";
            // Envoi de la requête
            WebResponse postResponse3 = postRequest3.GetResponse();
            // Récupération des données reçues et stockage dans un string
            StreamReader reader3 = new StreamReader(postResponse3.GetResponseStream());
            strJson = reader3.ReadToEnd();
            postResponse.Close();

            JObject joSpaceUsage = JObject.Parse(strJson);

            double dblUsedSpace = Convert.ToDouble(joSpaceUsage["used"]);
            double dblAllocatedSpace = Convert.ToDouble(joSpaceUsage["allocation"]["allocated"]);

            usedSpaceLabel.Text = (dblUsedSpace / 1000000000).ToString("0.##") + " Go";
            allocatedSpaceLabel.Text = (dblAllocatedSpace / 1000000000).ToString("0.##") + " Go";

            // Affiche les données concernat l'espace
            spaceLabel.Visible = true;
            allocatedSpaceLabel.Visible = true;
            separatorLabel.Visible = true;
            usedSpaceLabel.Visible = true;

            // Modification du statut
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void logoutButton_Click(object sender, EventArgs e)
        {
            // Cache l'interface des paramètres et affiche l'interface de connexion
            parametersLabel.Visible = false;
            loginPanel.Visible = true;

            // Cache les données concernat l'espace
            spaceLabel.Visible = false;
            allocatedSpaceLabel.Visible = false;
            separatorLabel.Visible = false;
            usedSpaceLabel.Visible = false;

            // Remet l'icône Paramètres
            parametersHomePictureBox.Image = bitmapParametersIcon;

            // Modification du statut
            currentStatusLabel.Text = STR_WAITING_LOGIN_STATUS;
        }

        /// <summary>
        /// Demande une confirmation à la fermeture de l'application
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void DropBoxClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si l'on clique sur le bouton "Non" 
            if (DialogResult.No == MessageBox.Show(STR_QUIT_BOX_MESSAGE, STR_QUIT_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                // Annule la fermeture
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void chooseFolderButton_Click(object sender, EventArgs e)
        {
            if(toSynchFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderToSynchPathLabel.Text = toSynchFolderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Sauvegarde le journal des opérations dans un fichier texte à l'emplacement désiré
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void saveLogsButton_Click(object sender, EventArgs e)
        {
             List<string> list_strLogs = new List<string>();

            // Demande à l'utilisateur l'emplacememt de sauvegarde du fichier
            if (logsSaveFileDialog.ShowDialog() == DialogResult.OK && logsSaveFileDialog.FileName.Substring(logsSaveFileDialog.FileName.Length - 3, 3) == "txt")
            {   // Crée et écrit le fichier
                System.IO.File.WriteAllLines(logsSaveFileDialog.FileName, list_strLogs);
                // Avertit l'utilisateur
                MessageBox.Show(STR_SAVE_BOX_MESSAGE_OK, STR_SAVE_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(STR_SAVE_BOX_MESSAGE_KO, STR_SAVE_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
