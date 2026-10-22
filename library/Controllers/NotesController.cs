
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using library.Models;

namespace library.Controllers
{
    public class NotesController : Controller
    {
        private readonly NpgsqlDataSource _dataSource;

        public NotesController(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IActionResult> Index()
        {
            var notes = new List<Note>();

            const string sql = """
                SELECT note_id, title, body, created_on
                FROM lending.note
                ORDER BY note_id;
                """;

            await using var connection =
                await _dataSource.OpenConnectionAsync();

            await using var command =
                new NpgsqlCommand(sql, connection);

            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notes.Add(new Note
                {
                    NoteId = reader.GetInt64(0),
                    Title = reader.GetString(1),
                    Body = reader.IsDBNull(2)
                        ? null
                        : reader.GetString(2),
                    CreatedOn = reader.GetDateTime(3)
                });
            }

            return View(notes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
                return View(note);

            const string sql = """
                INSERT INTO lending.note (title, body)
                VALUES (@title, @body);
                """;

            await using var connection =
                await _dataSource.OpenConnectionAsync();

            await using var command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("title", note.Title);
            command.Parameters.AddWithValue(
                "body", (object?)note.Body ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long id)
        {
            var note = await GetNoteAsync(id);

            if (note == null)
                return NotFound();

            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Note note)
        {
            if (!ModelState.IsValid)
                return View(note);

            const string sql = """
                UPDATE lending.note
                SET title = @title, body = @body
                WHERE note_id = @id;
                """;

            await using var connection =
                await _dataSource.OpenConnectionAsync();

            await using var command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("title", note.Title);
            command.Parameters.AddWithValue(
                "body", (object?)note.Body ?? DBNull.Value);
            command.Parameters.AddWithValue("id", note.NoteId);

            int rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(long id)
        {
            var note = await GetNoteAsync(id);

            if (note == null)
                return NotFound();

            return View(note);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            const string sql = """
                DELETE FROM lending.note
                WHERE note_id = @id;
                """;

            await using var connection =
                await _dataSource.OpenConnectionAsync();

            await using var command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("id", id);

            int rows = await command.ExecuteNonQueryAsync();

            if (rows == 0)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Note?> GetNoteAsync(long id)
        {
            const string sql = """
                SELECT note_id, title, body, created_on
                FROM lending.note
                WHERE note_id = @id;
                """;

            await using var connection =
                await _dataSource.OpenConnectionAsync();

            await using var command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("id", id);

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Note
            {
                NoteId = reader.GetInt64(0),
                Title = reader.GetString(1),
                Body = reader.IsDBNull(2)
                    ? null
                    : reader.GetString(2),
                CreatedOn = reader.GetDateTime(3)
            };
        }
    }
}
