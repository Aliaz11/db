# db — WinForms Bookstore & User Management

A small Windows desktop application: users register, sign in, browse a book
catalogue and keep a basket of saved books; an administrator manages the user
list, the catalogue and a simple sales report.

It is a .NET 8 WinForms app talking directly to a SQL Server LocalDB database.

<!-- Replace with a real screenshot: docs/images/screenshot.png -->
![Screenshot placeholder](docs/images/screenshot.png)

---

## Features

| Feature | Where |
| --- | --- |
| Register a new user (name, phone and/or email, birth date, gender, avatar, password) with field-by-field validation and a uniqueness check on username/email | `Form2` |
| Sign in with username + password | `Form4` |
| Password reset: enter your email, receive a numeric code by mail, verify it, choose a new password | `Form8` → `Passchg` |
| Admin: list, edit and delete users | `Form3` |
| Admin: list, edit and delete books; add a new book with a cover image | `Form6`, `Form7` |
| Browse the catalogue and save/buy selected books | `Form5` |
| Per-user basket of saved books, with removal | `Form9` |
| Admin: per-user purchase report with per-user and overall totals | `Form10` |

Passwords are stored as PBKDF2-SHA256 hashes (`PBKDF2$<iterations>$<salt>$<hash>`).
Rows left over from an earlier plaintext version are detected on sign-in and
re-hashed transparently.

There is no role table: **the account whose username is exactly `admin` is the
administrator.** The admin screens (`Form3`, `Form6`, `Form10`) check
`Security/Session.cs` on load and bounce anyone who is not a signed-in
administrator back to the sign-in screen.

---

## Tech stack

- **C# / .NET 8** (`net8.0-windows`), `Nullable` and `ImplicitUsings` enabled
- **Windows Forms** — one project, `db.csproj`, output `db.exe`
- **SQL Server LocalDB** via **Microsoft.Data.SqlClient** 6.0.2, database attached from `Stu2.mdf`
- **MailKit / MimeKit** 4.17 for sending verification codes over SMTP
- **System.Configuration.ConfigurationManager** 8.0.1 for `App.config`

