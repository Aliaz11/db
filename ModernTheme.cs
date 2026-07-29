using System.Drawing.Drawing2D;

namespace db
{
    /// <summary>
    /// A single visual system for the application. The original forms keep their existing controls
    /// and event handlers; this class gives them a consistent layout, typography and interaction style.
    /// </summary>
    public static class ModernTheme
    {
        private static bool darkMode = LoadDarkMode();

        public static bool IsDarkMode => darkMode;
        public static Color Ink => darkMode ? Color.FromArgb(230, 237, 234) : Color.FromArgb(28, 39, 36);
        public static Color Muted => darkMode ? Color.FromArgb(164, 179, 173) : Color.FromArgb(100, 116, 110);
        public static Color Canvas => darkMode ? Color.FromArgb(17, 23, 21) : Color.FromArgb(244, 247, 245);
        public static Color Surface => darkMode ? Color.FromArgb(27, 36, 33) : Color.FromArgb(255, 255, 255);
        public static Color SurfaceSoft => darkMode ? Color.FromArgb(39, 51, 47) : Color.FromArgb(236, 242, 239);
        public static Color Line => darkMode ? Color.FromArgb(61, 76, 70) : Color.FromArgb(217, 226, 222);
        public static Color Forest => darkMode ? Color.FromArgb(75, 156, 130) : Color.FromArgb(35, 83, 71);
        public static Color ForestDark => darkMode ? Color.FromArgb(25, 76, 62) : Color.FromArgb(24, 61, 52);
        public static Color Sage => darkMode ? Color.FromArgb(173, 201, 189) : Color.FromArgb(204, 220, 212);
        public static Color Terracotta => darkMode ? Color.FromArgb(226, 139, 101) : Color.FromArgb(205, 111, 72);
        public static Color TerracottaSoft => darkMode ? Color.FromArgb(249, 200, 177) : Color.FromArgb(249, 232, 223);
        public static Color Danger => darkMode ? Color.FromArgb(226, 108, 116) : Color.FromArgb(185, 65, 72);
        public static Color DangerSoft => darkMode ? Color.FromArgb(76, 40, 45) : Color.FromArgb(250, 231, 233);

        private static Color AlternatingSurface =>
            darkMode ? Color.FromArgb(32, 42, 38) : Color.FromArgb(249, 251, 250);
        private static Color SelectionSurface =>
            darkMode ? Color.FromArgb(49, 72, 63) : Color.FromArgb(222, 235, 229);

        private const int Radius = 16;

        public static void Apply(Form form)
        {
            bool firstApplication = form.Tag is not ThemeState;
            if (firstApplication)
            {
                form.Tag = new ThemeState();
                form.Resize += (_, _) => Arrange(form);
                form.Shown += (_, _) => Arrange(form);
            }

            form.SuspendLayout();
            ApplyBackdrop(form);
            form.Font = BodyFont(10F);
            form.ForeColor = Ink;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MaximizeBox = true;
            form.MinimizeBox = true;
            if (firstApplication)
            {
                form.WindowState = FormWindowState.Maximized;
            }

            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.KeyPreview = true;
            form.Text = ScreenWindowTitle(form.Name);

            StyleTree(form);
            PrepareScreen(form);
            EnsureThemeToggle(form);
            Arrange(form);

            form.ResumeLayout(true);
        }

        public static void ApplyBackdrop(Form form)
        {
            Image? previous = form.BackgroundImage;
            form.BackgroundImage = null;
            form.BackColor = Canvas;
            previous?.Dispose();
        }

        public static void Refresh(Form form)
        {
            StyleTree(form);
            EnsureThemeToggle(form);
            Arrange(form);
        }

        public static void SetDarkMode(bool enabled)
        {
            if (darkMode == enabled)
            {
                return;
            }

            darkMode = enabled;
            SaveDarkMode();

            Form[] openForms = Application.OpenForms.Cast<Form>().ToArray();
            foreach (Form form in openForms)
            {
                Apply(form);
                form.Invalidate(true);

                if (string.Equals(form.Name, "Form9", StringComparison.Ordinal) &&
                    Find<DataGridView>(form, "dataGridView1") is { } shelfGrid)
                {
                    ShelfExperience.Refresh(form, shelfGrid);
                }
            }
        }

        private static void ToggleMode() => SetDarkMode(!darkMode);

