using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace db
{
    public enum GridToolMode
    {
        Catalogue,
        AdminBooks,
        Shelf,
        Payments
    }

    /// <summary>
    /// Product-level conveniences shared by the catalogue, shelf and administration screens.
    /// Features are attached to the existing forms at runtime so database behavior remains unchanged.
    /// </summary>
    public static class AppFeatures
    {
        private static readonly ConditionalWeakTable<Form, FeatureState> States = new();

        public static void EnableGridTools(Form form, DataGridView grid, GridToolMode mode)
        {
            FeatureState state = States.GetOrCreateValue(form);
            if (!state.GridModes.Add(mode))
            {
                return;
            }

            string searchName = mode switch
            {
                GridToolMode.Catalogue => "modernCatalogueSearch",
                GridToolMode.AdminBooks => "modernBookSearch",
                GridToolMode.Shelf => "modernShelfSearch",
                GridToolMode.Payments => "modernPaymentSearch",
                _ => ""
            };
            string summaryName = mode switch
            {
                GridToolMode.Catalogue => "modernCatalogueResults",
                GridToolMode.AdminBooks => "modernBookResults",
                GridToolMode.Shelf => "modernShelfResults",
                GridToolMode.Payments => "modernPaymentResults",
                _ => ""
            };

            TextBox? search = Find<TextBox>(form, searchName);
            Label? summary = Find<Label>(form, summaryName);
            if (search == null || summary == null)
            {
                return;
            }

            search.TextChanged += (_, _) =>
            {
                ApplyGridFilter(grid, search.Text);
                UpdateGridSummary(grid, summary, mode);
            };
            grid.DataBindingComplete += (_, _) => UpdateGridSummary(grid, summary, mode);
            grid.RowsAdded += (_, _) => UpdateGridSummary(grid, summary, mode);
            grid.RowsRemoved += (_, _) => UpdateGridSummary(grid, summary, mode);
            grid.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (grid.IsCurrentCellDirty)
                {
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            grid.CellValueChanged += (_, _) => UpdateGridSummary(grid, summary, mode);

            form.KeyDown += (_, e) =>
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    search.Focus();
                    search.SelectAll();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape && search.TextLength > 0)
                {
                    search.Clear();
                    e.SuppressKeyPress = true;
                }
            };

            if (mode == GridToolMode.Payments)
            {
                Button? export = Find<Button>(form, "modernExportPayments");
                if (export != null)
                {
                    export.Click += (_, _) => ExportVisibleRows(grid, form);
                }
            }

            UpdateGridSummary(grid, summary, mode);
        }

        public static void RefreshGridTools(Form form, DataGridView grid, GridToolMode mode)
        {
            string summaryName = mode switch
            {
                GridToolMode.Catalogue => "modernCatalogueResults",
                GridToolMode.AdminBooks => "modernBookResults",
                GridToolMode.Shelf => "modernShelfResults",
                GridToolMode.Payments => "modernPaymentResults",
                _ => ""
            };
            Label? summary = Find<Label>(form, summaryName);
            if (summary != null)
            {
                UpdateGridSummary(grid, summary, mode);
            }
        }

        public static void EnableMemberTools(Form form, ListView list)
        {
            FeatureState state = States.GetOrCreateValue(form);
            if (state.MemberToolsAttached)
            {
                return;
            }

            TextBox? search = Find<TextBox>(form, "modernMemberSearch");
            Label? summary = Find<Label>(form, "modernMemberResults");
            if (search == null || summary == null)
            {
                return;
            }

            state.MemberToolsAttached = true;
            search.TextChanged += (_, _) => ApplyMemberFilter(list, state, search.Text, summary);
            form.KeyDown += (_, e) =>
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    search.Focus();
                    search.SelectAll();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape && search.TextLength > 0)
                {
                    search.Clear();
                    e.SuppressKeyPress = true;
                }
            };
        }

        public static void RefreshMemberTools(Form form, ListView list)
        {
            FeatureState state = States.GetOrCreateValue(form);
            state.MemberItems.Clear();
            foreach (ListViewItem item in list.Items)
            {
                state.MemberItems.Add((ListViewItem)item.Clone());
            }

            TextBox? search = Find<TextBox>(form, "modernMemberSearch");
            Label? summary = Find<Label>(form, "modernMemberResults");
            if (summary != null)
            {
                ApplyMemberFilter(list, state, search?.Text ?? "", summary);
            }
        }

        public static void EnablePasswordToggle(Form form, string checkBoxName, params TextBox[] passwordBoxes)
        {
            FeatureState state = States.GetOrCreateValue(form);
            if (!state.PasswordToggles.Add(checkBoxName))
            {
                return;
            }

            CheckBox? toggle = Find<CheckBox>(form, checkBoxName);
            if (toggle == null)
            {
                return;
            }

            toggle.CheckedChanged += (_, _) =>
            {
                foreach (TextBox box in passwordBoxes)
                {
                    box.UseSystemPasswordChar = !toggle.Checked;
                }
            };
        }

        private static void ApplyGridFilter(DataGridView grid, string query)
        {
            DataView? view = ResolveDataView(grid.DataSource);
            if (view == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                view.RowFilter = "";
                return;
            }

            string value = EscapeFilterValue(query.Trim());
            DataTable? table = view.Table;
            if (table == null)
            {
                return;
            }

            IEnumerable<DataColumn> columns = table.Columns
                .Cast<DataColumn>()
                .Where(column => column.DataType != typeof(byte[]));
            string filter = string.Join(
                " OR ",
                columns.Select(column =>
                    $"CONVERT([{column.ColumnName.Replace("]", "]]", StringComparison.Ordinal)}], 'System.String') LIKE '%{value}%'"));

            try
            {
                view.RowFilter = filter;
            }
            catch (EvaluateException)
            {
                view.RowFilter = "";
            }
            catch (SyntaxErrorException)
            {
                view.RowFilter = "";
            }
        }

        private static DataView? ResolveDataView(object? source)
        {
            if (source is BindingSource binding)
            {
                source = binding.DataSource;
            }

            return source switch
            {
                DataTable table => table.DefaultView,
                DataView view => view,
                _ => null
            };
        }

        private static void UpdateGridSummary(DataGridView grid, Label summary, GridToolMode mode)
        {
            int visibleRows = grid.Rows.Cast<DataGridViewRow>()
                .Count(row => !row.IsNewRow && row.Visible);

            summary.Text = mode switch
            {
                GridToolMode.Catalogue => CatalogueSummary(grid, visibleRows),
                GridToolMode.AdminBooks => $"{visibleRows} {(visibleRows == 1 ? "title" : "titles")} shown",
                GridToolMode.Shelf => ShelfSummary(grid, visibleRows),
                GridToolMode.Payments => $"{visibleRows} {(visibleRows == 1 ? "transaction" : "transactions")} shown",
                _ => $"{visibleRows} results"
            };
        }

        private static string CatalogueSummary(DataGridView grid, int visibleRows)
        {
            int selected = grid.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow && row.Visible)
                .Count(row => row.Cells.Cast<DataGridViewCell>()
                    .Any(cell => cell is DataGridViewCheckBoxCell && cell.Value is bool value && value));

            string books = $"{visibleRows} {(visibleRows == 1 ? "book" : "books")}";
            return selected > 0 ? $"{books} • {selected} selected" : books;
        }

        private static string ShelfSummary(DataGridView grid, int visibleRows)
        {
            decimal total = 0m;
            DataGridViewColumn? priceColumn = grid.Columns.Cast<DataGridViewColumn>()
                .FirstOrDefault(column =>
                    column.Name.Contains("price", StringComparison.OrdinalIgnoreCase) ||
                    column.HeaderText.Contains("price", StringComparison.OrdinalIgnoreCase));

            if (priceColumn != null)
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow || !row.Visible)
                    {
                        continue;
                    }

                    string raw = row.Cells[priceColumn.Index].Value?.ToString() ?? "";
                    if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal current) ||
                        decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out current))
                    {
                        total += current;
                    }
                }
            }

            string books = $"{visibleRows} saved {(visibleRows == 1 ? "book" : "books")}";
            return total > 0m ? $"{books} • {total:C}" : books;
        }

        private static void ApplyMemberFilter(ListView list, FeatureState state, string query, Label summary)
        {
            string value = query.Trim();
            IEnumerable<ListViewItem> matches = state.MemberItems.Where(item =>
                value.Length == 0 ||
                item.SubItems.Cast<ListViewItem.ListViewSubItem>()
                    .Any(part => part.Text.Contains(value, StringComparison.CurrentCultureIgnoreCase)));

            List<ListViewItem> visible = matches.Select(item => (ListViewItem)item.Clone()).ToList();
            list.BeginUpdate();
            try
            {
                list.Items.Clear();
                list.Items.AddRange(visible.ToArray());
            }
            finally
            {
                list.EndUpdate();
            }

            summary.Text = $"{visible.Count} {(visible.Count == 1 ? "member" : "members")} shown";
        }

        private static void ExportVisibleRows(DataGridView grid, Form owner)
        {
            List<DataGridViewColumn> columns = grid.Columns.Cast<DataGridViewColumn>()
                .Where(column =>
                    column.Visible &&
                    column is not DataGridViewImageColumn &&
                    column.ValueType != typeof(byte[]))
                .OrderBy(column => column.DisplayIndex)
                .ToList();
            List<DataGridViewRow> rows = grid.Rows.Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow && row.Visible)
                .ToList();

            if (rows.Count == 0)
            {
                MessageBox.Show(
                    owner,
                    "There are no visible report rows to export.",
                    "Export report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Export sales report",
                Filter = "CSV file (*.csv)|*.csv",
                FileName = $"sales-report-{DateTime.Now:yyyy-MM-dd}.csv",
                AddExtension = true,
                DefaultExt = "csv"
            };
            if (dialog.ShowDialog(owner) != DialogResult.OK)
            {
                return;
            }

            try
            {
                StringBuilder csv = new StringBuilder();
                csv.AppendLine(string.Join(",", columns.Select(column => Csv(column.HeaderText))));
                foreach (DataGridViewRow row in rows)
                {
                    csv.AppendLine(string.Join(",", columns.Select(column =>
                        Csv(row.Cells[column.Index].FormattedValue?.ToString() ?? ""))));
                }

                File.WriteAllText(dialog.FileName, csv.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                MessageBox.Show(
                    owner,
                    $"Exported {rows.Count} report rows successfully.",
                    "Export complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    owner,
                    "The report could not be saved. " + ex.Message,
                    "Export failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    owner,
                    "Choose a folder where you have permission to save files.",
                    "Export failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string Csv(string value)
            => "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

        private static string EscapeFilterValue(string value)
            => value
                .Replace("'", "''", StringComparison.Ordinal)
                .Replace("[", "[[]", StringComparison.Ordinal)
                .Replace("%", "[%]", StringComparison.Ordinal)
                .Replace("*", "[*]", StringComparison.Ordinal);

        private static T? Find<T>(Control root, string name) where T : Control
            => root.Controls.Find(name, true).OfType<T>().FirstOrDefault();

        private sealed class FeatureState
        {
            public HashSet<GridToolMode> GridModes { get; } = new();
            public HashSet<string> PasswordToggles { get; } = new(StringComparer.Ordinal);
            public bool MemberToolsAttached { get; set; }
            public List<ListViewItem> MemberItems { get; } = new();
        }
    }
}