Two namespaces coexist and both are load-bearing: `WinFormsApp3`
(`Program`, `Form1`, `Form2`, `Form3`, `CommonFieldValidatorFunctions`) and `db`
(everything else). This is historical; see [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

---

## Prerequisites

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server Express LocalDB** (ships with the *Data storage and processing*
  workload in the Visual Studio Installer, or as a standalone download)
- Either **Visual Studio 2022** (17.8+) with the *.NET desktop development*
  workload, or just the SDK and `dotnet build`

Check LocalDB is present:

```powershell
sqllocaldb info MSSQLLocalDB
```

---

## Setup

```powershell
git clone https://github.com/ali-az1/db.git
cd db
```

### 1. Create the database

Local `.mdf` / `.ldf` files are not tracked (see
[Known issues](#known-issues--roadmap)), so create the database yourself.

The shipped connection string attaches `|DataDirectory|Stu2.mdf`, and for a
desktop app `|DataDirectory|` is **the folder containing `db.exe`** — i.e.
`bin\Debug\net8.0-windows`, not the repository root. Build once so that folder
exists, then:

```powershell
sqllocaldb start MSSQLLocalDB

dotnet build
$dataDir = (Resolve-Path .\bin\Debug\net8.0-windows).Path

sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE Stu2 ON (NAME='Stu2', FILENAME='$dataDir\Stu2.mdf')"
sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql\schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql\seed.sql
```

If you would rather keep the database somewhere stable, create it wherever you
like and point `DB_CONNECTION` at the attached catalog instead — see the next
section.

> **Upgrading an existing database?** If your `Stu2.mdf` predates password
> hashing, `Stu1.Password` is `NVARCHAR(50)` — too narrow for the 83-character
> PBKDF2 record. Registration and password reset fail with
> `Msg 2628: String or binary data would be truncated`, and the automatic
> re-hash of legacy rows fails silently. Run this once:
>
> ```powershell
> sqlcmd -S "(localdb)\MSSQLLocalDB" -d Stu2 -i sql\migrations\001_widen_password_for_hashes.sql
> ```
>
> It is idempotent, leaves existing passwords untouched, and does nothing to a
> database created from `sql/schema.sql`, which already uses `NVARCHAR(200)`.

`sql/schema.sql` is idempotent — re-running it is safe. `sql/seed.sql` creates
the `admin` account and a few sample books; **it does not set a password**,
because password hashes can only be produced by the app. Set the admin password
by registering it on the sign-up screen or via the password-reset flow.

### 2. Configure the connection string

Resolution order (`db.Configuration.AppSettings.ConnectionString`):

1. `App.config` → `<connectionStrings><add name="Default" .../>`
2. the `DB_CONNECTION` environment variable
3. the built-in LocalDB default

The shipped `App.config` already points at `|DataDirectory|Stu2.mdf`. To use a
different server without editing the file:

```powershell
$env:DB_CONNECTION = "Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Stu2;Integrated Security=True;"
```

### 3. Configure SMTP (only needed for password reset)

Credentials are **never** read from `App.config` — `SMTP_PASSWORD` is
environment-only. The non-secret keys can come from either place.

| Environment variable | `App.config` `appSettings` key | Default | Notes |
| --- | --- | --- | --- |
| `SMTP_HOST` | `Smtp.Host` | `smtp.gmail.com` | |
| `SMTP_PORT` | `Smtp.Port` | `587` | |
| `SMTP_STARTTLS` | `Smtp.UseStartTls` | `true` | |
| `SMTP_USERNAME` | `Smtp.UserName` | *(empty)* | |
| `SMTP_PASSWORD` | — *(never)* | *(empty)* | Gmail requires an **app password**, not your account password |
| `SMTP_FROM` | `Smtp.From` | *(empty)* | |

```powershell
$env:SMTP_HOST     = "smtp.gmail.com"
$env:SMTP_PORT     = "587"
$env:SMTP_USERNAME = "you@example.com"
$env:SMTP_PASSWORD = "xxxx xxxx xxxx xxxx"
$env:SMTP_FROM     = "you@example.com"
```

If any of host / username / password is empty, `AppSettings.IsSmtpConfigured`
is `false` and the app reports that it could not send the code rather than
failing silently. Everything except password reset works without SMTP.

### 4. Build and run

```powershell
dotnet build
dotnet run --project db.csproj
```

Or open `db.sln` in Visual Studio 2022 and press <kbd>F5</kbd>.

### 5. Run the tests

The suite covers the password hashing and the validation patterns. It needs no
database and no SMTP server.

```powershell
dotnet test
```

---

## Project structure

Each screen is a separate `Form`; navigation goes through
`Navigation.GoTo(current, next)`, which repositions the next form, shows it and
then closes and disposes the current one. See
[`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for the full navigation graph.

### Screens

| File | Namespace | Purpose | Navigates to |
| --- | --- | --- | --- |
| `Program.cs` | `WinFormsApp3` | Entry point; runs the message loop until the last window closes | `Form1` |
| `Form1.cs` | `WinFormsApp3` | Main menu / landing screen; clears the session on load | `Form2`, `Form4` |
| `Form2.cs` | `WinFormsApp3` | Sign-up: validates the fields, checks username/email are free, inserts into `Stu1` | `Form1` |
| `Form3.cs` | `WinFormsApp3` | **Admin** user management: `ListView` over `Stu1`, edit and delete rows | `Form1`, `Form6`, `Form10` |
| `Form4.cs` | `db` | Sign-in. `admin` → `Form3`, anyone else → `Form9` | `Form3`, `Form9`, `Form8`, `Form1` |
| `Form5.cs` | `db` | Catalogue browser: grid over `Books` with a checkbox column; saves the ticked rows into `saver1` | `Form4`, `Form9` |
| `Form6.cs` | `db` | **Admin** book management: grid over `Books`, edit and delete | `Form3`, `Form7` |
| `Form7.cs` | `db` | **Admin** add a book (name, author, price, quantity, date, cover image) | `Form6` |
| `Form8.cs` | `db` | Forgot password: email address in, verification code out, 60-second resend cooldown | `Passchg`, `Form4` |
| `Form9.cs` | `db` | The signed-in user's saved books, with per-row delete | `Form5`, `Form1` |
| `Form10.cs` | `db` | **Admin** sales report: every `saver1` row grouped by user, with per-user and total takings | `Form3` |
| `Passchg.cs` | `db` | Choose a new password after the code was verified | `Form4` |

### Supporting types

| File | Purpose |
| --- | --- |
| `Locator.cs` | `GetConnectionString()` — the single connection-string accessor used by every screen |
| `Configuration/` | `AppSettings` — `App.config` → environment variable → default resolution for the connection string and all SMTP settings |
| `Security/` | `PasswordHasher` — PBKDF2 `Hash` / `Verify` / `NeedsUpgrade`; `Session` — the signed-in user and the admin-screen guard |
| `Data/` | `AuthService` / `IAuthService` — parameterised login lookup with transparent legacy-password upgrade |
| `Navigation.cs` | `GoTo(current, next)` — the single form-to-form transition, so no screen is left hidden and undisposed |
| `tests/db.Tests/` | xUnit tests for `PasswordHasher` and the validation patterns |
| `DataBaseCrud.cs` | CRUD for `Stu1`, plus the `Books` grid load/update/delete used by `Form6` |
| `DbCrudBook.cs` | `Books` insert and the `saver1` basket insert/delete |
| `LabelValidator.cs` | Field validation rules and the red inline error labels; also the username/email uniqueness pre-check |
| `dels.cs`, `Idels.cs` | `CommonFieldValidatorFunctions` and the validator delegate types |
| `regex.cs` | Validation patterns, including `Strong_Password_RegEx_Pattern` |
| `Book.cs`, `IUser.cs` | Transfer objects for the `Books` and `Stu1` rows (`IUser` is a class, not an interface) |
| `Emailverifycs.cs` | Generates the verification code, sends it, and checks the entry |
| `PasswordUpdator.cs` | Validates and writes the new password |
| `BackPhoto.cs` | Sets the shared background image on a form |
| `ListViewCre.cs` | Builds the `ListView` columns used by `Form2` and `Form3` |
| `sql/` | `schema.sql` (idempotent DDL), `seed.sql` (admin account + sample books) |

### Database

Three tables — `Stu1` (users), `Books` (catalogue), `saver1` (per-user basket).
Full definitions, with notes on every column whose type is dictated by the
current C# rather than by good sense, are in
[`sql/schema.sql`](sql/schema.sql).

---

## Known issues / roadmap

**Repository weight**

- `Form1.resx` is **12 MB** — a background image base64-encoded into the
  designer resource file. It is the single biggest reason a fresh clone is
  slow. The fix is to move the image out to `Resources/` and reference it
  through `Resource1.resx`, but that rewrites a designer file, so it has not
  been done yet.
- `Resources/` holds `home-button.png`, `home-button (1).png`,
  `home-button (2).png`, `home.png` and `that.png`. The three `home-button*`
  files look like duplicates left over from repeated "Save As" — they differ
  byte-for-byte but only one is referenced. `that.png` is a 1.9 MB background
  that is *also* embedded in `Resource1.resx`.
- `bin/`, `obj/`, `.vs/` and the `*.csproj.user` files used to be committed
  (~210 MB of build output and IDE state). They are now untracked and covered
  by `.gitignore`. History still contains them; shrinking that needs a
  `git filter-repo` rewrite and a force push, which has not been done.
- Local `*.mdf` / `*.ldf` files are gitignored. Rebuild from `sql/schema.sql`.

**Correctness / security**

- There is no role table: "admin" is the literal string `admin` in
  `Stu1.username`. `Security/Session.cs` holds the signed-in user in
  process-wide static state, which is the smallest change that made the admin
  guard reliable but is not a pattern to copy — the identity should be threaded
  through form constructors instead. The user identity below the guard is still
  a bare `string` (`Form4` → `Form9` → `Form5` → `DbCrudBook`).
- `saver1.iduser` stores that username rather than a foreign key to `Stu1.Id`,
  so renaming a user orphans their basket. `DbCrudBook`'s constructor parameter
  is called `id` and documented as `Stu1.Id`, but every caller passes a
  username.
- `Books.price` and `saver1.price` are text columns. Comparisons are string
  comparisons (`'5'` and `'5.00'` are different prices) and unparseable values
  are silently treated as `0` rather than reported.
- `Stu1.Birthdate` and `Books.Date` are text columns written in two different
  formats by two different call sites — `DateTime.ToString()` (culture
  dependent) from `Form2`/`Form7`, `ToString("yyyy-MM-dd")` from `Form3`.
- The data-access classes still call `MessageBox.Show` and still mutate the
  caller's controls, and `DataBaseCrud.selector` branches on `form is Form5` to
  decide whether to add a checkbox column. See
  [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).
- Test coverage reaches only the pure code (`PasswordHasher` and the validation
  patterns). Almost everything else below the UI takes a live `Form`,
  `ListView` or `DataGridView` and cannot be tested without one.

**Roadmap**

1. Replace the static `Session` with an identity threaded through the form
   constructors, carrying `Stu1.Id`, and make `saver1.iduser` a real foreign key.
2. Introduce a repository/service seam so the forms stop passing `ListView` and
   `DataGridView` instances into the data layer, and so `MessageBox.Show`
   disappears from everything below the UI — see
   [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).
3. Migrate `Birthdate`, `Date`, `price` and `quantity` to real SQL types
   (`DATE`, `DECIMAL(10,2)`, `INT`).
4. Unify the `db` and `WinFormsApp3` namespaces.
5. Extend the test suite. `Book.PriceValue` and `LabelValidator.Validate` are
   pure enough to cover today; the repositories become testable after step 2.

---

## License

[MIT](LICENSE) © 2026 ali-az1