        private static bool LoadDarkMode()
        {
            try
            {
                return string.Equals(
                    File.ReadAllText(ThemePreferencePath).Trim(),
                    "dark",
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static void SaveDarkMode()
        {
            try
            {
                string? directory = Path.GetDirectoryName(ThemePreferencePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(ThemePreferencePath, darkMode ? "dark" : "light");
            }
            catch (IOException)
            {
                // Theme switching still works for this session if the preference cannot be saved.
            }
            catch (UnauthorizedAccessException)
            {
                // Theme switching still works for this session if the preference cannot be saved.
            }
        }

        private static string ThemePreferencePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LeafAndLetter",
            "theme.txt");

        private static string ScreenWindowTitle(string name) => name switch
        {
            "Form1" => "Leaf & Letter",
            "Form2" => "Create account — Leaf & Letter",
            "Form3" => "Members — Leaf & Letter Admin",
            "Form4" => "Sign in — Leaf & Letter",
            "Form5" => "Browse books — Leaf & Letter",
            "Form6" => "Books — Leaf & Letter Admin",
            "Form7" => "Add a book — Leaf & Letter Admin",
            "Form8" => "Reset password — Leaf & Letter",
            "Form9" => "My shelf — Leaf & Letter",
            "Form10" => "Sales analytics — Leaf & Letter Admin",
            "Passchg" => "Choose a new password — Leaf & Letter",
            _ => "Leaf & Letter"
        };

        private static Font BodyFont(float size, FontStyle style = FontStyle.Regular)
            => new Font("Segoe UI", size, style, GraphicsUnit.Point);

        private static Font DisplayFont(float size, FontStyle style = FontStyle.Bold)
            => new Font("Segoe UI", size, style, GraphicsUnit.Point);

        private static void StyleTree(Control root)
        {
            foreach (Control control in root.Controls)
            {
                switch (control)
                {
                    case SalesDashboard salesDashboard:
                        salesDashboard.ApplyTheme();
                        break;
                    case SurfacePanel surfacePanel:
                        surfacePanel.BackColor = Surface;
                        surfacePanel.Invalidate();
                        break;
                    case ShelfHeroPanel shelfHero:
                        shelfHero.Invalidate();
                        break;
                    case LibraryArtPanel libraryArt:
                        libraryArt.Invalidate();
                        break;
                    case Button button:
                        StyleButton(button, ButtonKind.Primary);
                        break;
                    case TextBox textBox:
                        StyleTextBox(textBox);
                        break;
                    case DateTimePicker picker:
                        picker.Font = BodyFont(10.5F);
                        picker.CalendarForeColor = Ink;
                        picker.CalendarMonthBackground = Surface;
                        break;
                    case NumericUpDown numeric:
                        numeric.Font = BodyFont(10.5F);
                        numeric.BackColor = Surface;
                        numeric.ForeColor = Ink;
                        numeric.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case DataGridView grid:
                        StyleGrid(grid);
                        break;
                    case ListView list:
                        StyleList(list);
                        break;
                    case PictureBox picture:
                        picture.BackColor = SurfaceSoft;
                        picture.BorderStyle = BorderStyle.None;
                        picture.SizeMode = PictureBoxSizeMode.Zoom;
                        break;
                    case Label label:
                        PrepareLabel(label);
                        if (label.Name.StartsWith("label_", StringComparison.Ordinal))
                        {
                            label.ForeColor = Danger;
                            label.Font = BodyFont(8.5F, FontStyle.Bold);
                        }
                        else if (!label.Name.StartsWith("modern", StringComparison.Ordinal))
                        {
                            label.ForeColor = Ink;
                            label.Font = BodyFont(10F);
                        }
                        break;
                    case CheckBox check:
                        check.BackColor = Color.Transparent;
                        check.ForeColor = Muted;
                        check.Font = BodyFont(9.5F);
                        check.FlatStyle = FlatStyle.Flat;
                        break;
                    case RadioButton radio:
                        radio.BackColor = Color.Transparent;
                        radio.ForeColor = Ink;
                        radio.Font = BodyFont(10F);
                        radio.FlatStyle = FlatStyle.Flat;
                        break;
                }

                if (control.HasChildren)
                {
                    StyleTree(control);
                }
            }
        }

        private static void StyleButton(Button? button, ButtonKind kind)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = kind == ButtonKind.Outline ? 1 : 0;
            button.FlatAppearance.BorderColor = kind == ButtonKind.Outline ? Forest : Canvas;
            button.FlatAppearance.MouseOverBackColor = kind switch
            {
                ButtonKind.Primary => ForestDark,
                ButtonKind.Secondary => Color.FromArgb(222, 233, 228),
                ButtonKind.Outline => SurfaceSoft,
                ButtonKind.Danger => Color.FromArgb(164, 52, 60),
                ButtonKind.Ghost => SurfaceSoft,
                _ => ForestDark
            };
            button.FlatAppearance.MouseDownBackColor = kind switch
            {
                ButtonKind.Primary => Color.FromArgb(18, 50, 42),
                ButtonKind.Danger => Color.FromArgb(145, 42, 50),
                _ => Sage
            };
            button.BackColor = kind switch
            {
                ButtonKind.Primary => Forest,
                ButtonKind.Secondary => SurfaceSoft,
                ButtonKind.Outline => Surface,
                ButtonKind.Danger => Danger,
                ButtonKind.Ghost => Color.Transparent,
                _ => Forest
            };
            button.ForeColor = kind switch
            {
                ButtonKind.Primary or ButtonKind.Danger => Color.White,
                ButtonKind.Ghost => Muted,
                _ => Forest
            };
            button.Font = BodyFont(10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Dock = DockStyle.None;
            button.Image = null;
            button.BackgroundImage = null;
            button.BackgroundImageLayout = ImageLayout.None;
            button.UseVisualStyleBackColor = false;
            button.Padding = new Padding(12, 0, 12, 1);
            button.TextAlign = ContentAlignment.MiddleCenter;
            ApplyRoundedRegion(button, 10);
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.Font = BodyFont(10.5F);
            textBox.BackColor = Surface;
            textBox.ForeColor = Ink;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.SuspendLayout();
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Line;
            grid.RowHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 46;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Forest,
                ForeColor = Color.White,
                Font = BodyFont(9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 4, 10, 4),
                SelectionBackColor = Forest,
                SelectionForeColor = Color.White
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Surface,
                ForeColor = Ink,
                Font = BodyFont(9.5F),
                SelectionBackColor = SelectionSurface,
                SelectionForeColor = Ink,
                Padding = new Padding(10, 7, 10, 7),
                WrapMode = DataGridViewTriState.False
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AlternatingSurface,
                ForeColor = Ink,
                Font = BodyFont(9.5F),
                SelectionBackColor = SelectionSurface,
                SelectionForeColor = Ink,
                Padding = new Padding(10, 7, 10, 7)
            };
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowTemplate.Height = 56;
            grid.ResumeLayout();
        }

        private static void StyleList(ListView list)
        {
            list.BackColor = Surface;
            list.ForeColor = Ink;
            list.Font = BodyFont(9.5F);
            list.BorderStyle = BorderStyle.None;
            list.FullRowSelect = true;
            list.HideSelection = false;
            list.GridLines = false;
        }

        private static void PrepareScreen(Form form)
        {
            switch (form.Name)
            {
                case "Form1":
                    PrepareLanding(form);
                    break;
                case "Form2":
                    PrepareRegistration(form);
                    break;
                case "Form3":
                    PrepareMembers(form);
                    break;
                case "Form4":
                    PrepareSignIn(form);
                    break;
                case "Form5":
                    PrepareCatalogue(form);
                    break;
                case "Form6":
                    PrepareBooks(form);
                    break;
                case "Form7":
                    PrepareAddBook(form);
                    break;
                case "Form8":
                    PrepareReset(form);
                    break;
                case "Form9":
                    PrepareShelf(form);
                    break;
                case "Form10":
                    PreparePayments(form);
                    break;
                case "Passchg":
                    PreparePasswordChange(form);
                    break;
            }
        }

        private static void Arrange(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized || form.ClientSize.Width <= 0)
            {
                return;
            }

            form.SuspendLayout();

            switch (form.Name)
            {
                case "Form1":
                    ArrangeLanding(form);
                    break;
                case "Form2":
                    ArrangeRegistration(form);
                    break;
                case "Form3":
                    ArrangeMembers(form);
                    break;
                case "Form4":
                    ArrangeSignIn(form);
                    break;
                case "Form5":
                    ArrangeCatalogue(form);
                    break;
                case "Form6":
                    ArrangeBooks(form);
                    break;
                case "Form7":
                    ArrangeAddBook(form);
                    break;
                case "Form8":
                    ArrangeReset(form);
                    break;
                case "Form9":
                    ArrangeShelf(form);
                    break;
                case "Form10":
                    ArrangePayments(form);
                    break;
                case "Passchg":
                    ArrangePasswordChange(form);
                    break;
            }

            ApplyContentBackgrounds(form);
            ArrangeThemeToggle(form);
            form.ResumeLayout(true);
        }

        private static void PrepareLanding(Form form)
        {
            form.Size = new Size(1180, 720);
            form.MinimumSize = new Size(1000, 650);

            Panel? actions = Find<Panel>(form, "panel1");
            if (actions != null)
            {
                actions.BackColor = Surface;
                EnsureLabel(actions, "modernCardEyebrow", "YOUR LIBRARY, YOUR WAY", 10F, FontStyle.Bold, Forest);
                EnsureLabel(actions, "modernCardTitle", "Start your next chapter", 22F, FontStyle.Bold, Ink);
                EnsureLabel(actions, "modernCardCopy", "Sign in to return to your shelf, or create a free account in a minute.", 10F, FontStyle.Regular, Muted);
            }

            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernHeroEyebrow", "A QUIETER WAY TO COLLECT BOOKS", 10F, FontStyle.Bold, Terracotta);
            EnsureLabel(form, "modernHeroTitle", "Stories worth keeping,\nall in one place.", 34F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernHeroCopy", "Browse thoughtful picks, build a personal shelf, and come back whenever the reading mood strikes.", 11F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernHeroNote", "CURATED CATALOGUE  •  PERSONAL SHELVES  •  SIMPLE CHECKOUT", 9F, FontStyle.Bold, Forest);
            EnsureArt(form, "modernLandingArt");

            SetText<Button>(form, "button1", "Sign in");
            SetText<Button>(form, "button4", "Create an account");
            SetText<Button>(form, "button3", "Admin access");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button4"), ButtonKind.Outline);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Ghost);
        }

        private static void ArrangeLanding(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = Math.Max(48, w / 20);
            int cardWidth = Math.Min(410, Math.Max(360, w / 3));
            int cardHeight = Math.Min(470, h - 170);
            int cardX = w - margin - cardWidth;
            int cardY = Math.Max(100, (h - cardHeight) / 2);

            SetBounds(form, "modernBrand", margin, 34, 220, 28);
            SetBounds(form, "modernHeroEyebrow", margin, 132, Math.Max(420, cardX - margin - 40), 24);
            SetBounds(form, "modernHeroTitle", margin, 168, Math.Max(430, cardX - margin - 55), 150);
            SetBounds(form, "modernHeroCopy", margin, 330, Math.Max(430, cardX - margin - 75), 58);
            SetBounds(form, "modernHeroNote", margin, h - 72, Math.Max(450, cardX - margin - 30), 24);
            SetBounds(form, "modernLandingArt", margin, 398, Math.Max(420, cardX - margin - 70), Math.Max(115, h - 500));

            Panel? actions = Find<Panel>(form, "panel1");
            if (actions != null)
            {
                actions.Bounds = new Rectangle(cardX, cardY, cardWidth, cardHeight);
                ApplyRoundedRegion(actions, 22);
                SetBounds(actions, "modernCardEyebrow", 34, 34, cardWidth - 68, 24);
                SetBounds(actions, "modernCardTitle", 34, 70, cardWidth - 68, 88);
                SetBounds(actions, "modernCardCopy", 34, 160, cardWidth - 68, 54);
                SetBounds(actions, "button1", 34, 232, cardWidth - 68, 52);
                SetBounds(actions, "button4", 34, 298, cardWidth - 68, 52);
                SetBounds(actions, "button3", 34, 364, cardWidth - 68, 46);
            }
        }

        private static void PrepareSignIn(Form form)
        {
            form.Size = new Size(1000, 650);
            form.MinimumSize = new Size(900, 600);

            EnsureArt(form, "modernAuthArt");
            EnsureCard(form, "modernSignInCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernAuthQuote", "“A reader lives a thousand lives.”", 20F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernAuthByline", "— George R. R. Martin", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernSignInCopy", "Enter your details to return to your shelf.", 10F, FontStyle.Regular, Muted);
            EnsureCheckBox(form, "modernShowSignInPassword", "Show password");

            SetText<Label>(form, "label3", "Welcome back");
            SetText<Label>(form, "label1", "Username");
            SetText<Label>(form, "label2", "Password");
            SetText<Label>(form, "label4", "Forgot your password?");
            SetText<Button>(form, "button1", "Sign in");
            SetText<Button>(form, "button2", "←  Home");

            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Ghost);
            Label? forgot = Find<Label>(form, "label4");
            if (forgot != null)
            {
                forgot.ForeColor = Forest;
                forgot.Font = BodyFont(9.5F, FontStyle.Bold);
                forgot.Cursor = Cursors.Hand;
            }

            TextBox? password = Find<TextBox>(form, "textBox2");
            if (password != null)
            {
                password.UseSystemPasswordChar = true;
            }

            form.AcceptButton = Find<Button>(form, "button1");
        }

