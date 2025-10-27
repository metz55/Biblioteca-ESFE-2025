using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Library.Client.MVC.services
{
    public class BookImportService
    {
        private readonly DBContext _db;
        public BookImportService(DBContext db)
        {
            _db = db;
        }

        public async Task<BookImportResult> ImportarAsync(Stream excelStream, bool actualizarExistentes = true, bool crearCatalogosPorNombre = true, CancellationToken ct = default)
        {
            IWorkbook workbook;
            if (excelStream.CanSeek)
                excelStream.Position = 0;
            workbook = new XSSFWorkbook(excelStream);
            var sheet = workbook.GetSheetAt(0) ?? throw new InvalidOperationException("No se encontró ninguna hoja en el Excel.");
            var result = new BookImportResult();

            // Mapear cabeceras
            var map = MapHeaders(sheet);

            // Requeridos mínimos
            string[] req = { "Titulo*", "Año*", "Ejemplares*", "Existencias*" };
            foreach (var r in req)
                if (!map.ContainsKey(r))
                    throw new InvalidOperationException($"Falta la columna requerida: {r}");

            // Cargar catálogos
            var categorias = await _db.Categories.AsNoTracking().ToListAsync(ct);
            var autores = await _db.Authors.AsNoTracking().ToListAsync(ct);
            var editoriales = await _db.Editorials.AsNoTracking().ToListAsync(ct);
            var paises = await _db.Countries.AsNoTracking().ToListAsync(ct);
            var ediciones = await _db.Editions.AsNoTracking().ToListAsync(ct);
            var adquisiciones = await _db.Acquisition_Types.AsNoTracking().ToListAsync(ct);
            var catalogos = await _db.Catalogs.AsNoTracking().ToListAsync(ct);

            // Diccionarios rápidos usando las propiedades correctas
            var catByDesc = categorias.GroupBy(x => Norm(x.CATEGORY_NAME))
                                      .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var autorByDesc = autores.GroupBy(x => Norm(x.AUTHOR_NAME))
                                     .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var ediByDesc = editoriales.GroupBy(x => Norm(x.EDITORIAL_NAME))
                                       .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var paisByDesc = paises.GroupBy(x => Norm(x.COUNTRY_NAME))
                                   .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var edByDesc = ediciones.GroupBy(x => Norm(x.EDITION_NUMBER))
                                   .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var adqByDesc = adquisiciones.GroupBy(x => Norm(x.ACQUISITION_TYPE))
                                         .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            var catLogByDesc = catalogos.GroupBy(x => Norm(x.CATALOG_NAME))
                                        .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            int last = sheet.LastRowNum;

            using var trx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                for (int r = 1; r <= last; r++)
                {
                    var row = sheet.GetRow(r);
                    if (row == null || RowIsEmpty(row)) continue;

                    string titulo = Get(row, map, "Titulo*")?.Trim() ?? "";
                    string añoStr = Get(row, map, "Año*")?.Trim() ?? "";
                    string ejemplaresStr = Get(row, map, "Ejemplares*")?.Trim() ?? "";
                    string existenciasStr = Get(row, map, "Existencias*")?.Trim() ?? "";
                    string categoriaNom = Get(row, map, "Categoria")?.Trim() ?? "";
                    string autorNom = Get(row, map, "Autor")?.Trim() ?? "";
                    string editorialNom = Get(row, map, "Editorial")?.Trim() ?? "";
                    string paisNom = Get(row, map, "Pais")?.Trim() ?? "";
                    string edicionNom = Get(row, map, "Edicion")?.Trim() ?? "";
                    string adquisicionNom = Get(row, map, "Adquisicion")?.Trim() ?? "";
                    string catalogoNom = Get(row, map, "Catalogo")?.Trim() ?? "";
                    string coverPath = Get(row, map, "Cover")?.Trim() ?? "/ImagenesBiblioteca/default.jpg";
                    string dewey = Get(row, map, "DEWEY")?.Trim() ?? "0";
                    string cutter = Get(row, map, "CUTER")?.Trim() ?? "0";

                    if (string.IsNullOrWhiteSpace(titulo))
                    {
                        result.Errores.Add(new BookImportError { Fila = r + 1, Campo = "Titulo*", Mensaje = "El título es requerido." });
                        continue;
                    }
                    if (!int.TryParse(añoStr, out var año))
                    {
                        result.Errores.Add(new BookImportError { Fila = r + 1, Campo = "Año*", Mensaje = "Año inválido." });
                        continue;
                    }
                    if (!int.TryParse(ejemplaresStr, out var ejemplares))
                    {
                        result.Errores.Add(new BookImportError { Fila = r + 1, Campo = "Ejemplares*", Mensaje = "Ejemplares inválido." });
                        continue;
                    }
                    if (!int.TryParse(existenciasStr, out var existencias))
                    {
                        result.Errores.Add(new BookImportError { Fila = r + 1, Campo = "Existencias*", Mensaje = "Existencias inválido." });
                        continue;
                    }

                    // ----- Catálogos -----
                    int? categoriaId = await ObtenerOCrearAsync(catByDesc, _db.Categories, categoriaNom, crearCatalogosPorNombre, ct, "CATEGORY_NAME", "CATEGORY_ID");
                    int? autorId = await ObtenerOCrearAsync(autorByDesc, _db.Authors, autorNom, crearCatalogosPorNombre, ct, "AUTHOR_NAME", "AUTHOR_ID");
                    int? editorialId = await ObtenerOCrearAsync(ediByDesc, _db.Editorials, editorialNom, crearCatalogosPorNombre, ct, "EDITORIAL_NAME", "EDITORIAL_ID");
                    int? paisId = await ObtenerOCrearAsync(paisByDesc, _db.Countries, paisNom, crearCatalogosPorNombre, ct, "COUNTRY_NAME", "COUNTRY_ID");
                    int? edicionId = await ObtenerOCrearAsync(edByDesc, _db.Editions, edicionNom, crearCatalogosPorNombre, ct, "EDITION_NUMBER", "EDITION_ID");
                    int? adquisicionId = await ObtenerOCrearAsync(adqByDesc, _db.Acquisition_Types, adquisicionNom, crearCatalogosPorNombre, ct, "ACQUISITION_NAME", "ACQUISITION_ID");
                    int? catalogoId = await ObtenerOCrearAsync(catLogByDesc, _db.Catalogs, catalogoNom, crearCatalogosPorNombre, ct, "CATALOG_NAME", "CATALOG_ID");

                    // Buscar existente por título y año
                    var existente = await _db.Books.FirstOrDefaultAsync(x => x.TITLE == titulo && x.YEAR == año.ToString(), ct);
                    if (existente is null)
                    {
                        var nuevo = new Books
                        {
                            TITLE = titulo,
                            YEAR = año.ToString(),
                            EJEMPLARS = ejemplares,
                            EXISTENCES = existencias,
                            ID_CATEGORY = categoriaId ?? 0,
                            ID_AUTHOR = autorId ?? 0,
                            ID_EDITORIAL = editorialId ?? 0,
                            ID_COUNTRY = paisId ?? 0,
                            ID_EDITION = edicionId ?? 0,
                            ID_ACQUISITION = adquisicionId ?? 0,
                            ID_CATALOG = catalogoId ?? 0,
                            COVER = SaveCoverImageFromPath(coverPath),
                            DEWEY = dewey,
                            CUTER = cutter
                        };
                        _db.Books.Add(nuevo);
                        result.Creados++;
                    }
                    else
                    {
                        // Sumar ejemplares y existencias
                        existente.EJEMPLARS += ejemplares;
                        existente.EXISTENCES += existencias;

                        // Validar que existencias no superen ejemplares
                        if (existente.EXISTENCES > existente.EJEMPLARS)
                        {
                            existente.EXISTENCES = existente.EJEMPLARS;
                        }

                        // Actualizar otros campos
                        existente.ID_CATEGORY = categoriaId ?? existente.ID_CATEGORY;
                        existente.ID_AUTHOR = autorId ?? existente.ID_AUTHOR;
                        existente.ID_EDITORIAL = editorialId ?? existente.ID_EDITORIAL;
                        existente.ID_COUNTRY = paisId ?? existente.ID_COUNTRY;
                        existente.ID_EDITION = edicionId ?? existente.ID_EDITION;
                        existente.ID_ACQUISITION = adquisicionId ?? existente.ID_ACQUISITION;
                        existente.ID_CATALOG = catalogoId ?? existente.ID_CATALOG;
                        existente.COVER = SaveCoverImageFromPath(coverPath);
                        existente.DEWEY = dewey;
                        existente.CUTER = cutter;

                        if (actualizarExistentes) result.Actualizados++;
                    }
                }
                await _db.SaveChangesAsync(ct);
                await trx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await trx.RollbackAsync(ct);
                throw new Exception($"Error al importar libros: {ex.Message}", ex);
            }
            return result;
        }

        // ================= Helpers =================
        private static Dictionary<string, int> MapHeaders(ISheet sheet)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var headerRow = sheet.GetRow(0);
            if (headerRow == null) return map;
            for (int c = 0; c < headerRow.LastCellNum; c++)
            {
                var key = headerRow.GetCell(c)?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(key) && !map.ContainsKey(key))
                    map.Add(key, c);
            }
            return map;
        }

        private static string? Get(IRow row, Dictionary<string, int> map, string header)
        {
            if (map.TryGetValue(header, out var c))
            {
                var cell = row.GetCell(c);
                return cell?.ToString();
            }
            return null;
        }

        private static bool RowIsEmpty(IRow row)
        {
            if (row == null) return true;
            for (int c = 0; c < row.LastCellNum; c++)
            {
                var cell = row.GetCell(c);
                if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                    return false;
            }
            return true;
        }

        private static string Norm(string s) => s.Trim().ToLowerInvariant();

        private string SaveCoverImageFromPath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                return "/ImagenesBiblioteca/default.jpg";

            var uploadDir = Path.Combine("C:\\", "ImagenesBiblioteca");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var fileName = Path.GetFileName(imagePath);
            var destinationPath = Path.Combine(uploadDir, fileName);

            // Si ya existe, no la copiamos de nuevo
            if (!File.Exists(destinationPath))
            {
                try
                {
                    File.Copy(imagePath, destinationPath);
                }
                catch
                {
                    return "/ImagenesBiblioteca/default.jpg";
                }
            }

            return Path.Combine("/ImagenesBiblioteca", fileName);
        }

        private async Task<int?> ObtenerOCrearAsync<T>(
        Dictionary<string, T> dict,
        DbSet<T> dbSet,
        string nombre,
        bool crear,
        CancellationToken ct,
        string propNombre,
        string propId
        ) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            var key = Norm(nombre);
            if (dict.TryGetValue(key, out var val))
            {
                var idProp = val.GetType().GetProperty(propId);
                var value = idProp?.GetValue(val);
                if (value is long longValue)
                    return (int)longValue; // Convierte explícitamente de long a int
                return value as int?;
            }
            if (crear)
            {
                dynamic entity = new T();
                entity.GetType().GetProperty(propNombre)?.SetValue(entity, nombre);
                dbSet.Add(entity);
                await _db.SaveChangesAsync(ct);
                dict[key] = entity;
                var idProp = entity.GetType().GetProperty(propId);
                var value = idProp?.GetValue(entity);
                if (value is long longValue)
                    return (int)longValue; // Convierte explícitamente de long a int
                return value as int?;
            }
            return null;
        }

    }

    public class BookImportResult
    {
        public int Creados { get; set; }
        public int Actualizados { get; set; }
        public List<BookImportError> Errores { get; set; } = new();
    }

    public class BookImportError
    {
        public int Fila { get; set; }
        public string Campo { get; set; } = "";
        public string Mensaje { get; set; } = "";
    }
}
