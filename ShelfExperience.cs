using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace db
{
    /// <summary>
    /// Turns the shelf's bound table into a responsive, cover-first collection while preserving the
    /// existing DataGridView as an invisible data adapter for the current database code.
    /// </summary>
    public static class ShelfExperience
    {
        private static readonly ConditionalWeakTable<Form, ShelfState> States = new();

        public static void Attach(
            Form form,
            DataGridView grid,
            Action<int> removeBook,
            Action<int> downloadPdf)
        {
            ShelfState state = States.GetOrCreateValue(form);
            if (state.Attached)
            {
                return;
            }

            state.Attached = true;
            state.RemoveBook = removeBook;
            state.DownloadPdf = downloadPdf;
            state.Gallery = new FlowLayoutPanel
            {
                Name = "modernShelfGallery",
                AutoScroll = true,
                BackColor = ModernTheme.Surface,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(8),
                TabStop = true,
                AccessibleName = "Saved books"
            };
            form.Controls.Add(state.Gallery);
            state.Gallery.BringToFront();

            TextBox? search = Find<TextBox>(form, "modernShelfSearch");
            if (search != null)
            {
                search.TextChanged += (_, _) => RefreshLater(form, grid);
            }

            grid.DataBindingComplete += (_, _) => RefreshLater(form, grid);
            grid.RowsAdded += (_, _) => RefreshLater(form, grid);
            grid.RowsRemoved += (_, _) => RefreshLater(form, grid);
            form.Shown += (_, _) => Refresh(form, grid);
            form.Resize += (_, _) =>
            {
                ModernTheme.Refresh(form);
                RefreshEmptyStateWidth(state);
            };

            ModernTheme.Refresh(form);
            Refresh(form, grid);
        }

        public static void Refresh(Form form, DataGridView grid)
        {
            ShelfState state = States.GetOrCreateValue(form);
            FlowLayoutPanel? gallery = state.Gallery;
            if (gallery == null || gallery.IsDisposed)
            {
                return;
            }

            // The grid is intentionally hidden and only acts as a data adapter. A hidden
            // DataGridView reports its rows as not visible, so use every bound row here.
            List<DataGridViewRow> rows = grid.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .ToList();

            gallery.SuspendLayout();
            try
            {
                Control[] previous = gallery.Controls.Cast<Control>().ToArray();
                gallery.Controls.Clear();
                foreach (Control control in previous)
                {
                    control.Dispose();
                }

                if (rows.Count == 0)
                {
                    gallery.Controls.Add(new EmptyShelfPanel(Math.Max(520, gallery.ClientSize.Width - 40)));
                }
                else
                {
                    foreach (DataGridViewRow row in rows)
                    {
                        gallery.Controls.Add(new BookCard(
                            row,
                            () => state.RemoveBook?.Invoke(row.Index),
                            bookId => state.DownloadPdf?.Invoke(bookId)));
                    }
                }
            }
            finally
            {
                gallery.ResumeLayout(true);
            }

            UpdateMetrics(form, grid, rows.Count);
        }

        private static void RefreshLater(Form form, DataGridView grid)
        {
            if (form.IsDisposed || !form.IsHandleCreated)
            {
                return;
            }

            form.BeginInvoke(new Action(() => Refresh(form, grid)));
        }

        private static void UpdateMetrics(Form form, DataGridView grid, int visibleCount)
        {
            DataTable? table = grid.DataSource switch
            {
                DataTable dataTable => dataTable,
                DataView view => view.Table,
                BindingSource binding when binding.DataSource is DataTable boundTable => boundTable,
                _ => null
            };
            IEnumerable<DataRow> rows = table?.Rows.Cast<DataRow>()
                .Where(row => row.RowState != DataRowState.Deleted) ?? Enumerable.Empty<DataRow>();
            List<DataRow> collection = rows.ToList();
            decimal total = collection.Sum(row => ParsePrice(FindValue(row, "price")));

            SetLabel(form, "modernShelfCountValue", collection.Count.ToString(CultureInfo.CurrentCulture));
            SetLabel(form, "modernShelfValueValue", total > 0m ? total.ToString("C", CultureInfo.CurrentCulture) : "—");

            TextBox? search = Find<TextBox>(form, "modernShelfSearch");
            string summary = string.IsNullOrWhiteSpace(search?.Text)
                ? $"{collection.Count} {(collection.Count == 1 ? "book" : "books")} in your collection"
                : $"{visibleCount} of {collection.Count} {(collection.Count == 1 ? "book" : "books")} shown";
            SetLabel(form, "modernShelfResults", summary);
        }

        private static object? FindValue(DataRow row, string columnFragment)
        {
            DataColumn? column = row.Table.Columns.Cast<DataColumn>()
                .FirstOrDefault(candidate =>
                    candidate.ColumnName.Contains(columnFragment, StringComparison.OrdinalIgnoreCase));
            return column == null ? null : row[column];
        }

        private static decimal ParsePrice(object? value)
        {
            string raw = value?.ToString()?.Trim() ?? "";
            const NumberStyles styles = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            if (decimal.TryParse(raw, styles, CultureInfo.CurrentCulture, out decimal current))
            {
                return current;
            }

            return decimal.TryParse(raw, styles, CultureInfo.InvariantCulture, out decimal invariant)
                ? invariant
                : 0m;
        }

        private static void SetLabel(Form form, string name, string value)
        {
            Label? label = Find<Label>(form, name);
            if (label != null)
            {
                label.Text = value;
            }
        }

        private static void RefreshEmptyStateWidth(ShelfState state)
        {
            if (state.Gallery?.Controls.OfType<EmptyShelfPanel>().FirstOrDefault() is { } empty)
            {
                empty.Width = Math.Max(520, state.Gallery.ClientSize.Width - 40);
            }
        }

        private static T? Find<T>(Control root, string name) where T : Control
            => root.Controls.Find(name, true).OfType<T>().FirstOrDefault();

        private sealed class ShelfState
        {
            public bool Attached { get; set; }
            public FlowLayoutPanel? Gallery { get; set; }
            public Action<int>? RemoveBook { get; set; }
            public Action<int>? DownloadPdf { get; set; }
        }

        private sealed class BookCard : Panel
        {
            private readonly PictureBox? coverImage;

            public BookCard(DataGridViewRow row, Action remove, Action<int> download)
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
                Size = new Size(236, 420);
                Margin = new Padding(10);
                BackColor = ModernTheme.Surface;
                AccessibleName = "Saved book";

                string title = CellText(row, "book", fallbackIndex: 1, "Untitled book");
                string author = CellText(row, "author", fallbackIndex: 2, "Unknown author");
                string price = CellText(row, "price", fallbackIndex: 3, "Price unavailable");
                byte[]? bytes = CellBytes(row);
                int bookId = CellInt(row, "BookId");
                bool hasPdf = bookId > 0 && CellBool(row, "HasPdf");

                if (TryImage(bytes, out Image? image))
                {
                    coverImage = new PictureBox
                    {
                        Bounds = new Rectangle(26, 16, 184, 228),
                        BackColor = ModernTheme.SurfaceSoft,
                        Image = image,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };
                    ApplyRounded(coverImage, 12);
                    Controls.Add(coverImage);
                }
                else
                {
                    CoverPlaceholder placeholder = new CoverPlaceholder(title)
                    {
                        Bounds = new Rectangle(26, 16, 184, 228)
                    };
                    ApplyRounded(placeholder, 12);
                    Controls.Add(placeholder);
                }

                Controls.Add(new Label
                {
                    Text = title,
                    Bounds = new Rectangle(16, 252, 204, 48),
                    BackColor = Color.Transparent,
                    ForeColor = ModernTheme.Ink,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    AutoEllipsis = false,
                    UseCompatibleTextRendering = false
                });
                Controls.Add(new Label
                {
                    Text = author,
                    Bounds = new Rectangle(16, 303, 204, 36),
                    BackColor = Color.Transparent,
                    ForeColor = ModernTheme.Muted,
                    Font = new Font("Segoe UI", 9F),
                    AutoEllipsis = false,
                    UseCompatibleTextRendering = false
                });
                Controls.Add(new Label
                {
                    Text = price,
                    Bounds = new Rectangle(16, 340, 204, 26),
                    BackColor = Color.Transparent,
                    ForeColor = ModernTheme.Forest,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    AutoEllipsis = false,
                    UseCompatibleTextRendering = false
                });

                Button downloadButton = new Button
                {
                    Text = hasPdf ? "Download" : "No PDF",
                    AccessibleName = hasPdf ? $"Download {title} PDF" : $"No PDF available for {title}",
                    Bounds = new Rectangle(16, 374, 118, 34),
                    BackColor = hasPdf ? ModernTheme.Forest : ModernTheme.SurfaceSoft,
                    ForeColor = hasPdf ? Color.White : ModernTheme.Muted,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = hasPdf ? Cursors.Hand : Cursors.Default,
                    Enabled = hasPdf
                };
                downloadButton.FlatAppearance.BorderSize = 0;
                downloadButton.FlatAppearance.MouseOverBackColor = ModernTheme.ForestDark;
                ApplyRounded(downloadButton, 9);
                if (hasPdf)
                {
                    downloadButton.Click += (_, _) => download(bookId);
                }
                Controls.Add(downloadButton);

                Button removeButton = new Button
                {
                    Text = "Remove",
                    AccessibleName = $"Remove {title}",
                    Bounds = new Rectangle(142, 374, 78, 34),
                    BackColor = ModernTheme.DangerSoft,
                    ForeColor = ModernTheme.Danger,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                removeButton.FlatAppearance.BorderSize = 0;
                removeButton.FlatAppearance.MouseOverBackColor = ModernTheme.IsDarkMode
                    ? Color.FromArgb(96, 47, 54)
                    : Color.FromArgb(244, 211, 214);
                ApplyRounded(removeButton, 9);
                removeButton.Click += (_, _) => remove();
                Controls.Add(removeButton);

                AttachHover(this);
                ApplyRounded(this, 16);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using GraphicsPath path = RoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), 16);
                using Pen border = new Pen(ModernTheme.Line);
                e.Graphics.DrawPath(border, path);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    coverImage?.Image?.Dispose();
                }

                base.Dispose(disposing);
            }

            private static string CellText(
                DataGridViewRow row,
                string columnFragment,
                int fallbackIndex,
                string fallback)
            {
                DataGridViewCell? cell = row.Cells.Cast<DataGridViewCell>()
                    .FirstOrDefault(candidate =>
                        candidate.OwningColumn?.Name.Contains(columnFragment, StringComparison.OrdinalIgnoreCase) == true ||
                        candidate.OwningColumn?.HeaderText.Contains(columnFragment, StringComparison.OrdinalIgnoreCase) == true);
                if (cell == null && fallbackIndex >= 0 && fallbackIndex < row.Cells.Count)
                {
                    cell = row.Cells[fallbackIndex];
                }

                string value = cell?.Value?.ToString()?.Trim() ?? "";
                return value.Length == 0 ? fallback : value;
            }

            private static byte[]? CellBytes(DataGridViewRow row)
                => row.Cells.Cast<DataGridViewCell>()
                    .Select(cell => cell.Value)
                    .OfType<byte[]>()
                    .FirstOrDefault(bytes => bytes.Length > 0);

            private static int CellInt(DataGridViewRow row, string columnName)
            {
                object? value = CellValue(row, columnName);
                return value != null &&
                       value != DBNull.Value &&
                       int.TryParse(value.ToString(), out int parsed)
                    ? parsed
                    : 0;
            }

            private static bool CellBool(DataGridViewRow row, string columnName)
            {
                object? value = CellValue(row, columnName);
                if (value is bool typed)
                {
                    return typed;
                }

                return value != null &&
                       value != DBNull.Value &&
                       bool.TryParse(value.ToString(), out bool parsed) &&
                       parsed;
            }

            private static object? CellValue(DataGridViewRow row, string columnName)
                => row.Cells.Cast<DataGridViewCell>()
                    .FirstOrDefault(cell =>
                        string.Equals(
                            cell.OwningColumn?.Name,
                            columnName,
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(
                            cell.OwningColumn?.HeaderText,
                            columnName,
                            StringComparison.OrdinalIgnoreCase))
                    ?.Value;

            private static bool TryImage(byte[]? bytes, out Image? image)
            {
                image = null;
                if (bytes == null || bytes.Length == 0)
                {
                    return false;
                }

                try
                {
                    using MemoryStream stream = new MemoryStream(bytes, writable: false);
                    using Image source = Image.FromStream(stream);
                    image = new Bitmap(source);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            private static void AttachHover(Control root)
            {
                root.MouseEnter += (_, _) => root.FindForm()?.BeginInvoke(new Action(() =>
                {
                    if (!root.IsDisposed)
                    {
                        root.BackColor = ModernTheme.IsDarkMode
                            ? Color.FromArgb(34, 45, 41)
                            : Color.FromArgb(249, 251, 250);
                    }
                }));
                root.MouseLeave += (_, _) =>
                {
                    if (!root.IsDisposed && !root.ClientRectangle.Contains(root.PointToClient(Cursor.Position)))
                    {
                        root.BackColor = ModernTheme.Surface;
                    }
                };
            }
        }

        private sealed class CoverPlaceholder : Panel
        {
            private readonly string title;

            public CoverPlaceholder(string title)
            {
                this.title = title;
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using LinearGradientBrush gradient = new LinearGradientBrush(
                    ClientRectangle,
                    ModernTheme.ForestDark,
                    Color.FromArgb(93, 139, 119),
                    35F);
                e.Graphics.FillRectangle(gradient, ClientRectangle);
                using Pen page = new Pen(Color.FromArgb(100, 255, 255, 255), 1.5F);
                e.Graphics.DrawLine(page, 24, 30, Width - 24, 30);
                e.Graphics.DrawLine(page, 24, Height - 28, Width - 24, Height - 28);
                string initial = string.IsNullOrWhiteSpace(title) ? "L" : title.Trim()[0].ToString().ToUpperInvariant();
                using Font initialFont = new Font("Georgia", 34F, FontStyle.Bold);
                using SolidBrush text = new SolidBrush(Color.White);
                SizeF size = e.Graphics.MeasureString(initial, initialFont);
                e.Graphics.DrawString(initial, initialFont, text, (Width - size.Width) / 2, (Height - size.Height) / 2 - 6);
            }
        }

        private sealed class EmptyShelfPanel : Panel
        {
            public EmptyShelfPanel(int width)
            {
                Width = width;
                Height = 250;
                Margin = new Padding(10);
                BackColor = ModernTheme.Surface;

                Controls.Add(new Label
                {
                    Text = "Your shelf is ready for its first story",
                    Bounds = new Rectangle(0, 104, width, 38),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = ModernTheme.Ink,
                    Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                    AutoSize = false
                });
                Controls.Add(new Label
                {
                    Text = "Explore the catalogue and save the books you want to come back to.",
                    Bounds = new Rectangle(0, 145, width, 32),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = ModernTheme.Muted,
                    Font = new Font("Segoe UI", 9.5F),
                    AutoSize = false
                });
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using Pen book = new Pen(ModernTheme.Forest, 3F);
                Rectangle left = new Rectangle(Width / 2 - 52, 28, 48, 60);
                Rectangle right = new Rectangle(Width / 2 + 4, 28, 48, 60);
                e.Graphics.DrawArc(book, left, 205, 130);
                e.Graphics.DrawArc(book, right, 205, 130);
                e.Graphics.DrawLine(book, Width / 2, 40, Width / 2, 92);
                e.Graphics.DrawLine(book, Width / 2 - 49, 75, Width / 2, 92);
                e.Graphics.DrawLine(book, Width / 2 + 49, 75, Width / 2, 92);
            }
        }

        private static void ApplyRounded(Control control, int radius)
        {
            if (control.Width <= 1 || control.Height <= 1)
            {
                return;
            }

            using GraphicsPath path = RoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius);
            Region? old = control.Region;
            control.Region = new Region(path);
            old?.Dispose();
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
