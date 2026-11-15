using BilliardsManager.BUS;
using System.Data;

namespace BilliardsManager.GUI;

public sealed class MainForm : Form
{
    readonly BilliardsService service = new();
    readonly TabControl tabs = new() { Dock = DockStyle.Fill };
    readonly FlowLayoutPanel floor = new() { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(16), BackColor = Color.FromArgb(246, 248, 251) };
    readonly DataGridView menuGrid = Grid(), billGrid = Grid(), reportGrid = Grid();
    readonly ComboBox tableChoice = new() { Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
    readonly ComboBox serviceChoice = new() { Width = 235, DropDownStyle = ComboBoxStyle.DropDownList };
    readonly NumericUpDown amount = new() { Width = 75, Minimum = 1, Maximum = 999 };
    readonly TextBox name = new() { Width = 190 };
    readonly NumericUpDown price = new() { Width = 110, Minimum = 0, Maximum = 10000000, DecimalPlaces = 0, ThousandsSeparator = true };
    readonly NumericUpDown stock = new() { Width = 90, Minimum = 0, Maximum = 100000, ThousandsSeparator = true };
    readonly CheckBox rental = new() { Text = "Dịch vụ thuê (không trừ kho)", AutoSize = true };
    readonly Label current = new() { AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(30, 68, 112) };
    int? selectedTable, activeBill, selectedMenu;
    DataTable? lines;

    public MainForm()
    {
        Text = "Bida Club | Quản lý bàn & dịch vụ"; Width = 1440; Height = 900; MinimumSize = new Size(1100, 720);
        StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10); BackColor = Color.FromArgb(246, 248, 251);
        BuildFloor(); BuildOrders(); BuildMenu(); BuildReports(); Controls.Add(tabs); Shown += (_, _) => RefreshAll();
    }
    static DataGridView Grid() => new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
    static Label L(string s) => new() { Text = s, AutoSize = true, Margin = new Padding(7, 9, 2, 2) };
    static Button B(string s, EventHandler h) { var b = new Button { Text = s, AutoSize = true, Height = 36, Margin = new Padding(4), BackColor = Color.White, FlatStyle = FlatStyle.Flat }; b.Click += h; return b; }
    TabPage Page(string s) { var p = new TabPage(s) { Padding = new Padding(12), BackColor = BackColor }; tabs.TabPages.Add(p); return p; }
    FlowLayoutPanel Bar(int h) => new() { Dock = DockStyle.Top, Height = h, Padding = new Padding(7), WrapContents = true, BackColor = Color.White };

