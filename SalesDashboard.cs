using System.Drawing.Drawing2D;
using System.Globalization;

namespace db
{
    public sealed record SaleRecord(string User, string Book, string Author, decimal Price);

    /// <summary>
    /// Responsive, dependency-free sales charts for the admin report. The database currently stores
    /// no transaction date, so the dashboard analyses revenue, titles and customers instead of
    /// presenting a misleading time-series chart.
    /// </summary>
    public sealed class SalesDashboard : Control
    {
        private IReadOnlyList<SaleRecord> sales = Array.Empty<SaleRecord>();

        public SalesDashboard()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            AccessibleName = "Sales analysis dashboard";
            ApplyTheme();
        }

        public void SetSales(IEnumerable<SaleRecord> records)
        {
            sales = records is null ? Array.Empty<SaleRecord>() : records.ToList();
            Invalidate();
        }

        public void ApplyTheme()
        {
            BackColor = ModernTheme.Canvas;
            ForeColor = ModernTheme.Ink;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (Width < 520 || Height < 220)
            {
                return;
            }

            decimal revenue = sales.Sum(sale => sale.Price);
            int buyerCount = sales
                .Select(sale => sale.User)
                .Where(user => !string.IsNullOrWhiteSpace(user))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Count();
            decimal average = sales.Count == 0 ? 0m : revenue / sales.Count;

            int gap = 12;
            int metricHeight = 84;
            int metricWidth = (Width - gap * 3) / 4;
            DrawMetric(e.Graphics, new Rectangle(0, 0, metricWidth, metricHeight),
                "TOTAL REVENUE", FormatMoney(revenue), "Across recorded shelf sales");
            DrawMetric(e.Graphics, new Rectangle(metricWidth + gap, 0, metricWidth, metricHeight),
                "BOOKS SOLD", sales.Count.ToString("N0", CultureInfo.CurrentCulture), "Recorded line items");
            DrawMetric(e.Graphics, new Rectangle((metricWidth + gap) * 2, 0, metricWidth, metricHeight),
                "BUYERS", buyerCount.ToString("N0", CultureInfo.CurrentCulture), "Unique customer accounts");
            DrawMetric(e.Graphics, new Rectangle((metricWidth + gap) * 3, 0, metricWidth, metricHeight),
                "AVERAGE SALE", FormatMoney(average), "Revenue per book");

            int analysisY = metricHeight + 14;
            int analysisHeight = Height - analysisY;
            int leftWidth = Math.Max(430, (int)((Width - gap) * 0.62));
            leftWidth = Math.Min(leftWidth, Width - 330);

            Rectangle revenueBounds = new Rectangle(0, analysisY, leftWidth, analysisHeight);
            Rectangle mixBounds = new Rectangle(leftWidth + gap, analysisY, Width - leftWidth - gap, analysisHeight);
            DrawRevenueByTitle(e.Graphics, revenueBounds);
            DrawSalesMix(e.Graphics, mixBounds);
        }

        private void DrawMetric(Graphics graphics, Rectangle bounds, string label, string value, string note)
        {
            DrawCard(graphics, bounds);
            using Font labelFont = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            using Font valueFont = new Font("Segoe UI", 18F, FontStyle.Bold);
            using Font noteFont = new Font("Segoe UI", 7.5F);
            using SolidBrush labelBrush = new SolidBrush(ModernTheme.Muted);
            using SolidBrush valueBrush = new SolidBrush(ModernTheme.Forest);
            using SolidBrush noteBrush = new SolidBrush(ModernTheme.Muted);

            graphics.DrawString(label, labelFont, labelBrush, bounds.X + 16, bounds.Y + 10);
            graphics.DrawString(value, valueFont, valueBrush, bounds.X + 16, bounds.Y + 27);
            graphics.DrawString(note, noteFont, noteBrush, bounds.X + 16, bounds.Bottom - 20);
        }