        private static void ArrangeSignIn(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int cardW = 420;
            int cardH = 490;
            int cardX = Math.Max(w / 2 + 40, w - cardW - 70);
            int cardY = Math.Max(70, (h - cardH) / 2);

            SetBounds(form, "modernBrand", 44, 36, 220, 28);
            SetBounds(form, "button2", 34, 78, 115, 38);
            SetBounds(form, "modernAuthArt", 44, 142, Math.Max(340, cardX - 100), 250);
            SetBounds(form, "modernAuthQuote", 64, h - 178, Math.Max(330, cardX - 120), 60);
            SetBounds(form, "modernAuthByline", 64, h - 112, 300, 28);
            SetBounds(form, "modernSignInCard", cardX, cardY, cardW, cardH);
            SetBounds(form, "label3", cardX + 42, cardY + 42, cardW - 84, 48);
            StyleDisplayLabel(Find<Label>(form, "label3"), 24F);
            SetBounds(form, "modernSignInCopy", cardX + 42, cardY + 94, cardW - 84, 32);
            SetBounds(form, "label1", cardX + 42, cardY + 145, cardW - 84, 24);
            SetBounds(form, "textBox1", cardX + 42, cardY + 175, cardW - 84, 38);
            SetBounds(form, "label2", cardX + 42, cardY + 235, cardW - 84, 24);
            SetBounds(form, "textBox2", cardX + 42, cardY + 265, cardW - 84, 38);
            SetBounds(form, "modernShowSignInPassword", cardX + 42, cardY + 313, cardW - 84, 28);
            SetBounds(form, "button1", cardX + 42, cardY + 350, cardW - 84, 50);
            SetBounds(form, "label4", cardX + 42, cardY + 421, cardW - 84, 28);
            CenterText(Find<Label>(form, "label4"));
        }

        private static void PrepareRegistration(Form form)
        {
            form.Size = new Size(1200, 800);
            form.MinimumSize = new Size(1080, 740);
            EnsureCard(form, "modernRegistrationCard");
            EnsureCard(form, "modernPhotoCard");
            EnsureCard(form, "modernRecentCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernRegistrationCopy", "A few details and your personal shelf is ready.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernRecentTitle", "Recently registered", 11F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernPhotoCopy", "Add a profile photo so your shelf feels like yours.", 9.5F, FontStyle.Regular, Muted);

            SetText<Label>(form, "label8", "Create your account");
            SetText<Label>(form, "label1", "First name");
            SetText<Label>(form, "label2", "Last name");
            SetText<Label>(form, "label3", "Phone number");
            SetText<Label>(form, "label4", "Date of birth");
            SetText<Label>(form, "label5", "Gender");
            SetText<Label>(form, "label6", "Password");
            SetText<Label>(form, "label7", "Confirm password");
            SetText<Label>(form, "label9", "Profile photo");
            SetText<Label>(form, "label10", "Username");
            SetText<Button>(form, "button1", "Create account");
            SetText<Button>(form, "button2", "←  Home");
            SetText<Button>(form, "button4", "Choose photo");
            SetText<RadioButton>(form, "radioButton1", "Male");
            SetText<RadioButton>(form, "radioButton2", "Female");
            SetText<CheckBox>(form, "checkBox1", "Show passwords");

            Button? optionalEmail = Find<Button>(form, "button3");
            if (optionalEmail != null)
            {
                optionalEmail.Visible = false;
            }

            Label? emailLabel = Find<Label>(form, "emailLabel");
            if (emailLabel != null)
            {
                emailLabel.Text = "Email address";
            }

            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Ghost);
            StyleButton(Find<Button>(form, "button4"), ButtonKind.Outline);
            form.AcceptButton = Find<Button>(form, "button1");
        }

        private static void ArrangeRegistration(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 38;
            int sideW = 310;
            int gap = 24;
            int mainX = margin;
            int mainY = 126;
            int mainW = w - (margin * 2) - sideW - gap;
            int mainH = h - mainY - 34;
            int sideX = mainX + mainW + gap;

            SetBounds(form, "modernBrand", margin, 30, 220, 28);
            SetBounds(form, "label8", margin, 65, 500, 42);
            StyleDisplayLabel(Find<Label>(form, "label8"), 24F);
            SetBounds(form, "modernRegistrationCopy", margin + 300, 72, 480, 28);
            SetBounds(form, "button2", w - margin - 110, 40, 110, 38);
            SetBounds(form, "modernRegistrationCard", mainX, mainY, mainW, mainH);
            SetBounds(form, "modernPhotoCard", sideX, mainY, sideW, 390);
            SetBounds(form, "modernRecentCard", sideX, mainY + 414, sideW, Math.Max(150, mainH - 414));

            int innerX = mainX + 34;
            int colGap = 34;
            int colW = (mainW - 68 - colGap) / 2;
            int col2 = innerX + colW + colGap;
            int fieldTop = mainY + 34;
            int row = 89;

            LayoutField(form, "label1", "textBox1", innerX, fieldTop, colW);
            LayoutField(form, "label3", "textBox3", col2, fieldTop, colW);
            LayoutField(form, "label2", "textBox2", innerX, fieldTop + row, colW);
            LayoutField(form, "emailLabel", "emailTextBox", col2, fieldTop + row, colW);
            LayoutField(form, "label10", "textBox6", innerX, fieldTop + row * 2, colW);
            LayoutField(form, "label4", "dateTimePicker1", col2, fieldTop + row * 2, colW);
            LayoutField(form, "label6", "textBox4", innerX, fieldTop + row * 3, colW);
            SetBounds(form, "label5", col2, fieldTop + row * 3, colW, 22);
            SetBounds(form, "radioButton1", col2, fieldTop + row * 3 + 31, 100, 32);
            SetBounds(form, "radioButton2", col2 + 116, fieldTop + row * 3 + 31, 110, 32);
            LayoutField(form, "label7", "textBox5", innerX, fieldTop + row * 4, colW);
            SetBounds(form, "checkBox1", col2, fieldTop + row * 4 + 28, colW, 30);
            SetBounds(form, "button1", col2, mainY + mainH - 78, colW, 48);

            SetBounds(form, "label9", sideX + 26, mainY + 26, sideW - 52, 28);
            StyleSectionLabel(Find<Label>(form, "label9"));
            SetBounds(form, "modernPhotoCopy", sideX + 26, mainY + 58, sideW - 52, 44);
            SetBounds(form, "pictureBox1", sideX + 54, mainY + 116, sideW - 108, 190);
            ApplyRoundedRegion(Find<PictureBox>(form, "pictureBox1"), 18);
            SetBounds(form, "button4", sideX + 54, mainY + 324, sideW - 108, 42);
            SetBounds(form, "modernRecentTitle", sideX + 24, mainY + 438, sideW - 48, 26);
            SetBounds(form, "listView1", sideX + 18, mainY + 476, sideW - 36, Math.Max(96, mainH - 492));
        }

