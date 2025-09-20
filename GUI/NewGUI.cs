// DemoFile Parser
using DemoFile;
using DemoFile.Game.Cs;
// WinForms Framework ReaLTaiizor
using ReaLTaiizor.Colors;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Enum.Material;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using ReaLTaiizor.Util;
using SmallDemoManager.AudioExtract;
// My Class
using SmallDemoManager.HelperClass;
using SmallDemoManager.UtilClass;
// System
using System;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Security.Policy;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using static CDataGCCStrike15_v2_TournamentMatchDraft.Types;
using static CMsgGCCStrike15_v2_Account_RequestCoPlays.Types;

namespace SmallDemoManager.GUI
{
    /// <summary>
    /// Main window of the application. Controls the UI, demo parsing, and bitfield calculation.
    /// </summary>
    public partial class NewGUI : MaterialForm
    {
        // =====================================================
        // ====================== FIELDS =======================
        // =====================================================

        // Parser and player snapshots
        private CsDemoParser? _demo = null;
        private PlayerSnapshot[]? _snapshot = null;

        // Bitfields for voice indices
        private int[] _teamA_Array = new int[5];
        private int[] _teamB_Array = new int[5];

        // Tooltip for copy button
        private readonly ToolTip _copyToolTip = new ToolTip();

        // Recursion protection for "Select All"
        private bool _isSyncingSelectAll = false;
        private bool _isUpdatingCheckboxes = false;

        // Paths for demo and audio folders
        private string? _csDemoFolderPath = null;
        private string? _audioFolderPath = null;

        // Hashes for file comparison
        private byte[]? _sourceHash = null;
        private byte[]? _destinationHash = null;

        // Static links to player profile pages
        private static string _steamProfileLink = "http://steamcommunity.com/profiles/";
        private static string _cswatchProfileLink = "https://cswatch.in/player/";
        private static string _csStatsProfileLink = "https://csstats.gg/player/";
        private static string _leetifyProfileLink = "https://leetify.com/app/profile/";

        // Static links to other pages
        private const string _KOFIURL = "https://ko-fi.com/pythaeus";
        private const string _GITHUBURL = "https://github.com/pythaeusone";

        // Other Globals
        private string _mapName = "no Mapname!";
        private string _duration = "00:00:00";
        private string _hostName = "No Hostname";
        private int _teamAScore = 0;
        private int _teamBScore = 0;
        private bool _voicePlayerOpen = false;
        private List<AudioEntry> _audioEntries;
        private float _fontScaleFactor = 1f;
        private const string _CHECKUPDATEKEY = "CheckUpdate";
        private const string _THEMEMODEKEY = "IsDarkMode";
        private string _selectedPlayerVoicePlayer = "NoPlayerName";
        private bool _readDemoOnStartUp = false;

        // GUI version
        private const string _GUIVERSIONNR = GlobalVersionInfo.GUI_VERSION;

        // Material stuff
        private MaterialSkinManager _materialSkinManager;
        private int _colorSchemeIndex;
        public Color SnackBarBackColor { get; private set; }
        public Color SnackBarForeColor { get; private set; }

        // =====================================================
        // ================= CONSTRUCTOR & INIT ================
        // =====================================================

        /// <summary>
        /// Initializes the form, loads theme, registers handlers, and populates initial UI content.
        /// </summary>
        public NewGUI()
        {
            InitializeComponent();

            CheckSettings();
            FirstAppStart();
            LoadListViewSettings();
            InitializeEventHandlers();

            // Load Patchnotes if present
            //if (File.Exists("PatchNotes.rtf"))
            //    RTB_PatchNotes.LoadFile("PatchNotes.rtf", RichTextBoxStreamType.RichText);
            var bytes = Properties.Resources.PatchNotes; // <-- Name so wie in Resources.resx
            using var ms = new MemoryStream(bytes);
            RTB_PatchNotes.LoadFile(ms, RichTextBoxStreamType.RichText);

            LoadSavedAudioFiles();
        }

        /// <summary>
        /// Handles the form's Shown event to perform startup initialization.
        /// If demo mode is enabled on startup and a valid demo folder path exists,
        /// prepares the application for startup using the specified demo folder.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">An EventArgs object that contains no event data</param>
        private void NewGUI_Shown(object sender, EventArgs e)
        {
            if (_readDemoOnStartUp && _csDemoFolderPath != null)
            {
                PrepareStart(_csDemoFolderPath);
            }
        }

        /// <summary>
        /// One-time initialization for first app start (window title, tab locks, etc.).
        /// </summary>
        private void FirstAppStart()
        {
            // Small fix: enable Drag&Drop for the textbox and its inner control.
            TB_FilePath.AllowDrop = true;
            TB_FilePath.Controls[0].AllowDrop = true;

            //this.Text = "Demo Manager by Pythaeus v." + _GUIVERSIONNR;
            //this.Text = "Small Demo Manager v." + _GUIVERSIONNR;
            LBL_AboutVersion.Text = "v." + _GUIVERSIONNR;

            BTN_ExtractAudio.Enabled = false;
            TabLockHelper.LockTab(tabPageMatchResults);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();
        }

