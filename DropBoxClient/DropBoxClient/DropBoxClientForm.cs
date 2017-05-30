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
using Microsoft.Win32;
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
        private const string STR_ERROR_LOGIN_STATUS = "La connexion a échoué. Réessayez.";
        private const string STR_SYNCHRONIZING_STATUS = "Synchronisation en cours ...";
        private const string STR_CHANGING_FOLDER_STATUS = "Changement du dossier à synchroniser...";

        // RegEx
        // Crée le pattern que la RegEx doit chercher, ici "Nouveau"
        private const string STR_REGEX_NEW = @"(Nouveau)";
        // Crée la RegEx avec le pattern 
        Regex regexNew = new Regex(STR_REGEX_NEW);

        // RegEx pour fichier temporaire
        private const string STR_REGEX_TMP_HIDDEN = @"(Hidden)";
        private const string STR_REGEX_TMP_NAME = @"(~$)";
        Regex regexTmpHidden = new Regex(STR_REGEX_TMP_HIDDEN);
        Regex regexTmpName = new Regex(STR_REGEX_TMP_NAME);

        //////////// Icônes ////////////
        private Bitmap bitmapHomeIcon = Properties.Resources.homeIcon;
        private Bitmap bitmapParametersIcon = Properties.Resources.parametersIcon;

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
        private string strPostListFolderContinue = "https://api.dropboxapi.com/2/files/list_folder/continue";
        private string strPostDownload = "https://content.dropboxapi.com/2/files/download";


        //////////// Variables pour l'authentification ////////////
        private string strToken; // Châine de caractère contenant le token
        private byte[] tab_byteToken; // Tableau de byte pour la protection du token
        private string strAuthHeader = ""; // En-tête d'authentification

        // Journal des opérations
        private List<string> list_strLogs = new List<string>();

        // Permet de récupérer le JSON reçu après la requête à l'API
        private string strJson;

        // Chemin du dossier à synchroniser
        private string strFolderToSynchPath;

        // Coordonnée Y des labels dans le journal des opérations
        int intY = 0;

        // Liste des labels du journal des opérations
        private List<Label> list_logsLabels = new List<Label>();

        // Curseur founrit par Dropbox afin de le réutiliser dans /list_folder/continue pour voir ce qui a changé depuis la dernière fois.
        private string strCursor;

        // Lecteur de flux 
        StreamReader reader;

        // http://www.officepourtous.com/extensions-des-fichiers-office-2003-et-anterieures-et-2007-et-ulterieures/
        // Tableau contenant toutes les extensions des fichiers Office
        string[] tab_strOfficeExtension = { ".doc",".docx", ".dot", ".dotx", ".docm", ".dotm", ".xls", ".xlsx", ".xlt", ".xltx",
                                            ".xlsb", "xla", ".xlam", ".xlsm", ".xltm", ".ppt", ".pptx", ".pps", ".ppsx", ".pot",
                                            ".potx", ".ppa", ".ppam", ".pptm", ".ppsm", ".potm", ".pub", ".mdb", ".accdb", ".adp",
                                            ".mde", ".accde", ".adp", ".accdp", ".accdr", ".one", ".onepkg", ".vsd", ".vst", ".mpp", ".mpt"};

        /// <summary>
        /// Constructeur de la Form
        /// </summary>
        public DropBoxClientForm()
        {
            InitializeComponent();

            // Arrête de gérer l'évènement CheckedChanged
            appStartingCheckBox.CheckedChanged -= new EventHandler(appStartingCheckBox_CheckedChanged);

            // Remet la checkbox à son état sauvegardé.
            appStartingCheckBox.Checked = Properties.Settings.Default.boolCheckState;

            // Recommence à gérer l'évènement CheckedChanged
            appStartingCheckBox.CheckedChanged += new EventHandler(appStartingCheckBox_CheckedChanged);

            // Si le chemin du dossier à synchroniser est mémorisé  
            if (Properties.Settings.Default.strFolderPath != null)
            {
                // Récupère le chemin du dossier
                strFolderToSynchPath = Properties.Settings.Default.strFolderPath;
                folderToSynchPathLabel.Text = strFolderToSynchPath;

                // Surveille le dossier spécifié
                folderToSynchFileSystemWatcher.Path = strFolderToSynchPath;
            }

            // Initialise des paramètres de logsSaveFileDialog
            logsSaveFileDialog.Filter = "txt files (*.txt)|*.txt";
            logsSaveFileDialog.RestoreDirectory = true;
            logsSaveFileDialog.DefaultExt = ".txt";
            logsSaveFileDialog.AddExtension = true;

            // Création d'une clé de registre
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // Si la checkbox est cochée
            if (appStartingCheckBox.Checked)
            {   // Ecrit la clé de registre
                registryKey.SetValue("DropBoxClient", Application.ExecutablePath);
            }
        }

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
            // Ouvre le navigateur sur l'url permettant l'autorisation de l'application
            System.Diagnostics.Process.Start(strGetOauth2Authorize += "?client_id=" + STR_APP_KEY + "&response_type=code&force_reauthentication=true");

            // Affiche la textbox servant à récupérer le code fournit à l'utilisateur ainsi que le bouton "Continuer"
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
                topPanel.Visible = true;

                // Si le dossier à synchroniser est spécifié
                if (Properties.Settings.Default.strFolderPath != null)
                {   // Provoque la synchronisation
                    synchronizeLocalToDropbox();
                }

                // Modification du statut
                currentStatusLabel.Text = STR_CONNECTED_STATUS;

                // Commence la surveillance du dossier
                folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
                checkModificationTimer.Start();
            }
            else
            {
                // Modification du statut
                currentStatusLabel.Text = STR_ERROR_LOGIN_STATUS;
                addToLogs("Connexion échouée");
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
            checkModificationTimer.Stop();

            // Retourne à l'interface de connexion
            showLoginInterface();

            // Modification du statut
            currentStatusLabel.Text = STR_WAITING_LOGIN_STATUS;
            addToLogs("Déconnexion.");
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
            // Arrête la surveillance du dossier actuel
            folderToSynchFileSystemWatcher.EnableRaisingEvents = false;
            currentStatusLabel.Text = STR_CHANGING_FOLDER_STATUS;

            // Si l'utilisateur choisit un dossier
            if (toSynchFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Récupère son chemin
                string strFolderToTest = toSynchFolderBrowserDialog.SelectedPath;

                // Crée un fichier pour tester si l'écriture est possible
                string strTestFile = "Test.txt";
                string strFilePath = Path.Combine(strFolderToTest, strTestFile);

                // Par défaut, le dossier est considéré comme OK pour la synchronisation
                bool boolIsFolderOK = true;

                try // Essaie de créer un fichier
                {
                    File.Create(strFilePath).Close();
                }
                catch (Exception)
                {
                    boolIsFolderOK = false;
                }
                try // Essaie de supprimer le fichier
                {
                    File.Delete(strFilePath);
                }
                catch (Exception)
                {
                    boolIsFolderOK = false;
                }

                // Si le dossier est OK pour la synchronisation (Possède le droit NTFS "Modification")
                if (boolIsFolderOK)
                {
                    // Enregistre le chemin du dossier
                    strFolderToSynchPath = toSynchFolderBrowserDialog.SelectedPath;
                    folderToSynchPathLabel.Text = strFolderToSynchPath;
                    Properties.Settings.Default.strFolderPath = strFolderToSynchPath;

                    // Surveille le dossier spécifié
                    folderToSynchFileSystemWatcher.Path = strFolderToSynchPath;
                    addToLogs("Le dossier à synchroniser a été changé en " + strFolderToSynchPath);

                    // Synchronise le nouveau dossier avec Dropbox
                    synchronizeLocalToDropbox();
                }
                else
                {
                    MessageBox.Show("Le dossier n'a pas été modifié", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    strFolderToSynchPath = Properties.Settings.Default.strFolderPath;
                }
            }
            // Recommence à surveiller le dossier
            folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        } // END chooseFolderButton_Click()

        /// <summary>
        /// Sauvegarde le journal des opérations dans un fichier texte à l'emplacement désiré
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void saveLogsButton_Click(object sender, EventArgs e)
        {
            // Demande à l'utilisateur l'emplacememt de sauvegarde du fichier. Si l'utilisateur choisi un emplacement et que c'est bien un fichier texte
            if (logsSaveFileDialog.ShowDialog() == DialogResult.OK && logsSaveFileDialog.FileName.Substring(logsSaveFileDialog.FileName.Length - 3, 3) == "txt")
            {   // Crée et écrit le fichier
                File.WriteAllLines(logsSaveFileDialog.FileName, list_strLogs);
                // Avertit l'utilisateur
                MessageBox.Show(STR_SAVE_BOX_MESSAGE_OK, STR_SAVE_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                addToLogs("Journal des opérations sauvegardés.");
            }
            else
            {
                MessageBox.Show(STR_SAVE_BOX_MESSAGE_KO, STR_SAVE_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

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
            // Ajout de l'en-tête d'authentification 
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
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(strPostOauth2Token 
                                        + "?code=" + Uri.EscapeDataString(strCode) 
                                        + "&grant_type=" + Uri.EscapeDataString("authorization_code") 
                                        + "&client_id=" + Uri.EscapeDataString(STR_APP_KEY) 
                                        + "&client_secret=" + Uri.EscapeDataString(STR_APP_SECRET));
            // Ajout de la méthode, verbe http
            postRequest.Method = "POST";
            // Spécification du type de contenu
            postRequest.ContentType = "application/x-www-form-urlencoded";
            
            try // Essaie d'envoyer la requête
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
                tab_byteToken = Encoding.ASCII.GetBytes(strToken);

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

            try // Essaie d'envoyer la requête
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
        }

        /// <summary>
        /// Récupère le stockage utilisé et le stockage alloué à l'utilisateur
        /// </summary>
        private void getSpaceUsage()
        {
            // Crée la requête permettant d'obtenir les informations liées au stockage
            WebRequest postSpaceUsageRequest = createPostRequest(strAuthHeader, strPostGetSpaceUsage);

            try // Essaie d'envoyer la requête
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
                {   // Convertit en Go
                    strUsedSpace = (dblUsedSpace / 1000000000).ToString("0.## Go");
                }
                // Convertit l'espace alloué en Go
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
            topPanel.Visible = false;
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

            // Crée un label afin de l'ajouter dans la liste de label
            Label logsTmpLabel = new Label();
            logsTmpLabel.AutoSize = false;
            logsTmpLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            logsTmpLabel.Size = new System.Drawing.Size(475, 100);
            logsTmpLabel.Location = new System.Drawing.Point(0, intY);
            logsTmpLabel.BorderStyle = BorderStyle.FixedSingle;
            logsTmpLabel.Text = Convert.ToString(dateNow) + " - " + strMessage;
            // Ajoute le label au panel contenant le journal des opérations
            logsPanel.Controls.Add(logsTmpLabel);
            // Auguemente la coordonnée Y du prochain label
            intY += 100;
        }

        /// <summary>
        /// Crée un nouveau dossier sur Dropbox
        /// </summary>
        /// <param name="strFullPath">Chemin complet du dossier à créer</param>
        /// <param name="strParentFolderPath">Chemin complet du dossier parent</param>
        private void createFolder(string strFullPath, string strParentFolderPath)
        {
            DirectoryInfo newDirectory = new DirectoryInfo(strFullPath);
            
            // Vérifie que le chemin donné est bien un dossier
            bool boolExists = newDirectory.Exists;

            // Si le dossier existe 
            if (boolExists)
            {
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderToSynchPath.Length;
                // Enlève le chemin du dossier synchronisé du chemin du dossier à créer
                string strNewFolderPath = strFullPath.Substring(intCommonPath);
                // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format Dropbox
                strNewFolderPath = strNewFolderPath.Replace("\\", "/");

                // Création de la requête à l'API
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostCreateFolder);
                postRequest.ContentType = "application/json";

                // Chemin du dossier à créer en format JSON
                string strJSONparam = "{\"path\": \"" + strNewFolderPath + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strJSONparam);
                    stream.Write(content, 0, content.Length);
                    stream.Flush();
                    stream.Close();
                }
                try // Essaie d'envoyer la requête
                {
                    // Envoi et fermeture de la requête
                    WebResponse postResponse = postRequest.GetResponse();
                    postResponse.Close();
                    addToLogs("Le dossier " + strNewFolderPath + " a été créé");
                }
                catch (WebException WebE)
                {
                    // Récupère le message d'erreur et l'affiche dans une MessageBox
                    string strErreur = Convert.ToString(WebE.Message);
                    MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    addToLogs("Erreur " + strErreur);
                }
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
                int intCommonPath = strFolderToSynchPath.Length;
                // Enlève le chemin du dossier synchronisé du chemin du fichier à envoyer
                string strNewFilePath = strFullPath.Substring(intCommonPath);
                // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format Dropbox
                strNewFilePath = strNewFilePath.Replace("\\", "/");

                // Chemin du fichier à envoyer en format JSON, si le fichier est existant, le remplace
                string strJSONparam = "{\"path\": \"" + strNewFilePath + "\", \"mode\": \"overwrite\"}"; 

                // Création de la requête à l'API
                WebRequest postRequest = createPostRequest(strAuthHeader, strPostUpload);
                // Type du contenu de la requête
                postRequest.ContentType = "application/octet-stream";
                // En-tête contenant les paramètres en JSON
                postRequest.Headers.Add("Dropbox-API-Arg", strJSONparam);

                // Par défaut, le fichier à envoyer n'est pas un fichier Office
                bool boolOffice = false;

                // Parcourt le tableau contenant toutes les extensions Office
                for(int i = 0; i < tab_strOfficeExtension.Length; i++)
                {
                    // Si l'extension du fichier à envoyer correspond à l'extension du tableau
                    if(newFile.Extension == tab_strOfficeExtension[i])
                    {   // C'est un fichier Office
                        boolOffice = true;
                        break;
                    }
                }

                // Si c'est un fichier Office
                if(boolOffice)
                {
                    try // Tente de créer une copie de ce fichier
                    {
                        // Copie le fichier au même emplacement en ajoutant "Copie" dans son nom
                        File.Copy(strFullPath, strFullPath + "Copie" + newFile.Extension);
                        strFullPath = strFullPath + "Copie" + newFile.Extension;
                    }
                    catch {}
                }

                try // Essaie d'ajouter le contenu du fichier dans la requête et de l'envoyer
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
                    addToLogs("Le fichier " + strNewFilePath + " a été copié sur Dropbox");
                }
                catch (IOException)
                {
                    uploadFile(strFullPath);
                }
                catch (WebException)
                {
                    uploadFile(strFullPath);
                }
                // Si c'est un fichier Office
                if (boolOffice)
                {
                    // Supprime la copie du fichier
                    File.Delete(strFullPath);
                }
            } // END if (boolExists && newFile.Extension...
        } // END uploadFile()

        /// <summary>
        /// Retourne un tableau JSON contenant la liste de tous les dossiers et fichiers présents sur Dropbox
        /// </summary>
        /// <returns>jaFoldersFiles : Tableau JSON contenant la liste des dossiers et fichiers sur Dropbox</returns>
        private JArray listAllFoldersAndFiles()
        {
            // Création de la requête à l'API
            WebRequest postListFolderRequest = createPostRequest(strAuthHeader, strPostListFolder);
            // Type de contenu
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
            try
            {
                // Envoi de la requête
                WebResponse postListFolderResponse = postListFolderRequest.GetResponse();
                // Récupération des données reçues et stockage dans un string
                reader = new StreamReader(postListFolderResponse.GetResponseStream());
                strJson = reader.ReadToEnd();
                // Fermeture de la requête
                postListFolderResponse.Close();

                // Création d'un objet JSON avec le string des données reçues
                JObject joEntries = JObject.Parse(strJson);
                // Récupération du curseur
                strCursor = Convert.ToString(joEntries["cursor"]);

                // Récupération du tableau dans un string
                string strArray = Convert.ToString(joEntries["entries"]);
                // Création d'un tableau JSON
                JArray jaFoldersFiles = JArray.Parse(strArray);
                return jaFoldersFiles;
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addToLogs("Erreur " + strErreur);
                return null;
            }
        } // END listAllFoldersAndFiles()

        /// <summary>
        /// Détecte quand un fichier ou dossier est renommé
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void folderToSynchFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            // Change le statut de l'application
            currentStatusLabel.Text = STR_SYNCHRONIZING_STATUS;

            // Récupère les informations des deux chemins (ancien et nouveau)
            FileInfo oldFile = new FileInfo(e.OldFullPath);
            FileInfo file = new FileInfo(e.FullPath);

            // Si l'ancien nom contenait "Nouveau"
            if (regexNew.Match(e.OldName).Success)
            {   // Crée le dossier avec son chemin actuel
                createFolder(e.FullPath, strFolderToSynchPath);
                // Upload le fichier
                uploadFile(e.FullPath);
            }
            // Si c'est un fichier .tmp ou sans extension qui est renommé en fichier ayant une extension autre que .tmp
            else if ((oldFile.Extension == ".tmp" && file.Extension != ".tmp") || (oldFile.Extension == "" && file.Extension != ".tmp" && oldFile.Exists))
            {   // Renvoie le fichier avec l'extension normale
                uploadFile(e.FullPath);
            }
            else
            {   // Renomme le fichier ou dossier
                renameFileFolder(e.OldFullPath, e.FullPath);
            }
            // Change le statut de l'application
            currentStatusLabel.Text = STR_CONNECTED_STATUS;

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
            int intCommonPath = strFolderToSynchPath.Length;

            // Enlève le chemin du dossier synchronisé du chemin du dossier/fichier
            strOldName = strOldName.Substring(intCommonPath);
            strNewName = strNewName.Substring(intCommonPath);

            // Transforme le format du chemin au format utilisé par Dropbox
            strOldName = strOldName.Replace("\\", "/");
            strNewName = strNewName.Replace("\\", "/");

            // Récupère les informations des deux chemins (ancien et nouveau)
            FileInfo file = new FileInfo(strNewName);
            FileInfo oldFile = new FileInfo(strOldName);

            // Si ce n'est pas un fichier temporaire
            if (oldFile.Extension != "" && oldFile.Extension != ".tmp" && file.Extension != ".tmp" 
                && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) 
                && !(regexTmpName.Match(strOldName).Success) && !(regexTmpName.Match(strNewName).Success))
            {
                // Ancien et nouveau chemin du dossier ou fichier à renommer
                string strJSONparam = "{\"from_path\": \"" + strOldName + "\", \"to_path\": \"" + strNewName + "\"}";

                // Ajout du contenu JSON dans le corps de la requête
                using (Stream stream = postRequest.GetRequestStream())
                {
                    byte[] content = Encoding.UTF8.GetBytes(strJSONparam);
                    stream.Write(content, 0, content.Length);
                    stream.Flush();
                    stream.Close();
                }
                try // Essaie d'envoyer la requête
                {
                    // Envoi et fermeture de la requête
                    WebResponse postResponse = postRequest.GetResponse();
                    postResponse.Close();
                    addToLogs(strOldName + " a été renommé en " + strNewName);
                }
                catch (WebException WebE)
                {
                    // Récupère le message d'erreur et l'affiche dans une MessageBox
                    string strErreur = Convert.ToString(WebE.Message);
                    MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    addToLogs("Erreur " + strErreur);
                }
            }
        } // END renameFileFolder()

        /// <summary>
        /// Repère les évènements "Created", "Deleted" et "Changed", appelle les fonctions correspondantes aux évènements
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void folderToSynchFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Change le statut de l'application
            currentStatusLabel.Text = STR_SYNCHRONIZING_STATUS;
            // Récupère les informations du fichier
            FileInfo file = new FileInfo(e.FullPath);

            // Selon le type de changement
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    {
                        // Ignore les fichiers temporaires
                        if (file.Extension != ".tmp" && !(regexTmpHidden.Match(Convert.ToString(file.Attributes)).Success) && !(regexTmpName.Match(e.Name).Success))
                        {
                            // L'évènement Created réagit dès la création du dossier/fichier, même s'il n'a pas encore été nommé.
                            // C'est pour cela que si le dossier contien "Nouveau", il ne le crée pas tout de suite,
                            // sa création est faite dans l'évènement Renamed une fois son nom défini.
                            // Si le résultat ne correspond pas à "Nouveau"
                            if (!regexNew.Match(e.Name).Success)
                            {   // Crée un dossier
                                createFolder(e.FullPath, strFolderToSynchPath);
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
            // Change le statut de l'application
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        }

        /// <summary>
        /// Supprime de Dropbox un objet supprimé localement
        /// </summary>
        /// <param name="strPathToDelete">Chemin de l'élément à supprimer</param>
        private void delete(string strPathToDelete)
        {
            // Par défaut, l'objet à supprimer n'existe pas sur Dropbox
            bool boolExsist = false;

            // Récupère la liste de tous les dossiers et fichiers présents sur Dropbox
            JArray jaFoldersFiles = listAllFoldersAndFiles();

            // Calcule le nombre de caractères du chemin du dossier synchronisé
            int intCommonPath = strFolderToSynchPath.Length;
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
                // Type du contenu
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
                try // Essaie d'envoyer la requête
                {
                    // Envoi et fermeture de la requête
                    WebResponse postResponse = postRequest.GetResponse();
                    postResponse.Close();
                    addToLogs(strToDeletePath + " a été supprimé.");
                }
                catch (WebException WebE)
                {
                    // Récupère le message d'erreur et l'affiche dans une MessageBox
                    string strErreur = Convert.ToString(WebE.Message);
                    MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    addToLogs("Erreur " + strErreur);
                }
            }
        } // END delete()

        /// <summary>
        /// Lance la synchronisation des dossiers et fichiers
        /// </summary>
        private void synchronizeLocalToDropbox()
        {
            // S'il y a un dossier synchronisé
            if (Properties.Settings.Default.strFolderPath != "")
            {
                // Change le statut de l'application
                currentStatusLabel.Text = STR_SYNCHRONIZING_STATUS;

                // Arrête le timer 
                checkModificationTimer.Stop();
               // Arrête la surveillance du dossier
                folderToSynchFileSystemWatcher.EnableRaisingEvents = false;
                
                // Récupère la liste des fichiers et dossiers sur Dropbox
                JArray jaAllFoldersAndFiles = listAllFoldersAndFiles();

                // Répertoire synchronisé avec Dropbox
                DirectoryInfo root = new DirectoryInfo(Properties.Settings.Default.strFolderPath);

                synchronizeFile(root, jaAllFoldersAndFiles);
                synchronizeFolder(root, jaAllFoldersAndFiles);
                createLocalFolder(root, jaAllFoldersAndFiles);
                createLocalFile(jaAllFoldersAndFiles);

                // Recommence à observer le dossier
                folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
                // Réactive le timer
                checkModificationTimer.Start();
            }
            // Change le statut de l'application
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        }

        /// <summary>
        /// Synchronise les dossiers
        /// </summary>
        /// <param name="root">Dossier parent du dossier à synchroniser</param>
        /// <param name="jaAllFoldersAndFiles">Tableau JSON contenant la liste des fichiers et dossiers sur Dropbox</param>
        private void synchronizeFolder(DirectoryInfo root, JArray jaAllFoldersAndFiles)
        {
            // Parcourt tous les répertoires présents dans le répertoire donné
            foreach (DirectoryInfo directory in root.EnumerateDirectories())
            {   // Par défaut le dossier n'existe pas sur Dropbox
                bool boolExistAlready = false;
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderToSynchPath.Length;

                // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
                for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
                {
                    // Enlève le chemin du dossier synchronisé
                    string strDirectoryPath = directory.FullName.Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format Dropbox
                    strDirectoryPath = strDirectoryPath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]) == strDirectoryPath.ToLower())
                    {   // Le dossier est existant
                        boolExistAlready = true;
                    }
                }
                // Si le dossier n'existe pas
                if (!boolExistAlready)
                {
                    // Crée le dossier et continue la synchronisation de manière récursive
                    createFolder(directory.FullName, strFolderToSynchPath);
                    synchronizeFile(directory, jaAllFoldersAndFiles);
                    synchronizeFolder(directory, jaAllFoldersAndFiles);
                }
                else
                {
                    // Continue la synchronisation de manière récursive
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
            // Parcourt tous les fichiers présents dans le dossier
            foreach (FileInfo file in root.EnumerateFiles())
            {
                // Par défaut le fichier n'existe pas sur Dropbox
                bool boolExistAlready = false;
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderToSynchPath.Length;

                // Parcourt la liste de tous les dossiers et fichiers présents sur Dropbox
                for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
                {
                    // Enlève le chemin du dossier synchronisé
                    string strFilePath = file.FullName.Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format Dropbox
                    strFilePath = strFilePath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]) == strFilePath.ToLower())
                    {   // Le fichier est existant
                        boolExistAlready = true;
                        break;
                    }
                }
                // Si le fichier n'existe pas
                if (!boolExistAlready)
                {  
                    uploadFile(file.FullName);
                }
            }
        }

        /// <summary>
        /// Crée localement les répertoires présent sur Dropbox mais pas localement
        /// </summary>
        /// <param name="root">Dossier parent</param>
        /// <param name="jaAllFoldersAndFiles">Tableau contenant tous les fichiers et dossiers distants</param>
        private void createLocalFolder(DirectoryInfo root, JArray jaAllFoldersAndFiles)
        {
            // Parcourt la liste des dossiers et fichiers sur Dropbox
            for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
            {
                // Par défaut le dossier n'existe pas localement
                bool boolExistAlready = false;
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderToSynchPath.Length;
                // Chemin du dossier ou fichier sur Dropbox
                string strDropboxDirectory = Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]);
                // Tableau contenant tous les chemins des dossiers locaux
                string[] tab_strAllDirectories = listAllLocalDirectories();

                // Parcourt la liste de tous les dossiers et fichiers présents localement
                for (int z = 0; z < tab_strAllDirectories.Length; z++)
                {
                    // Enlève le chemin du dossier synchronisé 
                    string strFilePath = tab_strAllDirectories[z].Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format de ceux de Dropbox
                    strFilePath = strFilePath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (strDropboxDirectory == strFilePath.ToLower())
                    {   // Le dossier ou fichier est existant
                        boolExistAlready = true;
                        break;
                    }
                }
                // Si le dossier n'existe pas
                if (!boolExistAlready)
                {
                    // Crée un nouveau dossier localement
                    string strNewFolderPath = Convert.ToString(jaAllFoldersAndFiles[i]["path_display"]);
                    strNewFolderPath = strNewFolderPath.Replace("/", "\\");
                    DirectoryInfo newDirectory = new DirectoryInfo(strFolderToSynchPath + strNewFolderPath);
                    if (Convert.ToString(jaAllFoldersAndFiles[i][".tag"]) == "folder")
                    {
                        newDirectory.Create();
                        addToLogs("Le dossier " + strNewFolderPath + " a été créé localement.");
                    }
                }
            }
        }

        /// <summary>
        /// Obtient le chemin de tous les fichiers se trouvant dans le dossier synchronisé, ainsi que les sous-dossiers.
        /// https://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
        /// </summary>
        /// <returns>Tableau contenant tous les chemins des fichiers présents dans le dossier et ses sous-dossiers</returns>
        private string[] listAllLocalFiles()
        {
            string[] tab_strAllFiles = Directory.GetFiles(strFolderToSynchPath, "*", SearchOption.AllDirectories);
            return tab_strAllFiles;
        }

        /// <summary>
        /// Obtient le chemin de tous les dossiers et sous-dossiers se trouvant dans le dossier synchronisé
        /// </summary>
        /// <returns>Tableau contenant tous les chemins des dossiers et sous-dossiers présents dans le dossier synchronisé</returns>
        private string[] listAllLocalDirectories()
        {
            string[] tab_strAllDirectories = Directory.GetDirectories(strFolderToSynchPath, "*", SearchOption.AllDirectories);
            return tab_strAllDirectories;
        }

        /// <summary>
        /// Recherche les fichiers qui ne sont pas présent localement et appelle une fonction de téléchargement
        /// </summary>
        /// <param name="jaAllFoldersAndFiles">Tableau contenant les fichiers et dossiers de Dropbox</param>
        private void createLocalFile(JArray jaAllFoldersAndFiles)
        {
            // Parcourt la liste des fichiers et dossiers sur Dropbox
            for (int i = 0; i < jaAllFoldersAndFiles.Count; i++)
            {
                // Par défaut le fichier n'existe pas et n'a pas été modifié
                bool boolExistAlready = false;
                bool boolFileModified = false;
                
                // Calcule le nombre de caractères du chemin du dossier synchronisé
                int intCommonPath = strFolderToSynchPath.Length;
                // Chemin du fichier ou dossier sur Dropbox 
                string strDropboxFile = Convert.ToString(jaAllFoldersAndFiles[i]["path_lower"]);
                // Tableau contenant tous les chemins des fichiers locaux
                string[] tab_strAllFiles = listAllLocalFiles();

                // Parcourt la liste de tous les fichiers locaux
                for(int z = 0; z < tab_strAllFiles.Length; z++)
                {
                    // Enlève le chemin du dossier synchronisé
                    string strFilePath = tab_strAllFiles[z].Substring(intCommonPath);
                    // Remplace les "\\" du chemin en "/" afin d'avoir le chemin au format Dropbox
                    strFilePath = strFilePath.Replace("\\", "/");

                    // Si le chemin du dossier ou fichier sur Dropbox est identique
                    if (strDropboxFile == strFilePath.ToLower())
                    {   // Le fichier est existant
                        boolExistAlready = true;
    
                        // Obtient la date de modification du fichier sur Dropbox
                        string strDateDropbox = Convert.ToString(jaAllFoldersAndFiles[i]["server_modified"]);
                        DateTime dateLastModifiedDropbox = DateTime.Parse(strDateDropbox);
                        dateLastModifiedDropbox = dateLastModifiedDropbox.AddHours(2);

                        // Obtient la date de modification locale du fichier
                        FileInfo file = new FileInfo(tab_strAllFiles[z]);
                        DateTime dateLastModifiedLocal = file.LastWriteTime;

                        // Enlève les secondes, millisecondes et ticks pour comparer les dates à la minute
                        dateLastModifiedLocal = dateLastModifiedLocal.AddSeconds(-dateLastModifiedLocal.Second);
                        dateLastModifiedDropbox = dateLastModifiedDropbox.AddSeconds(-dateLastModifiedDropbox.Second);
                        dateLastModifiedLocal = dateLastModifiedLocal.AddMilliseconds(-dateLastModifiedLocal.Millisecond);
                        dateLastModifiedDropbox = dateLastModifiedDropbox.AddMilliseconds(-dateLastModifiedDropbox.Millisecond);
                        dateLastModifiedLocal = dateLastModifiedLocal.AddTicks(-dateLastModifiedLocal.Ticks);
                        dateLastModifiedDropbox = dateLastModifiedDropbox.AddTicks(-dateLastModifiedDropbox.Ticks);

                        // Si le fichier sur Dropbox a été modifié le plus récemment
                        if(dateLastModifiedDropbox > dateLastModifiedLocal)
                        {   //  Le fichier a été modifié
                            boolFileModified = true;
                        }
                        else if (dateLastModifiedDropbox < dateLastModifiedLocal)
                        {   // Envoie le fichier
                            uploadFile(tab_strAllFiles[z]);
                        }
                        break;
                    }
                }
                // Si le fichier n'existe pas en local ou qu'il a été modifié plus récemment sur Dropbox
                if (!boolExistAlready || boolFileModified)
                {
                    string strFilePath = strFolderToSynchPath + Convert.ToString(jaAllFoldersAndFiles[i]["path_display"]);
                    strFilePath = strFilePath.Replace("/", "\\");
                    FileInfo newFile = new FileInfo(strFilePath);
                    if (Convert.ToString(jaAllFoldersAndFiles[i][".tag"]) == "file")
                    {   // Télécharge le fichier
                        downloadFile(Convert.ToString(jaAllFoldersAndFiles[i]["path_display"]), strFilePath);
                    }
                }
            }
        } // createLocalFile ()

        /// <summary>
        /// Télécharge et crée localement un fichier de Dropbox
        /// </summary>
        /// <param name="strFilePath">Chemin du fichier sur Dropbox</param>
        /// <param name="strLocalPath">Chemin local du fichier</param>
        private void downloadFile(string strFilePath, string strLocalPath)
        {
            // Chemin du fichier à télécharger en format JSON
            string strJSONparam = "{\"path\": \"" + strFilePath + "\"}";

            // Création de la requête à l'API
            WebRequest postRequest = createPostRequest(strAuthHeader, strPostDownload);
            // En-tête contenant les paramètres en JSON
            postRequest.Headers.Add("Dropbox-API-Arg", strJSONparam);

            try // Essaie d'envoyer la requête
            {
                // Envoi de la requête
                WebResponse postResponse = postRequest.GetResponse();

                // https://stackoverflow.com/questions/137285/what-is-the-best-way-to-read-getresponsestream
                using (Stream responseStream = postResponse.GetResponseStream())
                {
                    using (FileStream localFileStream = new FileStream(strLocalPath, FileMode.Create))
                    {
                        byte[] tab_byteBuffer = new byte[4096];
                        long longTotalBytesRead = 0;
                        int intBytesRead;

                        while ((intBytesRead = responseStream.Read(tab_byteBuffer, 0, tab_byteBuffer.Length)) > 0)
                        {
                            longTotalBytesRead += intBytesRead;
                            localFileStream.Write(tab_byteBuffer, 0, intBytesRead);
                        }
                    }
                }
                postResponse.Close();
                addToLogs("Le fichier " + strFilePath + " a été copié localement");
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addToLogs("Erreur " + strErreur);
            }
        }

        /// <summary>
        /// Déclenche la synchronisation à l'affichage de l'application si un jeton est en mémoire
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void DropBoxClientForm_Shown(object sender, EventArgs e)
        {
            // Si un token est sauvegardé dans les paramètres d'application
            if (Properties.Settings.Default.tab_byteProtectedToken != null)
            {
                // Décode le tableau byte protégé en tableau byte contenant le token
                tab_byteToken = ProtectedData.Unprotect(Properties.Settings.Default.tab_byteProtectedToken, null, DataProtectionScope.CurrentUser);
                // Convertit le token en string
                strToken = Encoding.ASCII.GetString(tab_byteToken);
                addToLogs("Connexion effectuée avec succès");

                getDisplayName();
                getSpaceUsage();
                showMainInterface();

                // Si un dossier à synchroniser a été choisi
                if(Properties.Settings.Default.strFolderPath != null)
                {   // Effectue une synchronisation
                    synchronizeLocalToDropbox();
                    addToLogs("Synchronisation effectuée avec succès");
                }
                // Modification du statut
                currentStatusLabel.Text = STR_CONNECTED_STATUS;
                // Démarre le timer
                checkModificationTimer.Start();
            }
            else
            {
                checkModificationTimer.Stop();
                showLoginInterface();
            }
        }

        /// <summary>
        /// Se déclenche toutes les 10 secondes, envoie une requête permettant de voir
        /// ce qui a changé sur Dropbox depuis la dernière recherche
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void checkModificationTimer_Tick(object sender, EventArgs e)
        {   // Change le statut de l'application
            currentStatusLabel.Text = STR_SYNCHRONIZING_STATUS;

            // Création de la requête à l'API
            WebRequest postListFolderRequest = createPostRequest(strAuthHeader, strPostListFolderContinue);
            postListFolderRequest.ContentType = "application/json";

            // Curseur obtenu lors de la recherche précédente
            string strJSONparam = "{\"cursor\": \"" + strCursor + "\"}";

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
            try // Essaie d'envoyer la requête
            {
                // Envoi de la requête
                WebResponse postListFolderResponse = postListFolderRequest.GetResponse();
                // Récupération des données reçues et stockage dans un string
                reader = new StreamReader(postListFolderResponse.GetResponseStream());
                strJson = reader.ReadToEnd();
                // Fermeture de la requête
                postListFolderResponse.Close();

                // Création d'un objet JSON avec le string des données reçues
                JObject joEntries = JObject.Parse(strJson);
                // Récupération du curseur
                strCursor = Convert.ToString(joEntries["cursor"]);

                // Récupération du tableau 
                string strArray = Convert.ToString(joEntries["entries"]);
                // Création d'un tableau JSON
                JArray jaFoldersFiles = JArray.Parse(strArray);
                // Arrêt de la surveillance du dossier
                folderToSynchFileSystemWatcher.EnableRaisingEvents = false;

                // Si le tableau n'est pas vide
                if (strArray != "[]")
                {   // Parcourt le tableau
                    for (int i = 0; i < jaFoldersFiles.Count; i++)
                    {
                        // Si c'est un fichier
                        if (Convert.ToString(jaFoldersFiles[i][".tag"]) == "file")
                        {
                            string strFilePath = strFolderToSynchPath + Convert.ToString(jaFoldersFiles[i]["path_display"]);
                            strFilePath = strFilePath.Replace("/", "\\");
                            // Télécharge le fichier
                            downloadFile(Convert.ToString(jaFoldersFiles[i]["path_display"]), strFilePath);
                        }
                        // Si c'est un dossier 
                        else if (Convert.ToString(jaFoldersFiles[i][".tag"]) == "folder")
                        {
                            // Crée le dossier localement
                            string strNewFolderPath = Convert.ToString(jaFoldersFiles[i]["path_display"]);
                            strNewFolderPath = strNewFolderPath.Replace("/", "\\");
                            DirectoryInfo newDirectory = new DirectoryInfo(strFolderToSynchPath + strNewFolderPath);
                            newDirectory.Create();
                        }
                        // Si c'est un élément qui a été supprimé
                        else if (Convert.ToString(jaFoldersFiles[i][".tag"]) == "deleted")
                        {
                            string strToDeletePath = strFolderToSynchPath + Convert.ToString(jaFoldersFiles[i]["path_display"]);
                            strToDeletePath = strToDeletePath.Replace("/", "\\");

                            DirectoryInfo directory = new DirectoryInfo(strToDeletePath);
                            FileInfo file = new FileInfo(strToDeletePath);
                            // Supprime le dossier ou le fichier
                            if (directory.Exists)
                            {
                                Directory.Delete(strToDeletePath);
                            }
                            if (file.Exists)
                            {
                                File.Delete(strToDeletePath);
                            }
                        }
                    }
                }
            }
            catch (WebException WebE)
            {
                // Récupère le message d'erreur et l'affiche dans une MessageBox
                string strErreur = Convert.ToString(WebE.Message);
                MessageBox.Show(strErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addToLogs("Erreur " + strErreur);
            }
            // Recommence la surveillance du dossier
            folderToSynchFileSystemWatcher.EnableRaisingEvents = true;
            // Change le statut
            currentStatusLabel.Text = STR_CONNECTED_STATUS;
        } // checkModificationTimer_Tick ()


        /// <summary>
        /// Au changement de l'état de la checkbox, ajoute ou efface la clé de registre permettant le démarrage avec la session
        /// </summary>
        /// <param name="sender">Contrôle provoquant l'événement</param>
        /// <param name="e">Données liées à l'événement</param>
        private void appStartingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Création de la clé de registre
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // Si la checkbox est cochée
            if (appStartingCheckBox.Checked)
            {   // Ajoute la clé de registre
                registryKey.SetValue("DropBoxClient", Application.ExecutablePath);
            }
            else
            {   // Efface la clé de registre
                registryKey.DeleteValue("DropBoxClient");
            }
            // Sauvegarde l'état de la chekbox
            Properties.Settings.Default.boolCheckState = appStartingCheckBox.Checked;
        }
    }
}