        private static void PrepareMembers(Form form)
        {
            form.Size = new Size(1300, 780);
            form.MinimumSize = new Size(1120, 720);
            EnsureCard(form, "modernMemberTableCard");
            EnsureCard(form, "modernMemberEditCard");
            EnsureLabel(form, "modernMembersCopy", "Search the list, select a member, then update only the details that changed.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernEditTitle", "Member details", 13F, FontStyle.Bold, Ink);
            EnsureSearchBox(form, "modernMemberSearch", "Search members...");
            EnsureLabel(form, "modernMemberResults", "0 members", 9F, FontStyle.Bold, Muted);

            SetText<Label>(form, "label8", "Member management");
            SetText<Label>(form, "label1", "First name");
            SetText<Label>(form, "label2", "Last name");
            SetText<Label>(form, "label3", "Phone number");
            SetText<Label>(form, "label4", "Gender");
            SetText<Label>(form, "label5", "New password");
            SetText<Label>(form, "label6", "Profile photo");
            SetText<Label>(form, "label7", "Date of birth");
            SetText<Label>(form, "label9", "Email address");
            SetText<Label>(form, "label10", "Username");
            SetText<Button>(form, "button1", "Save changes");
            SetText<Button>(form, "button2", "Overview");
            SetText<Button>(form, "button3", "Delete member");
            SetText<Button>(form, "button4", "Choose photo");
            SetText<Button>(form, "button5", "Books");
            SetText<Button>(form, "button6", "Payments");

            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Danger);
            StyleButton(Find<Button>(form, "button4"), ButtonKind.Outline);
            PrepareAdminSidebar(form, "panel1", "Members");
        }

        private static void ArrangeMembers(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int sidebar = 214;
            int x = sidebar + 38;
            int contentW = w - x - 38;

            ArrangeAdminSidebar(form, "panel1", sidebar, h, "button2", "button5", "button6");
            SetBounds(form, "label8", x, 34, 520, 44);
            StyleDisplayLabel(Find<Label>(form, "label8"), 25F);
            SetBounds(form, "modernMembersCopy", x, 80, Math.Max(500, contentW - 210), 28);
            SetBounds(form, "button3", x + contentW - 156, 52, 156, 42);
            SetBounds(form, "modernMemberSearch", x, 122, 360, 38);
            SetBounds(form, "modernMemberResults", x + 378, 129, 190, 24);
            SetBounds(form, "modernMemberTableCard", x, 174, contentW, 244);
            SetBounds(form, "listView1", x + 18, 192, contentW - 36, 208);
            SetBounds(form, "modernMemberEditCard", x, 438, contentW, h - 470);
            SetBounds(form, "modernEditTitle", x + 28, 460, 220, 28);

            int innerX = x + 28;
            int editY = 500;
            int colGap = 24;
            int colW = (contentW - 56 - colGap * 2) / 3;
            int c2 = innerX + colW + colGap;
            int c3 = c2 + colW + colGap;

            LayoutCompactField(form, "label1", "textBox1", innerX, editY, colW);
            LayoutCompactField(form, "label9", "textBox5", c2, editY, colW);
            LayoutCompactField(form, "label7", "dateTimePicker1", c3, editY, colW);
            LayoutCompactField(form, "label2", "textBox2", innerX, editY + 76, colW);
            LayoutCompactField(form, "label10", "textBox6", c2, editY + 76, colW);
            SetBounds(form, "label4", c3, editY + 76, colW, 21);
            SetBounds(form, "radioButton1", c3, editY + 102, 88, 31);
            SetBounds(form, "radioButton2", c3 + 96, editY + 102, 100, 31);
            LayoutCompactField(form, "label3", "textBox3", innerX, editY + 152, colW);
            LayoutCompactField(form, "label5", "textBox4", c2, editY + 152, colW);
            SetBounds(form, "label6", c3, editY + 152, colW, 21);
            SetBounds(form, "button4", c3, editY + 178, colW, 36);
            SetBounds(form, "button1", x + contentW - 190, h - 76, 162, 44);
        }

