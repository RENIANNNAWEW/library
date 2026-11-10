
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

    // READ: every member, with how many books each has out now.
    public async Task<List<Member>> GetAllAsync()
    {
        // 1. SQL: every member with their open loans counted.
        const string sql =
            "SELECT m.member_id, m.full_name, m.email, m.member_type, COUNT(l.loan_id) " +
            "FROM lending.member m " +
            "LEFT JOIN lending.loan l ON l.member_id = m.member_id AND l.returned_on IS NULL " +
            "GROUP BY m.member_id " +
            "ORDER BY m.full_name;";

        // 2. Create an empty list.
        var members = new List<Member>();

        // 3. Create the command.
        await using var command = _dataSource.CreateCommand(sql);

        // 4. Execute the command.
        await using var reader = await command.ExecuteReaderAsync();

        // 5. Read every row.
        while (await reader.ReadAsync())
        {
            // 6. Copy the row into a Member.
            var member = ReadMember(reader);

            // 7. Get the number of open loans.
            member.OpenLoans = reader.GetInt64(4);

            // 8. Add the member.
            members.Add(member);
        }

        // 9. Return the list.
        return members;
    }

    // CREATE: add a new member.
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
            (object?)member.Email ?? DBNull.Value
        );
        command.Parameters.AddWithValue("@member_type", member.MemberType);

        await command.ExecuteNonQueryAsync();
    }

    // READ: find one member by ID.
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

    // UPDATE: change an existing member.
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
            (object?)member.Email ?? DBNull.Value
        );
        command.Parameters.AddWithValue("@member_type", member.MemberType);
        command.Parameters.AddWithValue("@id", member.MemberId);

        await command.ExecuteNonQueryAsync();
    }

    // DELETE: remove a member by ID.
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
