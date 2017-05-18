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
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading;
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

        private string strPostCreateFolder = "https://api.dropboxapi.com/2/files/create_folder";
        private string strPostDelete = "https://api.dropboxapi.com/2/files/delete";
        private string strPostMove = "https://api.dropboxapi.com/2/files/move";
        private string strPostUpload = "https://content.dropboxapi.com/2/files/upload";
        private string strPostListFolder = "https://api.dropboxapi.com/2/files/list_folder";


        //////////// Variables pour l'authentification ////////////
        private string strToken; // Châine de caractère contenant le token
        private byte[] tab_byteToken; // Tableau de byte pour la protection du token
        private string strAuthHeader = ""; // En-tête d'authentification

        // Journal des opérations
        private List<string> list_strLogs = new List<string>();

        // Permet de récupérer le JSON reçu après la requête à l'API
        private string strJson;

        // Chemin du dossier à synchroniser
        private string strFolderPath;

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

            // Si un token est sauvegardé dans les paramètres d'application
            if (Properties.Settings.Default.tab_byteProtectedToken != null)
            {
                // Décode le tableau byte protégé en tableau byte contenant le token
                tab_byteToken = ProtectedData.Unprotect(Properties.Settings.Default.tab_byteProtectedToken, null, DataProtectionScope.CurrentUser);
                // Convertit le token en string
                strToken = System.Text.Encoding.ASCII.GetString(tab_byteToken);

                getDisplayName();
                getSpaceUsage();
                showMainInterface();

                addToLogs("Connexion effectuée avec succès");

                // Modification du statut
                currentStatusLabel.Text = STR_CONNECTED_STATUS;

            }   
            // Si le chemin du dossier à synchroniser est mémorisé  
            if(Properties.Settings.Default.strFolderPath != null)
            {
                // Récupère le chemin du dossier
                strFolderPath = Properties.Settings.Default.strFolderPath;
                folderToSynchPathLabel.Text = strFolderPath;

                // Surveille le dossier spécifié
                folderToSynchFileSystemWatcher.Path = strFolderPath;
            }      
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
            {   
                showParametersInterface();
            }
            else
            {
                showMainInterface();
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
        /// Poursuit la connexion en obtenant le token et en passant à l'interface principale
        /// si son obtention est réussie 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void continueButton_Click(object sender, EventArgs e)
        {
            bool boolIsTokenOK = getToken();

            // Si l'obtention du token a réussi
            if (boolIsTokenOK)
            {
                // Obtient les informations du compte
                getDisplayName();
                getSpaceUsage();

                // Affiche l'interface principale
                showMainInterface();

                // Modification du statut
                currentStatusLabel.Text = STR_CONNECTED_STATUS;

                // Commence la surveillance du dossier
                folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
            }
            else
            {
                // Modification du statut
                currentStatusLabel.Text = "La connexion a échoué. Réessayez.";
            }
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
            Properties.Settings.Default.tab_byteProtectedToken = null;
            // Arrête la surveillance du dossier
            folderToSynchFileSystemWatcher.EnableRaisingEvents = false;

            showLoginInterface();

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
            // Sauvegarde les paramètres d'application
            Properties.Settings.Default.Save();

            // Si l'on clique sur le bouton "Non" 
            if (DialogResult.No == MessageBox.Show(STR_QUIT_BOX_MESSAGE, STR_QUIT_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                // Annule la fermeture
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Permet de choisir le dossier à synchroniser
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void chooseFolderButton_Click(object sender, EventArgs e)
        {
            folderToSynchFileSystemWatcher.EnableRaisingEvents = false;

            // Si l'utilisateur choisit un dossier
            if(toSynchFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupère son chemin
                strFolderPath = toSynchFolderBrowserDialog.SelectedPath;

                // Crée un fichier pour tester si l'écriture est possible
                string strTestFile = "Test.txt";
                string strFilePath = System.IO.Path.Combine(strFolderPath, strTestFile);

                // Par défaut, le dossier est considéré comme OK pour la synchronisation
                bool boolIsFolderOK = true;

                try // Essaie de créer un fichier
                {
                    System.IO.File.Create(strFilePath).Close();
                }
                catch (Exception FileE)
                {
                    boolIsFolderOK = false;
                }
                try // Essaie de supprimer le fichier
                {
                    System.IO.File.Delete(strFilePath);
                }
                catch (Exception FileE)
                {
                    boolIsFolderOK = false;
                }

                // Si le dossier est OK pour la synchronisation (Possède le droit NTFS "Modification")
                if (boolIsFolderOK)
                {
                    // Enregistre le chemin du dossier
                    folderToSynchPathLabel.Text = strFolderPath;
                    Properties.Settings.Default.strFolderPath = strFolderPath;

                    // Surveille le dossier spécifié
                    folderToSynchFileSystemWatcher.Path = strFolderPath;

                    addToLogs("Le dossier à synchroniser a été changé en " + strFolderPath);
                }
                else
                {
                    MessageBox.Show("Le dossier n'a pas été modifié car il ne possède pas les droits nécessaires", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    strFolderPath = Properties.Settings.Default.strFolderPath;
                }
            }

            folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
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
        private bool getToken()
        {
            // Récupère le code de l'utilisateur
            string strCode = dropboxCodeTextBox.Text;

            // Création de la requête avec l'url
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(strPostOauth2Token + "?code=" + Uri.EscapeDataString(strCode) + "&grant_type=" + Uri.EscapeDataString("authorization_code") + "&client_id=" + Uri.EscapeDataString(STR_APP_KEY) + "&client_secret=" + Uri.EscapeDataString(STR_APP_SECRET));
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

                // Convertit le token en byte
                tab_byteToken = System.Text.Encoding.ASCII.GetBytes(strToken);

                // Protège le token avec la méthode Protect de la classe ProtectedData
                tab_byteToken = ProtectedData.Protect(tab_byteToken, null, DataProtectionScope.CurrentUser);
                // Stocke le token protégé dans les paramètres de l'application afin de le conserver même à l'arrêt de celle-ci
                Properties.Settings.Default.tab_byteProtectedToken = tab_byteToken;

                addToLogs("Connexion effectuée avec succès");

                return true;
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

                addToLogs("Erreur " + strErreur);

                return false;
            }

        } // END getToken()

        /// <summary>
        /// Récupère le nom d'affichage de l'utilisateur
        /// </summary>
        private void getDisplayName()
        {
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

                addToLogs("Erreur " + strErreur);
            }
        } // END getDisplayName()

        /// <summary>
        /// Récupère le stockage utilisé et le stockage alloué à l'utilisateur
        /// </summary>
        private void getSpaceUsage()
        {
            // Crée la requête permettant d'obtenir les informations liées au stockage
            WebRequest postSpaceUsageRequest = createPostRequest(strAuthHeader, strPostGetSpaceUsage);
            try
            {
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

                addToLogs("Erreur " + strErreur);
            }

            // Affiche les données concernant l'espace
            spaceLabel.Visible = true;
            usedAndAllocatedSpaceLabel.Visible = true;
        } // END getSpaceUsage()

        /// <summary>
        /// Affiche l'interface principale
        /// </summary>
        private void showMainInterface()
        {
            loginPanel.Visible = false;
            dropboxCodeLabel.Visible = false;
            dropboxCodeTextBox.Visible = false;
            continueButton.Visible = false;
            parametersPanel.Visible = false;
            homePanel.Visible = true;

            // Change l'icône en celui des paramètres
            parametersHomePictureBox.Image = bitmapParametersIcon;
        }

        /// <summary>
        /// Affiche l'interface de connexion 
        /// </summary>
        private void showLoginInterface()
        {
            parametersPanel.Visible = false;
            homePanel.Visible = false;
            spaceLabel.Visible = false;
            usedAndAllocatedSpaceLabel.Visible = false;
            loginPanel.Visible = true;
        }

        /// <summary>
        /// Affiche l'interface des paramètres
        /// </summary>
        private void showParametersInterface()
        {
            homePanel.Visible = false;
            loginPanel.Visible = false;
            parametersPanel.Visible = true;

            // Change l'icône en Home
            parametersHomePictureBox.Image = bitmapHomeIcon;
        }

        /// <summary>
        /// Ajoute une entrée au jounral des opérations
        /// </summary>
        /// <param name="strMessage">Message à ajouter dans le journal</param>
        private void addToLogs(string strMessage)
        {
            // Récupère la date actuelle et ajoute une entrée dans le jounral des opérations
            DateTime dateNow = DateTime.Now;
            list_strLogs.Add(Convert.ToString(dateNow) + " - " + strMessage);
        }

        private void folderToSynchFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void folderToSynchFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            string strRegEx = @"(Nouveau dossier)";
            Regex regexNewFolder = new Regex(strRegEx);

            if (regexNewFolder.Match(e.Name).Length == 0)
            {
                createFolder(e.FullPath);
                uploadFile(e.FullPath);
            }
        }

        private void folderToSynchFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            bool boolExsist = false;

            WebRequest postListFolderRequest = createPostRequest(strAuthHeader, strPostListFolder);
            postListFolderRequest.ContentType = "application/json";

            string strPostBody = "{\"path\": \"\"}";
            
            // Ajout du contenu du body de la requête s'il y en a un.
            if (strPostBody.Length > 0)
            {
                using (Stream stream = postListFolderRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strPostBody);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }
            }
            WebResponse postListFolderResponse = postListFolderRequest.GetResponse();
            // Récupération des données reçues et stockage dans un string
            reader = new StreamReader(postListFolderResponse.GetResponseStream());
            strJson = reader.ReadToEnd();
            postListFolderResponse.Close();

            JObject joEntries = JObject.Parse(strJson);
            string strArray = Convert.ToString(joEntries["entries"]);

            JArray jaFoldersFiles = JArray.Parse(strArray);

            int intCommonPath = strFolderPath.Length;
            string strFolderToDeletePath = e.FullPath.Substring(intCommonPath);

            strFolderToDeletePath = strFolderToDeletePath.Replace("\\", "/");

            for (int i = 0; i < jaFoldersFiles.Count; i++)
            {
                if(Convert.ToString(jaFoldersFiles[i]["path_display"]) == strFolderToDeletePath)
                {
                    boolExsist = true;
                }
            }

            if(boolExsist)
            {
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostDelete);
                postRequest.ContentType = "application/json";

                strPostBody = "{\"path\": \"" + strFolderToDeletePath + "\"}";

                // Ajout du contenu du body de la requête s'il y en a un.
                if (strPostBody.Length > 0)
                {
                    using (Stream stream = postRequest.GetRequestStream())
                    {
                        byte[] content = Encoding.UTF8.GetBytes(strPostBody);
                        stream.Write(content, 0, content.Length);

                        stream.Flush();
                        stream.Close();
                    }
                }
                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();
            }
        }

        private void folderToSynchFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            string strRegEx = @"(Nouveau dossier)";
            Regex regexNewFolder = new Regex(strRegEx);

            if (regexNewFolder.Match(e.OldName) != null)
            {
                createFolder(e.FullPath);
            }
        }

        private void createFolder(string strFullPath)
        {
            DirectoryInfo newDirectory = new DirectoryInfo(strFullPath);
        
            bool boolExists = newDirectory.Exists;

            if (boolExists)
            {
                int intCommonPath = strFolderPath.Length;
                string strNewFolderPath = strFullPath.Substring(intCommonPath);

                strNewFolderPath = strNewFolderPath.Replace("\\", "/");

                WebRequest postRequest = createPostRequest(strAuthHeader, strPostCreateFolder);
                postRequest.ContentType = "application/json";

                string strPostBody = "{\"path\": \"" + strNewFolderPath + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strPostBody);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }

                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();
            }
        }

        private void uploadFile(string strFullPath)
        {
            FileInfo newFile = new FileInfo(strFullPath);
            bool boolExists = newFile.Exists;

            if (boolExists)
            {

                int intCommonPath = strFolderPath.Length;
                string strNewFilePath = strFullPath.Substring(intCommonPath);

                strNewFilePath = strNewFilePath.Replace("\\", "/");

                string strPostBody = "{\"path\": \"" + strNewFilePath + "\"}";

                WebRequest postRequest = createPostRequest(strAuthHeader, strPostUpload);
                postRequest.ContentType = "application/octet-stream";
                postRequest.Headers.Add("Dropbox-API-Arg", strPostBody);

                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = File.ReadAllBytes(strFullPath);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }

                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();
            }
        }
    }
}