        private static void PrepareCatalogue(Form form)
        {
            form.Size = new Size(1200, 740);
            form.MinimumSize = new Size(1000, 650);
            EnsureCard(form, "modernCatalogueCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 10F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernCatalogueCopy", "Select the books that catch your eye, then add them to your shelf.", 10F, FontStyle.Regular, Muted);
            EnsureSearchBox(form, "modernCatalogueSearch", "Search by title, author or price...");
            EnsureLabel(form, "modernCatalogueResults", "0 books", 9F, FontStyle.Bold, Muted);

            SetText<Label>(form, "label1", "Explore the catalogue");
            SetText<Button>(form, "button1", "Sign out");
            SetText<Button>(form, "button2", "Add selected to shelf");
            SetText<Button>(form, "button3", "My shelf");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Ghost);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Secondary);
        }

        private static void ArrangeCatalogue(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 38;
            SetBounds(form, "modernBrand", margin, 26, 190, 26);
            SetBounds(form, "label1", margin, 58, 520, 44);
            StyleDisplayLabel(Find<Label>(form, "label1"), 25F);
            SetBounds(form, "modernCatalogueCopy", margin, 102, 620, 28);
            SetBounds(form, "button1", w - margin - 98, 34, 98, 38);
            SetBounds(form, "button3", w - margin - 232, 34, 122, 38);
            SetBounds(form, "modernCatalogueSearch", margin, 140, 420, 38);
            SetBounds(form, "modernCatalogueResults", margin + 438, 147, 250, 24);
            SetBounds(form, "modernCatalogueCard", margin, 194, w - margin * 2, h - 264);
            SetBounds(form, "dataGridView1", margin + 18, 212, w - margin * 2 - 36, h - 300);
            SetBounds(form, "button2", w - margin - 226, h - 58, 226, 44);
        }

        private static void PrepareBooks(Form form)
        {
            form.Size = new Size(1280, 760);
            form.MinimumSize = new Size(1100, 690);
            EnsureCard(form, "modernBookEditorCard");
            EnsureCard(form, "modernBookTableCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER  /  ADMIN", 10F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernBooksTitle", "Book catalogue", 25F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernBooksCopy", "Select a row to edit its details, or add a new title to the collection.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernBookEditorTitle", "Edit selected book", 13F, FontStyle.Bold, Ink);
            EnsureSearchBox(form, "modernBookSearch", "Search the catalogue...");
            EnsureLabel(form, "modernBookResults", "0 titles", 9F, FontStyle.Bold, Muted);

            SetText<Label>(form, "label1", "Book title");
            SetText<Label>(form, "label2", "Author");
            SetText<Label>(form, "label3", "Price");
            SetText<Label>(form, "label4", "Quantity");
            SetText<Label>(form, "label5", "Publish date");
            SetText<Button>(form, "button1", "←  Members");
            SetText<Button>(form, "button3", "+  Add book");
            SetText<Button>(form, "button2", "Save changes");
            SetText<Button>(form, "button4", "Delete book");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Ghost);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Secondary);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button4"), ButtonKind.Danger);
        }

        private static void ArrangeBooks(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 36;
            int editorW = 300;
            int gridX = margin + editorW + 24;
            int gridW = w - gridX - margin;

            SetBounds(form, "modernBrand", margin, 24, 260, 24);
            SetBounds(form, "modernBooksTitle", margin, 54, 420, 42);
            SetBounds(form, "modernBooksCopy", margin, 96, 650, 28);
            SetBounds(form, "button1", w - margin - 236, 34, 110, 38);
            SetBounds(form, "button3", w - margin - 116, 34, 116, 38);
            SetBounds(form, "modernBookSearch", gridX, 98, Math.Min(360, gridW - 190), 36);
            SetBounds(form, "modernBookResults", gridX + Math.Min(378, gridW - 172), 104, 170, 24);
            SetBounds(form, "modernBookEditorCard", margin, 150, editorW, h - 186);
            SetBounds(form, "modernBookTableCard", gridX, 150, gridW, h - 186);
            SetBounds(form, "modernBookEditorTitle", margin + 24, 174, editorW - 48, 28);

            int fx = margin + 24;
            int fw = editorW - 48;
            LayoutCompactField(form, "label1", "textBox1", fx, 218, fw);
            LayoutCompactField(form, "label2", "textBox2", fx, 294, fw);
            LayoutCompactField(form, "label3", "textBox3", fx, 370, fw);
            LayoutCompactField(form, "label4", "numericUpDown1", fx, 446, fw);
            LayoutCompactField(form, "label5", "dateTimePicker1", fx, 522, fw);
            SetBounds(form, "button2", fx, h - 118, fw, 42);
            SetBounds(form, "button4", fx, h - 68, fw, 38);

            SetBounds(form, "dataGridView1", gridX + 16, 166, gridW - 32, h - 218);
        }

        private static void PrepareAddBook(Form form)
        {
            form.Size = new Size(1120, 720);
            form.MinimumSize = new Size(1000, 660);
            EnsureCard(form, "modernAddBookCard");
            EnsureCard(form, "modernCoverCard");
            EnsureLabel(form, "modernAddBookCopy", "Add the details readers need to discover this title.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernCoverTitle", "Cover artwork", 13F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernCoverCopy", "Use a portrait image with a clear title for the best result.", 9.5F, FontStyle.Regular, Muted);

            SetText<Label>(form, "label6", "Add a new book");
            SetText<Label>(form, "label1", "Book title");
            SetText<Label>(form, "label2", "Author");
            SetText<Label>(form, "label3", "Price");
            SetText<Label>(form, "label4", "Quantity");
            SetText<Label>(form, "label5", "Publish date");
            SetText<Button>(form, "button1", "Add to catalogue");
            SetText<Button>(form, "button2", "Choose cover");
            SetText<Button>(form, "button3", "←  Back to books");

            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Outline);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Ghost);
            form.AcceptButton = Find<Button>(form, "button1");
        }

        private static void ArrangeAddBook(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 50;
            int header = 104;
            int gap = 28;
            int coverW = 340;
            int cardW = w - margin * 2 - gap - coverW;
            int cardY = 132;
            int cardH = h - cardY - 38;
            int coverX = margin + cardW + gap;

            Panel? headerPanel = Find<Panel>(form, "panel1");
            if (headerPanel != null)
            {
                headerPanel.Bounds = new Rectangle(0, 0, w, header);
                headerPanel.BackColor = Surface;
                headerPanel.Region = null;
                SetBounds(headerPanel, "label6", margin, 24, 440, 44);
                StyleDisplayLabel(Find<Label>(headerPanel, "label6"), 25F);
                SetBounds(headerPanel, "button3", w - margin - 150, 30, 150, 42);
            }

            SetBounds(form, "modernAddBookCopy", margin, 74, 560, 26);
            SetBounds(form, "modernAddBookCard", margin, cardY, cardW, cardH);
            SetBounds(form, "modernCoverCard", coverX, cardY, coverW, cardH);

            int fx = margin + 34;
            int fw = cardW - 68;
            LayoutField(form, "label1", "textBox1", fx, cardY + 34, fw);
            LayoutField(form, "label2", "textBox2", fx, cardY + 123, fw);
            LayoutField(form, "label3", "textBox3", fx, cardY + 212, fw);
            LayoutField(form, "label4", "numericUpDown1", fx, cardY + 301, fw);
            LayoutField(form, "label5", "dateTimePicker1", fx, cardY + 390, fw);
            SetBounds(form, "button1", fx, cardY + cardH - 68, fw, 46);

            SetBounds(form, "modernCoverTitle", coverX + 30, cardY + 30, coverW - 60, 28);
            SetBounds(form, "modernCoverCopy", coverX + 30, cardY + 66, coverW - 60, 48);
            SetBounds(form, "pictureBox1", coverX + 56, cardY + 128, coverW - 112, Math.Max(220, cardH - 236));
            ApplyRoundedRegion(Find<PictureBox>(form, "pictureBox1"), 16);
            SetBounds(form, "button2", coverX + 56, cardY + cardH - 68, coverW - 112, 42);
        }

        private static void PrepareReset(Form form)
        {
            form.Size = new Size(960, 650);
            form.MinimumSize = new Size(860, 600);
            EnsureCard(form, "modernResetCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernResetCopy", "Enter the email connected to your account. We’ll send you a one-time verification code.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernResetHelp", "The code expires shortly for your security.", 9F, FontStyle.Regular, Muted);

            SetText<Label>(form, "label4", "Reset your password");
            SetText<Label>(form, "label1", "Email address");
            SetText<Label>(form, "label2", "Verification code");
            SetText<Label>(form, "label3", "Ready");
            SetText<Button>(form, "button1", "Send verification code");
            SetText<Button>(form, "button2", "Verify and continue");
            SetText<Button>(form, "button3", "←  Back to sign in");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Outline);
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Primary);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Ghost);

            Label? status = Find<Label>(form, "label3");
            if (status != null)
            {
                status.BackColor = Color.Transparent;
                status.ForeColor = Forest;
                status.Font = BodyFont(9F, FontStyle.Bold);
                status.TextAlign = ContentAlignment.MiddleCenter;
            }

            form.AcceptButton = Find<Button>(form, "button2");
        }

        private static void ArrangeReset(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int cardW = 500;
            int cardH = 510;
            int x = (w - cardW) / 2;
            int y = Math.Max(80, (h - cardH) / 2 + 20);

            SetBounds(form, "modernBrand", 38, 28, 220, 28);
            SetBounds(form, "button3", w - 38 - 162, 22, 162, 38);
            SetBounds(form, "modernResetCard", x, y, cardW, cardH);
            SetBounds(form, "label4", x + 42, y + 38, cardW - 84, 46);
            StyleDisplayLabel(Find<Label>(form, "label4"), 23F);
            SetBounds(form, "modernResetCopy", x + 42, y + 88, cardW - 84, 55);
            LayoutField(form, "label1", "textBox1", x + 42, y + 166, cardW - 84);
            SetBounds(form, "button1", x + 42, y + 252, cardW - 84, 42);
            LayoutField(form, "label2", "textBox2", x + 42, y + 320, cardW - 164);
            SetBounds(form, "label3", x + cardW - 106, y + 347, 64, 38);
            SetBounds(form, "button2", x + 42, y + 416, cardW - 84, 46);
            SetBounds(form, "modernResetHelp", x + 42, y + 470, cardW - 84, 24);
            CenterText(Find<Label>(form, "modernResetHelp"));
        }

        private static void PrepareShelf(Form form)
        {
            form.Size = new Size(1260, 800);
            form.MinimumSize = new Size(1060, 700);
            EnsureShelfHero(form, "modernShelfHero");
            EnsureCard(form, "modernShelfCard");
            EnsureCard(form, "modernShelfCountCard");
            EnsureCard(form, "modernShelfValueCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 10F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernShelfKicker", "YOUR PERSONAL COLLECTION", 9F, FontStyle.Bold, TerracottaSoft);
            EnsureLabel(form, "modernShelfCopy", "A quiet place for the stories you want to keep close.", 10.5F, FontStyle.Regular, Sage);
            EnsureSearchBox(form, "modernShelfSearch", "Search your shelf...");
            EnsureLabel(form, "modernShelfResults", "0 saved books", 9F, FontStyle.Bold, Muted);
            EnsureLabel(form, "modernShelfCountLabel", "SAVED TITLES", 8F, FontStyle.Bold, Muted);
            EnsureLabel(form, "modernShelfCountValue", "0", 22F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernShelfValueLabel", "COLLECTION VALUE", 8F, FontStyle.Bold, Muted);
            EnsureLabel(form, "modernShelfValueValue", "—", 18F, FontStyle.Bold, Forest);

            SetText<Label>(form, "label1", "Your reading shelf");
            SetText<Label>(form, "label2", "SIGNED IN AS");
            SetText<Button>(form, "button2", "+  Explore catalogue");
            SetText<Button>(form, "button3", "Sign out");
            StyleButton(Find<Button>(form, "button2"), ButtonKind.Secondary);
            StyleButton(Find<Button>(form, "button3"), ButtonKind.Ghost);
            Button? legacyRemove = Find<Button>(form, "button1");
            if (legacyRemove != null)
            {
                legacyRemove.Visible = false;
            }
            DataGridView? legacyGrid = Find<DataGridView>(form, "dataGridView1");
            if (legacyGrid != null)
            {
                legacyGrid.Visible = false;
                legacyGrid.TabStop = false;
            }

            Label? user = Find<Label>(form, "label3");
            if (user != null)
            {
                user.Font = BodyFont(10.5F, FontStyle.Bold);
                user.ForeColor = Ink;
            }
        }

        private static void ArrangeShelf(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 40;
            int heroX = margin;
            int heroY = 72;
            int heroW = w - margin * 2;

            SetBounds(form, "modernBrand", margin + 8, 24, 190, 24);
            SetBounds(form, "button3", w - margin - 90, 20, 90, 36);
            SetBounds(form, "modernShelfHero", heroX, heroY, heroW, 198);
            Reparent(form, "modernShelfHero", "modernShelfKicker", "label1", "modernShelfCopy",
                "button2", "label2", "label3", "pictureBox1");

            SetBounds(form, "pictureBox1", heroW - 54, 0, 54, 54);
            ApplyRoundedRegion(Find<PictureBox>(form, "pictureBox1"), 27);
            SetBounds(form, "label2", heroW - 250, 1, 174, 18);
            Label? signedIn = Find<Label>(form, "label2");
            if (signedIn != null)
            {
                signedIn.Font = BodyFont(7.5F, FontStyle.Bold);
                signedIn.ForeColor = Sage;
                signedIn.TextAlign = ContentAlignment.MiddleRight;
            }
            SetBounds(form, "label3", heroW - 250, 20, 174, 26);
            Label? user = Find<Label>(form, "label3");
            if (user != null)
            {
                user.TextAlign = ContentAlignment.MiddleRight;
                user.ForeColor = Color.White;
            }

            SetBounds(form, "modernShelfKicker", 32, 29, 360, 22);
            SetBounds(form, "label1", 32, 54, 560, 56);
            StyleDisplayLabel(Find<Label>(form, "label1"), 29F);
            Label? title = Find<Label>(form, "label1");
            if (title != null) title.ForeColor = Color.White;
            SetBounds(form, "modernShelfCopy", 34, 116, 560, 32);
            SetBounds(form, "button2", heroW - 224, 124, 188, 42);

            SetBounds(form, "modernShelfCountCard", heroX + 30, heroY + 158, 190, 92);
            SetBounds(form, "modernShelfValueCard", heroX + 236, heroY + 158, 210, 92);
            Reparent(form, "modernShelfCountCard", "modernShelfCountLabel", "modernShelfCountValue");
            Reparent(form, "modernShelfValueCard", "modernShelfValueLabel", "modernShelfValueValue");
            SetBounds(form, "modernShelfCountLabel", 24, 19, 145, 18);
            SetBounds(form, "modernShelfCountValue", 24, 40, 145, 38);
            SetBounds(form, "modernShelfValueLabel", 24, 19, 165, 18);
            SetBounds(form, "modernShelfValueValue", 24, 42, 165, 34);

            SetBounds(form, "modernShelfSearch", heroX + 30, heroY + 276, 410, 40);
            SetBounds(form, "modernShelfResults", heroX + 458, heroY + 284, 300, 24);
            int galleryY = heroY + 336;
            SetBounds(form, "modernShelfCard", heroX, galleryY, heroW, Math.Max(220, h - galleryY - 30));
            SetBounds(form, "modernShelfGallery", heroX + 18, galleryY + 18, heroW - 36, Math.Max(180, h - galleryY - 66));
            SetBounds(form, "dataGridView1", 0, 0, 1, 1);

            // Keep the decorative surfaces behind the interactive content. WinForms z-order can
            // change as generated controls are added, so make the intended layering explicit.
            Find<ShelfHeroPanel>(form, "modernShelfHero")?.SendToBack();
            BringFront(form, "modernShelfKicker", "label1", "modernShelfCopy", "button2",
                "label2", "label3", "pictureBox1");
            BringFront(form, "modernShelfCountCard");
            BringFront(form, "modernShelfCountLabel", "modernShelfCountValue");
            BringFront(form, "modernShelfValueCard");
            BringFront(form, "modernShelfValueLabel", "modernShelfValueValue");
            BringFront(form, "modernShelfCard");
            BringFront(form, "modernShelfGallery");
            BringFront(form, "modernShelfSearch", "modernShelfResults", "modernBrand", "button3");
        }

        private static void PreparePayments(Form form)
        {
            form.Size = new Size(1280, 820);
            form.MinimumSize = new Size(1080, 700);
            EnsureCard(form, "modernPaymentCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER  /  ADMIN", 10F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernPaymentCopy", "A complete view of recorded revenue, title performance and customer activity.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernReportBadge", "LIVE SALES ANALYSIS", 9F, FontStyle.Bold, Forest);
            EnsureSearchBox(form, "modernPaymentSearch", "Search customers, books or authors...");
            EnsureLabel(form, "modernPaymentResults", "0 transactions", 9F, FontStyle.Bold, Muted);
            EnsureFeatureButton(form, "modernExportPayments", "Export CSV", ButtonKind.Outline);
            EnsureLabel(form, "modernTransactionTitle", "Transaction details", 12F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernTransactionCopy", "Searchable line items behind the analysis above.", 8.5F, FontStyle.Regular, Muted);

            SetText<Label>(form, "label1", "Sales analytics");
            SetText<Button>(form, "button1", "←  Members");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Ghost);
        }

        private static void ArrangePayments(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int margin = 40;
            SetBounds(form, "modernBrand", margin, 24, 260, 24);
            SetBounds(form, "label1", margin, 55, 360, 44);
            StyleDisplayLabel(Find<Label>(form, "label1"), 25F);
            SetBounds(form, "modernPaymentCopy", margin, 99, 660, 28);
            SetBounds(form, "modernReportBadge", w - margin - 190, 88, 190, 28);
            Label? badge = Find<Label>(form, "modernReportBadge");
            if (badge != null)
            {
                badge.BackColor = Color.Transparent;
                badge.TextAlign = ContentAlignment.MiddleCenter;
                badge.Region = null;
            }
            SetBounds(form, "button1", w - margin - 120, 30, 120, 38);
            SetBounds(form, "modernPaymentSearch", margin, 138, 360, 38);
            SetBounds(form, "modernPaymentResults", margin + 378, 145, 210, 24);
            SetBounds(form, "modernExportPayments", w - margin - 132, 136, 132, 40);

            int dashboardY = 192;
            int dashboardHeight = Math.Clamp(h / 2 - 40, 320, 360);
            SetBounds(form, "modernSalesDashboard", margin, dashboardY, w - margin * 2, dashboardHeight);

            int transactionTitleY = dashboardY + dashboardHeight + 16;
            SetBounds(form, "modernTransactionTitle", margin, transactionTitleY, 220, 27);
            SetBounds(form, "modernTransactionCopy", margin + 224, transactionTitleY + 3, 420, 22);

            int cardY = transactionTitleY + 36;
            SetBounds(form, "modernPaymentCard", margin, cardY, w - margin * 2, Math.Max(120, h - cardY - 30));
            SetBounds(form, "dataGridView1", margin + 18, cardY + 18, w - margin * 2 - 36, Math.Max(80, h - cardY - 66));

            BringFront(form, "modernSalesDashboard", "modernTransactionTitle", "modernTransactionCopy",
                "dataGridView1", "modernPaymentSearch", "modernPaymentResults", "modernExportPayments",
                "modernBrand", "label1", "modernPaymentCopy", "modernReportBadge", "button1");
        }

        private static void PreparePasswordChange(Form form)
        {
            form.Size = new Size(900, 600);
            form.MinimumSize = new Size(800, 560);
            EnsureCard(form, "modernPasswordCard");
            EnsureLabel(form, "modernBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Forest);
            EnsureLabel(form, "modernPasswordTitle", "Choose a new password", 23F, FontStyle.Bold, Ink);
            EnsureLabel(form, "modernPasswordCopy", "Use a strong password you haven’t used for this account before.", 10F, FontStyle.Regular, Muted);
            EnsureLabel(form, "modernPasswordHint", "Use at least 8 characters with a mix of letters, numbers and symbols.", 9F, FontStyle.Regular, Muted);
            EnsureCheckBox(form, "modernShowNewPasswords", "Show passwords");

            SetText<Label>(form, "label1", "New password");
            SetText<Label>(form, "label2", "Confirm new password");
            SetText<Button>(form, "button1", "Update password");
            StyleButton(Find<Button>(form, "button1"), ButtonKind.Primary);

            TextBox? password = Find<TextBox>(form, "textBox1");
            TextBox? confirmation = Find<TextBox>(form, "textBox2");
            if (password != null) password.UseSystemPasswordChar = true;
            if (confirmation != null) confirmation.UseSystemPasswordChar = true;
            form.AcceptButton = Find<Button>(form, "button1");
        }

        private static void ArrangePasswordChange(Form form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int cardW = 500;
            int cardH = 450;
            int x = (w - cardW) / 2;
            int y = Math.Max(78, (h - cardH) / 2 + 20);
            SetBounds(form, "modernBrand", 38, 28, 220, 28);
            SetBounds(form, "modernPasswordCard", x, y, cardW, cardH);
            SetBounds(form, "modernPasswordTitle", x + 42, y + 38, cardW - 84, 42);
            SetBounds(form, "modernPasswordCopy", x + 42, y + 83, cardW - 84, 48);
            LayoutField(form, "label1", "textBox1", x + 42, y + 151, cardW - 84);
            LayoutField(form, "label2", "textBox2", x + 42, y + 243, cardW - 84);
            SetBounds(form, "modernShowNewPasswords", x + 42, y + 321, cardW - 84, 28);
            SetBounds(form, "modernPasswordHint", x + 42, y + 353, cardW - 84, 38);
            SetBounds(form, "button1", x + 42, y + 397, cardW - 84, 46);
        }

        private static void PrepareAdminSidebar(Form form, string panelName, string active)
        {
            Panel? panel = Find<Panel>(form, panelName);
            if (panel == null)
            {
                return;
            }

            panel.BackColor = ForestDark;
            EnsureLabel(panel, "modernAdminBrand", "LEAF & LETTER", 11F, FontStyle.Bold, Color.White);
            EnsureLabel(panel, "modernAdminLabel", "ADMIN WORKSPACE", 8F, FontStyle.Bold, Sage);
            EnsureLabel(panel, "modernAdminActive", active.ToUpperInvariant(), 8F, FontStyle.Bold, TerracottaSoft);

            foreach (Button button in panel.Controls.OfType<Button>())
            {
                StyleButton(button, ButtonKind.Ghost);
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(49, 99, 84);
                button.TextAlign = ContentAlignment.MiddleLeft;
            }
        }

        private static void ArrangeAdminSidebar(Form form, string panelName, int width, int height, params string[] buttonNames)
        {
            Panel? panel = Find<Panel>(form, panelName);
            if (panel == null)
            {
                return;
            }

            panel.Bounds = new Rectangle(0, 0, width, height);
            panel.Region = null;
            SetBounds(panel, "modernAdminBrand", 26, 30, width - 52, 28);
            SetBounds(panel, "modernAdminLabel", 26, 63, width - 52, 22);
            SetBounds(panel, "modernAdminActive", 26, 105, width - 52, 22);

            int y = 142;
            foreach (string name in buttonNames)
            {
                SetBounds(panel, name, 16, y, width - 32, 46);
                y += 54;
            }
        }

        private static void LayoutField(Form form, string labelName, string inputName, int x, int y, int width)
        {
            SetBounds(form, labelName, x, y, width, 22);
            SetBounds(form, inputName, x, y + 28, width, 38);
        }

        private static void LayoutCompactField(Form form, string labelName, string inputName, int x, int y, int width)
        {
            SetBounds(form, labelName, x, y, width, 20);
            SetBounds(form, inputName, x, y + 24, width, 34);
        }

        private static TextBox EnsureSearchBox(Form form, string name, string placeholder)
        {
            TextBox? existing = Find<TextBox>(form, name);
            if (existing != null)
            {
                existing.PlaceholderText = placeholder;
                return existing;
            }

            TextBox search = new TextBox
            {
                Name = name,
                PlaceholderText = placeholder,
                AccessibleName = placeholder,
                TabIndex = 1
            };
            StyleTextBox(search);
            form.Controls.Add(search);
            search.BringToFront();
            return search;
        }

        private static CheckBox EnsureCheckBox(Form form, string name, string text)
        {
            CheckBox? existing = Find<CheckBox>(form, name);
            if (existing != null)
            {
                existing.Text = text;
                return existing;
            }

            CheckBox checkBox = new CheckBox
            {
                Name = name,
                Text = text,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Muted,
                Font = BodyFont(9.5F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                AccessibleName = text
            };
            form.Controls.Add(checkBox);
            checkBox.BringToFront();
            return checkBox;
        }

        private static Button EnsureFeatureButton(Form form, string name, string text, ButtonKind kind)
        {
            Button? existing = Find<Button>(form, name);
            if (existing != null)
            {
                existing.Text = text;
                StyleButton(existing, kind);
                return existing;
            }

            Button button = new Button
            {
                Name = name,
                Text = text,
                AccessibleName = text
            };
            StyleButton(button, kind);
            form.Controls.Add(button);
            button.BringToFront();
            return button;
        }

        private static void EnsureThemeToggle(Form form)
        {
            Button? existing = Find<Button>(form, "modernThemeToggle");
            if (existing != null)
            {
                existing.Text = darkMode ? "Light" : "Dark";
                existing.AccessibleName = darkMode
                    ? "Switch to light mode"
                    : "Switch to dark mode";
                StyleButton(existing, ButtonKind.Outline);
                return;
            }

            Button toggle = new Button
            {
                Name = "modernThemeToggle",
                Text = darkMode ? "Light" : "Dark",
                AccessibleName = darkMode
                    ? "Switch to light mode"
                    : "Switch to dark mode",
                TabStop = true
            };
            toggle.Click += (_, _) => ToggleMode();
            StyleButton(toggle, ButtonKind.Outline);
            form.Controls.Add(toggle);
            toggle.BringToFront();
        }

        private static void ArrangeThemeToggle(Form form)
        {
            int x = Math.Max(16, form.ClientSize.Width - 330);
            SetBounds(form, "modernThemeToggle", x, 20, 132, 36);
            Find<Button>(form, "modernThemeToggle")?.BringToFront();
        }

        private static void ApplyContentBackgrounds(Form form)
        {
            List<Panel> backgroundPanels = form.Controls
                .OfType<Panel>()
                .Where(panel =>
                    panel.Visible &&
                    panel.BackColor != Color.Transparent &&
                    panel.Width > 0 &&
                    panel.Height > 0)
                .ToList();

            foreach (Control control in form.Controls)
            {
                if (control.Parent != form ||
                    control is not Label and not CheckBox and not RadioButton)
                {
                    continue;
                }

                Point center = new Point(
                    control.Left + control.Width / 2,
                    control.Top + control.Height / 2);
                Panel? underlyingPanel = backgroundPanels
                    .Where(panel => panel.Bounds.Contains(center))
                    .OrderBy(panel => (long)panel.Width * panel.Height)
                    .FirstOrDefault();

                // WinForms transparency only reveals the parent, not a sibling panel behind the
                // control. Matching that panel's fill removes dark/light rectangles around text.
                control.BackColor = underlyingPanel?.BackColor ?? Canvas;
            }
        }

        private static void EnsureCard(Form form, string name)
        {
            if (Find<SurfacePanel>(form, name) != null)
            {
                return;
            }

            SurfacePanel panel = new SurfacePanel
            {
                Name = name,
                BackColor = Surface,
                TabStop = false
            };
            form.Controls.Add(panel);
            panel.SendToBack();
        }

        private static void EnsureArt(Form form, string name)
        {
            if (Find<LibraryArtPanel>(form, name) != null)
            {
                return;
            }

            LibraryArtPanel art = new LibraryArtPanel
            {
                Name = name,
                BackColor = Color.Transparent,
                TabStop = false
            };
            form.Controls.Add(art);
            art.SendToBack();
        }

        private static void EnsureShelfHero(Form form, string name)
        {
            if (Find<ShelfHeroPanel>(form, name) != null)
            {
                return;
            }

            ShelfHeroPanel hero = new ShelfHeroPanel
            {
                Name = name,
                TabStop = false
            };
            form.Controls.Add(hero);
            hero.SendToBack();
        }

        private static Label EnsureLabel(Control parent, string name, string text, float size, FontStyle style, Color color)
        {
            Label? existing = Find<Label>(parent, name);
            if (existing != null)
            {
                PrepareLabel(existing);
                existing.Text = text;
                existing.Font = name.Contains("Title", StringComparison.Ordinal) || name.Contains("Quote", StringComparison.Ordinal)
                    ? DisplayFont(size, style)
                    : BodyFont(size, style);
                existing.ForeColor = color;
                return existing;
            }

            Label label = new Label
            {
                Name = name,
                Text = text,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = color,
                Font = name.Contains("Title", StringComparison.Ordinal) || name.Contains("Quote", StringComparison.Ordinal)
                    ? DisplayFont(size, style)
                    : BodyFont(size, style),
                UseCompatibleTextRendering = false,
                UseMnemonic = false
            };
            PrepareLabel(label);
            parent.Controls.Add(label);
            label.BringToFront();
            return label;
        }

        private static void PrepareLabel(Label label)
        {
            label.AutoSize = false;
            label.AutoEllipsis = false;
            label.BackColor = Color.Transparent;
            label.Padding = Padding.Empty;
            Region? previousRegion = label.Region;
            label.Region = null;
            previousRegion?.Dispose();
            label.UseCompatibleTextRendering = false;
            label.UseMnemonic = false;
        }

        private static void SetText<T>(Control root, string name, string text) where T : Control
        {
            T? control = Find<T>(root, name);
            if (control != null)
            {
                control.Text = text;
                control.AccessibleName = text.Replace("&", string.Empty, StringComparison.Ordinal);
            }
        }

        private static T? Find<T>(Control root, string name) where T : Control
        {
            if (root is T typed && string.Equals(root.Name, name, StringComparison.Ordinal))
            {
                return typed;
            }

            return root.Controls.Find(name, true).OfType<T>().FirstOrDefault();
        }

        private static void SetBounds(Control root, string name, int x, int y, int width, int height)
        {
            Control? control = root.Controls.Find(name, true).FirstOrDefault();
            if (control == null)
            {
                return;
            }

            control.Bounds = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
            if (control is Button)
            {
                ApplyRoundedRegion(control, 10);
            }
            else if (control is SurfacePanel)
            {
                control.SendToBack();
            }
        }

        private static void BringFront(Control root, params string[] names)
        {
            foreach (string name in names)
            {
                root.Controls.Find(name, true).FirstOrDefault()?.BringToFront();
            }
        }

        private static void Reparent(Control root, string parentName, params string[] childNames)
        {
            Control? parent = root.Controls.Find(parentName, true).FirstOrDefault();
            if (parent == null)
            {
                return;
            }

            foreach (string childName in childNames)
            {
                Control? child = root.Controls.Find(childName, true).FirstOrDefault();
                if (child != null && child.Parent != parent)
                {
                    parent.Controls.Add(child);
                }
            }
        }

        private static void StyleDisplayLabel(Label? label, float size)
        {
            if (label == null)
            {
                return;
            }

            label.Font = DisplayFont(size);
            label.ForeColor = Ink;
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void StyleSectionLabel(Label? label)
        {
            if (label == null)
            {
                return;
            }

            label.Font = BodyFont(12F, FontStyle.Bold);
            label.ForeColor = Ink;
        }

        private static void CenterText(Label? label)
        {
            if (label != null)
            {
                label.TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private static void ApplyRoundedRegion(Control? control, int radius)
        {
            if (control == null || control.Width <= 1 || control.Height <= 1)
            {
                return;
            }

            int diameter = Math.Min(radius * 2, Math.Min(control.Width, control.Height));
            using GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            Region? previous = control.Region;
            control.Region = new Region(path);
            previous?.Dispose();
        }

        private sealed class ThemeState
        {
        }

        private enum ButtonKind
        {
            Primary,
            Secondary,
            Outline,
            Danger,
            Ghost
        }

        private sealed class SurfacePanel : Panel
        {
            public SurfacePanel()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                ApplyRoundedRegion(this, Radius);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle border = new Rectangle(0, 0, Width - 1, Height - 1);
                using GraphicsPath path = RoundedPath(border, Radius);
                using Pen pen = new Pen(Line, 1F);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private sealed class ShelfHeroPanel : Panel
        {
            public ShelfHeroPanel()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                ApplyRoundedRegion(this, 22);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using LinearGradientBrush gradient = new LinearGradientBrush(
                    ClientRectangle,
                    ForestDark,
                    Color.FromArgb(48, 105, 87),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(gradient, ClientRectangle);

                using SolidBrush glow = new SolidBrush(Color.FromArgb(24, 255, 255, 255));
                e.Graphics.FillEllipse(glow, Width - 260, -90, 320, 320);
                using SolidBrush accent = new SolidBrush(Color.FromArgb(38, 226, 157, 112));
                e.Graphics.FillEllipse(accent, Width - 120, 92, 110, 110);
            }
        }

        private sealed class LibraryArtPanel : Panel
        {
            public LibraryArtPanel()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                int w = Width;
                int h = Height;
                if (w < 40 || h < 40)
                {
                    return;
                }

                using SolidBrush soft = new SolidBrush(SurfaceSoft);
                using SolidBrush sage = new SolidBrush(Sage);
                using SolidBrush forest = new SolidBrush(Forest);
                using SolidBrush clay = new SolidBrush(Terracotta);
                using Pen line = new Pen(Color.FromArgb(160, 184, 174), 2F);
                using Pen shelf = new Pen(ForestDark, 5F);

                e.Graphics.FillEllipse(soft, new Rectangle(4, 4, Math.Min(w - 8, h + 60), Math.Min(w - 8, h + 60)));

                int baseY = h - 20;
                int startX = Math.Max(28, w / 8);
                int available = Math.Max(180, w - startX * 2);
                int bookW = Math.Max(20, available / 11);
                int gap = Math.Max(7, bookW / 3);
                Color[] colors = { Forest, Terracotta, Color.FromArgb(116, 144, 132), ForestDark, Color.FromArgb(226, 176, 135) };
                int[] heights = { 82, 116, 98, 136, 105, 126, 90 };

                for (int i = 0; i < heights.Length; i++)
                {
                    int bh = Math.Min(heights[i], h - 42);
                    int bx = startX + i * (bookW + gap);
                    Rectangle book = new Rectangle(bx, baseY - bh, bookW, bh);
                    using SolidBrush bookBrush = new SolidBrush(colors[i % colors.Length]);
                    e.Graphics.FillRectangle(bookBrush, book);
                    e.Graphics.DrawLine(line, bx + 6, baseY - bh + 14, bx + bookW - 6, baseY - bh + 14);
                }

                int plantX = Math.Min(w - 100, startX + 7 * (bookW + gap) + 20);
                e.Graphics.DrawLine(shelf, startX - 12, baseY + 2, Math.Min(w - 18, plantX + 82), baseY + 2);
                e.Graphics.FillRectangle(clay, new Rectangle(plantX + 15, baseY - 54, 55, 48));
                e.Graphics.DrawBezier(line, plantX + 42, baseY - 54, plantX + 20, baseY - 90, plantX + 24, baseY - 120, plantX + 8, baseY - 135);
                e.Graphics.DrawBezier(line, plantX + 42, baseY - 54, plantX + 64, baseY - 90, plantX + 66, baseY - 115, plantX + 82, baseY - 130);
                e.Graphics.FillEllipse(sage, new Rectangle(plantX - 2, baseY - 145, 30, 18));
                e.Graphics.FillEllipse(forest, new Rectangle(plantX + 65, baseY - 140, 30, 18));
            }
        }

        private static GraphicsPath RoundedPath(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(rectangle.Width, rectangle.Height));
            path.AddArc(rectangle.Left, rectangle.Top, diameter, diameter, 180, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Top, diameter, diameter, 270, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rectangle.Left, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