        /// <summary>
        /// Wires UI events that are not using designer event hooks.
        /// </summary>
        private void InitializeEventHandlers()
        {
            Btn_CopyToClipboard.Click += (s, e) =>
            {
                System.Windows.Clipboard.SetText(TB_ConsoleCommand.Text);
                ShowCopyTooltip();
                this.ActiveControl = null;
            };

            Btn_MoveToCS2.Click += (s, e) => { MoveToCSFolder(); };
        }

        /// <summary>
        /// Checks application settings from the JSON configuration
        /// and applies them (theme, shell integration, and paths).
        /// </summary>
        private void CheckSettings()
        {
            // Retrieve the theme setting from JSON; default to dark mode if the key is missing.
            bool isDarkMode = JsonClass.KeyExists(_THEMEMODEKEY)
                ? JsonClass.ReadJson<bool>(_THEMEMODEKEY)
                : true;

            // Apply the theme (light or dark).
            LoadTheme(isDarkMode);

            // Check if shell integration is enabled and update the settings checkbox accordingly.
            if (AddShellContextMenu.ValidateShellIntegration(this))
                CB_AddToShellContectMenu.Checked = true;

            // Load configured paths (demo folder, saved voice files).
            GetPathFromConfig();
        }

        /// <summary>
        /// Reads file paths (e.g., CS2 demo and saved audio folder) from the configuration
        /// and updates the settings UI fields if valid paths are found.
        /// </summary>
        private void GetPathFromConfig()
        {
            // Read paths from configuration
            var cs2DemoFolderPath = PathPicker.ReadPath(this, "CS2DemoPath");
            var savedVoiceFilesPath = PathPicker.ReadPath(this, "SavedVoiceFilesPath");

            // Update text boxes in the settings UI if paths are available.
            if (!string.IsNullOrEmpty(cs2DemoFolderPath))
            {
                TB_SettingsDemoPath.Text = cs2DemoFolderPath;
                BTN_ChangeDemoPath.Text = "Change";
            }
            else
            {
                BTN_ChangeDemoPath.Text = "Add Path";
            }


            if (!string.IsNullOrEmpty(savedVoiceFilesPath))
            {
                TB_SettingsSavedAudioPath.Text = savedVoiceFilesPath;
                BTN_ChangeAudioSavePath.Text = "Change";
            }
            else
            {
                BTN_ChangeAudioSavePath.Text = "Add Path";
            }

        }


        // =====================================================
        // ================== THEME & UI HELPERS ===============
        // =====================================================

        /// <summary>
        /// Shows a tooltip when copying to clipboard.
        /// </summary>
        private void ShowCopyTooltip()
        {
            _copyToolTip.AutomaticDelay = 0;
            _copyToolTip.AutoPopDelay = 1000;
            _copyToolTip.InitialDelay = 0;
            _copyToolTip.ReshowDelay = 0;
            _copyToolTip.ShowAlways = true;
            var offset = new System.Drawing.Point(Btn_CopyToClipboard.Width / 2, -Btn_CopyToClipboard.Height / 2);
            _copyToolTip.Show("Copied!", Btn_CopyToClipboard, offset, 1000);
        }

        /// <summary>
        /// Applies the Material theme (light/dark) and updates Snackbar colors.
        /// </summary>
        public void LoadTheme(bool dark)
        {
            // Initialize MaterialSkinManager
            _materialSkinManager = MaterialSkinManager.Instance;

            // Enforce backcolor on non-materialSkin components (must be set before AddFormToManage).
            _materialSkinManager.EnforceBackcolorOnAllComponents = true;

            // Bind this form to Material manager.
            _materialSkinManager.AddFormToManage(this);

            if (!dark)
            {
                _materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
                _materialSkinManager.ColorScheme = new MaterialColorScheme("#87CEEB", "#57aed1", "#57aed1", "#57aed1", MaterialTextShade.DARK);
                MSwitch.Checked = false;

                // PictureBox Change Color
                PBoxShell.Image = Properties.Resources.iconShellB;
                PBoxTheme.Image = Properties.Resources.iconBrush;
                PBoxFolder.Image = Properties.Resources.iconFolderB;

            }
            else
            {
                _materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
                _materialSkinManager.ColorScheme = new MaterialColorScheme(MaterialPrimary.BlueGrey600, MaterialPrimary.BlueGrey700, MaterialPrimary.BlueGrey400, MaterialAccent.LightBlue400, MaterialTextShade.LIGHT);
                MSwitch.Checked = true;

                // PictureBox Change Color
                PBoxShell.Image = Properties.Resources.iconShellW;
                PBoxTheme.Image = Properties.Resources.iconBrushW;
                PBoxFolder.Image = Properties.Resources.iconFolderW;

            }

            SnackBarBackColor = _materialSkinManager.Theme == MaterialSkinManager.Themes.DARK
                ? _materialSkinManager.ColorScheme.DarkPrimaryColor
                : _materialSkinManager.ColorScheme.PrimaryColor;

            SnackBarForeColor = _materialSkinManager.ColorScheme.TextColor;

            Invalidate();
            Refresh();
        }

        /// <summary>
        /// Configures initial ListView properties such as alignment and checkbox behavior.
        /// </summary>
        private void LoadListViewSettings()
        {
            columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            LV_TeamA.OwnerDraw = false;
            LV_TeamB.OwnerDraw = false;

            LV_TeamA.CheckBoxes = true;
            LV_TeamB.CheckBoxes = true;
        }

