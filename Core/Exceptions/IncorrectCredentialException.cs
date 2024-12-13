using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions;

public class IncorrectCredentialException(string message, string username) : Exception(message)
{
    public string Username { get; } = username;
}