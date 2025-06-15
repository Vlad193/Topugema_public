using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

public static class Validator
{

    public static async Task<UInt64> GetNewID(AppDbContext db) {
        var counter = await db.Counters.FindAsync(0UL);
        if (counter == null)
        {
            counter = new Counter { ID = 0UL, Value = 1 };
            db.Counters.Add(counter);
        }
        else
        {
            counter.Value++;
        }
        await db.SaveChangesAsync();
        return counter.Value;

    }
    public static bool Name(string name, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Name is empty";
            return false;
        }

        if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_.-]+$"))
        {
            error = "Name must contain only a-z A-Z 0-9 . - _ symbols";
            return false;
        }

        if (name.Length < 3)
        {
            error = "Name must contain more than 3 symbols";
            return false;
        }

        if (name.Length > 16)
        {
            error = "Name must contain less than 16 symbols";
            return false;
        }

        return true;
    }

    public static bool Password(string password, out string error)
    {
        error = "";

        if (string.IsNullOrEmpty(password))
        {
            error = "Password is empty";
            return false;
        }

        if (password.Length < 8)
        {
            error = "Password must have at least 8 characters";
            return false;
        }

        return true;
    }
}