        // =====================================================
        // ================= FILE / PATH ACTIONS ===============
        // =====================================================

        /// <summary>
        /// Moves the demo file to the CS folder and verifies the file hash. Updates UI accordingly.
        /// </summary>
        private void MoveToCSFolder()
        {
            string? movedFullFilePath = null;
            string? sourceFile = null;
            _sourceHash = null; _destinationHash = null;
            TabLockHelper.LockTab(tabPageMatchResults);
            TabLockHelper.LockTab(tabPageAudioPlayer);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();

            if (string.IsNullOrWhiteSpace(_csDemoFolderPath) || !Directory.Exists(_csDemoFolderPath))
            {
                var title = "Select the CS2 Demo folder";
                var pathKey = "CS2DemoPath";

                _csDemoFolderPath = PathPicker.GetPath(this, title, pathKey);
                if (!Directory.Exists(_csDemoFolderPath))
                {
                    PathPicker.EnsurePathConfigured(this, title, pathKey);
                    _csDemoFolderPath = PathPicker.GetPath(this, title, pathKey);
                }
                GetPathFromConfig();
            }

            if (!string.IsNullOrWhiteSpace(_csDemoFolderPath) && Directory.Exists(_csDemoFolderPath))
            {
                sourceFile = TB_FilePath.Text;
                if (!string.IsNullOrWhiteSpace(sourceFile))
                {
                    try
                    {
                        _sourceHash = sourceFile.ComputeFileHash();
                        movedFullFilePath = sourceFile.MoveAndRenameFile(_csDemoFolderPath, this);
                    }
                    catch
                    {
                        movedFullFilePath = null;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(movedFullFilePath))
            {
                _destinationHash = movedFullFilePath.ComputeFileHash();
                if (_sourceHash != null && _destinationHash != null)
                {
                    if (_sourceHash.HashesAreEqual(_destinationHash))
                    {
                        TB_FilePath.Text = movedFullFilePath;
                        ReadDemoFile(movedFullFilePath);
                        MaterialUiHelper.ShowSnack(this, "File was moved successfully.", false);
                    }
                    else
                    {
                        MaterialUiHelper.ShowSnack(this, "Error while moving the file!", true);
                    }
                }
            }
            else
            {
                MaterialUiHelper.ShowSnack(this, "File move was canceled or failed..", true);
                TabLockHelper.UnlockTab(tabPageMatchResults);
                TabLockHelper.UnlockTab(tabPageAudioPlayer);
                DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();
                BTN_ExtractAudio.Enabled = true;
            }
        }

        // =====================================================
        // ===================== DEMO FLOW =====================
        // =====================================================

        /// <summary>
        /// Sets the demo file on startup if the file exists and is a .dem file.
        /// </summary>
        public void SetDemoFileOnStartup(string filePath)
        {
            if (!File.Exists(filePath) || Path.GetExtension(filePath).ToLower() != ".dem") return;
            _csDemoFolderPath = filePath;
            _readDemoOnStartUp = true;
            if (materialTabControlMain.TabPages.Count > 1)
                materialTabControlMain.SelectedIndex = 1;
        }

        /// <summary>
        /// Prepares the UI and state for loading a new demo file and triggers parsing.
        /// </summary>
        private async void PrepareStart(string demoPath)
        {
            PBar_LoadDemo.Value = 0;

            Btn_CopyToClipboard.Enabled = false;
            Btn_MoveToCS2.Enabled = false;
            BTN_ExtractAudio.Enabled = false;

            // Bitfield Page
            LBL_TeamA.Text = "Reading Team-A data..";
            LBL_TeamB.Text = "Reading Team-B data..";
            LV_TeamA.Items.Clear();
            LV_TeamB.Items.Clear();
            LBL_MapName.Text = "";
            LBL_Duration.Text = "";

            // MatchDetails Page
            LBL_MatchDetailsPoints_TeamA.Text = "";
            LBL_MatchDetailsPoints_TeamB.Text = "";
            LBL_MatchDetails_TeamA.Text = "";
            LBL_MatchDetails_TeamB.Text = "";
            LV_MatchDetails_A.Items.Clear();
            LV_MatchDetails_B.Items.Clear();

            TB_FilePath.Text = demoPath;
            TB_ConsoleCommand.Text = "";

            // Lock the Tabs
            TabLockHelper.LockTab(tabPageMatchResults);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();

            LB_PlayerListAudio.Clear();
            LB_PlayerAudios.Clear();

            RemoveAudioFolder();

            ReadDemoFile(demoPath);
        }

        /// <summary>
        /// Reads a CS demo file, collects player snapshots at match end, and updates UI state.
        /// </summary>
        private async void ReadDemoFile(string demoPath)
        {
            _demo = new CsDemoParser();
            _snapshot = null;
            var tcs = new TaskCompletionSource<bool>();
            var collected = new List<PlayerSnapshot>();
            bool readingFinished = false;

            // Progress bar setup
            PBar_LoadDemo.Minimum = 0;
            PBar_LoadDemo.Maximum = 100;
            PBar_LoadDemo.Value = 0;

            // Use RoundEnd but check explicitly for MatchEnded phase.
            _demo.Source1GameEvents.RoundEnd += (Source1RoundEndEvent e) =>
            {
                if (_demo.GameRules.CSGamePhase != CSGamePhase.MatchEnded)
                    return;

                collected.Clear();

                foreach (var team in new[] { _demo.TeamCounterTerrorist, _demo.TeamTerrorist })
                {
                    foreach (var p in team.CSPlayerControllers)
                    {
                        if (string.IsNullOrWhiteSpace(p.PlayerName))
                            continue;

                        var matchStats = p.ActionTrackingServices?.MatchStats ?? new CSMatchStats();

                        // skip spectators/admins/bots
                        if (p.PlayerInfo?.Fakeplayer == true)
                            continue;
                        if (matchStats.LiveTime == 0 || (matchStats.Kills + matchStats.Deaths + matchStats.Assists + matchStats.Damage) == 0)
                            continue;


                        double kd = matchStats.Deaths > 0 ? (double)matchStats.Kills / matchStats.Deaths : matchStats.Kills;
                        double hsPercent = matchStats.Kills > 0 ? (double)matchStats.HeadShotKills * 100 / matchStats.Kills : 0;

                        collected.Add(new PlayerSnapshot
                        {
                            UserId = (int)p.EntityIndex.Value,
                            PlayerName = p.PlayerName!,
                            TeamNumber = (int)p.CSTeamNum,
                            TeamName = p.Team.ClanTeamname.IsNullOrEmptyOrWhiteSpace() ? "No Team name.." : p.Team.ClanTeamname,
                            PlayerSteamID = p.PlayerInfo?.Steamid ?? p.SteamID,
                            Kills = matchStats.Kills,
                            Death = matchStats.Deaths,
                            Assists = matchStats.Assists,
                            HeadShotKill = matchStats.HeadShotKills,
                            HeadShotPerecent = hsPercent,
                            Kd = kd,
                            Damage = matchStats.Damage,
                            MVP = p.MVPs,
                            ThreeK = matchStats.Enemy3Ks,
                            FourK = matchStats.Enemy4Ks,
                            FiveK = matchStats.Enemy5Ks,
                            EndScore = p.Team.Score
                        });
                    }
                }
                _snapshot = collected.ToArray();
                _mapName = _demo.ServerInfo?.MapName ?? "no Mapname!";
                _duration = TicksToTimeString(_demo.TickCount.Value);
                _hostName = _demo.ServerInfo?.HostName ?? "No Hostname";

                tcs.TrySetResult(true);
            };

            try
            {
                using var stream = new FileStream(demoPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096 * 1024);
                var reader = DemoFileReader.Create(_demo, stream);

                var progressTask = Task.Run(async () =>
                {
                    while (!readingFinished)
                    {
                        double ratio = (double)stream.Position / stream.Length;
                        int percent = (int)(ratio * 100);
                        if (percent >= 0 && percent <= 100)
                        {
                            PBar_LoadDemo.Invoke((Action)(() =>
                            {
                                PBar_LoadDemo.Value = percent;
                            }));
                        }
                        await Task.Delay(100);
                    }
                });

                await reader.ReadAllAsync();
                readingFinished = true;
                await tcs.Task; // Wait until MatchEnded is detected
            }
            catch (Exception ex)
            {
                MaterialUiHelper.ShowLongMessageBox($"Error when reading the demo", $"{ex.Message}", MessageBoxButtons.OK);
                return;
            }
            finally
            {
                PBar_LoadDemo.Value = 100; // Ensure it's full at the end
            }

            LoadListViewData();
        }

        /// <summary>
        /// Fills all UI list views (bitfield and match details) with the parsed snapshot data.
        /// </summary>
        private void LoadListViewData()
        {
            if (_snapshot == null)
            {
                MaterialUiHelper.ShowMaterialMsgBox(this, "Error", "Error reading demo file!", "OK", false, "CANCLE");
                return;
            }

            // Bitfield tables
            var ctPlayers = _snapshot.Where(p => p.TeamNumber == 3).OrderBy(p => p.UserId).ToList();
            var tPlayers = _snapshot.Where(p => p.TeamNumber == 2).OrderBy(p => p.UserId).ToList();

            // Match details tables
            var ctDetailPlayer = _snapshot.Where(p => p.TeamNumber == 3).OrderByDescending(p => p.Kills).ToList();
            var tDetailPlayer = _snapshot.Where(p => p.TeamNumber == 2).OrderByDescending(p => p.Kills).ToList();

            LBL_TeamA.Text = ctPlayers.FirstOrDefault()?.TeamName ?? "Unknown";
            LBL_TeamB.Text = tPlayers.FirstOrDefault()?.TeamName ?? "Unknown";

            LBL_MatchDetails_TeamA.Text = ctPlayers.FirstOrDefault()?.TeamName ?? "Unknown";
            LBL_MatchDetails_TeamB.Text = tPlayers.FirstOrDefault()?.TeamName ?? "Unknown";

            LBL_MatchDetailsPoints_TeamA.Text = ctPlayers.FirstOrDefault()?.EndScore.ToString() + (ctPlayers.FirstOrDefault()?.EndScore > tPlayers.FirstOrDefault()?.EndScore ? " - Wins" : " - Lose");
            LBL_MatchDetailsPoints_TeamB.Text = tPlayers.FirstOrDefault()?.EndScore.ToString() + (tPlayers.FirstOrDefault()?.EndScore > ctPlayers.FirstOrDefault()?.EndScore ? " - Wins" : " - Lose");

            LBL_MapName.Text = _mapName;
            LBL_Duration.Text = _duration;

            FillListView(LV_TeamA, ctPlayers, true);
            FillListView(LV_TeamB, tPlayers, false);

            FillMatchDetailsListView(LV_MatchDetails_A, ctDetailPlayer);
            FillMatchDetailsListView(LV_MatchDetails_B, tDetailPlayer);

            CostumContextMenu.ConfigureContextMenuMainGrid(this, LV_TeamA, ctPlayers, _steamProfileLink, _cswatchProfileLink, _leetifyProfileLink, _csStatsProfileLink, true);
            CostumContextMenu.ConfigureContextMenuMainGrid(this, LV_TeamB, tPlayers, _steamProfileLink, _cswatchProfileLink, _leetifyProfileLink, _csStatsProfileLink, false);

            Btn_MoveToCS2.Enabled = true;
            if (!_hostName.Contains("SourceTV"))
            {
                TB_ConsoleCommand.Hint = "The loaded demo comes from competitive mode and has no audio...";
                TB_ConsoleCommand.Text = "";
                Btn_CopyToClipboard.Enabled = false;
                BTN_ExtractAudio.Enabled = false;
            }
            else
            {
                TB_ConsoleCommand.Hint = "Select one or more players you would like to hear in the demo...";
                TabLockHelper.UnlockTab(tabPageAudioPlayer);
                BTN_ExtractAudio.Enabled = true;
                Btn_CopyToClipboard.Enabled = true;
            }

            TabLockHelper.UnlockTab(tabPageMatchResults);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();

            ClearLVSelection();
        }

        /// <summary>
        /// Populates a team ListView with player rows and an extra checkbox row (index 5) for select-all.
        /// </summary>
        private void FillListView(MaterialListView listView, List<PlayerSnapshot> players, bool left)
        {
            listView.Items.Clear(); // First clear old items

            var selectAll = "Select all";
            var empty = "";

            int counter = 0;
            foreach (var p in players)
            {
                string[] row = { "", left ? p.UserId.ToString() : p.PlayerName, left ? p.PlayerName : p.UserId.ToString() };
                ListViewItem item = new(row);
                listView.Items.Add(item);

                counter++;

                // After the 5th player → insert an empty line with only a checkbox (used for select-all)
                if (counter == 5)
                {
                    string[] emptyRow = { "", left ? empty : selectAll, left ? selectAll : empty };
                    ListViewItem emptyItem = new(emptyRow);
                    listView.Items.Add(emptyItem);
                }
            }
        }

        /// <summary>
        /// Populates the match-details ListView with per-player stats (kills, deaths, KD, HS%, MVP, etc.).
        /// </summary>
        private void FillMatchDetailsListView(MaterialListView listView, List<PlayerSnapshot> players)
        {
            listView.Items.Clear();

            foreach (var p in players)
            {
                string team = p.TeamNumber == 2 ? "T" : "CT";
                string kd = $"{p.Kd:F2}";
                string hs = $"{p.HeadShotPerecent:F2}";

                string[] row = {
                    p.PlayerName,
                    p.Kills.ToString(),
                    p.Death.ToString(),
                    p.Assists.ToString(),
                    kd,
                    p.HeadShotKill.ToString(),
                    hs,
                    p.MVP.ToString(),
                    p.ThreeK.ToString(),
                    p.FourK.ToString(),
                    p.FiveK.ToString(),
                    p.Damage.ToString()
                };
                ListViewItem item = new(row);
                listView.Items.Add(item);
            }
        }

        // =====================================================
        // ======== BITFIELD / CHECKBOXES & CONSOLE CMD ========
        // =====================================================

        /// <summary>
        /// Auto-select logic for item check/uncheck on team list views, including select-all row.
        /// Updates bitfield arrays and regenerates the console command.
        /// </summary>
        private void CheckBoxAutoChecker(ItemCheckEventArgs e, MaterialListView lv, int[] _team, int leftRight)
        {
            if (_isUpdatingCheckboxes)
                return;

            if (e.Index <= 4)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    _team[e.Index] = GetPlayBitField(lv.Items[e.Index].SubItems[leftRight].Text);
                }
                else if (e.NewValue == CheckState.Unchecked)
                {
                    _team[e.Index] = 0;
                }

                int countChecked = 0;
                for (int i = 0; i <= 4; i++)
                {
                    if (i == e.Index)
                    {
                        if (e.NewValue == CheckState.Checked)
                            countChecked++;
                    }
                    else
                    {
                        if (lv.Items[i].Checked)
                            countChecked++;
                    }
                }

                _isUpdatingCheckboxes = true;
                lv.Items[5].Checked = (countChecked == 5);
                _isUpdatingCheckboxes = false;
            }
            else if (e.Index == 5)
            {
                _isUpdatingCheckboxes = true;

                bool targetState = e.NewValue == CheckState.Checked;
                for (int i = 0; i <= 4; i++)
                {
                    lv.Items[i].Checked = targetState;

                    if (targetState)
                    {
                        _team[i] = GetPlayBitField(lv.Items[i].SubItems[leftRight].Text);
                    }
                    else
                    {
                        _team[i] = 0;
                    }
                }

                _isUpdatingCheckboxes = false;
            }

            ChangeConsoleCommand();
        }

