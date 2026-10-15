using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about members lives in this one class.
public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Member.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            MemberType = reader.GetString(3)
        };
    }

    // READ: every member, sorted by full name.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql =
            "SELECT member_id, full_name, email, member_type " +
            "FROM lending.member ORDER BY full_name;";

        var members = new List<Member>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            members.Add(ReadMember(reader));
        }

        return members;
    }

    public async Task AddAsync(Member member)
    {
        const string sql = """
        INSERT INTO lending.member
            (full_name, email, member_type)
        VALUES
            (@full_name, @email, @member_type);
        """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@full_name", member.FullName);
        command.Parameters.AddWithValue(
            "@email",
            (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@member_type", member.MemberType);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<Member?> GetByIdAsync(long id)
    {
        const string sql = """
        SELECT member_id, full_name, email, member_type
        FROM lending.member
        WHERE member_id = @id;
        """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return ReadMember(reader);
        }

        return null;
    }

    public async Task UpdateAsync(Member member)
    {
        const string sql = """
        UPDATE lending.member
        SET full_name = @full_name,
            email = @email,
            member_type = @member_type
        WHERE member_id = @id;
        """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@full_name", member.FullName);
        command.Parameters.AddWithValue(
            "@email",
            (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@member_type", member.MemberType);
        command.Parameters.AddWithValue("@id", member.MemberId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = """
        DELETE FROM lending.member
        WHERE member_id = @id;
        """;

        await using var connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }





}