    void BuildFloor()
    {
        var p = Page("Sơ đồ bàn"); var head = Bar(100);
        head.Controls.AddRange(new Control[] { new Label { Text = "BIDA CLUB  |  SƠ ĐỒ BÀN TRỰC QUAN", AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(30, 68, 112), Margin = new Padding(10, 8, 30, 0) }, B("Làm mới sơ đồ", (_, _) => LoadFloor()) });
        floor.Top = head.Bottom; p.Controls.Add(floor); p.Controls.Add(head);
    }
    void BuildOrders()
    {
        var p = Page("Gọi dịch vụ / thanh toán"); var top = Bar(104);
        top.Controls.AddRange(new Control[] { L("Bàn đang chơi"), tableChoice, L("Dịch vụ"), serviceChoice, L("Số lượng"), amount, B("Thêm dịch vụ", (_, _) => AddLine()), B("Lưu chi tiết (DataAdapter)", (_, _) => SaveLines()), B("Thanh toán", (_, _) => Pay()), B("Làm mới", (_, _) => LoadCurrent()) });
        top.Controls.Add(current); billGrid.ReadOnly = false; billGrid.AllowUserToAddRows = false; billGrid.Columns.Clear();
        p.Controls.Add(billGrid); p.Controls.Add(top); tableChoice.SelectedIndexChanged += (_, _) => LoadCurrent();
    }
    void BuildMenu()
    {
        var p = Page("Danh mục dịch vụ"); var top = Bar(102);
        top.Controls.AddRange(new Control[] { L("Tên dịch vụ"), name, L("Đơn giá"), price, L("Tồn kho"), stock, rental, B("Thêm mới", (_, _) => SaveMenu(null)), B("Cập nhật", (_, _) => SaveMenu(selectedMenu)), B("Tải danh sách", (_, _) => LoadMenu()) });
        p.Controls.Add(menuGrid); p.Controls.Add(top); menuGrid.SelectionChanged += (_, _) => SelectMenu();
    }
    void BuildReports()
    {
        var p = Page("Hóa đơn / báo cáo"); var top = Bar(50); top.Controls.Add(B("Tải hóa đơn gần đây", (_, _) => LoadReports())); p.Controls.Add(reportGrid); p.Controls.Add(top);
    }
    void RefreshAll() { try { LoadMenu(); LoadFloor(); LoadActiveTables(); LoadReports(); } catch (Exception ex) { Error(ex); } }
    void LoadFloor()
    {
        try
        {
            floor.Controls.Clear();
            foreach (DataRow r in service.Data.Tables().Rows)
            {
                int id = r.Field<int>("MaBan"); bool busy = r.Field<string>("TrangThai") == "Đang chơi";
                var b = new Button { Width = 225, Height = 145, Margin = new Padding(12), FlatStyle = FlatStyle.Flat, BackColor = busy ? Color.FromArgb(244, 151, 61) : Color.FromArgb(56, 160, 105), ForeColor = Color.White, Font = new Font("Segoe UI", 15, FontStyle.Bold), Text = $"{r["TenBan"]}\n{r["LoaiBan"]}\n{(busy ? "ĐANG CHƠI" : "TRỐNG")}\n{r.Field<decimal>("GiaTheoGio"):N0} đ / giờ", Tag = id };
                b.Click += (_, _) => SelectTable((int)b.Tag); floor.Controls.Add(b);
            }
        }
        catch (Exception ex) { Error(ex); }
    }
    void SelectTable(int id)
    {
        var row = service.Data.Tables().Select($"MaBan={id}").FirstOrDefault(); if (row is null) return;
        selectedTable = id; if (row["TrangThai"].ToString() == "Trống")
        {
            if (MessageBox.Show($"Mở {row["TenBan"]} với giá {Convert.ToDecimal(row["GiaTheoGio"]):N0} đ/giờ?", "Xác nhận mở bàn", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { activeBill = service.Data.OpenTable(id); MessageBox.Show("Đã mở bàn và tạo hóa đơn.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            catch (Exception ex) { Error(ex); return; }
        }
        else activeBill = service.Data.ActiveBill(id);
        LoadFloor(); LoadActiveTables(); tabs.SelectedIndex = 1; LoadCurrent();
    }
    void LoadActiveTables()
    {
        var old = selectedTable; var dt = service.Data.ActiveTables(); tableChoice.DataSource = dt; tableChoice.DisplayMember = "TenBan"; tableChoice.ValueMember = "MaBan";
        if (old.HasValue && dt.Select($"MaBan={old.Value}").Length > 0) tableChoice.SelectedValue = old.Value; else if (dt.Rows.Count > 0) tableChoice.SelectedIndex = 0;
    }
    void LoadCurrent()
    {
        if (tableChoice.SelectedValue is null || tableChoice.SelectedValue is DataRowView || !int.TryParse(tableChoice.SelectedValue.ToString(), out var id)) return;
        selectedTable = id; activeBill = service.Data.ActiveBill(id); if (activeBill == 0) { lines = null; billGrid.DataSource = null; current.Text = "Chưa có hóa đơn mở."; return; }
        lines = service.Data.BillLines(activeBill.Value); billGrid.DataSource = lines; if (billGrid.Columns.Contains("MaHD")) billGrid.Columns["MaHD"].Visible = false; if (billGrid.Columns.Contains("MaDV")) billGrid.Columns["MaDV"].ReadOnly = true; if (billGrid.Columns.Contains("TenDV")) billGrid.Columns["TenDV"].ReadOnly = true; if (billGrid.Columns.Contains("DonGia")) billGrid.Columns["DonGia"].ReadOnly = true;
        var row = service.Data.Tables().Select($"MaBan={id}").FirstOrDefault(); current.Text = $"{row?["TenBan"]}  •  Hóa đơn #{activeBill}";
    }
    void AddLine()
    {
        try
        {
            if (activeBill is null || activeBill == 0 || lines is null) throw new InvalidOperationException("Chọn bàn đang chơi trước.");
            if (serviceChoice.SelectedItem is not DataRowView item) throw new InvalidOperationException("Danh mục chưa có dịch vụ.");
            int id = Convert.ToInt32(item["MaDV"]); var existing = lines.Select($"MaDV={id}").FirstOrDefault();
            if (existing is not null) existing["SoLuong"] = Convert.ToInt32(existing["SoLuong"]) + (int)amount.Value;
            else { var r = lines.NewRow(); r["MaHD"] = activeBill.Value; r["MaDV"] = id; r["TenDV"] = item["TenDV"]; r["SoLuong"] = (int)amount.Value; r["DonGia"] = Convert.ToDecimal(item["DonGia"]); lines.Rows.Add(r); }
            billGrid.Refresh();
        }
        catch (Exception ex) { Error(ex); }
    }
    void SaveLines() { try { if (activeBill is null || activeBill == 0 || lines is null) throw new InvalidOperationException("Chọn hóa đơn đang mở."); service.Data.SaveBillLines(lines, activeBill.Value); LoadCurrent(); MessageBox.Show("Đã lưu chi tiết gọi món bằng SqlDataAdapter / DataTable."); } catch (Exception ex) { Error(ex); } }
    void Pay()
    {
        try { if (activeBill is null || activeBill == 0) throw new InvalidOperationException("Không có hóa đơn đang mở."); if (lines?.GetChanges() is not null) service.Data.SaveBillLines(lines, activeBill.Value); var bill = service.Data.Pay(activeBill.Value); MessageBox.Show($"Thanh toán thành công\nTiền giờ: {Convert.ToDecimal(bill.Rows[0]["TienGioChoi"]):N0} đ\nTiền dịch vụ: {Convert.ToDecimal(bill.Rows[0]["TienDichVu"]):N0} đ\nTổng cộng: {Convert.ToDecimal(bill.Rows[0]["TongTien"]):N0} đ", "Hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information); activeBill = null; LoadFloor(); LoadActiveTables(); LoadReports(); LoadCurrent(); }
        catch (Exception ex) { Error(ex); }
    }
    void LoadMenu() { menuGrid.DataSource = service.Data.Menu(); var dt = service.Data.Menu(); serviceChoice.DataSource = dt; serviceChoice.DisplayMember = "TenDV"; serviceChoice.ValueMember = "MaDV"; serviceChoice.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1; }
    void SelectMenu() { if (menuGrid.CurrentRow?.DataBoundItem is not DataRowView r) return; selectedMenu = Convert.ToInt32(r["MaDV"]); name.Text = r["TenDV"].ToString(); price.Value = Math.Min(price.Maximum, Convert.ToDecimal(r["DonGia"])); stock.Value = Math.Min(stock.Maximum, Convert.ToDecimal(r["TonKho"])); rental.Checked = Convert.ToBoolean(r["LaDichVuThue"]); }
    void SaveMenu(int? id) { try { service.ValidateMenu(name.Text, price.Value, (int)stock.Value); service.Data.SaveMenu(id, name.Text.Trim(), price.Value, (int)stock.Value, rental.Checked); LoadMenu(); } catch (Exception ex) { Error(ex); } }
    void LoadReports() { reportGrid.DataSource = service.Data.Bills(); }
    void Error(Exception ex) => MessageBox.Show(ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