        private void DrawRevenueByTitle(Graphics graphics, Rectangle bounds)
        {
            DrawCard(graphics, bounds);
            DrawSectionTitle(
                graphics,
                bounds,
                "Revenue by title",
                "Top books ranked by recorded revenue");

            List<BookPerformance> books = sales
                .GroupBy(sale => sale.Book, StringComparer.CurrentCultureIgnoreCase)
                .Select(group => new BookPerformance(
                    group.Key,
                    group.Count(),
                    group.Sum(item => item.Price)))
                .OrderByDescending(item => item.Revenue)
                .ThenByDescending(item => item.Units)
                .ThenBy(item => item.Title, StringComparer.CurrentCultureIgnoreCase)
                .Take(4)
                .ToList();

            if (books.Count == 0)
            {
                DrawEmptyState(graphics, bounds, "No sales have been recorded yet.");
                return;
            }

            decimal maxRevenue = books.Max(book => book.Revenue);
            int maxUnits = books.Max(book => book.Units);
            int contentTop = bounds.Y + 57;
            int rowHeight = Math.Max(31, (bounds.Height - 70) / Math.Max(books.Count, 1));
            int labelWidth = Math.Clamp(bounds.Width / 3, 180, 280);
            int valueWidth = 92;
            int barX = bounds.X + 18 + labelWidth;
            int barWidth = Math.Max(80, bounds.Right - barX - valueWidth - 20);

            using Font titleFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            using Font detailFont = new Font("Segoe UI", 7.5F);
            using SolidBrush titleBrush = new SolidBrush(ModernTheme.Ink);
            using SolidBrush detailBrush = new SolidBrush(ModernTheme.Muted);
            using SolidBrush trackBrush = new SolidBrush(ModernTheme.SurfaceSoft);
            using SolidBrush barBrush = new SolidBrush(ModernTheme.Forest);
            using StringFormat ellipsis = new StringFormat
            {
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.LineLimit,
                LineAlignment = StringAlignment.Near
            };

            for (int index = 0; index < books.Count; index++)
            {
                BookPerformance book = books[index];
                int rowY = contentTop + index * rowHeight;
                Rectangle titleBounds = new Rectangle(
                    bounds.X + 18,
                    rowY,
                    labelWidth - 10,
                    Math.Min(30, rowHeight - 12));
                graphics.DrawString(book.Title, titleFont, titleBrush, titleBounds, ellipsis);
                graphics.DrawString(
                    $"{book.Units} {(book.Units == 1 ? "sale" : "sales")}",
                    detailFont,
                    detailBrush,
                    bounds.X + 18,
                    rowY + rowHeight - 15);

                Rectangle track = new Rectangle(barX, rowY + (rowHeight - 12) / 2, barWidth, 12);
                FillRoundedRectangle(graphics, trackBrush, track, 6);
                double ratio = maxRevenue > 0m
                    ? (double)(book.Revenue / maxRevenue)
                    : (double)book.Units / Math.Max(maxUnits, 1);
                Rectangle bar = new Rectangle(track.X, track.Y, Math.Max(6, (int)(track.Width * ratio)), track.Height);
                FillRoundedRectangle(graphics, barBrush, bar, 6);

                using StringFormat right = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Center
                };
                graphics.DrawString(
                    FormatMoney(book.Revenue),
                    titleFont,
                    titleBrush,
                    new Rectangle(track.Right + 8, rowY, valueWidth - 8, rowHeight),
                    right);
            }
        }

        private void DrawSalesMix(Graphics graphics, Rectangle bounds)
        {
            DrawCard(graphics, bounds);
            SaleRecord? topBuyerSale = sales
                .GroupBy(sale => sale.User, StringComparer.CurrentCultureIgnoreCase)
                .Select(group => new SaleRecord(
                    group.Key,
                    "",
                    "",
                    group.Sum(item => item.Price)))
                .OrderByDescending(item => item.Price)
                .FirstOrDefault();
            string subtitle = topBuyerSale == null
                ? "Share of revenue by leading titles"
                : $"Revenue share by title. Top buyer: {topBuyerSale.User}";
            DrawSectionTitle(
                graphics,
                bounds,
                "Sales mix",
                subtitle);

            List<BookPerformance> books = sales
                .GroupBy(sale => sale.Book, StringComparer.CurrentCultureIgnoreCase)
                .Select(group => new BookPerformance(group.Key, group.Count(), group.Sum(item => item.Price)))
                .OrderByDescending(item => item.Revenue)
                .ThenByDescending(item => item.Units)
                .Take(4)
                .ToList();

            if (books.Count == 0)
            {
                DrawEmptyState(graphics, bounds, "Sales composition will appear here.");
                return;
            }

            decimal totalRevenue = books.Sum(book => book.Revenue);
            double totalWeight = totalRevenue > 0m
                ? (double)totalRevenue
                : books.Sum(book => book.Units);

            Color[] colors =
            {
                ModernTheme.Forest,
                ModernTheme.Terracotta,
                Color.FromArgb(105, 145, 130),
                Color.FromArgb(211, 169, 104)
            };

            int chartSize = Math.Min(118, Math.Max(86, bounds.Width / 3));
            Rectangle chart = new Rectangle(
                bounds.X + 18,
                bounds.Y + 62,
                chartSize,
                chartSize);

            float startAngle = -90F;
            for (int index = 0; index < books.Count; index++)
            {
                double weight = totalRevenue > 0m ? (double)books[index].Revenue : books[index].Units;
                float sweep = index == books.Count - 1
                    ? 270F - startAngle
                    : (float)(weight / Math.Max(totalWeight, 1D) * 360D);
                using SolidBrush slice = new SolidBrush(colors[index]);
                graphics.FillPie(slice, chart, startAngle, sweep);
                startAngle += sweep;
            }

            int holeSize = (int)(chartSize * 0.56);
            Rectangle hole = new Rectangle(
                chart.X + (chart.Width - holeSize) / 2,
                chart.Y + (chart.Height - holeSize) / 2,
                holeSize,
                holeSize);
            using SolidBrush holeBrush = new SolidBrush(ModernTheme.Surface);
            graphics.FillEllipse(holeBrush, hole);

            using Font centerValue = new Font("Segoe UI", 10F, FontStyle.Bold);
            using Font centerLabel = new Font("Segoe UI", 6.8F, FontStyle.Bold);
            using SolidBrush ink = new SolidBrush(ModernTheme.Ink);
            using SolidBrush muted = new SolidBrush(ModernTheme.Muted);
            using StringFormat centered = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(
                sales.Count.ToString("N0", CultureInfo.CurrentCulture),
                centerValue,
                ink,
                new Rectangle(hole.X, hole.Y + 8, hole.Width, 22),
                centered);
            graphics.DrawString("SALES", centerLabel, muted,
                new Rectangle(hole.X, hole.Y + 27, hole.Width, 16), centered);

            int legendX = chart.Right + 14;
            int legendY = bounds.Y + 60;
            int legendWidth = Math.Max(80, bounds.Right - legendX - 14);
            int legendRowHeight = Math.Max(40, (bounds.Height - 68) / Math.Max(books.Count, 1));
            using Font legendFont = new Font("Segoe UI", 8F, FontStyle.Bold);
            using Font legendDetail = new Font("Segoe UI", 7.2F);
            using SolidBrush legendText = new SolidBrush(ModernTheme.Ink);
            using SolidBrush detailText = new SolidBrush(ModernTheme.Muted);
            using StringFormat legendFormat = new StringFormat
            {
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.LineLimit
            };

            for (int index = 0; index < books.Count; index++)
            {
                int y = legendY + index * legendRowHeight;
                graphics.DrawString(
                    books[index].Title,
                    legendFont,
                    legendText,
                    new Rectangle(legendX, y, legendWidth, Math.Min(30, legendRowHeight - 13)),
                    legendFormat);
                double percentage = totalWeight <= 0D
                    ? 0D
                    : (totalRevenue > 0m ? (double)books[index].Revenue : books[index].Units) / totalWeight;
                graphics.DrawString(
                    $"{percentage:P0} of revenue, {books[index].Units} units",
                    legendDetail,
                    detailText,
                    legendX,
                    y + legendRowHeight - 15);
            }

        }

        private static void DrawSectionTitle(Graphics graphics, Rectangle bounds, string title, string subtitle)
        {
            using Font titleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            using Font subtitleFont = new Font("Segoe UI", 7.7F);
            using SolidBrush titleBrush = new SolidBrush(ModernTheme.Ink);
            using SolidBrush subtitleBrush = new SolidBrush(ModernTheme.Muted);
            using StringFormat fullText = new StringFormat
            {
                Trimming = StringTrimming.Word,
                FormatFlags = StringFormatFlags.LineLimit
            };
            graphics.DrawString(
                title,
                titleFont,
                titleBrush,
                new Rectangle(bounds.X + 18, bounds.Y + 10, bounds.Width - 36, 24),
                fullText);
            graphics.DrawString(
                subtitle,
                subtitleFont,
                subtitleBrush,
                new Rectangle(bounds.X + 18, bounds.Y + 34, bounds.Width - 36, 29),
                fullText);
        }

        private static void DrawEmptyState(Graphics graphics, Rectangle bounds, string message)
        {
            using Font font = new Font("Segoe UI", 9F);
            using SolidBrush brush = new SolidBrush(ModernTheme.Muted);
            using StringFormat centered = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(message, font, brush,
                new Rectangle(bounds.X + 18, bounds.Y + 55, bounds.Width - 36, bounds.Height - 70), centered);
        }

        private static void DrawCard(Graphics graphics, Rectangle bounds)
        {
            Rectangle card = new Rectangle(bounds.X, bounds.Y, Math.Max(1, bounds.Width - 1), Math.Max(1, bounds.Height - 1));
            using GraphicsPath path = RoundedPath(card, 15);
            using SolidBrush fill = new SolidBrush(ModernTheme.Surface);
            using Pen border = new Pen(ModernTheme.Line);
            graphics.FillPath(fill, path);
            graphics.DrawPath(border, path);
        }

        private static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            using GraphicsPath path = RoundedPath(bounds, radius);
            graphics.FillPath(brush, path);
        }

        private static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static string FormatMoney(decimal value)
            => value.ToString("C2", CultureInfo.CurrentCulture);

        private sealed record BookPerformance(string Title, int Units, decimal Revenue);
    }
}