        /// <summary>
        /// ItemCheck handler for Team A list view. Delegates to <see cref="CheckBoxAutoChecker"/>.
        /// </summary>
        private void LV_TeamA_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckBoxAutoChecker(e, LV_TeamA, _teamA_Array, 1);
        }

        /// <summary>
        /// ItemCheck handler for Team B list view. Delegates to <see cref="CheckBoxAutoChecker"/>.
        /// </summary>
        private void LV_TeamB_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckBoxAutoChecker(e, LV_TeamB, _teamB_Array, 2);
        }

        /// <summary>
        /// Recomputes the voice bitfield from both teams and updates the console command text.
        /// </summary>
        public void ChangeConsoleCommand()
        {
            int voiceBitField = _teamA_Array[0] + _teamA_Array[1] + _teamA_Array[2] + _teamA_Array[3] + _teamA_Array[4]
                + _teamB_Array[0] + _teamB_Array[1] + _teamB_Array[2] + _teamB_Array[3] + _teamB_Array[4];

            if (voiceBitField != 0 && _hostName.Contains("SourceTV"))
            {
                TB_ConsoleCommand.Text =
                    $"tv_listen_voice_indices {voiceBitField}; tv_listen_voice_indices_h {voiceBitField}";
                Btn_CopyToClipboard.Enabled = true;
            }
            else
            {
                TB_ConsoleCommand.Text = "";
                Btn_CopyToClipboard.Enabled = false;
            }
        }

        /// <summary>
        /// Calculates the bitfield value (1 &lt;&lt; (specId-1)) for a given player spec ID. Validates input range 1..20.
        /// </summary>
        private int GetPlayBitField(object cellValue)
        {
            if (cellValue == null || !int.TryParse(cellValue.ToString(), out int specPlayerId))
                throw new ArgumentException("Invalid spec ID in the cell.");
            if (specPlayerId < 1 || specPlayerId > 20)
                throw new ArgumentOutOfRangeException(nameof(specPlayerId), "Spec-ID must be between 1 and 20.");
            return 1 << (specPlayerId - 1);
        }

        /// <summary>
        /// Selects or deselects all items in the provided list view. Uses a guard to prevent recursion.
        /// </summary>
        private void ToggleListViewItems(MaterialListView listView, bool isChecked)
        {
            _isSyncingSelectAll = true;
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = isChecked;
            }
            _isSyncingSelectAll = false;
        }

        // =====================================================
        // ===================== UTILITIES =====================
        // =====================================================

        /// <summary>
        /// Converts demo ticks to a time string (hh:mm:ss). Uses <paramref name="tickRate"/> to compute seconds.
        /// </summary>
        public static string TicksToTimeString(int ticks, double tickRate = 64.0)
        {
            if (tickRate <= 0)
                throw new ArgumentException("Tick rate must be greater than 0.");
            double totalSeconds = ticks / tickRate;
            TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);
            return duration.ToString(@"hh\:mm\:ss");
        }

        // =====================================================
        // ===================== DRAG & DROP ===================
        // =====================================================

        /// <summary>
        /// DragEnter for the demo path textbox; shows copy effect only for file drops.
        /// </summary>
        private void TB_FilePath_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop)
                ? System.Windows.Forms.DragDropEffects.Copy
                : System.Windows.Forms.DragDropEffects.None;
        }

        /// <summary>
        /// DragDrop for the demo path textbox; accepts first .dem file and starts parsing.
        /// </summary>
        private void TB_FilePath_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop)) return;
            string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);

            foreach (string file in files)
            {
                if (Path.GetExtension(file).Equals(".dem", StringComparison.OrdinalIgnoreCase))
                {
                    PrepareStart(file);
                    return;
                }
            }
            MaterialUiHelper.ShowSnack(this, "Please only drop files with the extension .dem", true);
        }

        // =====================================================
        // =============== SELECTION & COLUMN SIZING ===========
        // =====================================================

        /// <summary>
        /// Clears both team list view selections.
        /// </summary>
        private void ClearLVSelection()
        {
            LV_TeamA.SelectedItems.Clear();
            LV_TeamB.SelectedItems.Clear();
        }

        /// <summary>
        /// When an item in Team B is selected, clear previous selections (keep single-select UX).
        /// </summary>
        private void LV_TeamB_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                ClearLVSelection();
            }
        }

        /// <summary>
        /// When an item in Team A is selected, clear previous selections (keep single-select UX).
        /// </summary>
        private void LV_TeamA_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                ClearLVSelection();
            }
        }

        /// <summary>
        /// Prevent column width changes for Match Details A.
        /// </summary>
        private void LV_MatchDetails_A_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = LV_MatchDetails_A.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Prevent column width changes for Match Details B.
        /// </summary>
        private void LV_MatchDetails_B_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = LV_MatchDetails_B.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Clear selection when mouse leaves Team A list view (avoids lingering highlights).
        /// </summary>
        private void LV_TeamA_MouseLeave(object sender, EventArgs e)
        {
            ClearLVSelection();
        }

        // =====================================================
        // ===================== AUDIO FLOW ====================
        // =====================================================

        /// <summary>
        /// Click handler to extract audio from the current demo. Shows progress and loads folders on success.
        /// </summary>
        private async void BTN_ExtractAudio_Click(object sender, EventArgs e)
        {
            PBar_LoadAudio.Value = 0;
            LB_PlayerListAudio.Clear();
            LB_PlayerAudios.Clear();
            BTN_ExtractAudio.Enabled = false;
            TabLockHelper.LockTab(tabPageBitfieldCalc);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();

            var progress = new Progress<float>(value =>
            {
                PBar_LoadAudio.Value = Math.Min(100, (int)(value * 100));
            });

            bool result = await Task.Run(() => AudioExtractor.ExtractAsync(TB_FilePath.Text, progress));

            BTN_ExtractAudio.Enabled = true;

            if (result)
            {
                MaterialUiHelper.ShowSnack(this, "Extraction complete!", false);
                LoadPlayerFolders();
            }
            else
                MaterialUiHelper.ShowSnack(this, "Error during extraction!", true);

            TabLockHelper.UnlockTab(tabPageBitfieldCalc);
            DrawerNonClickTabPage = TabLockHelper.GetLockedTabs();
        }

        /// <summary>
        /// Loads per-player audio folders for the current demo and populates the player list.
        /// </summary>
        private void LoadPlayerFolders()
        {
            string appPath = System.Windows.Forms.Application.StartupPath;
            string demoPath = TB_FilePath.Text;
            string? demoName = demoPath != null ? Path.GetFileNameWithoutExtension(demoPath) : null;
            string audioFolder = LocalAppDataFolder.EnsureSubDirectoryExists("Audio");
            _audioFolderPath = demoName != null ? Path.Combine(appPath, audioFolder, demoName) : null;

            var displayItems = PlayerAudioHelper.LoadPlayerFolders(_snapshot, _audioFolderPath);

            LB_PlayerListAudio.Items.Clear();

            if (displayItems.Count > 0)
            {
                foreach (var item in displayItems)
                {
                    LB_PlayerListAudio.Items.Add(new ReaLTaiizor.Child.Material.MaterialListBoxItem()
                    {
                        Text = item.DisplayName,
                        SecondaryText = item.SteamId
                    });
                }
            }
            else
            {
                MaterialUiHelper.ShowLongMessageBox("Error", "The 'Audio' folder could not be found.\\nTry running ‘Extract voice from demo’!");
            }
        }

        /// <summary>
        /// Removes focus visual leftovers after tab page change.
        /// </summary>
        private void materialTabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            PBar_LoadAudio.Focus();
            PBar_LoadDemo.Focus();
        }

        /// <summary>
        /// Loads voice entries for a specific player (by SteamID), sorts them, and populates the audio list.
        /// </summary>
        public void LoadVoiceData(string steamID)
        {
            LB_PlayerAudios.Clear();

            _audioEntries = AudioReadHelper.GetAudioEntries(steamID, _audioFolderPath ?? "");
            var sorted = _audioEntries.OrderBy(x => x.Round).ToList();
            _audioEntries = sorted;

            foreach (var entry in sorted)
            {
                LB_PlayerAudios.Items.Add(new ReaLTaiizor.Child.Material.MaterialListBoxItem()
                {
                    Text = $"Round {entry.Round}",
                    SecondaryText = "Demo-Time: " + entry.Time.ToString(@"hh\:mm\:ss") + " | Duration: " + $"{entry.DurationSeconds:F1} s"
                });

            }
        }

        /// <summary>
        /// Handles selection of a player in the audio list and loads corresponding voice files.
        /// </summary>
        private void LB_PlayerListAudio_SelectedIndexChanged(object sender, ReaLTaiizor.Child.Material.MaterialListBoxItem selectedItem)
        {
            // SteamId is in SecondaryText, DisplayName in Text
            string? steamId = selectedItem.SecondaryText;
            string? displayName = selectedItem.Text;

            string? demoPath = TB_FilePath.Text;
            string? demoName = demoPath != null ? Path.GetFileNameWithoutExtension(demoPath) : null;
            string? audioFolder = LocalAppDataFolder.EnsureSubDirectoryExists("Audio");
            string? folderPath = demoName != null
                ? Path.Combine(audioFolder, demoName, steamId ?? "00")
                : string.Empty;

            _selectedPlayerVoicePlayer = displayName;

            if (!string.IsNullOrEmpty(steamId) && Directory.Exists(folderPath))
            {
                LoadVoiceData(steamId);
            }
            else
            {
                MaterialUiHelper.ShowLongMessageBox("Error", $"The folder '{steamId}' could not be found in '{demoName}'.");
            }
        }

        /// <summary>
        /// Double-click on a voice entry plays the selected audio file.
        /// </summary>
        private void LB_PlayerAudios_DoubleClick(object sender, EventArgs e)
        {
            if (LB_PlayerAudios.SelectedItem is ReaLTaiizor.Child.Material.MaterialListBoxItem selectedItem)
            {
                // Determine index in the internal list
                int index = LB_PlayerAudios.SelectedIndex;

                if (index >= 0 && index < _audioEntries.Count)
                {
                    var entry = _audioEntries[index];
                    AudioReadHelper.PlayAudio(this, entry);
                }
            }
        }

        /// <summary>
        /// Removes the Audio folder under local app data (if present) to ensure a clean run.
        /// </summary>
        private void RemoveAudioFolder()
        {
            try
            {
                string audioFolderPath = LocalAppDataFolder.EnsureSubDirectoryExists("Audio");

                if (Directory.Exists(audioFolderPath))
                {
                    Directory.Delete(audioFolderPath, true);
                }
            }
            catch (Exception ex)
            {
                MaterialUiHelper.ShowLongMessageBox("Error deleting the folder", $"ex.Message");
            }
        }

        /// <summary>
        /// Theme toggle switch handler, updates theme and persists in config.
        /// </summary>
        private void MSwitch_CheckStateChanged(object sender, EventArgs e)
        {
            if (MSwitch.Checked)
            {
                LoadTheme(true);
            }
            else
            {
                LoadTheme(false);
            }

            JsonClass.WriteJson(_THEMEMODEKEY, MSwitch.Checked);
        }

        /// <summary>
        /// Loads saved audio files from the configured path and populates the saved list.
        /// </summary>
        private void LoadSavedAudioFiles()
        {
            LB_PlayerAudios.SelectedIndex = -1; // Clear selection from copied file.

            if (!PathPicker.HasPath(this, "SavedVoiceFilesPath"))
                return;

            string savedAudioPath = PathPicker.ReadPath(this, "SavedVoiceFilesPath");

            if (savedAudioPath != null && AudioFileManager.SavedVoiceFilesExists(savedAudioPath))
            {
                AudioFileManager.LoadAudioFiles(savedAudioPath);
                AudioFileManager.PopulateListBox(LB_PlayerSavedAudios);
            }

            if (string.IsNullOrEmpty(TB_SettingsSavedAudioPath.Text))
            {
                // Load configured paths (demo folder, saved voice files).
                GetPathFromConfig();
            }
        }

        /// <summary>
        /// Double-click on saved audio plays it.
        /// </summary>
        private void LB_PlayerSavedAudios_DoubleClick(object sender, EventArgs e)
        {
            if (LB_PlayerSavedAudios.SelectedItem is ReaLTaiizor.Child.Material.MaterialListBoxItem selectedItem)
            {
                // Determine index in the internal list
                int index = LB_PlayerSavedAudios.SelectedIndex;

                if (index >= 0 && index < SavedAudioFiles.Files.Count)
                {
                    var entry = SavedAudioFiles.Files[index];
                    AudioReadHelper.PlaySavedAudio(this, entry);
                }
            }
        }

        /// <summary>
        /// Handles right-click actions on the <c>LB_PlayerAudios</c> list box.  
        /// If no audio item is selected, a Material message box is shown to inform the user.  
        /// Otherwise, configures the context menu for copying the selected audio file.
        /// </summary>
        /// <param name="sender">The source of the event (the list box).</param>
        /// <param name="e">Mouse event data that provides button information and cursor position.</param>
        private void LB_PlayerAudios_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && LB_PlayerAudios.SelectedIndex < 0)
            {
                MaterialUiHelper.ShowMaterialMsgBox(this, "Info", "You must select an audio voice before attempting to copy it.", "OK", false, "Cancel");

                return;
            }

            CostumContextMenu.ConfigureContextMenuAudioFileCopy(this, LB_PlayerAudios, _audioEntries, _selectedPlayerVoicePlayer, TB_FilePath.Text, () => LoadSavedAudioFiles());
        }

        // =====================================================
        // ========= EVENT HANDLERS FOR MENUS AND UI ===========
        // =====================================================

        /// <summary>
        /// Opens a folder picker and stores the CS2 demo folder path.
        /// </summary>
        private void BTN_ChangeDemoPath_Click(object sender, EventArgs e)
        {
            var title = "Select the CS2 Demo folder";
            var pathKey = "CS2DemoPath";
            PathPicker.EnsurePathConfigured(this, title, pathKey);
            _csDemoFolderPath = PathPicker.GetPath(this, title, pathKey);

            GetPathFromConfig();
        }

        /// <summary>
        /// Opens a folder picker and stores the path where copied voice files should be saved.
        /// </summary>
        private void BTN_ChangeAudioSavePath_Click(object sender, EventArgs e)
        {
            var title = "Select the path where the copied voice files should be saved.";
            var pathKey = "SavedVoiceFilesPath";
            PathPicker.EnsurePathConfigured(this, title, pathKey);

            GetPathFromConfig();
            LoadSavedAudioFiles();
        }

        /// <summary>
        /// Adds or Removes the shell context menu integration.
        /// </summary>
        private void CB_AddToShellContectMenu_Click(object sender, EventArgs e)
        {
            if (CB_AddToShellContectMenu.Checked)
            {
                AddShellContextMenu.AddShellIntegration(this);
            }
            else
            {
                AddShellContextMenu.RemoveShellIntegration(this);
            }
        }

        /// <summary>
        /// Opens the Ko-fi support page in the user's default web browser.
        /// </summary>
        private void BTN_SupportKoFi_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = _KOFIURL,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// Opens the GitHub repository page in the user's default web browser.
        /// </summary>
        private void BTN_GitHub_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = _GITHUBURL,
                UseShellExecute = true
            });
        }
    }
}
