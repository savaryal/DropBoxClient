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

        // RegEx
        // Crée le pattern que la RegEx doit chercher, ici "Nouveau"
        private const string STR_REGEX_NEW = @"(Nouveau)";
        // Crée la RegEx avec le pattern 
        Regex regexNew = new Regex(STR_REGEX_NEW);

        private const string STR_REGEX_TMP_HIDDEN = @"(Hidden)";
        private const string STR_REGEX_TMP_NAME = @"(~$)";
        Regex regexTmpHidden = new Regex(STR_REGEX_TMP_HIDDEN);
        Regex regexTmpName = new Regex(STR_REGEX_TMP_NAME);
        
        //// Crée le pattern que la RegEx doit chercher, ici exactement "Nouveau dossier"
        //private const string STR_REGEX_FILE = @"(Nouveau)";
        //// Crée la RegEx avec le pattern 
        //Regex regexNewFile = new Regex(STR_REGEX_FILE);

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

        int intY = 0;

        private List<Label> list_logsLabels = new List<Label>();

        StreamReader reader;

        /// <summary>
        /// Constructeur de la Form
        /// </summary>
        public DropBoxClientForm()
        {
            InitializeComponent();

            // Si le chemin du dossier à synchroniser est mémorisé  
            if (Properties.Settings.Default.strFolderPath != null)
            {
                // Récupère le chemin du dossier
                strFolderPath = Properties.Settings.Default.strFolderPath;
                folderToSynchPathLabel.Text = strFolderPath;

                // Surveille le dossier spécifié
                folderToSynchFileSystemWatcher.Path = strFolderPath;
            }

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

                if (Properties.Settings.Default.strFolderPath != null)
                {
                    synchronizeLocalToDropbox();

                    addToLogs("Synchronisation effectuée avec succès");
                }
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

                if (Properties.Settings.Default.strFolderPath != null)
                {
                    synchronizeLocalToDropbox();
                }

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
        } // END chooseFolderButton_Click()

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

            Label logsTmpLabel = new Label();
            logsTmpLabel.AutoSize = false;
            logsTmpLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            logsTmpLabel.Size = new System.Drawing.Size(475, 100);
            logsTmpLabel.Location = new System.Drawing.Point(0, intY);
            logsTmpLabel.BorderStyle = BorderStyle.FixedSingle;
            logsTmpLabel.Text = Convert.ToString(dateNow) + " - " + strMessage;

            logsPanel.Controls.Add(logsTmpLabel);

            intY += 100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        //private void folderToSynchFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        //{
        //    FileInfo file = new FileInfo(e.FullPath);

        //    if(file.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) && !(regexTmpName.Match(e.Name).Success))
        //    {
        //        // L'évènement Created réagit dès la création du dossier, même s'il n'a pas encore été nommé.
        //        // C'est pour cela que si le dossier est nommé "Nouveau dossier", il ne le crée pas tout de suite,
        //        // sa création est faite dans l'évènement Renamed une fois son nom défini.
        //        // Si le résultat ne correspond pas à "Nouveau dossier"
        //        if (!regexNew.Match(e.Name).Success)
        //        {   // Crée un dossier
        //            createFolder(e.FullPath, strFolderPath);
        //            // Upload le fichier
        //            uploadFile(e.FullPath);
        //        }
        //        synchronizeLocalToDropbox();
        //    }
        //}

        /// <summary>
        /// Détecte la suppression d'un dossier ou fichier et le supprime de Dropbox
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        //private void folderToSynchFileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        //{
        //    // Par défaut, l'objet à supprimer n'existe pas sur Dropbox
        //    bool boolExsist = false;

        //    // Récupère la liste de tous les dossiers et fichiers présents sur Dropbox
        //    JArray jaFoldersFiles = listAllFoldersAndFiles();

        //    // Calcule le nombre de caractères du chemin du dossier synchronisé
        //    int intCommonPath = strFolderPath.Length;
        //    // Enlève le chemin du dossier synchronisé du chemin du dossier ou fichier à supprimer
        //    string strFolderToDeletePath = e.FullPath.Substring(intCommonPath);
        //    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
        //    strFolderToDeletePath = strFolderToDeletePath.Replace("\\", "/");

        //    // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
        //    for (int i = 0; i < jaFoldersFiles.Count; i++)
        //    {   
        //        // Si le chemin du dossier ou fichier sur Dropbox est identique à celui qu'il faut supprimer
        //        if(Convert.ToString(jaFoldersFiles[i]["path_lower"]) == strFolderToDeletePath.ToLower())
        //        {   // Le dossier ou fichier est existant
        //            boolExsist = true;
        //        }
        //    }

        //    // Si le dossier ou fichier est existant
        //    if(boolExsist)
        //    {
        //        // Création de la requête à l'API
        //        WebRequest postRequest = createPostRequest(strAuthHeader, strPostDelete);
        //        postRequest.ContentType = "application/json";

        //        // Chemin du dossier ou fichier à supprimer en format JSON
        //        string strJSONParam = "{\"path\": \""+ strFolderToDeletePath + "\"}";

        //        // Ajout du contenu JSON dans le corps de la requête
        //        if (strJSONParam.Length > 0)
        //        {
        //            using (Stream stream = postRequest.GetRequestStream())
        //            {
        //                byte[] content = Encoding.UTF8.GetBytes(strJSONParam);
        //                stream.Write(content, 0, content.Length);

        //                stream.Flush();
        //                stream.Close();
        //            }
        //        }
        //        // Envoi et fermeture de la requête
        //        WebResponse postResponse = postRequest.GetResponse();
        //        postResponse.Close();

        //        addToLogs(strFolderToDeletePath + " a été supprimé.");
        //    }
        //}

        /// <summary>
        /// Crée un nouveau dossier sur Dropbox
        /// </summary>
        /// <param name="strFullPath">Chemin complet du dossier local</param>
        private void createFolder(string strFullPath, string strParentFolderPath)
        {
            DirectoryInfo newDirectory = new DirectoryInfo(strFullPath);
            
            // Vérifie que le chemin donné est bien un dossier
            bool boolExists = newDirectory.Exists;

            // Si le dossier existe 
            if (boolExists)
            {
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderPath.Length;
                // Enlève le chemin du dossier synchronisé du chemin du dossier à créer
                string strNewFolderPath = strFullPath.Substring(intCommonPath);
                // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
                strNewFolderPath = strNewFolderPath.Replace("\\", "/");

                // Création de la requête à l'API
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostCreateFolder);
                postRequest.ContentType = "application/json";

                // Chemin du dossier ou fichier à supprimer en format JSON
                string strJSONparam = "{\"path\": \"" + strNewFolderPath + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strJSONparam);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }
                // Envoi et fermeture de la requête
                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();

                addToLogs("Le dossier " + strNewFolderPath + " a été créé");
            }
        }

        /// <summary>
        /// Upload un fichier sur Dropbox
        /// </summary>
        /// <param name="strFullPath">Chemin complet du fichier local</param>
        private void uploadFile(string strFullPath)
        {
            FileInfo newFile = new FileInfo(strFullPath);

            // Vérifie que le chemin donné est bien un fichier
            bool boolExists = newFile.Exists;

            // Si le fichier existe et n'est pas un fichier temporaire
            if (boolExists && newFile.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(newFile.Attributes)).Success) && !(regexTmpName.Match(newFile.Name).Success))
            {
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderPath.Length;
                // Enlève le chemin du dossier synchronisé du chemin du fichier à envoyer
                string strNewFilePath = strFullPath.Substring(intCommonPath);
                // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
                strNewFilePath = strNewFilePath.Replace("\\", "/");

                // Chemin du fichier à envoyer en format JSON
                string strJSONparam = "{\"path\": \"" + strNewFilePath + "\", \"mode\": \"overwrite\"}"; 

                // Création de la requête à l'API
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostUpload);
                postRequest.ContentType = "application/octet-stream";
                postRequest.Headers.Add("Dropbox-API-Arg", strJSONparam);

                try
                {
                    // Ajoute tout le contenu du fichier dans le corps de la requête sous forme d'octet
                    using (Stream stream = postRequest.GetRequestStream())
                    {
                        byte[] content = File.ReadAllBytes(strFullPath);
                        stream.Write(content, 0, content.Length);

                        stream.Flush();
                        stream.Close();
                    }
                    // Envoi et fermeture de la requête
                    WebResponse postResponse = postRequest.GetResponse();
                    postResponse.Close();

                    addToLogs("Le fichier " + strFullPath + " a été ajouté à Dropbox");
                }
                catch (System.IO.IOException e)
                {
                    uploadFile(strFullPath);
                }
            }
        }

        /// <summary>
        /// Retourne un tableau JSON contenant la liste de tous les dossiers et fichiers présents sur Dropbox
        /// </summary>
        /// <returns>jaFoldersFiles : Tableau JSON contenant la liste des dossiers et fichiers sur Dropbox</returns>
        private JArray listAllFoldersAndFiles()
        {
            // Création de la requête à l'API
            WebRequest postListFolderRequest = createPostRequest(strAuthHeader, strPostListFolder);
            postListFolderRequest.ContentType = "application/json";

            // Chemin du dossier racine de Dropbox en JSON
            string strJSONparam = "{\"path\": \"\", \"recursive\": true}";

            // Ajout du contenu JSON dans le corps de la requête
            if (strJSONparam.Length > 0)
            {
                using (Stream stream = postListFolderRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strJSONparam);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }
            }
            // Envoi de la requête
            WebResponse postListFolderResponse = postListFolderRequest.GetResponse();
            // Récupération des données reçues et stockage dans un string
            reader = new StreamReader(postListFolderResponse.GetResponseStream());
            strJson = reader.ReadToEnd();
            // Fermeture de la requête
            postListFolderResponse.Close();

            // Création d'un objet JSON avec le string des données reçues
            JObject joEntries = JObject.Parse(strJson);
            // Récupération du tableau dans un string
            string strArray = Convert.ToString(joEntries["entries"]);
            // Création d'un tableau JSON
            JArray jaFoldersFiles = JArray.Parse(strArray);

            return jaFoldersFiles;
        }

        /// <summary>
        /// Détecte quand un fichier ou dossier est renommé
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void folderToSynchFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            FileInfo oldFile = new FileInfo(e.OldFullPath);
            FileInfo file = new FileInfo(e.FullPath);

            // Si le nom d'avant du dossier contenait "Nouveau"
            if (regexNew.Match(e.OldName).Success)
            {   // Crée le dossier avec son chemin actuel
                createFolder(e.FullPath, strFolderPath);
                // Upload le fichier
                uploadFile(e.FullPath);
            }

            else if ((oldFile.Extension == ".tmp" && file.Extension != ".tmp") || (oldFile.Extension == "" && file.Extension != ".tmp"))
            {
                uploadFile(e.FullPath);
            }
            else
            {
                renameFileFolder(e.OldFullPath, e.FullPath);
            }

        }

        private void folderToSynchFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //if (file.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) && !(regexTmpName.Match(e.Name).Success))

            FileInfo file = new FileInfo(e.FullPath);

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    {
                        if (file.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) && !(regexTmpName.Match(e.Name).Success))
                        {
                            // L'évènement Created réagit dès la création du dossier, même s'il n'a pas encore été nommé.
                            // C'est pour cela que si le dossier est nommé "Nouveau dossier", il ne le crée pas tout de suite,
                            // sa création est faite dans l'évènement Renamed une fois son nom défini.
                            // Si le résultat ne correspond pas à "Nouveau dossier"
                            if (!regexNew.Match(e.Name).Success)
                            {   // Crée un dossier
                                createFolder(e.FullPath, strFolderPath);
                                // Upload le fichier
                                uploadFile(e.FullPath);
                                synchronizeLocalToDropbox();
                            }
                        }
                        break;
                    }
                case WatcherChangeTypes.Deleted:
                    {
                        delete(e.FullPath);
                        break;
                    }
                case WatcherChangeTypes.Changed:
                    {
                        uploadFile(e.FullPath);
                        break;
                    }
            }
        }

        private void delete(string strPathToDelete)
        {
            // Par défaut, l'objet à supprimer n'existe pas sur Dropbox
            bool boolExsist = false;

            // Récupère la liste de tous les dossiers et fichiers présents sur Dropbox
            JArray jaFoldersFiles = listAllFoldersAndFiles();

            // Calcule le nombre de caractères du chemin du dossier synchronisé
            int intCommonPath = strFolderPath.Length;
            // Enlève le chemin du dossier synchronisé du chemin du dossier ou fichier à supprimer
            string strToDeletePath = strPathToDelete.Substring(intCommonPath);
            // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
            strToDeletePath = strToDeletePath.Replace("\\", "/");

            // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
            for (int i = 0; i < jaFoldersFiles.Count; i++)
            {
                // Si le chemin du dossier ou fichier sur Dropbox est identique à celui qu'il faut supprimer
                if (Convert.ToString(jaFoldersFiles[i]["path_lower"]) == strToDeletePath.ToLower())
                {   // Le dossier ou fichier est existant
                    boolExsist = true;
                }
            }

            // Si le dossier ou fichier est existant
            if (boolExsist)
            {
                // Création de la requête à l'API
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostDelete);
                postRequest.ContentType = "application/json";

                // Chemin du dossier ou fichier à supprimer en format JSON
                string strJSONParam = "{\"path\": \"" + strToDeletePath + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                if (strJSONParam.Length > 0)
                {
                    using (Stream stream = postRequest.GetRequestStream())
                    {
                        byte[] content = Encoding.UTF8.GetBytes(strJSONParam);
                        stream.Write(content, 0, content.Length);

                        stream.Flush();
                        stream.Close();
                    }
                }
                // Envoi et fermeture de la requête
                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();

                addToLogs(strToDeletePath + " a été supprimé.");
            }
        }

        /// <summary>
        /// Lance la synchronisation des dossiers et fichiers du local au distant
        /// </summary>
        private void synchronizeLocalToDropbox()
        {
            JArray jaAllFoldersAndFiles = listAllFoldersAndFiles();

            // Répertoire synchronisé avec Dropbox
            DirectoryInfo root = new DirectoryInfo(Properties.Settings.Default.strFolderPath);

            synchronizeFile(root, jaAllFoldersAndFiles);
            synchronizeFolder(root, jaAllFoldersAndFiles);
        }

        /// <summary>
        /// Synchronise les dossiers
        /// </summary>
        /// <param name="root">Dossier parent du dossier à synchroniser</param>
        /// <param name="jaAllFoldersAndFiles">Tableau JSON contenant la liste des fichiers et dossiers sur Dropbox</param>
        private void synchronizeFolder(DirectoryInfo root, JArray jaAllFoldersAndFiles)
        {
            foreach (DirectoryInfo directory in root.EnumerateDirectories())
            {
                bool boolExistAlready = false;
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderPath.Length;

                // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
                for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
                {
                    // Enlève le chemin du dossier synchronisé du chemin du fichier à envoyer
                    string strDirectoryPath = directory.FullName.Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
                    strDirectoryPath = strDirectoryPath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]) == strDirectoryPath.ToLower())
                    {   // Le dossier ou fichier est existant
                        boolExistAlready = true;
                    }
                }
                if (!boolExistAlready)
                {
                    createFolder(directory.FullName, strFolderPath);
                    synchronizeFile(directory, jaAllFoldersAndFiles);
                    synchronizeFolder(directory, jaAllFoldersAndFiles);
                }
                else
                {
                    synchronizeFile(directory, jaAllFoldersAndFiles);
                    synchronizeFolder(directory, jaAllFoldersAndFiles);
                }
            }
        }

        /// <summary>
        /// Synchronise les fichiers
        /// </summary>
        /// <param name="root">Dossier parent du dossier à synchroniser</param>
        /// <param name="jaAllFoldersAndFiles">Tableau JSON contenant la liste des fichiers et dossiers sur Dropbox</param>
        private void synchronizeFile(DirectoryInfo root, JArray jaAllFoldersAndFiles)
        {
            // Pour chaque fichier, récupère le texte
            foreach (FileInfo file in root.EnumerateFiles())
            {
                bool boolExistAlready = false;
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderPath.Length;

                // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
                for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
                {
                    // Enlève le chemin du dossier synchronisé du chemin du fichier à envoyer
                    string strFilePath = file.FullName.Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
                    strFilePath = strFilePath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]) == strFilePath.ToLower())
                    {   // Le dossier ou fichier est existant
                        boolExistAlready = true;
                        break;
                    }
                }
                if (!boolExistAlready)
                {
                    uploadFile(file.FullName);
                }
            }
        }

        /// <summary>
        /// Renomme un dossier ou un fichier
        /// </summary>
        /// <param name="strOldName">Ancien chemin du fichier ou dossier</param>
        /// <param name="strNewName">Nouveau chemin</param>
        private void renameFileFolder(string strOldName, string strNewName)
        {
            // Création de la requête à l'API
            WebRequest postRequest = createPostRequest(strAuthHeader, strPostMove);
            postRequest.ContentType = "application/json";

            // Calcule le nombre de caractères du chemin du dossier synchronisé
            int intCommonPath = strFolderPath.Length;

            // Enlève le chemin du dossier synchronisé du chemin du dossier/fichier
            strOldName = strOldName.Substring(intCommonPath);
            strNewName = strNewName.Substring(intCommonPath);

            // Transformer le format du chemin au format utilisé par Dropbox
            strOldName = strOldName.Replace("\\", "/");
            strNewName = strNewName.Replace("\\", "/");

            FileInfo file = new FileInfo(strNewName);
            FileInfo oldFile = new FileInfo(strOldName);
            if(oldFile.Extension != ".tmp" && file.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) && !(regexTmpName.Match(strOldName).Success) && !(regexTmpName.Match(strNewName).Success))
            {
                // Chemin du dossier ou fichier à supprimer en format JSON
                string strJSONparam = "{\"from_path\": \"" + strOldName + "\", \"to_path\": \"" + strNewName + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strJSONparam);
                    stream.Write(content, 0, content.Length);

                    stream.Flush();
                    stream.Close();
                }
                // Envoi et fermeture de la requête
                WebResponse postResponse = postRequest.GetResponse();
                postResponse.Close();

                addToLogs(strOldName + " a été renommé en " + strNewName);
            }
        }
    }
}
