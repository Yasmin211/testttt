# CLAUDE.md - AI Assistant Guide for QLNhaThuoc

This document provides comprehensive guidance for AI assistants working with the QLNhaThuoc (Pharmacy Management System) codebase.

## Table of Contents

- [Project Overview](#project-overview)
- [Technical Stack](#technical-stack)
- [Codebase Structure](#codebase-structure)
- [Database Schema](#database-schema)
- [Development Workflows](#development-workflows)
- [Key Conventions](#key-conventions)
- [Important Files](#important-files)
- [Coding Patterns](#coding-patterns)
- [Working with This Codebase](#working-with-this-codebase)
- [Common Tasks](#common-tasks)
- [Troubleshooting](#troubleshooting)

---

## Project Overview

**Project Name:** QLNhaThuoc (Quản lý Nhà Thuốc - Pharmacy Management System)

**Purpose:** Desktop application for comprehensive pharmacy business management including inventory control, supplier relationships, employee management, goods import/export, and business analytics.

**Primary Language:** Vietnamese (UI, comments, documentation)

**Team Location:** Vietnam-based development team

**Current Status:** Active development on branch `claude/claude-md-mhzu9jdrtc50j52z-01Cm14pSuA1ZqwqyZwo8AK5d`

---

## Technical Stack

### Core Framework
- **Language:** C#
- **Framework:** .NET 8.0-windows
- **Application Type:** Windows Forms Desktop Application
- **Target Platform:** Windows x64/x86 (Any CPU)

### UI Components
- **DevExpress.Win.Design** (v25.1.5) - Professional WinForms UI controls
  - XtraForm, XtraGrid, XtraEditors suite
  - Ribbon-style navigation
  - Advanced data grids with filtering/sorting
- **HIC.System.Windows.Forms.DataVisualization** (v1.0.1) - Charts and data visualization
- **Standard WinForms Controls** - DataGridView, Panel, TableLayoutPanel

### Database
- **Database System:** Microsoft SQL Server (Express Edition)
- **Server Instance:** `YEN\SQLEXPRESS01`
- **Database Name:** `QLBH_NhaThuoc`
- **Data Access:** Direct ADO.NET (SqlClient)
- **Authentication:** Windows Authentication (default)
- **Connection String Location:** `QLNhaThuoc/App.config`

### Build System
- **Build Tool:** MSBuild via .NET SDK
- **Project Format:** SDK-style .csproj
- **Package Manager:** NuGet
- **Solution Format:** Visual Studio 2017+ (.sln)

---

## Codebase Structure

```
/home/user/testttt/
├── .git/                                    # Git repository
├── .gitignore                               # Standard Visual Studio gitignore
├── .gitattributes                           # Git line ending configuration
├── QLNhaThuoc.sln                          # Visual Studio solution file
└── QLNhaThuoc/                             # Main project directory
    ├── App.config                          # ⚙️ Configuration (DB connection, DevExpress)
    ├── Program.cs                          # 🚀 Application entry point
    ├── QLNhaThuoc.csproj                   # Project file with dependencies
    ├── logo.jpg                            # Application logo
    │
    ├── Properties/                         # Project properties
    │   ├── Settings.Designer.cs
    │   └── Settings.settings
    │
    ├── Core Forms/                         # Main functional forms
    │   ├── FormWarehouse.cs                # 📦 Inventory management (387 lines)
    │   ├── frmmain.cs                      # 🏠 Main application window (80 lines)
    │   ├── nhaphang.cs                     # 📥 Goods import management (698 lines)
    │   ├── nhacungcap.cs                   # 🏢 Supplier management (399 lines)
    │   └── nhanvien.cs                     # 👥 Employee management (398 lines)
    │
    ├── Detail Forms/                       # Detail/dialog forms
    │   ├── chitietncc.cs                   # Supplier details
    │   ├── chitietnhanvien.cs              # Employee details
    │   └── chitietnhaphang.cs              # Import details
    │
    ├── Report Forms/                       # Reporting and analytics
    │   ├── FormRevenueReport.cs            # 💰 Sales revenue analysis (368 lines)
    │   ├── FormImportReport.cs             # 📊 Import statistics (340 lines)
    │   ├── FormInventoryReport.cs          # 📈 Inventory reports (363 lines)
    │   └── FormReturnReport.cs             # 🔄 Return management (330 lines)
    │
    ├── Legacy Forms/                       # Older/placeholder forms
    │   ├── Form1.cs                        # Legacy form
    │   ├── Form2.cs                        # Legacy form (NhaThuoc_QLBH namespace)
    │   ├── Form11.cs                       # Legacy form
    │   └── Form thêm mới.cs                # Legacy add form
    │
    └── Designer Files/                     # Auto-generated UI definitions
        ├── *.Designer.cs                   # 12 designer files
        └── *.resx                          # Resource files (including Form11.vi-VN.resx)
```

### File Statistics
- **Total Size:** 1.2 MB
- **Total Code Lines:** 7,733 lines of C#
- **Code Files:** 29 .cs files
- **Designer Files:** 12 .Designer.cs files
- **Resource Files:** Multiple .resx files with Vietnamese localization

---

## Database Schema

### Connection Configuration

**Location:** `/home/user/testttt/QLNhaThuoc/App.config`

**Default Connection:**
```xml
<connectionStrings>
    <add name="Db"
         connectionString="Data Source=YEN\SQLEXPRESS01;Initial Catalog=QLBH_NhaThuoc;
         Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Database Tables

#### **Thuoc** (Medications)
Primary table for drug/medication inventory.

| Column | Type | Description |
|--------|------|-------------|
| MaThuoc | PK | Drug code (unique identifier) |
| TenThuoc | String | Drug name |
| HoatChat | String | Active ingredient |
| DongGoi | String | Packaging information |
| MaDonViTinh | FK | Unit of measure reference |
| MaNCC | FK | Supplier reference |
| GiaBan | Decimal | Selling price |

#### **TonKho** (Inventory/Stock)
Tracks current stock levels and expiration.

| Column | Type | Description |
|--------|------|-------------|
| MaLoSX | String | Manufacturing lot/batch code |
| MaThuoc | FK | Drug code reference |
| TonKho | Int | Quantity in stock |
| HanSuDung | DateTime | Expiration date |

#### **DonViTinh** (Units of Measure)
Reference table for measurement units.

| Column | Type | Description |
|--------|------|-------------|
| MaDonViTinh | PK | Unit identifier |
| TenDonViTinh | String | Unit name (Viên, Hộp, Chai, etc.) |

#### **NhaCungCap** (Suppliers)
Supplier information and relationships.

| Column | Type | Description |
|--------|------|-------------|
| MaNCC | PK | Supplier ID |
| TenNCC | String | Supplier name |
| [Additional Fields] | Various | Contact info, address, etc. |

#### **NhanVien** (Employees)
Employee records and management.

| Column | Type | Description |
|--------|------|-------------|
| MaNV | PK | Employee ID |
| [Additional Fields] | Various | Name, position, contact, etc. |

#### **PhieuNhap** (Import Orders)
Goods receipt/import transactions.

| Column | Type | Description |
|--------|------|-------------|
| MaPhieuNhap | PK | Import order ID |
| NgayNhap | DateTime | Import date |
| MaNCC | FK | Supplier reference |
| [Additional Fields] | Various | Total, status, etc. |

#### **ChiTietPhieuNhap** (Import Order Details)
Line items for import orders.

| Column | Type | Description |
|--------|------|-------------|
| MaPhieuNhap | FK | Import order reference |
| MaThuoc | FK | Drug code reference |
| SoLuong | Int | Quantity imported |
| DonGia | Decimal | Unit price |
| [Additional Fields] | Various | Lot, expiry, etc. |

### SQL Query Patterns

**Standard JOIN pattern used throughout:**
```sql
SELECT
    t.MaThuoc,
    t.TenThuoc,
    t.HoatChat,
    t.DongGoi,
    d.TenDonViTinh AS DonViTinh,
    k.MaLoSX,
    k.HanSuDung,
    k.TonKho,
    n.TenNCC AS NhaCungCap,
    t.GiaBan
FROM Thuoc t
LEFT JOIN TonKho k ON t.MaThuoc = k.MaThuoc
LEFT JOIN DonViTinh d ON t.MaDonViTinh = d.MaDonViTinh
LEFT JOIN NhaCungCap n ON t.MaNCC = n.MaNCC
```

---

## Development Workflows

### Git Workflow

**Current Branch Strategy:**
- Development happens on feature branches prefixed with `claude/`
- Branch naming: `claude/claude-md-<session-id>-<unique-id>`
- Current branch: `claude/claude-md-mhzu9jdrtc50j52z-01Cm14pSuA1ZqwqyZwo8AK5d`

**Commit Message Style:**
- Vietnamese language
- Concise descriptions
- Examples from history:
  - "Nối SQL" (Connect SQL)
  - "sửa" (fix)
  - "Update project on branch Yến"

**Push Requirements:**
```bash
# Always use -u flag for first push
git push -u origin <branch-name>

# Branch must start with 'claude/' and match session ID
# Otherwise push will fail with 403 HTTP code
```

### Build Workflow

**Debug Build:**
```bash
dotnet build QLNhaThuoc.sln --configuration Debug
```

**Release Build:**
```bash
dotnet build QLNhaThuoc.sln --configuration Release
```

**Run Application:**
```bash
cd QLNhaThuoc
dotnet run
```

**Restore Dependencies:**
```bash
dotnet restore QLNhaThuoc.sln
```

### Database Setup Workflow

1. **Install SQL Server Express** (if not already installed)
2. **Create Database:**
   ```sql
   CREATE DATABASE QLBH_NhaThuoc;
   ```
3. **Run Schema Scripts** (create tables in order due to FK dependencies):
   - DonViTinh (no dependencies)
   - NhaCungCap (no dependencies)
   - NhanVien (no dependencies)
   - Thuoc (depends on DonViTinh, NhaCungCap)
   - TonKho (depends on Thuoc)
   - PhieuNhap (depends on NhaCungCap)
   - ChiTietPhieuNhap (depends on PhieuNhap, Thuoc)

4. **Update Connection String** in `App.config` if needed
5. **Test Connection** - Application tests on startup

---

## Key Conventions

### Naming Conventions

**Namespaces:**
- **Primary:** `QLNhaThuoc` (use for all new code)
- **Legacy:** `NhaThuoc_QLBH` (being phased out)

**Classes:**
- **Forms:** PascalCase with "Form" prefix or suffix
  - `FormWarehouse`, `FormRevenueReport`
  - `frmmain`, `nhaphang`, `nhacungcap`, `nhanvien` (legacy style)
- **Models:** PascalCase, typically nested in form classes
  - `FormWarehouse.Thuoc`
  - `FormRevenueReport.SaleOrder`

**Variables:**
- **Private fields:** `_camelCase` with underscore prefix
  - `_all`, `_view`, `_connectionString`
- **Controls:** `controlTypePrefix` + `PascalCase`
  - `dgv` (DataGridView), `btn` (Button), `txt` (TextBox)
  - `gb` (GroupBox), `cb` (ComboBox), `dt` (DateTimePicker)
  - Examples: `dgvThuoc`, `btnAdd`, `txtSearch`, `gbFilter`, `dtFrom`

**Database Objects:**
- **Tables:** PascalCase Vietnamese names
  - `Thuoc`, `NhaCungCap`, `TonKho`
- **Columns:** PascalCase with prefixes
  - `Ma` prefix for IDs: `MaThuoc`, `MaNCC`, `MaNV`
  - `Ten` prefix for names: `TenThuoc`, `TenNCC`, `TenDonViTinh`
  - Descriptive names: `HoatChat`, `DongGoi`, `HanSuDung`

### Code Organization

**Form Structure Pattern:**
```csharp
namespace QLNhaThuoc
{
    public class FormName : Form  // or DevExpress.XtraEditors.XtraForm
    {
        // ===== MODEL =====
        public class ModelName { /* properties */ }

        // ===== DATA =====
        private readonly BindingList<ModelName> _all = new();
        private readonly BindingList<ModelName> _view = new();

        // ===== UI CONTROLS =====
        private readonly ControlType controlName = new() { /* initializers */ };

        // ===== CONSTRUCTOR =====
        public FormName()
        {
            // Set form properties
            BuildUI();
            LoadDataFromDatabase();
        }

        // ===== DATA LOADING =====
        private void LoadDataFromDatabase() { /* ADO.NET code */ }

        // ===== UI BUILDING =====
        private void BuildUI() { /* control layout */ }

        // ===== EVENT HANDLERS =====
        private void ControlName_Event(object sender, EventArgs e) { /* logic */ }

        // ===== HELPER METHODS =====
        private void HelperMethod() { /* utility code */ }
    }
}
```

### Comment Style

**Section Headers:**
```csharp
// ===== SECTION NAME =====
// ========= SECTION NAME =========
```

**Inline Comments:**
```csharp
// Vietnamese comments explaining logic
// Example: 🔍 KIỂM TRA KẾT NỐI SQL SERVER TRƯỚC
```

**Emoji Usage:**
- Common in comments and MessageBox text
- Examples: ✅ ❌ 🔍 🚀 📦 💰 📊

### Vietnamese Language Standards

**UI Text:** All labels, buttons, MessageBox text in Vietnamese
- Use proper Vietnamese diacritics (â, ă, ê, ô, ơ, ư, đ)
- Formal/professional tone

**Code Comments:** Vietnamese explanations
- Clear explanations of business logic
- Technical details in Vietnamese

**Variable Names:** Mix of English (technical) and Vietnamese (domain)
- Technical: `connectionString`, `dataGridView`, `bindingList`
- Domain: `Thuoc`, `NhaCungCap`, `HanSuDung`

---

## Important Files

### Critical Configuration Files

#### `/home/user/testttt/QLNhaThuoc/App.config`
**Purpose:** Application configuration
**Key Contents:**
- SQL Server connection string (name: "Db")
- DevExpress DPI awareness settings
- Application settings

**When to Modify:**
- Changing database server/instance
- Switching authentication method
- Updating DevExpress settings

#### `/home/user/testttt/QLNhaThuoc/QLNhaThuoc.csproj`
**Purpose:** Project definition and dependencies
**Key Contents:**
- Target framework (.NET 8.0-windows)
- NuGet package references
- Build configuration

**When to Modify:**
- Adding/updating NuGet packages
- Changing target framework
- Adjusting build settings

#### `/home/user/testttt/.gitignore`
**Purpose:** Git ignore rules
**Key Contents:**
- Standard Visual Studio ignore patterns
- Build artifacts (bin/, obj/, Debug/, Release/)
- User-specific files (*.user, *.suo)
- NuGet packages folder

**When to Modify:**
- Adding custom build outputs to ignore
- Excluding new temporary file types

### Core Application Files

#### `/home/user/testttt/QLNhaThuoc/Program.cs` (80 lines)
**Purpose:** Application entry point
**Key Responsibilities:**
1. Initialize DevExpress visual styles
2. Test SQL Server connectivity
3. Launch main form (`FormWarehouse`)

**Location Reference:** `QLNhaThuoc/Program.cs:1`

#### `/home/user/testttt/QLNhaThuoc/FormWarehouse.cs` (387 lines)
**Purpose:** Inventory management form (main/startup form)
**Key Features:**
- Drug inventory listing with DataGridView
- Multi-criteria filtering (date, supplier, stock level)
- Search functionality
- Add new medications
- Inventory report generation

**Model Classes:**
- `FormWarehouse.Thuoc` - Drug/medication model

**Location Reference:** `QLNhaThuoc/FormWarehouse.cs:1`

#### `/home/user/testttt/QLNhaThuoc/frmmain.cs` (80 lines)
**Purpose:** Main application shell with navigation
**Key Features:**
- DevExpress Ribbon-style menu
- Dynamic form loading into panel
- Navigation to: Employee, Supplier, Import modules
- Real-time clock in status bar

**Location Reference:** `QLNhaThuoc/frmmain.cs:1`

### Module Implementation Files

#### `/home/user/testttt/QLNhaThuoc/nhaphang.cs` (698 lines)
**Purpose:** Goods import/receipt management
**Key Features:**
- Import order creation and editing
- Multi-item import with line items
- Supplier selection
- Date tracking
- Detailed import records

**Location Reference:** `QLNhaThuoc/nhaphang.cs:1`

#### `/home/user/testttt/QLNhaThuoc/nhacungcap.cs` (399 lines)
**Purpose:** Supplier relationship management
**Key Features:**
- Supplier listing and search
- Supplier detail viewing
- Add/edit/delete suppliers
- Link suppliers to drugs

**Location Reference:** `QLNhaThuoc/nhacungcap.cs:1`

#### `/home/user/testttt/QLNhaThuoc/nhanvien.cs` (398 lines)
**Purpose:** Employee management
**Key Features:**
- Employee listing
- Employee detail viewing
- Add/edit/delete employees
- Role/position tracking

**Location Reference:** `QLNhaThuoc/nhanvien.cs:1`

### Report Implementation Files

#### `/home/user/testttt/QLNhaThuoc/FormRevenueReport.cs` (368 lines)
**Purpose:** Sales revenue analysis and reporting
**Key Features:**
- Revenue trends and charts
- Date range filtering
- CSV export
- Print functionality
- Visual data representation

**Location Reference:** `QLNhaThuoc/FormRevenueReport.cs:1`

#### `/home/user/testttt/QLNhaThuoc/FormImportReport.cs` (340 lines)
**Purpose:** Import statistics and analytics
**Key Features:**
- Supplier import analysis
- Import trend charts
- Date range filtering
- CSV export and printing

**Location Reference:** `QLNhaThuoc/FormImportReport.cs:1`

#### `/home/user/testttt/QLNhaThuoc/FormInventoryReport.cs` (363 lines)
**Purpose:** Inventory status reporting
**Key Features:**
- Stock level visualization
- Low stock alerts
- Expiration date tracking
- Export and print capabilities

**Location Reference:** `QLNhaThuoc/FormInventoryReport.cs:1`

#### `/home/user/testttt/QLNhaThuoc/FormReturnReport.cs` (330 lines)
**Purpose:** Return/refund management and reporting
**Key Features:**
- Return transaction tracking
- Return reason analysis
- Date filtering
- Export functionality

**Location Reference:** `QLNhaThuoc/FormReturnReport.cs:1`

---

## Coding Patterns

### Data Access Pattern

**Standard ADO.NET Approach:**
```csharp
private void LoadDataFromDatabase()
{
    _all.Clear();
    try
    {
        string cs = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        using SqlConnection con = new(cs);
        con.Open();

        string sql = @"SELECT ... FROM ... JOIN ...";
        using SqlCommand cmd = new(sql, con);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            _all.Add(new ModelClass
            {
                Property1 = reader["Column1"].ToString(),
                Property2 = Convert.ToInt32(reader["Column2"]),
                // ... manual mapping
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

**Key Characteristics:**
- Direct SqlConnection, SqlCommand, SqlDataReader
- Connection string from App.config
- Manual data mapping to POCOs
- Using statements for proper disposal
- Try-catch with user-friendly error messages
- BindingList<T> for data binding

### UI Building Pattern

**Programmatic Control Creation:**
```csharp
private readonly GroupBox gbFilter = new()
{
    Text = "Bộ lọc",
    Width = 410,
    Dock = DockStyle.Left,
    Padding = new Padding(12)
};

private readonly DataGridView dgv = new()
{
    Dock = DockStyle.Fill,
    BackgroundColor = Color.White,
    AutoGenerateColumns = false,
    ReadOnly = true,
    AllowUserToAddRows = false,
    SelectionMode = DataGridViewSelectionMode.FullRowSelect
};
```

**Layout Approach:**
- Mix of Dock-based layout and TableLayoutPanel
- Programmatic control initialization with object initializers
- Custom styling with Color.FromArgb()
- FlowLayoutPanel for button groups

### Data Binding Pattern

**BindingList with DataGridView:**
```csharp
// Data containers
private readonly BindingList<Thuoc> _all = new();  // All data from DB
private readonly BindingList<Thuoc> _view = new(); // Filtered view

// Bind to grid
dgv.DataSource = _view;

// Filter logic (LINQ on _all, results to _view)
private void ApplyFilter()
{
    _view.Clear();
    var filtered = _all.Where(x => /* filter conditions */);
    foreach (var item in filtered)
        _view.Add(item);
}
```

**Benefits:**
- Automatic UI updates when BindingList changes
- Clean separation between all data and filtered view
- LINQ-based filtering on in-memory data

### Search/Filter Pattern

**Multi-Criteria In-Memory Filtering:**
```csharp
private void ApplyFilter()
{
    _view.Clear();

    var query = _all.AsEnumerable();

    // Filter by date range
    if (chkDateRange.Checked)
        query = query.Where(x => x.Date >= dtFrom.Value && x.Date <= dtTo.Value);

    // Filter by supplier
    if (cbNcc.SelectedIndex > 0)
        query = query.Where(x => x.NhaCungCap == cbNcc.Text);

    // Filter by stock level
    if (chkLow.Checked)
        query = query.Where(x => x.TonKho < nudStock.Value);

    // Search by text
    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
    {
        string search = txtSearch.Text.ToLower();
        query = query.Where(x =>
            x.TenThuoc.ToLower().Contains(search) ||
            x.MaThuoc.ToLower().Contains(search));
    }

    foreach (var item in query)
        _view.Add(item);
}
```

### Event Handler Pattern

**Button Click Events:**
```csharp
private void BtnAdd_Click(object sender, EventArgs e)
{
    using FormAdd dialog = new();
    if (dialog.ShowDialog() == DialogResult.OK)
    {
        LoadDataFromDatabase(); // Reload after changes
        ApplyFilter();
    }
}
```

**DataGridView Events:**
```csharp
private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    var selected = _view[e.RowIndex];
    using FormDetail dialog = new(selected);
    dialog.ShowDialog();
}
```

### Export/Report Pattern

**CSV Export:**
```csharp
private void ExportToCsv()
{
    using SaveFileDialog sfd = new()
    {
        Filter = "CSV Files|*.csv",
        FileName = $"BaoCao_{DateTime.Now:yyyyMMdd}.csv"
    };

    if (sfd.ShowDialog() != DialogResult.OK) return;

    using StreamWriter sw = new(sfd.FileName, false, Encoding.UTF8);

    // Header
    sw.WriteLine("Cột1,Cột2,Cột3");

    // Data rows
    foreach (var item in _view)
        sw.WriteLine($"{item.Field1},{item.Field2},{item.Field3}");

    MessageBox.Show("Xuất file thành công!", "Thông báo");
}
```

**Chart Generation:**
```csharp
private void BuildChart()
{
    chart.Series.Clear();
    var series = chart.Series.Add("Doanh thu");
    series.ChartType = SeriesChartType.Column;

    foreach (var item in data)
        series.Points.AddXY(item.Label, item.Value);

    chart.ChartAreas[0].AxisX.Title = "Thời gian";
    chart.ChartAreas[0].AxisY.Title = "Doanh thu (VNĐ)";
}
```

### Error Handling Pattern

**Standard Try-Catch with User Feedback:**
```csharp
try
{
    // Database or business logic
}
catch (SqlException ex)
{
    MessageBox.Show($"❌ Lỗi cơ sở dữ liệu:\n{ex.Message}",
        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
catch (Exception ex)
{
    MessageBox.Show($"❌ Lỗi:\n{ex.Message}",
        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

**Startup Connection Test:**
```csharp
// In Program.cs
try
{
    using SqlConnection con = new(connectionString);
    con.Open();
    MessageBox.Show("✅ Kết nối SQL Server thành công!",
        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
catch (Exception ex)
{
    MessageBox.Show($"❌ Lỗi kết nối SQL Server:\n{ex.Message}",
        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return; // Exit application
}
```

### Custom UI Styling Pattern

**Rounded Corners and Custom Drawing:**
```csharp
private GraphicsPath TaoPath(Rectangle rect, int radius)
{
    GraphicsPath path = new();
    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
    path.CloseFigure();
    return path;
}

private void BoGocVaVien(Control ctrl, int radius, Color borderColor)
{
    ctrl.Region = new Region(TaoPath(ctrl.ClientRectangle, radius));
    // Custom border drawing in Paint event
}
```

---

## Working with This Codebase

### Prerequisites

**Development Environment:**
- Visual Studio 2022 (or later) or Visual Studio Code with C# extension
- .NET 8.0 SDK
- SQL Server 2019/2022 Express or higher
- DevExpress WinForms components (v25.1.5)

**Database Setup:**
- SQL Server instance running
- Windows Authentication enabled OR SQL Server authentication configured
- Database `QLBH_NhaThuoc` created and populated

### Getting Started

**1. Clone Repository:**
```bash
git clone <repository-url>
cd testttt
```

**2. Restore Dependencies:**
```bash
dotnet restore QLNhaThuoc.sln
```

**3. Configure Database Connection:**
Edit `QLNhaThuoc/App.config`:
```xml
<add name="Db"
     connectionString="Data Source=YOUR_SERVER\INSTANCE;Initial Catalog=QLBH_NhaThuoc;
     Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
     providerName="System.Data.SqlClient" />
```

**4. Build Solution:**
```bash
dotnet build QLNhaThuoc.sln
```

**5. Run Application:**
```bash
cd QLNhaThuoc
dotnet run
```

### Development Guidelines

**When Adding New Features:**

1. **Create Feature Branch:**
   ```bash
   git checkout -b claude/feature-name-<unique-id>
   ```

2. **Follow Namespace Convention:**
   - Use `QLNhaThuoc` namespace (not `NhaThuoc_QLBH`)

3. **Follow Form Structure Pattern:**
   - Models nested in form class
   - Use BindingList for data
   - Separate _all (all data) and _view (filtered)
   - LoadDataFromDatabase method for data loading

4. **Database Changes:**
   - Create SQL migration scripts
   - Update schema documentation
   - Test with existing data

5. **UI Consistency:**
   - Use DevExpress controls when possible
   - Follow existing color scheme
   - Vietnamese labels and messages
   - Consistent control naming (btn, txt, dgv, etc.)

6. **Error Handling:**
   - Always wrap database calls in try-catch
   - Show user-friendly Vietnamese error messages
   - Log errors if logging is implemented

7. **Testing:**
   - Manual test all UI interactions
   - Test database CRUD operations
   - Verify filters and search functionality
   - Test with various data scenarios

8. **Commit and Push:**
   ```bash
   git add .
   git commit -m "Vietnamese description of changes"
   git push -u origin claude/feature-name-<unique-id>
   ```

**When Fixing Bugs:**

1. **Identify Location:**
   - Check which form/module has the issue
   - Review related database queries

2. **Reproduce Issue:**
   - Test with sample data
   - Note error messages or unexpected behavior

3. **Fix and Test:**
   - Apply fix following existing patterns
   - Test affected functionality thoroughly
   - Check for side effects in related features

4. **Update Documentation:**
   - Update this CLAUDE.md if patterns change
   - Add comments explaining non-obvious fixes

**When Refactoring:**

1. **Maintain Compatibility:**
   - Don't break existing functionality
   - Keep database schema compatible

2. **Migrate Legacy Code:**
   - Move from `NhaThuoc_QLBH` to `QLNhaThuoc` namespace
   - Update to modern C# patterns (using, object initializers)

3. **Extract Common Code:**
   - Consider creating utility classes for repeated patterns
   - Database connection helper
   - Common UI styling methods

4. **Document Changes:**
   - Update CLAUDE.md with new patterns
   - Comment why refactoring was needed

---

## Common Tasks

### Task: Add a New Form/Module

**Steps:**

1. **Create Form Class:**
```csharp
namespace QLNhaThuoc
{
    public class FormNewFeature : Form
    {
        // Follow FormWarehouse.cs pattern
        public class DataModel { /* properties */ }

        private readonly BindingList<DataModel> _all = new();
        private readonly BindingList<DataModel> _view = new();

        // UI controls...

        public FormNewFeature()
        {
            Text = "Tên Tính Năng";
            // Setup...
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase() { /* ... */ }
    }
}
```

2. **Add Navigation:**
In `frmmain.cs`, add button click handler:
```csharp
private void BtnNewFeature_ItemClick(object sender, EventArgs e)
{
    panel.Controls.Clear();
    FormNewFeature frm = new() { TopLevel = false, Dock = DockStyle.Fill };
    panel.Controls.Add(frm);
    frm.Show();
}
```

3. **Create Database Tables** (if needed)
4. **Test Functionality**
5. **Commit Changes**

### Task: Add a New Report

**Steps:**

1. **Create Report Form:**
Follow pattern from `FormRevenueReport.cs`, `FormImportReport.cs`

2. **Add Chart Component:**
```csharp
using System.Windows.Forms.DataVisualization.Charting;

private readonly Chart chart = new() { Dock = DockStyle.Fill };
```

3. **Implement Data Loading:**
```csharp
private void LoadReportData()
{
    // Query database with date filtering
    // Populate data model
    // Update chart/grid
}
```

4. **Add Export Functionality:**
```csharp
private void ExportToCsv() { /* see pattern above */ }
private void PrintReport() { /* use System.Drawing.Printing */ }
```

5. **Add to Main Menu**

### Task: Modify Database Schema

**Steps:**

1. **Create Migration Script:**
```sql
-- File: migrations/001_add_column_xyz.sql
USE QLBH_NhaThuoc;
GO

ALTER TABLE Thuoc
ADD NewColumn NVARCHAR(100) NULL;
GO
```

2. **Update Model Classes:**
```csharp
public class Thuoc
{
    // ... existing properties
    public string NewColumn { get; set; } = "";
}
```

3. **Update SQL Queries:**
Add new column to SELECT and INSERT statements

4. **Update UI:**
Add new column to DataGridView if needed

5. **Test Migration:**
- Backup database
- Run migration
- Test all affected forms

6. **Document Changes:**
Update database schema section in this file

### Task: Fix a Database Connection Issue

**Checklist:**

1. **Verify SQL Server is Running:**
```bash
# Windows Services or SQL Server Configuration Manager
```

2. **Check Connection String:**
In `App.config`, verify:
- Data Source (server\instance)
- Initial Catalog (database name)
- Authentication method

3. **Test Connection Manually:**
Use SQL Server Management Studio (SSMS) with same credentials

4. **Check Firewall:**
Ensure SQL Server port (default 1433) is not blocked

5. **Review Application Startup:**
`Program.cs:22-38` tests connection before launching

6. **Check Error Message:**
Connection test shows specific error in MessageBox

### Task: Add Filtering to Existing Form

**Steps:**

1. **Add Filter UI Controls:**
```csharp
private readonly ComboBox cbFilter = new() { DropDownStyle = ComboBoxStyle.DropDownList };
private readonly Button btnApply = new() { Text = "Áp dụng" };
```

2. **Populate Filter Options:**
```csharp
cbFilter.Items.Add("Tất cả");
// Load options from database or enum
```

3. **Implement Filter Logic:**
```csharp
private void BtnApply_Click(object sender, EventArgs e)
{
    ApplyFilter();
}

private void ApplyFilter()
{
    _view.Clear();
    var filtered = _all.Where(x => /* filter conditions */);
    foreach (var item in filtered)
        _view.Add(item);
}
```

4. **Wire Up Events:**
```csharp
btnApply.Click += BtnApply_Click;
```

5. **Test Filter Combinations**

### Task: Debug a Runtime Error

**Common Debugging Steps:**

1. **Read Error Message:**
MessageBox shows Vietnamese error description

2. **Check Database Connection:**
Verify connection string and SQL Server status

3. **Review SQL Query:**
Copy query from code, test in SSMS

4. **Verify Data Types:**
Check reader casting: `Convert.ToInt32()`, `.ToString()`, etc.

5. **Check Null Values:**
Use null-conditional operators or null checks

6. **Add Try-Catch:**
```csharp
try
{
    // Problematic code
}
catch (Exception ex)
{
    MessageBox.Show($"Debug: {ex.Message}\n{ex.StackTrace}");
}
```

7. **Use Visual Studio Debugger:**
- Set breakpoint
- Step through code (F10/F11)
- Inspect variables
- Check call stack

---

## Troubleshooting

### Issue: Application Won't Start

**Possible Causes:**

1. **SQL Server Not Running**
   - Solution: Start SQL Server service
   - Check: Services → SQL Server (SQLEXPRESS01)

2. **Connection String Incorrect**
   - Solution: Verify `App.config` connection string
   - Check: Server name, database name, authentication

3. **Database Doesn't Exist**
   - Solution: Create `QLBH_NhaThuoc` database
   - Run schema creation scripts

4. **Missing Dependencies**
   - Solution: `dotnet restore QLNhaThuoc.sln`
   - Check: DevExpress license

5. **Port Conflict**
   - Solution: Close other apps using same port
   - Check: Task Manager

### Issue: DevExpress Controls Not Rendering

**Possible Causes:**

1. **Missing NuGet Package**
   - Solution: Install DevExpress.Win.Design v25.1.5
   - Command: `dotnet add package DevExpress.Win.Design --version 25.1.5`

2. **License Issue**
   - Solution: Verify DevExpress license
   - Check: DevExpress License Manager

3. **DPI Scaling Issue**
   - Solution: Check App.config DPIAwarenessMode setting
   - Try: Set to "PerMonitorV2" or "System"

### Issue: Data Not Loading

**Debugging Steps:**

1. **Check Database Connection:**
```csharp
// In LoadDataFromDatabase method
MessageBox.Show($"Connection: {connectionString}"); // Verify string
```

2. **Test SQL Query:**
Copy query from code → Run in SSMS → Check results

3. **Verify Table Data:**
```sql
SELECT COUNT(*) FROM Thuoc;
SELECT TOP 10 * FROM Thuoc;
```

4. **Check Field Mapping:**
Ensure model properties match database column names

5. **Review Exception:**
Catch SqlException separately to see SQL-specific errors

### Issue: Filters Not Working

**Debugging Steps:**

1. **Check _all vs _view:**
Verify _all is populated from database

2. **Debug Filter Logic:**
```csharp
private void ApplyFilter()
{
    MessageBox.Show($"_all count: {_all.Count}");
    _view.Clear();
    var filtered = _all.Where(/* conditions */);
    MessageBox.Show($"Filtered count: {filtered.Count()}");
    // ...
}
```

3. **Verify Control Values:**
Check ComboBox.SelectedIndex, TextBox.Text, etc.

4. **Test LINQ Query:**
Simplify filter conditions one at a time

### Issue: UI Layout Problems

**Common Fixes:**

1. **Dock Order:**
Add controls to parent in correct order

2. **Anchor vs Dock:**
Use Dock for full container filling, Anchor for resizing

3. **TableLayoutPanel:**
Verify RowCount, ColumnCount match controls

4. **Custom Sizing:**
Set MinimumSize, MaximumSize for constraints

5. **DPI Scaling:**
Check App.config DPI settings, use AutoScaleMode.Dpi

### Issue: Vietnamese Text Displays Incorrectly

**Fixes:**

1. **File Encoding:**
Save .cs files as UTF-8 with BOM

2. **Database Collation:**
Use Vietnamese collation (e.g., Vietnamese_CI_AS)

3. **Control Font:**
Set Font to support Vietnamese (Segoe UI, Arial)

4. **CSV Export:**
Use UTF8 encoding:
```csharp
using StreamWriter sw = new(path, false, Encoding.UTF8);
```

### Issue: Git Push Fails with 403

**Solution:**

Branch name must start with `claude/` and match session ID

**Fix:**
```bash
# Rename branch to correct format
git branch -m claude/feature-name-<session-id>

# Push with -u flag
git push -u origin claude/feature-name-<session-id>
```

---

## Best Practices for AI Assistants

### 1. Always Read Before Writing
- Read existing form files to understand patterns
- Check current database schema before queries
- Review related code before making changes

### 2. Maintain Consistency
- Follow existing naming conventions
- Use established patterns (BindingList, LoadDataFromDatabase, etc.)
- Keep Vietnamese language for UI and comments

### 3. Test Database Changes
- Verify connection string before suggesting changes
- Consider foreign key constraints
- Check for existing data impact

### 4. Respect the Architecture
- UI-centric model is intentional (not a bug)
- Direct ADO.NET is preferred (don't suggest EF Core)
- DevExpress controls should be used when available

### 5. Vietnamese Context
- All user-facing text must be in Vietnamese
- Use proper diacritics
- Maintain professional/formal tone

### 6. Documentation
- Update this CLAUDE.md when adding significant features
- Add inline comments explaining complex logic
- Document database schema changes

### 7. Error Handling
- Always wrap database operations in try-catch
- Provide meaningful Vietnamese error messages
- Consider user experience in error scenarios

### 8. Git Hygiene
- Commit messages in Vietnamese
- Follow branch naming conventions
- Test before committing

---

## Additional Resources

### Key Technologies Documentation

- **.NET 8.0:** https://learn.microsoft.com/en-us/dotnet/
- **Windows Forms:** https://learn.microsoft.com/en-us/dotnet/desktop/winforms/
- **DevExpress WinForms:** https://docs.devexpress.com/WindowsForms/
- **ADO.NET:** https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/
- **SQL Server:** https://learn.microsoft.com/en-us/sql/

### Development Tools

- **Visual Studio 2022:** https://visualstudio.microsoft.com/
- **SQL Server Management Studio (SSMS):** https://learn.microsoft.com/en-us/sql/ssms/
- **.NET SDK:** https://dotnet.microsoft.com/download

### Project-Specific Commands

**Useful Git Commands:**
```bash
# Check current branch
git branch --show-current

# View recent commits
git log --oneline -5

# Create feature branch
git checkout -b claude/feature-<session-id>

# Push with upstream
git push -u origin claude/feature-<session-id>
```

**Useful .NET Commands:**
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build QLNhaThuoc.sln

# Run project
dotnet run --project QLNhaThuoc/QLNhaThuoc.csproj

# Clean build artifacts
dotnet clean

# List NuGet packages
dotnet list package
```

**Useful SQL Commands:**
```sql
-- Check database size
USE QLBH_NhaThuoc;
EXEC sp_spaceused;

-- List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check table schema
EXEC sp_help 'Thuoc';

-- View foreign keys
EXEC sp_fkeys 'Thuoc';
```

---

## Version History

**Version 1.0** (2025-11-15)
- Initial comprehensive documentation
- Documented current codebase structure
- Established coding patterns and conventions
- Created troubleshooting guide

---

## Contact & Support

**Development Team:** Vietnam-based pharmacy software team

**Language:** Vietnamese (primary), English (technical documentation)

**Git Repository:** /home/user/testttt

**Current Branch:** claude/claude-md-mhzu9jdrtc50j52z-01Cm14pSuA1ZqwqyZwo8AK5d

---

*This document is maintained for AI assistants working with the QLNhaThuoc codebase. Keep it updated as the project evolves.*
