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
        private const string STR_TOKEN_TYPE = "Bearer ";

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

        //////////// Variables pour l'authentification ////////////
        private string strCode;
        private string strToken;
        private string strAuthHeader = "";

        // Journal des opérations
        private List<string> list_strLogs = new List<string>();

        private string strJson;

        StreamReader reader;

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
        /// Provoque l'ouverture du navigateur pour l'autorisation de l'application
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            // Ouvre le navigateur sur l'url permettant l'autorisation de l'app
            System.Diagnostics.Process.Start(strGetOauth2Authorize += "?client_id=" + STR_APP_KEY + "&response_type=code&force_reauthentication=true");

            // Affiche la textbox servant à récupérer le code fournit à l'utilisateur ainsi que le bouton continuer
            dropboxCodeLabel.Visible = true;
            dropboxCodeTextBox.Visible = true;
            continueButton.Visible = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continueButton_Click(object sender, EventArgs e)
        {
            // Obtient le token
            getToken();

            // Crée le Header d'authentification
            strAuthHeader = STR_TOKEN_TYPE + Uri.EscapeDataString(strToken);

            // Crée la requête permettant d'obtenir le display_name
            WebRequest postDisplayNameRequest = createPostRequest(strAuthHeader, strPostGetAccount);

            try
            {
                // Envoi de la requête
                WebResponse postDisplayNameResponse = postDisplayNameRequest.GetResponse();
                // Récupération des données reçues et stockage dans un string
                reader = new StreamReader(postDisplayNameResponse.GetResponseStream());
                strJson = reader.ReadToEnd();
                postDisplayNameResponse.Close();

                // Création d'un objet JSON avec le string des données reçues
                JObject joAccount = JObject.Parse(strJson);

                // Récupère le display_name 
                string strDisplayName = Convert.ToString(joAccount["name"]["display_name"]);
                displayNameLabel.Text = strDisplayName;
                connectedProfileLabel.Text = strDisplayName;
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Cache l'interface de connexion et affiche l'interface principale
            loginPanel.Visible = false;
            dropboxCodeLabel.Visible = false;
            dropboxCodeTextBox.Visible = false;
            continueButton.Visible = false;
            homePanel.Visible = true;

            try
            {
                // Crée la requête permettant d'obtenir les informations liées au stockage
                WebRequest postSpaceUsageRequest = createPostRequest(strAuthHeader, strPostGetSpaceUsage);
                // Envoi de la requête
                WebResponse postSpaceUsageResponse = postSpaceUsageRequest.GetResponse();
                // Récupération des données reçues et stockage dans un string
                reader = new StreamReader(postSpaceUsageResponse.GetResponseStream());
                strJson = reader.ReadToEnd();
                postSpaceUsageResponse.Close();

                // Création d'un objet JSON avec le string des données reçues
                JObject joSpaceUsage = JObject.Parse(strJson);

                // Récupère le stockage utilisé et le stockage total (en octet)
                double dblUsedSpace = Convert.ToDouble(joSpaceUsage["used"]);
                double dblAllocatedSpace = Convert.ToDouble(joSpaceUsage["allocation"]["allocated"]);
                string strUsedSpace;

                // Si la conversion en Go donne un résultat plus petit que 0.1 Go
                if (dblUsedSpace / 1000000000 < 0.1)
                {
                    // Convertit le résultat en Mo
                    strUsedSpace = (dblUsedSpace / 1000000).ToString("0.## Mo");
                }
                else
                {
                    strUsedSpace = (dblUsedSpace / 1000000000).ToString("0.## Go");
                }
                // Convertit les valeurs en Go
                string strAllocatedSpace = (dblAllocatedSpace / 1000000000).ToString("0.## Go");

                usedAndAllocatedSpaceLabel.Text = string.Format("{0} / {1}", strUsedSpace, strAllocatedSpace);
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // Affiche les données concernat l'espace
            spaceLabel.Visible = true;
            usedAndAllocatedSpaceLabel.Visible = true;

            // Modification du statut
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        }

        /// <summary>
        /// Provoque la déconnexion 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void logoutButton_Click(object sender, EventArgs e)
        {
            // Met la valeur du token à null
            strToken = null;

            // Cache l'interface des paramètres et affiche l'interface de connexion
            parametersPanel.Visible = false;
            loginPanel.Visible = true;

            // Cache les données concernat l'espace
            spaceLabel.Visible = false;
            usedAndAllocatedSpaceLabel.Visible = false;

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

        ///////////////////////

        /// <summary>
        /// Crée une requête http POST avec les paramètres donnés
        /// </summary>
        /// <param name="strAuthHeader">Header d'authentification</param>
        /// <param name="strPostUrl">URL Resource</param>
        /// <returns>Requête http POST prête à être envoyée </returns>
        private WebRequest createPostRequest(string strAuthHeader, string strPostUrl)
        {
            // Création de la requête avec l'url
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(strPostUrl);
            postRequest.Headers.Add("Authorization", strAuthHeader);
            // Ajout de la méthode, verbe http
            postRequest.Method = "POST";

            return postRequest;
        }

        /// <summary>
        /// Récupère le token grâce au code fournit par l'utilisateur
        /// </summary>
        private void getToken()
        {
            // Récupère le code de l'utilisateur
            strCode = dropboxCodeTextBox.Text;

            // Création de la requête avec l'url
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(strPostOauth2Token += "?code=" + Uri.EscapeDataString(strCode) + "&grant_type=" + Uri.EscapeDataString("authorization_code") + "&client_id=" + Uri.EscapeDataString(STR_APP_KEY) + "&client_secret=" + Uri.EscapeDataString(STR_APP_SECRET));
            // Ajout de la méthode, verbe http
            postRequest.Method = "POST";
            // Spécification du type de contenu
            postRequest.ContentType = "application/x-www-form-urlencoded";
            
            try
            {
                // Envoi de la requête
                WebResponse postResponse = postRequest.GetResponse();
                // Récupération des données reçues et stockage dans un string
                reader = new StreamReader(postResponse.GetResponseStream());
                string strJson = reader.ReadToEnd();
                postResponse.Close();

                // Création d'un objet JSON avec le string des données reçues
                JObject joToken = JObject.Parse(strJson);
                // Récupère le token
                strToken = Convert.ToString(joToken["access_token"]);

                DateTime dateNow = DateTime.Now;
                list_strLogs.Add(Convert.ToString(dateNow) + " - Connexion effectuée avec succès");
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
